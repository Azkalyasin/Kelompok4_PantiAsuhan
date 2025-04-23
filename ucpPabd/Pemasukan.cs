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
    public partial class Pemasukan: Form
    {
        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Pemasukan()
        {
            InitializeComponent();
            comboPemasukan.Items.AddRange(new string[] { "Donasi", "Bantuan Pemerintah", "Sponsor", "Lainnya" });
            LoadData();
            dataGridViewPemasukan.CellClick += DataGridViewPemasukan_CellClick;
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Pemasukan";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewPemasukan.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        private void DataGridViewPemasukan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewPemasukan.Rows[e.RowIndex];

                comboPemasukan.Text = row.Cells["kategori"].Value.ToString();
                txtJumlah.Text = row.Cells["jumlah"].Value.ToString();

                if (row.Cells["tanggal"].Value != DBNull.Value)
                {
                    dateTime.Value = Convert.ToDateTime(row.Cells["tanggal"].Value);
                }
            }
        }


        private void ClearForm()
        {
            txtJumlah.Clear();
            dateTime.Value = DateTime.Now;
            comboPemasukan.SelectedIndex = -1;
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            // Validasi input kategori dan jumlah
            if (string.IsNullOrWhiteSpace(comboPemasukan.Text) ||
                !decimal.TryParse(txtJumlah.Text, out decimal jumlah))
            {
                MessageBox.Show("Kategori harus dipilih dan jumlah harus berupa angka desimal.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime tanggal = dateTime.Value;
            string kategori = comboPemasukan.Text;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Pemasukan (kategori, jumlah, tanggal) " +
                               "VALUES (@kategori, @jumlah, @tanggal)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@kategori", kategori);
                cmd.Parameters.AddWithValue("@jumlah", jumlah);
                cmd.Parameters.AddWithValue("@tanggal", tanggal);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data pemasukan berhasil ditambahkan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menambahkan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewPemasukan.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewPemasukan.CurrentRow.Cells["pemasukan_id"].Value);

                if (string.IsNullOrWhiteSpace(comboPemasukan.Text) ||
                    !decimal.TryParse(txtJumlah.Text, out decimal jumlah))
                {
                    MessageBox.Show("Kategori harus dipilih dan jumlah harus berupa angka desimal.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime tanggal = dateTime.Value;
                string kategori = comboPemasukan.Text;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Pemasukan SET kategori=@kategori, jumlah=@jumlah, tanggal=@tanggal WHERE pemasukan_id=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                    cmd.Parameters.AddWithValue("@jumlah", jumlah);
                    cmd.Parameters.AddWithValue("@tanggal", tanggal);
                    cmd.Parameters.AddWithValue("@id", id);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Data pemasukan berhasil diperbarui", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal memperbarui data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewPemasukan.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewPemasukan.CurrentRow.Cells["pemasukan_id"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data pemasukan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Pemasukan WHERE pemasukan_id = @id";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);

                        try
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Data pemasukan berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

    }
}
