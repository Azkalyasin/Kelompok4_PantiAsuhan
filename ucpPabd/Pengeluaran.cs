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
                    SqlCommand cmd = new SqlCommand("sp_ReadPengeluaran", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
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

        private decimal GetSaldoSaatIni()
        {
            decimal saldo = 0;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT TOP 1 total_saldo FROM Saldo ORDER BY saldo_id DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    saldo = Convert.ToDecimal(result);
                }
            }
            return saldo;
        }


        private void btnTambah_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(comboPengeluaran.Text) || !decimal.TryParse(txtJumlah.Text, out decimal jumlah))
            {
                MessageBox.Show("Kategori harus dipilih dan jumlah harus berupa angka desimal.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (jumlah <= 0)
            {
                MessageBox.Show("Jumlah pengeluaran harus lebih dari 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime tanggal = dateTime.Value;
            if (tanggal.Date > DateTime.Today)
            {
                MessageBox.Show("Tanggal pengeluaran tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal saldoSaatIni = GetSaldoSaatIni();
            if (saldoSaatIni == 0)
            {
                MessageBox.Show("Saldo saat ini 0. Tidak bisa melakukan pengeluaran.", "Saldo Kosong", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (jumlah > saldoSaatIni)
            {
                MessageBox.Show($"Jumlah pengeluaran lebih besar dari saldo saat ini ({saldoSaatIni:C}).", "Saldo Tidak Cukup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string kategori = comboPengeluaran.Text;

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
                return;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("sp_TambahPengeluaran", con, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                    cmd.Parameters.AddWithValue("@jumlah", jumlah);
                    cmd.Parameters.AddWithValue("@tanggal", tanggal);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    MessageBox.Show("Data pengeluaran berhasil ditambahkan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadData();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Gagal menambahkan data: " + ex.Message);
                }
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewPengeluaran.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewPengeluaran.CurrentRow.Cells["pengeluaran_id"].Value);

                if (string.IsNullOrWhiteSpace(comboPengeluaran.Text) || !decimal.TryParse(txtJumlah.Text, out decimal jumlah))
                {
                    MessageBox.Show("Kategori dan jumlah harus valid.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (jumlah <= 0)
                {
                    MessageBox.Show("Jumlah harus lebih dari 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime tanggal = dateTime.Value;
                if (tanggal.Date > DateTime.Today)
                {
                    MessageBox.Show("Tanggal tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string kategori = comboPengeluaran.Text;

                var confirm = MessageBox.Show(
                    $"Yakin ingin mengupdate data berikut?\nKategori: {kategori}\nJumlah: {jumlah:C}\nTanggal: {tanggal:dd MMM yyyy}",
                    "Konfirmasi Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                if (confirm == DialogResult.No)
                    return;

                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        SqlCommand cmd = new SqlCommand("sp_UpdatePengeluaran", con, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pengeluaran_id", id);
                        cmd.Parameters.AddWithValue("@kategori", kategori);
                        cmd.Parameters.AddWithValue("@jumlah", jumlah);
                        cmd.Parameters.AddWithValue("@tanggal", tanggal);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        MessageBox.Show("Data berhasil diperbarui", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Gagal update: " + ex.Message);
                    }
                }
            }

        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridViewPengeluaran.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewPengeluaran.CurrentRow.Cells["pengeluaran_id"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlTransaction transaction = con.BeginTransaction();


                        try
                        {
                            SqlCommand cmd = new SqlCommand("sp_DeletePengeluaran", con, transaction);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@pengeluaran_id", id);
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Gagal hapus: " + ex.Message);
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
