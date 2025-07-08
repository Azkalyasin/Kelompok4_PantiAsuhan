using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Caching;


namespace ucpPabd
{
    public partial class Pengeluaran: Form
    {
        private MemoryCache cache = MemoryCache.Default;
        private string cacheKey = "data_pengeluaran";
        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
        };

        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Pengeluaran()
        {
            InitializeComponent();
            EnsureIndexesPengeluaran();
            comboPengeluaran.Items.AddRange(new string[] { "Makanan", "Pendidikan", "Kesehatan", "Operasional", "Lainnya" });
            dateTime.MinDate = DateTime.Today.AddDays(-100); // 1 minggu lalu
            dateTime.MaxDate = DateTime.Today;
            LoadData();
            dataGridViewPengeluaran.CellClick += DataGridViewPengeluaran_CellClick;
        }


        private void LoadData()
        {
            try
            {
                DataTable dt = cache.Get(cacheKey) as DataTable;

                if (dt == null)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("sp_ReadPengeluaran", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        da.Fill(dt);

                        // Cache selama 5 menit
                        CacheItemPolicy policy = new CacheItemPolicy
                        {
                            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
                        };
                        cache.Set(cacheKey, dt, policy);
                    }
                }

                dataGridViewPengeluaran.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtJumlah.Clear();
            dateTime.Value = DateTime.Today;
            comboPengeluaran.SelectedIndex = -1;
        }

        private void ClearCache()
        {
            if (cache.Contains(cacheKey))
            {
                cache.Remove(cacheKey);
            }
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
                    DateTime tanggal = Convert.ToDateTime(row.Cells["tanggal"].Value);

                    if (tanggal < dateTime.MinDate)
                        dateTime.Value = dateTime.MinDate;
                    else if (tanggal > dateTime.MaxDate)
                        dateTime.Value = dateTime.MaxDate;
                    else
                        dateTime.Value = tanggal;
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
            if (tanggal < DateTime.Today.AddDays(-7) || tanggal > DateTime.Today)
            {
                MessageBox.Show("Tanggal hanya boleh dari 1 minggu yang lalu hingga hari ini.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    ClearCache();
                    MessageBox.Show("Data pengeluaran berhasil ditambahkan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadData();
                }
                catch (Exception ex)
                {
                    // Cek apakah transaksi masih bisa di-rollback
                    try
                    {
                        if (transaction.Connection != null)
                            transaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        MessageBox.Show("Gagal melakukan rollback: " + rollbackEx.Message, "Rollback Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    MessageBox.Show("Gagal menambahkan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (tanggal < DateTime.Today.AddDays(-7) || tanggal > DateTime.Today)
                {
                    MessageBox.Show("Tanggal hanya boleh dari 1 minggu yang lalu hingga hari ini.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        ClearCache();
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
                            ClearCache();
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

        private void EnsureIndexesPengeluaran()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string indexScript = @"
        IF OBJECT_ID('dbo.Pengeluaran', 'U') IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_Pengeluaran_Kategori' AND object_id = OBJECT_ID('dbo.Pengeluaran'))
                CREATE NONCLUSTERED INDEX idx_Pengeluaran_Kategori ON dbo.Pengeluaran(kategori);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_Pengeluaran_Tanggal' AND object_id = OBJECT_ID('dbo.Pengeluaran'))
                CREATE NONCLUSTERED INDEX idx_Pengeluaran_Tanggal ON dbo.Pengeluaran(tanggal);
        END";

                using (var cmd = new SqlCommand(indexScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
