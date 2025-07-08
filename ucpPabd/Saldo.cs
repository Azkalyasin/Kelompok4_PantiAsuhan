using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ucpPabd
{
    public partial class Saldo : Form
    {
        string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Saldo()
        {
            InitializeComponent();
            this.Load += Saldo_Load;
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
                    using (SqlCommand cmd = new SqlCommand("sp_GetSaldoData", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridViewSaldo.DataSource = dt;

                        FormatDataGridView();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data saldo:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dataGridViewSaldo.Columns.Contains("saldo_id"))
                dataGridViewSaldo.Columns["saldo_id"].HeaderText = "ID Saldo";

            if (dataGridViewSaldo.Columns.Contains("total_saldo"))
            {
                dataGridViewSaldo.Columns["total_saldo"].HeaderText = "Total Saldo";
                dataGridViewSaldo.Columns["total_saldo"].DefaultCellStyle.Format = "C0";
                dataGridViewSaldo.Columns["total_saldo"].DefaultCellStyle.FormatProvider = new CultureInfo("id-ID");
                dataGridViewSaldo.Columns["total_saldo"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridViewSaldo.Columns.Contains("tanggal_update"))
            {
                dataGridViewSaldo.Columns["tanggal_update"].HeaderText = "Tanggal Update";

                // Format: 29/05/2025 14:35
                dataGridViewSaldo.Columns["tanggal_update"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dataGridViewSaldo.Columns["tanggal_update"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridViewSaldo.AutoResizeColumns();
            dataGridViewSaldo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewSaldo.MultiSelect = false;
            dataGridViewSaldo.ReadOnly = true;
        }
        private void dataGridViewSaldo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewSaldo_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}