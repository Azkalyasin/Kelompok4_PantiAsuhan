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
    public partial class Anak_Asuh: Form
    {

        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly string _cacheKey = "AnakAsuhData";
        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
        };
        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";

        public Anak_Asuh()
        {
            InitializeComponent();
            EnsureIndexes();
            LoadData();
            comboJenisKelamin.Items.AddRange(new string[] { "L", "P"});
            dataGridViewAnak.CellClick += DataGridViewAnak_CellClick;
            comboStatusPendidkan.Items.AddRange(new string[] { "SD", "SMP", "SMA", "Kuliah" });
            dateTimeTanggalLahir.MaxDate = DateTime.Today;
            dateTime.MinDate = DateTime.Today.AddDays(-7);
            dateTime.MaxDate = DateTime.Today;

        }

        private void btnTambahData_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text.Trim();
            string tempat = txtTempat.Text.Trim();
            string tanggalMasuk = dateTime.Value.ToString("yyyy-MM-dd");
            string tanggalLahir = dateTimeTanggalLahir.Value.ToString("yyyy-MM-dd");
            string tempatTanggalLahir = $"{tempat}, {Convert.ToDateTime(tanggalLahir):dd MMMM yyyy}";
            string jenisKelamin = comboJenisKelamin.Text;
            string statusPendidkan = comboStatusPendidkan.Text;

            DateTime tanggalLahirDate = dateTimeTanggalLahir.Value;
            if (tanggalLahirDate > DateTime.Today)
            {
                MessageBox.Show("Tanggal lahir tidak boleh di masa depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // Validasi input
            if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(tempat) ||
                string.IsNullOrWhiteSpace(jenisKelamin) || string.IsNullOrWhiteSpace(statusPendidkan))
            {
                MessageBox.Show("Semua field harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateTime.Value.Date < DateTime.Today.AddDays(-7) || dateTime.Value.Date > DateTime.Today)
            {
                MessageBox.Show("Tanggal masuk hanya diperbolehkan dari 7 hari yang lalu hingga hari ini.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (!System.Text.RegularExpressions.Regex.IsMatch(nama, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Nama anak tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(tempat, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Tempat lahir tidak boleh mengandung angka atau karakter spesial.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            
            var konfirmasi = MessageBox.Show(
                $"Apakah Anda yakin ingin menyimpan data berikut?\n\n" +
                $"Nama: {nama}\n" +
                $"Tempat & Tanggal Lahir: {tempatTanggalLahir}\n" +
                $"Jenis Kelamin: {jenisKelamin}\n" +
                $"Tanggal Masuk: {Convert.ToDateTime(tanggalMasuk):dd MMMM yyyy}\n" +
                $"Status Pendidikan: {statusPendidkan}",
                "Konfirmasi Simpan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (konfirmasi == DialogResult.No)
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    try
                    {
                     
                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Anak_Asuh WHERE nama = @nama AND tempat_tanggal_lahir = @ttl", con, transaction);
                        checkCmd.Parameters.AddWithValue("@nama", nama);
                        checkCmd.Parameters.AddWithValue("@ttl", tempatTanggalLahir);
                        int existing = (int)checkCmd.ExecuteScalar();

                        if (existing > 0)
                        {
                            MessageBox.Show("Data anak asuh dengan nama dan tempat & tanggal lahir yang sama sudah ada.", "Data Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }

                 
                        SqlCommand cmd = new SqlCommand("sp_TambahAnakAsuh", con, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nama", nama);
                        cmd.Parameters.AddWithValue("@ttl", tempatTanggalLahir);
                        cmd.Parameters.AddWithValue("@jk", jenisKelamin);
                        cmd.Parameters.AddWithValue("@tanggal_masuk", tanggalMasuk);
                        cmd.Parameters.AddWithValue("@status_pendidikan", statusPendidkan);

                        int result = cmd.ExecuteNonQuery();
                        transaction.Commit();

                        if (result > 0)
                        {
                            _cache.Remove(_cacheKey);
                            MessageBox.Show("Data berhasil ditambahkan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Data gagal ditambahkan.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (SqlException ex)
                    {
                        transaction.Rollback();

                        if (ex.Number == 2627 || ex.Number == 2601) // pelanggaran UNIQUE constraint
                        {
                            MessageBox.Show("Data anak asuh dengan nama dan tempat & tanggal lahir yang sama sudah ada.", "Data Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Terjadi kesalahan saat menyimpan data.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void ClearForm()
        {
            txtNama.Clear();
            comboJenisKelamin.SelectedIndex = -1;
            comboStatusPendidkan.SelectedIndex = -1;
            txtTempat.Clear();
            dateTime.Value = dateTime.MaxDate;
            dateTimeTanggalLahir.Value = DateTime.Today;
        }

        private void LoadData()
        {
            try
            {
                DataTable cachedData = _cache.Get(_cacheKey) as DataTable;

                if (cachedData != null)
                {
                    dataGridViewAnak.DataSource = cachedData;
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetAllAnakAsuh", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Simpan ke cache
                        _cache.Set(_cacheKey, dt, _cachePolicy);

                        dataGridViewAnak.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private void DataGridViewAnak_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewAnak.Rows[e.RowIndex];
                txtNama.Text = row.Cells["Nama"].Value.ToString();
                comboJenisKelamin.Text = row.Cells["Jenis_Kelamin"].Value.ToString();
                comboStatusPendidkan.Text = row.Cells["Status_Pendidikan"].Value.ToString();

                string tempatTanggalLahir = row.Cells["Tempat_Tanggal_Lahir"].Value.ToString();
                string[] parts = tempatTanggalLahir.Split(new string[] { ", " }, StringSplitOptions.None);

                if (parts.Length == 2)
                {
                    txtTempat.Text = parts[0];

                    DateTime parsedTanggalLahir;
                    if (DateTime.TryParse(parts[1], out parsedTanggalLahir))
                    {
                        if (parsedTanggalLahir <= dateTimeTanggalLahir.MaxDate && parsedTanggalLahir >= dateTimeTanggalLahir.MinDate)
                        {
                            dateTimeTanggalLahir.Value = parsedTanggalLahir;
                        }
                        else
                        {
                            dateTimeTanggalLahir.Value = DateTime.Today; // fallback aman
                        }
                    }
                    else
                    {
                        dateTimeTanggalLahir.Value = DateTime.Today; // fallback kalau gagal parse
                    }
                }
                else
                {
                    txtTempat.Text = tempatTanggalLahir;
                    dateTimeTanggalLahir.Value = DateTime.Today;
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewAnak.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewAnak.CurrentRow.Cells["anak_id"].Value);
                string nama = txtNama.Text.Trim();
                string jenisKelamin = comboJenisKelamin.Text;
                string statusPendidikan = comboStatusPendidkan.Text;
                string tanggalMasuk = dateTime.Value.ToString("yyyy-MM-dd");
                string tempat = txtTempat.Text.Trim();
                DateTime tanggalLahir = dateTimeTanggalLahir.Value;
                string tempatTanggalLahir = $"{tempat}, {tanggalLahir:dd MMMM yyyy}";

                if(tanggalLahir > DateTime.Today)
                {
                    MessageBox.Show("Tanggal lahir tidak boleh di masa depan.", "Validasi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dateTime.Value.Date < DateTime.Today.AddDays(-7) || dateTime.Value.Date > DateTime.Today)
                {
                    MessageBox.Show("Tanggal masuk hanya diperbolehkan dari 7 hari yang lalu hingga hari ini.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                if (!System.Text.RegularExpressions.Regex.IsMatch(nama, @"^[a-zA-Z\s]+$"))
                {
                    MessageBox.Show("Nama anak tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(tempat, @"^[a-zA-Z\s]+$"))
                {
                    MessageBox.Show("Tempat lahir tidak boleh mengandung angka atau karakter spesial.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                var confirm = MessageBox.Show("Apakah Anda yakin ingin mengupdate data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    try
                    {
                        
                        SqlCommand cek = new SqlCommand("SELECT COUNT(*) FROM Anak_Asuh WHERE nama = @nama AND tempat_tanggal_lahir = @ttl AND anak_id <> @id", con, transaction);
                        cek.Parameters.AddWithValue("@nama", nama);
                        cek.Parameters.AddWithValue("@ttl", tempatTanggalLahir);
                        cek.Parameters.AddWithValue("@id", id);
                        int exists = (int)cek.ExecuteScalar();

                        if (exists > 0)
                        {
                            MessageBox.Show("Data dengan nama dan tempat/tanggal lahir yang sama sudah ada.", "Data Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }

                      
                        SqlCommand cmd = new SqlCommand("sp_UpdateAnakAsuh", con, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@anak_id", id);
                        cmd.Parameters.AddWithValue("@nama", nama);
                        cmd.Parameters.AddWithValue("@jk", jenisKelamin);
                        cmd.Parameters.AddWithValue("@tanggal_masuk", tanggalMasuk);
                        cmd.Parameters.AddWithValue("@ttl", tempatTanggalLahir);
                        cmd.Parameters.AddWithValue("@status_pendidikan", statusPendidikan);

                        int result = cmd.ExecuteNonQuery();
                        transaction.Commit();

                        if (result > 0)
                        {
                            _cache.Remove(_cacheKey);
                            MessageBox.Show("Data berhasil diperbarui.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Update gagal.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        MessageBox.Show("Terjadi kesalahan saat menyimpan data. Silakan coba lagi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Silakan pilih baris data yang ingin diperbarui terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewAnak.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewAnak.CurrentRow.Cells["anak_id"].Value);

                var confirm = MessageBox.Show("Apakah Anda yakin ingin menghapus data ini?", "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();
                    try
                    {
                        SqlCommand cmd = new SqlCommand("sp_DeleteAnakAsuh", con, transaction);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@anak_id", id);
                        int result = cmd.ExecuteNonQuery();
                        transaction.Commit();
                        if (result > 0)
                        {
                            _cache.Remove(_cacheKey);

                            MessageBox.Show("Data berhasil dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Hapus gagal.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        MessageBox.Show("Terjadi kesalahan saat menghapus data. Silakan coba lagi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Silakan pilih baris data yang ingin dihapus terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Anak_Asuh_Load(object sender, EventArgs e)
        {

        }

        private void txtTempatTglLahir_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtNama_TextChanged(object sender, EventArgs e)
        {

        }

        private void EnsureIndexes()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string indexScript = @"
            IF OBJECT_ID('dbo.Anak_Asuh', 'U') IS NOT NULL
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_AnakAsuh_Nama' AND object_id = OBJECT_ID('dbo.Anak_Asuh'))
                    CREATE NONCLUSTERED INDEX idx_AnakAsuh_Nama ON dbo.Anak_Asuh(nama);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_AnakAsuh_JenisKelamin' AND object_id = OBJECT_ID('dbo.Anak_Asuh'))
                    CREATE NONCLUSTERED INDEX idx_AnakAsuh_JenisKelamin ON dbo.Anak_Asuh(jenis_kelamin);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_AnakAsuh_TanggalMasuk' AND object_id = OBJECT_ID('dbo.Anak_Asuh'))
                    CREATE NONCLUSTERED INDEX idx_AnakAsuh_TanggalMasuk ON dbo.Anak_Asuh(tanggal_masuk);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_AnakAsuh_StatusPendidikan' AND object_id = OBJECT_ID('dbo.Anak_Asuh'))
                    CREATE NONCLUSTERED INDEX idx_AnakAsuh_StatusPendidikan ON dbo.Anak_Asuh(status_pendidikan);
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
    }
}
