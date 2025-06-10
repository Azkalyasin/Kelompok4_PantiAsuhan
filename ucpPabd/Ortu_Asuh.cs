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

        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Ortu_Asuh()
        {
            InitializeComponent();
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
            if (string.IsNullOrWhiteSpace(txtNamaOrtu.Text) ||
    string.IsNullOrWhiteSpace(txtTelepon.Text) ||
    string.IsNullOrWhiteSpace(txtAlamat.Text) ||
    string.IsNullOrWhiteSpace(comboPekerjaan.Text) ||
    string.IsNullOrWhiteSpace(comboStatus.Text))
            {
                MessageBox.Show("Semua field harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtNamaOrtu.Text, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Nama anak tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var konfirmasi = MessageBox.Show(
                $"Apakah Anda yakin ingin menyimpan data berikut?\n\n" +
                $"Nama: {txtNamaOrtu.Text}\n" +
                $"Telepon: {txtTelepon.Text}\n" +
                $"Alamat: {txtAlamat.Text}\n" +
                $"Pekerjaan: {comboPekerjaan.Text}\n" +
                $"Status: {comboStatus.Text}",
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
                using (SqlCommand cmd = new SqlCommand("sp_TambahOrtu", con, transaction))
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parameter sesuai stored procedure
                        cmd.Parameters.AddWithValue("@nama", txtNamaOrtu.Text);
                        cmd.Parameters.AddWithValue("@telepon", txtTelepon.Text);
                        cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@pekerjaan", comboPekerjaan.Text);
                        cmd.Parameters.AddWithValue("@status", comboStatus.Text);


                        cmd.ExecuteNonQuery();
                        transaction.Commit();

                        MessageBox.Show("Data berhasil ditambahkan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        cache.Remove(cacheKey);
                        ClearForm();
                        LoadData(); // pastikan LoadData() sudah pakai procedure atau query yang tepat
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Gagal menambahkan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrtu.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewOrtu.CurrentRow.Cells["orang_tua_id"].Value);

                if (string.IsNullOrWhiteSpace(txtNamaOrtu.Text) ||
                    string.IsNullOrWhiteSpace(txtTelepon.Text) ||
                    string.IsNullOrWhiteSpace(txtAlamat.Text) ||
                    string.IsNullOrWhiteSpace(comboPekerjaan.Text) ||
                    string.IsNullOrWhiteSpace(comboStatus.Text))
                {
                    MessageBox.Show("Semua field harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var konfirmasi = MessageBox.Show(
                    $"Apakah Anda yakin ingin mengupdate data ini dengan informasi berikut?\n\n" +
                    $"Nama: {txtNamaOrtu.Text}\n" +
                    $"Telepon: {txtTelepon.Text}\n" +
                    $"Alamat: {txtAlamat.Text}\n" +
                    $"Pekerjaan: {comboPekerjaan.Text}\n" +
                    $"Status: {comboStatus.Text}",
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

                        using (SqlCommand cmd = new SqlCommand("sp_UpdateOrtu", con, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@orang_tua_id", id);
                            cmd.Parameters.AddWithValue("@nama", txtNamaOrtu.Text);
                            cmd.Parameters.AddWithValue("@telepon", txtTelepon.Text);
                            cmd.Parameters.AddWithValue("@alamat", txtAlamat.Text);
                            cmd.Parameters.AddWithValue("@pekerjaan", comboPekerjaan.Text);
                            cmd.Parameters.AddWithValue("@status", comboStatus.Text);

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

    }
}
   


