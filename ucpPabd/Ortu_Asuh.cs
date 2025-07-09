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
    public partial class Ortu_Asuh: Form
    {
        private MemoryCache cache = MemoryCache.Default;
        private string cacheKey = "data_orangtua_asuh";
        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
        };

        Koneksi kn = new Koneksi();
        string connectionString = "";

        public Ortu_Asuh()
        {
            InitializeComponent();
            connectionString = kn.connectionString();
            EnsureIndexesOrangTuaAsuh(); 
            LoadData();
            comboPekerjaan.Items.AddRange(new string[] { "PNS", "Jasa Profesional", "Wirausahawan", "TNI/Polri", "Pegawai Swasta" });
            comboStatus.Items.AddRange(new string[] { "Menunggu", "Disetujui", "Ditolak" });

            dataGridViewOrtu.CellClick += DataGridViewOrtu_CellClick;
        }

        private void Ortu_Asuh_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void LoadData()
        {
            try
            {
                DataTable dt;

                // Cek apakah data sudah ada di cache
                if (cache.Contains(cacheKey))
                {
                    dt = (DataTable)cache.Get(cacheKey);
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_GetAllOrangTua", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            da.Fill(dt);

                            // Simpan ke cache selama 5 menit
                            CacheItemPolicy policy = new CacheItemPolicy
                            {
                                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
                            };
                            cache.Set(cacheKey, dt, policy);
                        }
                    }
                }

                dataGridViewOrtu.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtNamaOrtu.Clear();
            txtTelepon.Clear();
            txtAlamat.Clear();
            comboPekerjaan.SelectedIndex = -1;
            comboStatus.SelectedIndex = -1;
        }

        private void DataGridViewOrtu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewOrtu.Rows[e.RowIndex];
                txtNamaOrtu.Text = row.Cells["nama"].Value.ToString();
                txtTelepon.Text = row.Cells["telepon"].Value.ToString();
                txtAlamat.Text = row.Cells["alamat"].Value.ToString();
                comboPekerjaan.Text = row.Cells["pekerjaan"].Value.ToString();
                comboStatus.Text = row.Cells["status"].Value.ToString();
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            string nama = txtNamaOrtu.Text.Trim();
            string telepon = txtTelepon.Text.Trim();
            string alamat = txtAlamat.Text.Trim();
            string pekerjaan = comboPekerjaan.Text;
            string status = comboStatus.Text;

            if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(telepon) ||
                string.IsNullOrWhiteSpace(alamat) || string.IsNullOrWhiteSpace(pekerjaan) || string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show("Semua field harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(nama, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Nama tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(alamat, @"^[a-zA-Z0-9\s.,-]+$"))
            {
                MessageBox.Show("Alamat hanya boleh mengandung huruf, angka, spasi, koma, titik, atau strip.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            if (!System.Text.RegularExpressions.Regex.IsMatch(telepon, @"^\d{10,13}$"))
            {
                MessageBox.Show("Nomor telepon hanya boleh berisi angka (10-13 digit).", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            var konfirmasi = MessageBox.Show(
                $"Apakah Anda yakin ingin menyimpan data berikut?\n\n" +
                $"Nama: {nama}\n" +
                $"Telepon: {telepon}\n" +
                $"Alamat: {alamat}\n" +
                $"Pekerjaan: {pekerjaan}\n" +
                $"Status: {status}",
                "Konfirmasi Simpan",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (konfirmasi == DialogResult.No)
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {

                    using (SqlCommand cek = new SqlCommand("SELECT COUNT(*) FROM Orang_Tua_Asuh WHERE telepon = @telepon", con, transaction))
                    {
                        cek.Parameters.AddWithValue("@telepon", telepon);
                        int count = (int)cek.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Nomor telepon ini sudah terdaftar.", "Telepon Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            transaction.Rollback();
                            return;
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("sp_TambahOrtu", con, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@nama", nama);
                        cmd.Parameters.AddWithValue("@telepon", telepon);
                        cmd.Parameters.AddWithValue("@alamat", alamat);
                        cmd.Parameters.AddWithValue("@pekerjaan", pekerjaan);
                        cmd.Parameters.AddWithValue("@status", status);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();

                        MessageBox.Show("Data berhasil ditambahkan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cache.Remove(cacheKey);
                        ClearForm();
                        LoadData();
                    }
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
            if (dataGridViewOrtu.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewOrtu.CurrentRow.Cells["orang_tua_id"].Value);

                string nama = txtNamaOrtu.Text.Trim();
                string telepon = txtTelepon.Text.Trim();
                string alamat = txtAlamat.Text.Trim();
                string pekerjaan = comboPekerjaan.Text;
                string status = comboStatus.Text;

                if (string.IsNullOrWhiteSpace(nama) ||
                    string.IsNullOrWhiteSpace(telepon) ||
                    string.IsNullOrWhiteSpace(alamat) ||
                    string.IsNullOrWhiteSpace(pekerjaan) ||
                    string.IsNullOrWhiteSpace(status))
                {
                    MessageBox.Show("Semua field harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(nama, @"^[a-zA-Z\s]+$"))
                {
                    MessageBox.Show("Nama tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(alamat, @"^[a-zA-Z0-9\s.,-]+$"))
                {
                    MessageBox.Show("Alamat hanya boleh mengandung huruf, angka, spasi, koma, titik, atau strip.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }



                if (!System.Text.RegularExpressions.Regex.IsMatch(telepon, @"^\d{10,13}$"))
                {
                    MessageBox.Show("Nomor telepon hanya boleh berisi angka (10-13 digit).", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                var konfirmasi = MessageBox.Show(
                    $"Apakah Anda yakin ingin mengupdate data ini dengan informasi berikut?\n\n" +
                    $"Nama: {nama}\n" +
                    $"Telepon: {telepon}\n" +
                    $"Alamat: {alamat}\n" +
                    $"Pekerjaan: {pekerjaan}\n" +
                    $"Status: {status}",
                    "Konfirmasi Update",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (konfirmasi == DialogResult.No)
                {
                    return;
                }

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlTransaction transaction = con.BeginTransaction();

                 
                        using (SqlCommand cek = new SqlCommand("SELECT COUNT(*) FROM Orang_Tua_Asuh WHERE telepon = @telepon AND orang_tua_id != @id", con, transaction))
                        {
                            cek.Parameters.AddWithValue("@telepon", telepon);
                            cek.Parameters.AddWithValue("@id", id);
                            int count = (int)cek.ExecuteScalar();
                            if (count > 0)
                            {
                                MessageBox.Show("Nomor telepon ini sudah digunakan oleh entri lain.", "Telepon Duplikat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                transaction.Rollback();
                                return;
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("sp_UpdateOrtu", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@orang_tua_id", id);
                            cmd.Parameters.AddWithValue("@nama", nama);
                            cmd.Parameters.AddWithValue("@telepon", telepon);
                            cmd.Parameters.AddWithValue("@alamat", alamat);
                            cmd.Parameters.AddWithValue("@pekerjaan", pekerjaan);
                            cmd.Parameters.AddWithValue("@status", status);

                            cmd.ExecuteNonQuery();
                            transaction.Commit();

                            MessageBox.Show("Data berhasil diperbarui", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            cache.Remove(cacheKey);
                            ClearForm();
                            LoadData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal memperbarui data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrtu.CurrentRow == null || dataGridViewOrtu.CurrentRow.Cells["orang_tua_id"].Value == DBNull.Value)
            {
                MessageBox.Show("Silakan pilih data yang ingin dihapus.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dataGridViewOrtu.CurrentRow.Cells["orang_tua_id"].Value);
            string nama = dataGridViewOrtu.CurrentRow.Cells["nama"].Value?.ToString() ?? "(Tidak diketahui)";

            var konfirmasi = MessageBox.Show(
                $"Apakah Anda yakin ingin menghapus data orang tua:\n\nNama: {nama}\nID: {id}?",
                "Konfirmasi Hapus Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (konfirmasi == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlTransaction transaction = con.BeginTransaction();

                        using (SqlCommand cmd = new SqlCommand("sp_DeleteOrtu", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@orang_tua_id", id);

                            cmd.ExecuteNonQuery();
                            transaction.Commit();

                            MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            cache.Remove(cacheKey);
                            ClearForm();
                            LoadData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void EnsureIndexesOrangTuaAsuh()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string indexScript = @"
        IF OBJECT_ID('dbo.Orang_Tua_Asuh', 'U') IS NOT NULL
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_OrangTuaAsuh_Nama' AND object_id = OBJECT_ID('dbo.Orang_Tua_Asuh'))
                CREATE NONCLUSTERED INDEX idx_OrangTuaAsuh_Nama ON dbo.Orang_Tua_Asuh(nama);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_OrangTuaAsuh_Telepon' AND object_id = OBJECT_ID('dbo.Orang_Tua_Asuh'))
                CREATE NONCLUSTERED INDEX idx_OrangTuaAsuh_Telepon ON dbo.Orang_Tua_Asuh(telepon);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_OrangTuaAsuh_Pekerjaan' AND object_id = OBJECT_ID('dbo.Orang_Tua_Asuh'))
                CREATE NONCLUSTERED INDEX idx_OrangTuaAsuh_Pekerjaan ON dbo.Orang_Tua_Asuh(pekerjaan);

            IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='idx_OrangTuaAsuh_Status' AND object_id = OBJECT_ID('dbo.Orang_Tua_Asuh'))
                CREATE NONCLUSTERED INDEX idx_OrangTuaAsuh_Status ON dbo.Orang_Tua_Asuh(status);
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
   


