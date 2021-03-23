using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PP02
{
    public partial class SoonWriteService : Form
    {
        string Server = @"Data Source=LAPTOP-BQ8RM7MB\SQLEXPRESS;Initial Catalog=PP02;Integrated Security=True";
        SqlDataAdapter sql;
        DataSet inf;

        public string z, time, name_ser, name_cl, email, phone;
        public int n, rez_min,s=0;
        string[] ost_min, ost_ch;

        public SoonWriteService()
        {
            InitializeComponent();
        }

        //Обновление окна каждые 30 сек.
        private void timer1_Tick(object sender, EventArgs e)
        {
            s++;
            if (s == 30)
            {
                ShowInf();
                s = 0;
            }
            
        }

        private void SoonWriteService_Load(object sender, EventArgs e)
        {
            ShowInf();
            timer1.Enabled = true;
            
        }
        //Поиск инфорамации для отображения
        public void ShowInf()
        {
             z = "declare @today datetime; " +
                "declare @tomorrow datetime; " +
                "set @today = GETDATE();" +
                "set @tomorrow = DATEADD(DAY, 1, GETDATE());" +
                "select s.Title, c.FirstName,c.LastName, c.Email, c.Phone, cs.StartTime " +
                "from Client c join ClientService cs on c.ID = cs.ClientID join Service s on s.ID = cs.ServiceID " +
                "where StartTime>@today order by StartTime asc";
            UsingZ();
        }

        //Выполнение запроса
        public void UsingZ()
        {
            using (SqlConnection podkl = new SqlConnection(Server))
            {
                podkl.Open();
                inf = new DataSet();
                sql = new SqlDataAdapter(z, podkl);
                sql.Fill(inf);
               
            }
            n = Convert.ToInt32(inf.Tables[0].Rows.Count);
            ShowList();

        }

        //Отображение информации на листе
        public void ShowList()
        {
            DateTime now = DateTime.Now;//текущая дата и время

            ListViewItem list;

            //ТЕКУЩЕЕ ВРЕМЯ
            string[] datetime_now =now.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);//Разделение на дату и время
            DateTime date_now = Convert.ToDateTime( datetime_now[0]);//текущая дата
            string[] time_now = datetime_now[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);//разделение времени на часы и минуты
            int min_now =  Convert.ToInt32(time_now[1]);//текущие минуты
            int chas_now =  Convert.ToInt32(time_now[0]);//текущие часы

            listView1.Items.Clear();

            ost_min = new string[n];//Массив записывающий минуты
            ost_ch = new string[n];//Массив записывающий часы

            for (int i = 0; i < n; i++)
            {
                //ВВЕДЕННАЯ ДАТА
                //Разделение на дату и время записи на услугу
                string[] datetime = inf.Tables[0].Rows[i]["StartTime"].ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                DateTime date = Convert.ToDateTime( datetime[0]);//Выбранная дата
                time = datetime[1];
                //Разделение на часы и минуты записи на услугу
                string[] times = time.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                int min = Convert.ToInt32(times[1]);
                int chas = Convert.ToInt32(times[0]);
      
                int rez_chas;
                int rez_cc;
                int rez_date = DateTime.Compare(date_now, date);
                rez_chas = chas_now - chas;
                if (time == "0:00:00")
                {
                    time = "";
                }
                if (rez_date < 0)//если завтра

                    rez_cc = 24 - rez_chas;
                else
                    rez_cc = -rez_chas;
                if (rez_cc < 0)//ЕСЛИ время до начала записи вышло
                {
                    ost_ch[i] = "";
                    ost_min[i] = "Время закончилось";
                }
                else
                {
                    if (min > 0 && min <= 30)//Если время в первой половине часа
                    {
                        rez_min = min - min_now;
                        if (rez_min < 0)
                        {
                            rez_cc--;
                            rez_min += 60;
                        }

                        ost_min[i] = rez_min + " мин";
                    }

                    else
                    {
                        if (min == 0)//Если время записи 00
                        {
                            min = 60;
                        }
                        rez_min = min_now - min;
                        if (rez_min < 0)
                        {
                            rez_min += 60;

                        }
                        ost_min[i] = rez_min + " мин";

                    }
                    
                        
                    ost_ch[i] = rez_cc + " ч";
                }
                        
                //Запись в лист
                list = new ListViewItem(new string[] { name_ser=inf.Tables[0].Rows[i]["Title"].ToString(),
                    name_cl=inf.Tables[0].Rows[i]["FirstName"].ToString()+" "+ inf.Tables[0].Rows[i]["LastName"].ToString(),
                    email=inf.Tables[0].Rows[i]["Email"].ToString(),
                    phone=inf.Tables[0].Rows[i]["Phone"].ToString(),
                    date.ToShortDateString(),
                    time, ost_ch[i]+" "+ost_min[i] });
                //Красный текст для окончания времени 
                if (rez_cc < 1 || (rez_cc == 1) && (rez_min == 0))
                {
                    list.SubItems[6].ForeColor = Color.Red;//стиль текста 
                    list.UseItemStyleForSubItems = false;//применение стиля отдельно для каждой ячейки 
                }
                list.ImageIndex = i;
                listView1.Items.Add(list);

            }

        }

        //Кнопка все записи 
        private void button1_Click(object sender, EventArgs e)
        {
            z= z.Replace("where StartTime>@today", " ");
            UsingZ();
            
        }
        //Кнопка ближайшие записи 
        private void button2_Click(object sender, EventArgs e)
        {
            z = z.Replace(" ", "where StartTime>@today");
            ShowInf();
            
        }

        //Кнопка назад
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            AdminStart start = new AdminStart();
            start.Show();
            this.Hide();
        }

    }
}
