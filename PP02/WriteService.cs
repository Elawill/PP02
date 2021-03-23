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
    public partial class WriteService : Form
    {

        string Server = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=PP02;Integrated Security=True";
        SqlDataAdapter sql;
        DataSet cl, inf, ins, dat, dat1;

        Bitmap img;
        public string photo, firstName, LastName, otch, id, dt, id_service, path, photo_path;
        string[] mas;
        public int n, i, sec, n_t;

        //НАЗАД ФОТО 
        private void back_arrow_Click(object sender, EventArgs e)
        {
            if (i > 0)
            {
                i--;
                photo_path = inf.Tables[0].Rows[i]["PhotoPath"].ToString();
                photo_picture.Image = new Bitmap(photo_path);
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
        //ВПЕРЕД ФОТО
        private void next_arrow_Click(object sender, EventArgs e)
        {
            if (i < n)
            {
                photo_path = inf.Tables[0].Rows[i]["PhotoPath"].ToString();
                photo_picture.Image = new Bitmap(photo_path);
                i++;
            }
            else
                next_arrow.Visible = false;

            back_arrow.Visible = true;
            iNF();
        }
        

        //Кнопка записать 
        private void button2_Click(object sender, EventArgs e)
        {
            WriteClient();
        }

        // Кнопка Назад
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Service service = new Service();
            service.Show();
            this.Hide();
        }

        public WriteService()
        {
            InitializeComponent();
        }

        //Поиск дополнительных фото
        public void iNF()
        {
            string u = "select * from ServicePhoto where ServiceID=" + id_service;
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

                    
                //}
                //else
                //{
                //    photo = path + "\\" + photo;
                //    img = new Bitmap(photo);
                //}
                photo_picture.Image = img;
            }
        }
         
        //Вывод при открытии
        private void WriteService_Load(object sender, EventArgs e)
        {
            name_txt.Text = Storage.Name;
            sec_txt.Text = Storage.Sec;
            opic_txt.Text = Storage.Opic;
            id_service = Storage.ID.ToString();
            ShowPhoto();
            sec = Convert.ToInt32(Storage.Sec);
            int min, h;
            min = sec / 60;
            if (min >= 60)
            {
                h = min / 60;
                min = min % 60;
                sec_txt.Text = h + " ч. " + min + " мин.";
            }
            else
                sec_txt.Text = min + " мин.";
            Client();
            iNF();

        }

        public void Client()//Заполнение списка клиентами 
        {
            string za = "select * from Client ";
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                cl = new DataSet();
                sql = new SqlDataAdapter(za, podkl);
                sql.Fill(cl);
            }
            int n = Convert.ToInt32(cl.Tables[0].Rows.Count);
            mas = new string[n];

            clientBox.Items.Clear();
            for (int i = 0; i < n; i++)
            {
                firstName = cl.Tables[0].Rows[i]["FirstName"].ToString();
                LastName = cl.Tables[0].Rows[i]["LastName"].ToString();
                otch=cl.Tables[0].Rows[i]["Patronymic"].ToString();
                mas[i] += firstName+" "+LastName+" "+otch;               
                clientBox.Items.Add(mas[i]);
            }

        }

        //Выбор клиента 
        private void clientBox_TextChanged(object sender, EventArgs e)
        {
            string [] clients= clientBox.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);//Разделение имени и фамилии
            string za = "select * from Client where FirstName='"+ clients[0]+ "' and LastName='"+ clients[1] + "'";
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                inf = new DataSet();
                sql = new SqlDataAdapter(za, podkl);
                sql.Fill(inf);
            }
            id = inf.Tables[0].Rows[0]["ID"].ToString();//ID клиента
            
        }


        //Запись на услугу 
        public void WriteClient()
        {
            string date = dateTimePicker1.Value.ToShortDateString(); //Выбранная дата         
            string time = time_txt.Text + ":00";//выбранное время услуги 

            string[] t = time.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);//разделение времени на часы и минуты
            int chas = Convert.ToInt32(t[0]);//Часы
            int min = Convert.ToInt32(t[1]);//Минуты

            dt = date + " " + time;
            DateTime dateTime = Convert.ToDateTime(dt);//Выбранная дата и время
            DateTime now = DateTime.Now;//текущая дата и время

            bool flag = true;
           
            int rez = DateTime.Compare(dateTime,now);

            if (chas > 23 && min >59 )//Проверка на верность времени 
            {
                MessageBox.Show("Не верный формат времени!","Ошибка", MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                if (rez < 0)//Проверка что услуга записывается не на прошлое время
                {
                    MessageBox.Show("Услуга на эту запись больше не доступна !", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //Проеврка есть ли дата в бд
                    string d = "select * from ClientService where StartTime='" + dt + "'";
                    using (SqlConnection podkl = new SqlConnection(Server))
                    {
                        podkl.Open();
                        dat = new DataSet();
                        sql = new SqlDataAdapter(d, podkl);
                        sql.Fill(dat);
                    }

                    //Поиск всех записей на заданую услугу
                    string d1 = "select StartTime from ClientService where ServiceID ="+id_service+ " and StartTime>GetDate()";
                    using (SqlConnection podkl = new SqlConnection(Server))
                    {
                        podkl.Open();
                        dat1 = new DataSet();
                        sql = new SqlDataAdapter(d1, podkl);
                        sql.Fill(dat1);
                    }
                    n_t = Convert.ToInt32(dat1.Tables[0].Rows.Count);

                    string [] times = new string [n_t];

                    DateTime addsec = dateTime.AddSeconds(sec);//прибавление времени к записи
                    DateTime minussec = dateTime.AddSeconds(-sec);//вычитание времени к записи 
                    DateTime adtime=new DateTime();

                    //Цикл проверяющий может ли веденое время быть записанными в промежутке записей
                    for (int j=0; j < n_t; j++)
                    {                       
                        times[j] += dat1.Tables[0].Rows[j][0].ToString();//записи услуг 
                        if (Convert.ToDateTime(times[j]) > minussec && Convert.ToDateTime(times[j]) < addsec)
                        {
                            flag = false;
                            adtime = Convert.ToDateTime(times[j]).AddSeconds(sec);
                            break;
                        }

                    }                   

                    if (dat.Tables[0].Rows.Count == 0)//Если записи нет в бд
                    {
                        if (flag == true)
                            Inserts();
                        else
                            MessageBox.Show("Запись в это время не доступна, проходит занятие.\nБлижайшая свободная запись: " + adtime
                                + "", "", MessageBoxButtons.OK, MessageBoxIcon.Error);   
                    }

                    else
                        MessageBox.Show("Запись занята!\nБлижайшая свободная запись: " + adtime + "", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }     
                   
            }

        }

        //Добавление записи 
        public void Inserts()
        {
            string insert = "insert into ClientService (ClientID,ServiceID,StartTime) " +
                    "values(" + id + ", " + id_service + ", '" + dt + "')";
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                ins = new DataSet();
                sql = new SqlDataAdapter(insert, podkl);
                sql.Fill(ins);
            }
            MessageBox.Show("Вы записаны!", "", MessageBoxButtons.OK);
        }
    }
}
