using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PP02
{
    public partial class Image : Form
    {
        string Server = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=PP02;Integrated Security=True";
        SqlDataAdapter sql;
        DataSet inf, ins, del;
        Bitmap img;

        public int n, id_service;
        public string photo, path, photo_path;
        string [] mas;
        public int i;

        public Image()
        {
            InitializeComponent();
        }
        //ВПЕРЕД ФОТО
        private void next_arrow_Click(object sender, EventArgs e)
        {
            if (i < n)
            {

                photo_path = inf.Tables[0].Rows[i]["PhotoPath"].ToString();
                pictureBox2.Image = new Bitmap(photo_path);
                i++;
            }
            else
                next_arrow.Visible = false;

            back_arrow.Visible = true;
            iNF();
        }

        //НАЗАД ФОТО
        private void back_arrow_Click(object sender, EventArgs e)
        {
            if (i > 0)
            {
                i--;
                photo_path = inf.Tables[0].Rows[i]["PhotoPath"].ToString();
                pictureBox2.Image = new Bitmap(photo_path);
            }
            else if (i == 0)
            {
                ShowPhoto();              
            }
            else
                back_arrow.Visible = false;

            next_arrow.Visible = true;
            iNF();
        }

        //Кнопка добавить фото 
        private void button1_Click(object sender, EventArgs e)
        {
            Inser_ServicePhoto();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Admin admin = new Admin();
            admin.Show();
            this.Hide();
        }

        //Кнопка Удалить фото 
        private void button2_Click(object sender, EventArgs e)
        {
            if (i != 0)//Если не главное изображение 
            {
                string d = "delete from ServicePhoto where PhotoPath='"+ photo_path+"'";
                using (SqlConnection podkl = new SqlConnection(Server))
                {
                    podkl.Open();
                    del = new DataSet();
                    sql = new SqlDataAdapter(d, podkl);
                    sql.Fill(del);
                }
                MessageBox.Show("Фото удалено!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                photo_path = inf.Tables[0].Rows[0]["PhotoPath"].ToString();
                pictureBox2.Image = new Bitmap(photo_path);
            }
            else
                MessageBox.Show("Вы не можете удалить это изображение!","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Image_Load(object sender, EventArgs e)
        {
            ShowPhoto();
            id_service = Convert.ToInt32(Storage.ID);
            iNF();
            
        }

        //Вывод  изображения выбранной услуги 
        public void ShowPhoto()
        {
            //path = Convert.ToString(Environment.CurrentDirectory);
            photo = Storage.Photo;
            if (photo == "")
                photo = Convert.ToString(PP02.Properties.Resources._120nophoto_100007);
            else
            {
                img = new Bitmap(photo);
                //int result = String.Compare(path, photo);
                //if (result < 0)
                //{

                //    img = new Bitmap(photo);
                //}
                //else
                //{
                //    photo = path + "\\" + photo;
                //    img = new Bitmap(photo);
                //}
                pictureBox2.Image = img;              
            }
        }

        //Поиск дополнительных фото
        public void iNF()
        {
            string u = "select * from ServicePhoto where ServiceID="+id_service;
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                inf = new DataSet();
                sql = new SqlDataAdapter(u, podkl);
                sql.Fill(inf);

            }           
            if (inf.Tables[0].Rows.Count != 0)
            {
                
                n = Convert.ToInt32(inf.Tables[0].Rows.Count);
                next_arrow.Visible = true;
            }
            
        }

        //Добавление изображения
        public void Inser_ServicePhoto()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "image (*.JPG; *.PNG)|*.JPG;*.PNG";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    photo = ofd.FileName;
                    img = new Bitmap(photo);
                    photo_path =photo;
                    string insert = "insert into ServicePhoto(ServiceID, PhotoPath) values(" + id_service + ", '" + photo_path + "')";
                    using (SqlConnection podkl = new SqlConnection(Server))
                    {
                        podkl.Open();
                        ins = new DataSet();
                        sql = new SqlDataAdapter(insert, podkl);
                        sql.Fill(ins);
                    }
                    MessageBox.Show("Изображение добавлено!", "Успешно", MessageBoxButtons.OK);
                    pictureBox2.Image = img;
                    pictureBox2.Invalidate();
                    iNF();

                }
                catch
                {
                    MessageBox.Show("Ошибка в изображении!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } 
        }
    }
}
