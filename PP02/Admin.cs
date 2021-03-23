using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PP02
{
    public partial class Admin : Form
    {
        string Server = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=PP02;Integrated Security=True";
        SqlDataAdapter sql;
        DataSet  inf1, db, up, ins, max, del, yn;

        public int sec, n,min,id;
        public string name, photo, price, sale, opic;
        Bitmap img;

        //Дополнительные изображения
        private void button1_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show("Элемент не выбран", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Storage.Photo = photo;
                Storage.ID = id;
                Image image = new Image();
                image.Show();
                this.Hide();
            }
           
        }

        //public string path = Convert.ToString(Environment.CurrentDirectory);//Путь к папке с проектом

        //Кнопка назад
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            AdminStart start = new AdminStart();
            start.Show();
            this.Hide();
        }

        public Admin()
        {
            InitializeComponent();
        }

        //Вывод при открытии
        private void Admin_Load(object sender, EventArgs e)
        {
            Informatoin();
            ToolTip t = new ToolTip();
            t.SetToolTip(photo_picture, "Добавить фото");
        }
        //Вывод информации в таблицу
        public void Informatoin()
        {
            string z = "select Title,Cost,Discount from Service  ";
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                inf1 = new DataSet();
                sql = new SqlDataAdapter(z, podkl);
                sql.Fill(inf1);
                dataGridView1.DataSource = inf1.Tables[0];
            }

        }

        //Очистить
        public void Clear()
        {
            name_txt.Clear();
            price_txt.Clear();
            opic_txt.Clear();
            sale_txt.Clear();
            id_txt.Clear();
            sec_mask.Clear();
            photo_picture.Image=(PP02.Properties.Resources._120nophoto_100007);
        }


        //Удаление
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (id == 0)
                MessageBox.Show("Элемент не выбран", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                DialogResult question = MessageBox.Show("Хотите удалить услугу?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (question == DialogResult.Yes)
                {
                    string d = "delete from Service where id="+id;
                    using (SqlConnection podkl = new SqlConnection(Server))
                    {
                        podkl.Open();
                        del = new DataSet();
                        sql = new SqlDataAdapter(d, podkl);
                        sql.Fill(del);
                    }
                    MessageBox.Show("Услуга удалена!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Informatoin();
                    Clear();
                }               
            }
        }

        //Добавление
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if(String.IsNullOrEmpty(name_txt.Text)|| String.IsNullOrEmpty(price_txt.Text)|| String.IsNullOrEmpty(sec_mask.Text)||String.IsNullOrEmpty(photo))
                MessageBox.Show("Заполните поля", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                //поиск на совпадении имени услуги
                string y = "select Title from Service where Title='" + name_txt.Text + "'";
                using (SqlConnection podkl = new SqlConnection(Server))
                {
                    podkl.Open();
                    yn = new DataSet();
                    sql = new SqlDataAdapter(y, podkl);
                    sql.Fill(yn);
                }

                if (yn.Tables[0].Rows.Count == 0)//если совпадениц не найдено
                {
                    if (String.IsNullOrEmpty(sale_txt.Text))//Добавление при пустой скидки
                    {
                        Insert();                       
                        Informatoin();
                    }
                    else
                    {
                        if (Convert.ToInt32(sale_txt.Text) < 100)
                        {
                            Insert();                           
                            Informatoin();
                        }
                        else
                            MessageBox.Show("Ошибка в заполнении", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show("Услуга с таким именем уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Clear();
            }
            
        }

        //Метод добавления
        public void Insert()
        {
            string m = "select MAX(ID) from Service";
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                max = new DataSet();
                sql = new SqlDataAdapter(m, podkl);
                sql.Fill(max);

            }
            int max_id = Convert.ToInt32(max.Tables[0].Rows[0][0])+1; //Максимальный ID
            id_txt.Text =""+ max_id;//Вывод Макс ID

                int sec = Convert.ToInt32(sec_mask.Text) * 60;
                int chas = (sec / 60) / 60;
                if (chas > 4)//Проверка на допустимое время услуги
                    MessageBox.Show("Недопустимое время услуги!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    string insert = "insert into Service(Title,Cost,DurationInSeconds,Description,Discount, MainImagePath)" +
                    " values('" + name_txt.Text + "', '" + price_txt.Text + "', " + sec.ToString() + ", '" + opic_txt.Text + "', '" + sale_txt.Text + "', '" + photo + "')";
                    using (SqlConnection podkl = new SqlConnection(Server))
                    {
                        podkl.Open();
                        ins = new DataSet();
                        sql = new SqlDataAdapter(insert, podkl);
                        sql.Fill(ins);
                    }
                MessageBox.Show("Услуга добавлена!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           

        }

        //******************редактирование*****************************
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (id == 0)
                MessageBox.Show("Элемент не выбран", "Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                if (String.IsNullOrEmpty(sale_txt.Text))
                {
                        Updates();
                        Informatoin();
                        MessageBox.Show("Услуга изменена", "", MessageBoxButtons.OK);
                }
                else
                {
                    if (Convert.ToInt32(sale_txt.Text) < 100)
                    {
                        Updates();
                        Informatoin();
                        MessageBox.Show("Услуга изменена", "", MessageBoxButtons.OK);
                    }
                    else
                        MessageBox.Show("Ошибка в заполнении", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
        }

        //Добавление фото
        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "image (*.JPG; *.PNG)|*.JPG;*.PNG";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    photo = ofd.FileName;
                    img = new Bitmap(photo);
                    photo_picture.Image = img;
                    photo_picture.Invalidate();
                }
                catch
                {
                    MessageBox.Show("Ошибка в изображении!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        //Метод редатирования
        public void Updates()
        {
            
            int sec = Convert.ToInt32(sec_mask.Text);
            int chas = (sec / 60)/60;
            double p = Convert.ToDouble(price_txt.Text);
            p = Math.Round(p,0);
            if(chas>4)
                MessageBox.Show("Недопустимое вренмя услуги!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                sec = sec * 60;
                string u = "update Service set Title='" + name_txt.Text + "', Cost='" + p.ToString() + "'," +
               " DurationInSeconds=" + sec.ToString() + ",Description='" + opic_txt.Text + "'," +
               " Discount='" + sale_txt.Text + "' ,MainImagePath='" + photo + "' where id=" + id;
                using (SqlConnection podkl = new SqlConnection(Server))
                {
                    podkl.Open();
                    up = new DataSet();
                    sql = new SqlDataAdapter(u, podkl);
                    sql.Fill(up);

                }
            }
           
        }
        //*******************************************************************

        //Выбор элемента
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            name = dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();
            string z = "select * from Service where Title='"+name+"'";
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                db = new DataSet();
                sql = new SqlDataAdapter(z, podkl);
                sql.Fill(db);
            }

            //Значения взятые из таблицы для редатирования
            id = Convert.ToInt32 (db.Tables[0].Rows[0]["ID"]);
            price = db.Tables[0].Rows[0]["Cost"].ToString();
            sale = db.Tables[0].Rows[0]["Discount"].ToString();
            min =Convert.ToInt32(db.Tables[0].Rows[0]["DurationInSeconds"])/60;
            opic = db.Tables[0].Rows[0]["Description"].ToString();
            photo =db.Tables[0].Rows[0]["MainImagePath"].ToString();

            if (photo == "")//Отображение если фото пустое
                photo = Convert.ToString(PP02.Properties.Resources._120nophoto_100007);
            else
            {
                img = new Bitmap(photo);
                //int result = String.Compare(path, photo);
                //if(result< 0)
                //{                    
                    
                //}
                //else
                //{
                //    photo = path + "\\" + photo;
                //    img = new Bitmap(photo);
                //}
                photo_picture.Image = img;

            }
                //Вывод
            name_txt.Text = name;
            id_txt.Text = id.ToString();
            price_txt.Text = price;
            opic_txt.Text = opic;
            sale_txt.Text = sale;
            sec_mask.Text = min +" мин.";


        }

        
    }
}
