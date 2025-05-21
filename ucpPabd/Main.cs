using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ucpPabd
{
    public partial class Main: Form
    {

        public Main()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAnak_Click(object sender, EventArgs e)
        {
            Anak_Asuh aa = new Anak_Asuh();
            aa.Show();
        }

        private void btnOrtu_Click(object sender, EventArgs e)
        { 
            Ortu_Asuh ortu_Asuh = new Ortu_Asuh();
            ortu_Asuh.Show();
        }

        private void btnAdopsi_Click(object sender, EventArgs e){
            Adopsi ad = new Adopsi();
            ad.Show();
        
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Pemasukan pm = new Pemasukan();
            pm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Pengeluaran pg = new Pengeluaran();
            pg.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Form1 loginForm = new Form1();
            loginForm.Show();

            this.Close();
        }
    }
}
