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
using System.Runtime.Caching;


namespace ucpPabd
{
    public partial class Pemasukan: Form
    {
        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly string _cacheKey = "PemasukanData";
        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
        };

        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Pemasukan()
        {
            InitializeComponent();
            EnsureIndexesPemasukan();
            comboPemasukan.Items.AddRange(new string[] { "Donasi", "Bantuan Pemerintah", "Sponsor", "Lainnya" });
            LoadData();
            dataGridViewPemasukan.CellClick += DataGridViewPemasukan_CellClick;
        }

        private void LoadData()
        {
            try
            {
                if (_cache.Contains(_cacheKey))
                {
                    dataGridViewPemasukan.DataSource = _cache.Get(_cacheKey) as DataTable;
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ReadPemasukan", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewPemasukan.DataSource = dt;

                    // Simpan ke cache
                    _cache.Set(_cacheKey, dt, _cachePolicy);
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
            if (string.IsNullOrWhiteSpace(comboPemasukan.Text) ||
                !decimal.TryParse(txtJumlah.Text, out decimal jumlah))
            {
                MessageBox.Show("Kategori harus dipilih dan jumlah harus berupa angka desimal.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (jumlah <= 0)
            {
                MessageBox.Show("Jumlah pemasukan harus lebih dari 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime tanggal = dateTime.Value;

            if (tanggal > DateTime.Now)
            {
                MessageBox.Show("Tanggal pemasukan tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string kategori = comboPemasukan.Text;

            var confirm = MessageBox.Show(
                $"Apakah Anda yakin ingin menambahkan data pemasukan dengan keterangan :\n\nKategori: {kategori}\nJumlah: {jumlah}\nTanggal: {tanggal.ToShortDateString()}",
                "Konfirmasi Tambah Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.No)
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();


                try
                {
                    SqlCommand cmd = new SqlCommand("sp_TambahPemasukan", con, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                    cmd.Parameters.AddWithValue("@jumlah", jumlah);
                    cmd.Parameters.AddWithValue("@tanggal", tanggal);
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    MessageBox.Show("Data pemasukan berhasil ditambahkan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    _cache.Remove(_cacheKey);
                    LoadData();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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

                if (jumlah <= 0)
                {
                    MessageBox.Show("Jumlah pemasukan harus lebih dari 0.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime tanggal = dateTime.Value;

                if (tanggal > DateTime.Now)
                {
                    MessageBox.Show("Tanggal pemasukan tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string kategori = comboPemasukan.Text;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    try
                    {
                        
                        SqlCommand cmd = new SqlCommand("sp_UpdatePemasukan", con, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@pemasukan_id", id);
                        cmd.Parameters.AddWithValue("@kategori", kategori);
                        cmd.Parameters.AddWithValue("@jumlah", jumlah);
                        cmd.Parameters.AddWithValue("@tanggal", tanggal);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        MessageBox.Show("Data pemasukan berhasil diperbarui", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        _cache.Remove(_cacheKey);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Gagal memperbarui data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih baris data yang ingin diperbarui.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        con.Open();
                        SqlTransaction transaction = con.BeginTransaction();

                        try
                        {
                            SqlCommand cmd = new SqlCommand("sp_DeletePemasukan", con, transaction);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@pemasukan_id", id);
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            MessageBox.Show("Data pemasukan berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            _cache.Remove(_cacheKey);
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
     
        }
        private void EnsureIndexesPemasukan()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string indexScript = @"
        IF OBJECT_ID('dbo.Pemasukan', 'U') IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_Pemasukan_Kategori' AND object_id = OBJECT_ID('dbo.Pemasukan'))
                CREATE NONCLUSTERED INDEX idx_Pemasukan_Kategori ON dbo.Pemasukan(kategori);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_Pemasukan_Tanggal' AND object_id = OBJECT_ID('dbo.Pemasukan'))
                CREATE NONCLUSTERED INDEX idx_Pemasukan_Tanggal ON dbo.Pemasukan(tanggal);
        END";

                using (var cmd = new SqlCommand(indexScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
