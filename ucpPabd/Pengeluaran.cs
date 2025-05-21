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
    public partial class Pengeluaran: Form
    {
        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Pengeluaran()
        {
            InitializeComponent();
            comboPengeluaran.Items.AddRange(new string[] { "Makanan", "Pendidikan", "Kesehatan", "Operasional", "Lainnya" });
            LoadData();
            dataGridViewPengeluaran.CellClick += DataGridViewPengeluaran_CellClick;
        }


        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Pengeluaran";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewPengeluaran.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtJumlah.Clear();
            dateTime.Value = DateTime.Now;
            comboPengeluaran.SelectedIndex = -1;
        }

        private void DataGridViewPengeluaran_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewPengeluaran.Rows[e.RowIndex];

                comboPengeluaran.Text = row.Cells["kategori"].Value.ToString();
                txtJumlah.Text = row.Cells["jumlah"].Value.ToString();

                if (row.Cells["tanggal"].Value != DBNull.Value)
                {
                    dateTime.Value = Convert.ToDateTime(row.Cells["tanggal"].Value);
                }
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboPengeluaran.Text) || !decimal.TryParse(txtJumlah.Text, out decimal jumlah))
            {
                MessageBox.Show("Kategori harus dipilih dan jumlah harus berupa angka desimal.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi jumlah harus lebih dari 0
            if (jumlah <= 0)
            {
                MessageBox.Show("Jumlah pengeluaran harus lebih dari 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime tanggal = dateTime.Value;

            // Validasi tanggal tidak boleh di masa depan
            if (tanggal.Date > DateTime.Today)
            {
                MessageBox.Show("Tanggal pengeluaran tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string kategori = comboPengeluaran.Text;

            // Konfirmasi sebelum simpan
            var konfirmasi = MessageBox.Show(
                $"Apakah Anda yakin ingin menyimpan data pengeluaran berikut?\n\n" +
                $"Kategori : {kategori}\n" +
                $"Jumlah   : {jumlah:C}\n" +
                $"Tanggal  : {tanggal:dd MMMM yyyy}",
                "Konfirmasi Simpan Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (konfirmasi == DialogResult.No)
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Pengeluaran (kategori, jumlah, tanggal) VALUES (@kategori, @jumlah, @tanggal)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@kategori", kategori);
                cmd.Parameters.AddWithValue("@jumlah", jumlah);
                cmd.Parameters.AddWithValue("@tanggal", tanggal);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Data pengeluaran berhasil ditambahkan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (dataGridViewPengeluaran.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewPengeluaran.CurrentRow.Cells["pengeluaran_id"].Value);

                if (string.IsNullOrWhiteSpace(comboPengeluaran.Text) ||
                    !decimal.TryParse(txtJumlah.Text, out decimal jumlah))
                {
                    MessageBox.Show("Kategori harus dipilih dan jumlah harus berupa angka desimal.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validasi jumlah harus lebih dari 0
                if (jumlah <= 0)
                {
                    MessageBox.Show("Jumlah pengeluaran harus lebih dari 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime tanggal = dateTime.Value;

                // Validasi tanggal tidak boleh di masa depan
                if (tanggal.Date > DateTime.Today)
                {
                    MessageBox.Show("Tanggal pengeluaran tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string kategori = comboPengeluaran.Text;

                // Konfirmasi sebelum update
                var konfirmasi = MessageBox.Show(
                    $"Apakah Anda yakin ingin memperbarui data pengeluaran berikut?\n\n" +
                    $"Kategori : {kategori}\n" +
                    $"Jumlah   : {jumlah:C}\n" +
                    $"Tanggal  : {tanggal:dd MMMM yyyy}",
                    "Konfirmasi Update Data",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (konfirmasi == DialogResult.No)
                {
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Pengeluaran SET kategori=@kategori, jumlah=@jumlah, tanggal=@tanggal WHERE pengeluaran_id=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                    cmd.Parameters.AddWithValue("@jumlah", jumlah);
                    cmd.Parameters.AddWithValue("@tanggal", tanggal);
                    cmd.Parameters.AddWithValue("@id", id);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Data pengeluaran berhasil diperbarui", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (dataGridViewPengeluaran.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewPengeluaran.CurrentRow.Cells["pengeluaran_id"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data pemasukan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM pengeluaran WHERE pengeluaran_id = @id";
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

        private void txtJumlah_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
