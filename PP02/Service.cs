using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PP02
{
    public partial class Service : Form
    {
        string Server = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=PP02;Integrated Security=True";
        SqlDataAdapter sql;
        DataSet inf, inf1, db, up;

        public int n, sale, price, sec, id;
        public string zap, photo, name, opic;
        int[] mas_sale, mas_price, mas_price_sale;
        string[] chas, min;

       

        bool x, y, z;
        //Кнопка назад
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Start start = new Start();
            start.Show();
            this.Hide();
        }

        public void Sale()//Поиск по условию скидки 
        {
            if (sale_box.Text == "0%-5%")
                zap += "where  Discount<5 or Discount is null";

            else if (sale_box.Text == "5%-15%")
                zap += "where Discount>=5 AND Discount<15";

            else if (sale_box.Text == "15%-30%")
                zap += "where Discount>=15 AND Discount<30";

            else if (sale_box.Text == "30%-70%")
                zap += "where Discount>=30 AND Discount<70";

            else if (sale_box.Text == "70%-100%")
                zap += "where Discount>=30AND Discount<70";

        }

        //Фильтр скидки 
        private void sale_box_TextChanged(object sender, EventArgs e)
        {
            if (sale_box.Text == "ВСЕ")
                y = false;
            else
            {
                y = true;
                Sale();
            }
            Information();
        }

        //Фильтр цены
        private void orderby_box_TextChanged(object sender, EventArgs e)
        {
            if (orderby_box.Text == "По убыванию")
                x = true;
            else if (orderby_box.Text == "По возрастанию")
                x = false;

            Information();
        }

        //Фильтр НАЙТИ
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                z = false;
            else
                z = true;
            Information();
        }

        public Service()
        {
            InitializeComponent();
        }

        private void Service_Load(object sender, EventArgs e)
        {
            Information();
        }


        //Вывод всей информации
        public void Information()
        {
            listView1.Items.Clear();
       
            //string path = Convert.ToString(Environment.CurrentDirectory);//Путь к папке с проектом    
            string za = "select * from Service ";
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                inf1 = new DataSet();
                sql = new SqlDataAdapter(za, podkl);
                sql.Fill(inf1);
            }
            int alln = Convert.ToInt32(inf1.Tables[0].Rows.Count);//Все значнения в базе 

            //Вариации запроса поиска по фильтрам
            zap = "select * from Service ";

            if (y == true && x == true&& z == true)//Если выбраны все значения
            {
                Sale();
                zap += " and Title like'"+textBox1.Text+ "%' order by Cost desc";

            }
            else if(y == true && x == true)//Если выбраны скидка и цена
            {
                Sale();
                zap += " order by Cost desc";
            }
            else if (y == true && z == true)//Если выбраны скидка и текст
            {
                Sale();
                zap += " and Title like'" + textBox1.Text + "%'";
            }
            else if (x == true && z == true)//Если выбраны цена и текст
            {
                zap += " where Title like'" + textBox1.Text + "%' order by Cost desc";
            }
            else if (y == true)//Если скидка
                Sale();

            else if (z==true)//Если текст
                zap += " where Title like'"+textBox1.Text+"%'";
            else if (x == false)//если цена
                zap += " order by Cost asc";

            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                inf = new DataSet();
                sql = new SqlDataAdapter(zap, podkl);
                sql.Fill(inf);
            }
            n = Convert.ToInt32(inf.Tables[0].Rows.Count);//кол-во выведенных товаров
            label1.Text = "Кол-во предоженных услуг " + n + " из " + alln;

            ImageList image = new ImageList();
            image.ImageSize = new Size(150, 150);

            //Цикл записывающий фото из бд в лист
            for (int i = 0; i < n; i++)
            {
                photo =inf.Tables[0].Rows[i]["MainImagePath"].ToString();
                image.Images.Add(new Bitmap(photo));
                //if (photo != "")
                //{
                //    int result = String.Compare(path, photo);
                //    if (result < 0)
                //    {

                //        image.Images.Add(new Bitmap(photo));
                //    }
                //    else
                //    {
                //        photo = path + "\\" + photo;
                //        image.Images.Add(new Bitmap(photo));
                //    }

                //}

            }

            Bitmap img = new Bitmap(150, 150);
            image.Images.Add(img);
            listView1.SmallImageList = image;

            mas_sale = new int[n];
            mas_price = new int[n];
            mas_price_sale = new int[n];
            chas = new string[n];
            min = new string[n];

            //Скидка
            for (int i = 0; i < n; i++)
            {
                price = Convert.ToInt32(inf.Tables[0].Rows[i]["Cost"]);
                mas_price[i] += price;
                if (inf.Tables[0].Rows[i]["Discount"].ToString() != "")
                {
                    sale = Convert.ToInt32(inf.Tables[0].Rows[i]["Discount"]);
                    mas_sale[i] += sale;
                    price -= (price / 100 * sale);
                    mas_price_sale[i] += price;
                }
            }

            //Время
            for (int i = 0; i < n; i++)
            {

                int mins,h;
                sec = Convert.ToInt32(inf.Tables[0].Rows[i]["DurationInSeconds"]);
                mins = sec / 60;
                if (mins >= 60)
                {
                    h = mins / 60;
                    mins = mins % 60;

                    chas[i] += Convert.ToString(h) + " " + "ч ";
                }

                min[i] += Convert.ToString(mins) + " " + "мин";               
                
            }


            //Занесение в список
            for (int i = 0; i < n; i++)
            {
                ListViewItem list;
                //Записи где есть скидка
                if (mas_sale[i] != 0)
                {
                    //Занесение всех записей 
                    list = new ListViewItem(new string[] {"", name= inf.Tables[0].Rows[i]["Title"].ToString(),
                    mas_price[i].ToString(),
                    mas_price_sale[i].ToString(),
                    mas_sale[i].ToString(), chas[i]+" "+ min[i] });

                    //Применение стилей для текста с ценой
                    Font f = new Font("Comic Sans MS", 10, FontStyle.Strikeout);
                    list.SubItems[2].Font = f;//стиль текста 
                    Font f1 = new Font("Comic Sans MS", 12, FontStyle.Bold);
                    list.SubItems[3].Font = f1;//стиль текста 
                    for (int j = 0; j < listView1.Columns.Count; j++)
                    {
                        list.SubItems[j].BackColor = Color.FromArgb(179, 247, 111);
                    }
                    list.UseItemStyleForSubItems = false;//применение стиля отдельно для каждой ячейки 

                }
                else
                {
                    list = new ListViewItem(new string[] {"", name= inf.Tables[0].Rows[i]["Title"].ToString(),
                   mas_price[i].ToString(),"","", chas[i]+" "+ min[i]  });
                }
                list.ImageIndex = i;
                listView1.Items.Add(list);

            }

        }

        //Записаться
        private void button1_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show("Выберите услугу","Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //Сохранение данных для передачи на другую форму
                Storage.Name = db.Tables[0].Rows[0]["Title"].ToString();
                Storage.Opic = db.Tables[0].Rows[0]["Description"].ToString();
                Storage.Sec = db.Tables[0].Rows[0]["DurationInSeconds"].ToString();
                Storage.Photo = db.Tables[0].Rows[0]["MainImagePath"].ToString();
                Storage.ID = id;
                WriteService writeService = new WriteService();
                writeService.Show();
                this.Hide();
            }
               

        }

        //ВЫБОР УСЛУГИ 
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                //selected = item.ImageIndex;
                selected = item.SubItems[1].Text;

                string a = "select * from Service where Title='" + selected + "'";
                using (SqlConnection podkl = new SqlConnection(Server))
                {
                    podkl.Open();
                    db = new DataSet();
                    sql = new SqlDataAdapter(a, podkl);
                    sql.Fill(db);
                }
                id =Convert.ToInt32( db.Tables[0].Rows[0]["ID"]);              


            } 
        }
    }
}
