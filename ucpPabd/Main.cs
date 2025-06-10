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


        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
    "Apakah Anda yakin ingin keluar?",
    "Konfirmasi Logout",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Form1 loginForm = new Form1();
                loginForm.Show();
                this.Close();
            }
        }



        private void btnPemasukan_Click(object sender, EventArgs e)
        {
            Pemasukan pe = new Pemasukan();
            pe.Show();
        }

        private void btnPengeluaran_Click(object sender, EventArgs e)
        {
            Pengeluaran pl = new Pengeluaran();
            pl.Show();
        }

        private void btnSaldo_Click(object sender, EventArgs e)
        {
            Saldo s = new Saldo();
            s.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            reportpengeluaran rpl = new reportpengeluaran();
            rpl.Show();
        }

        private void buttonlaporanpemasukan_Click(object sender, EventArgs e)
        {
            reportpemasukan rpm = new reportpemasukan();
            rpm.Show();
        }

        private void laporansaldo_Click(object sender, EventArgs e)
        {
            reportsaldo rs = new reportsaldo();
            rs.Show();  
        }
    }
}
