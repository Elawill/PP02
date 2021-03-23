using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PP02
{
    public partial class Start : Form
    {
        public Start()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Service service = new Service();
            service.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = Microsoft.VisualBasic.Interaction.InputBox("Введите код:");
            if (result == "000")
            {
                AdminStart start = new AdminStart();
                start.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Данные не верны!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
