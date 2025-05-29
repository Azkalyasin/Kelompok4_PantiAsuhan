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

namespace ucpPabd
{
    public partial class Saldo: Form
    {
        string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Saldo()
        {
            InitializeComponent();
        }

        private void Saldo_Load(object sender, EventArgs e)
        {
            LoadSaldoData();
        }

        private void LoadSaldoData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT saldo_id, total_saldo, tanggal_update FROM Saldo ORDER BY tanggal_update DESC";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dataGridViewSaldo.DataSource = dt;

                    // Opsional: Format tampilan DataGridView
                    dataGridViewSaldo.Columns["saldo_id"].HeaderText = "ID Saldo";
                    dataGridViewSaldo.Columns["total_saldo"].HeaderText = "Total Saldo";
                    dataGridViewSaldo.Columns["tanggal_update"].HeaderText = "Tanggal Update";
                    dataGridViewSaldo.AutoResizeColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error load data saldo: " + ex.Message);
            }
        }

        private void dataGridViewSaldo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
