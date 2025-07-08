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
    public partial class Adopsi: Form
    {
        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly string _cacheKey = "AdopsiData";
        private readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
        };

        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Adopsi()
        {
            InitializeComponent();
            EnsureIndexesAdopsi();
            comboStatus.Items.AddRange(new string[] { "Proses", "Selesai", "Dibatalkan" });
            _cache.Remove(_cacheKey);
            dateTime.MinDate = DateTime.Today.AddDays(-100);
            dateTime.MaxDate = DateTime.Today;
            LoadComboBoxes();
            LoadData();
            dataGridViewAsuh.CellClick += DataGridViewAdopsi_CellClick;
        }

        private void LoadComboBoxes()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Isi nama anak
                SqlCommand cmdAnak = new SqlCommand("SELECT nama FROM Anak_Asuh", con);
                SqlDataReader rdAnak = cmdAnak.ExecuteReader();
                while (rdAnak.Read())
                {
                    comboNamaAnak.Items.Add(rdAnak.GetString(0));
                }
                rdAnak.Close();

                // Isi nama orang tua
                SqlCommand cmdOrtu = new SqlCommand("SELECT nama FROM Orang_Tua_Asuh", con);
                SqlDataReader rdOrtu = cmdOrtu.ExecuteReader();
                while (rdOrtu.Read())
                {
                    comboNamaOrangtua.Items.Add(rdOrtu.GetString(0));
                }
                rdOrtu.Close();
            }
        }


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            try
            {
                if (_cache.Contains(_cacheKey))
                {
                    // Ambil data dari cache
                    dataGridViewAsuh.DataSource = _cache.Get(_cacheKey) as DataTable;
                    return;
                }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("sp_ReadAdopsi", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewAsuh.DataSource = dt;

                    // Simpan ke cache
                    _cache.Set(_cacheKey, dt, _cachePolicy);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            comboNamaAnak.SelectedIndex = -1;
            comboNamaOrangtua.SelectedIndex = -1;
            dateTime.Value = DateTime.Today;
            comboStatus.SelectedIndex = -1;
        }


        private void DataGridViewAdopsi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewAsuh.Rows[e.RowIndex];

                object anakIdValue = row.Cells["anak_id"].Value;
                object ortuIdValue = row.Cells["orang_tua_id"].Value;

                if (anakIdValue == DBNull.Value || ortuIdValue == DBNull.Value)
                {
                    MessageBox.Show("Data anak atau orang tua tidak tersedia.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int anakId = Convert.ToInt32(anakIdValue);
                int ortuId = Convert.ToInt32(ortuIdValue);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmdAnak = new SqlCommand("SELECT nama FROM Anak_Asuh WHERE anak_id = @id", con);
                    cmdAnak.Parameters.AddWithValue("@id", anakId);
                    object namaAnakObj = cmdAnak.ExecuteScalar();
                    comboNamaAnak.Text = namaAnakObj != null ? namaAnakObj.ToString() : "";

                    SqlCommand cmdOrtu = new SqlCommand("SELECT nama FROM Orang_Tua_Asuh WHERE orang_tua_id = @id", con);
                    cmdOrtu.Parameters.AddWithValue("@id", ortuId);
                    object namaOrtuObj = cmdOrtu.ExecuteScalar();
                    comboNamaOrangtua.Text = namaOrtuObj != null ? namaOrtuObj.ToString() : "";
                }

                if (row.Cells["tanggal_adopsi"].Value != DBNull.Value)
                {
                    dateTime.Value = Convert.ToDateTime(row.Cells["tanggal_adopsi"].Value);
                }
                else
                {
                    dateTime.Value = DateTime.Today;  // Atau default lain sesuai kebutuhan
                }

                comboStatus.Text = row.Cells["status_adopsi"].Value != DBNull.Value
                    ? row.Cells["status_adopsi"].Value.ToString()
                    : "";
            }
        }


        private void btnTambah_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // Validasi input awal
                    if (string.IsNullOrWhiteSpace(comboNamaAnak.Text) ||
                        string.IsNullOrWhiteSpace(comboNamaOrangtua.Text) ||
                        string.IsNullOrWhiteSpace(comboStatus.Text))
                    {
                        MessageBox.Show("Semua field harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(comboNamaAnak.Text, @"^[a-zA-Z\s]+$"))
                    {
                        MessageBox.Show("Nama anak tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(comboNamaOrangtua.Text, @"^[a-zA-Z\s]+$"))
                    {
                        MessageBox.Show("Nama orang tua tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (dateTime.Value < DateTime.Today.AddDays(-7) || dateTime.Value > DateTime.Today)
                    {
                        MessageBox.Show("Tanggal adopsi hanya boleh antara 1 minggu yang lalu hingga 1 minggu ke depan.");
                        return;
                    }

                    // Ambil anak_id
                    string queryAnak = "SELECT anak_id FROM Anak_Asuh WHERE nama = @namaAnak";
                    SqlCommand cmdAnak = new SqlCommand(queryAnak, con, transaction);
                    cmdAnak.Parameters.AddWithValue("@namaAnak", comboNamaAnak.Text);
                    object resultAnak = cmdAnak.ExecuteScalar();

                    if (resultAnak == null)
                        throw new Exception("Nama anak tidak ditemukan.");

                    // Ambil orang_tua_id
                    string queryOrtu = "SELECT orang_tua_id FROM Orang_Tua_Asuh WHERE nama = @namaOrangtua";
                    SqlCommand cmdOrtu = new SqlCommand(queryOrtu, con, transaction);
                    cmdOrtu.Parameters.AddWithValue("@namaOrangtua", comboNamaOrangtua.Text);
                    object resultOrtu = cmdOrtu.ExecuteScalar();

                    if (resultOrtu == null)
                        throw new Exception("Nama orang tua tidak ditemukan.");

                    // Konfirmasi user
                    var konfirmasi = MessageBox.Show(
                        $"Apakah Anda yakin ingin menambahkan data adopsi berikut?\n\n" +
                        $"Nama Anak: {comboNamaAnak.Text}\n" +
                        $"Nama Orang Tua: {comboNamaOrangtua.Text}\n" +
                        $"Tanggal Adopsi: {dateTime.Value:dd MMMM yyyy}\n" +
                        $"Status Adopsi: {comboStatus.Text}",
                        "Konfirmasi Tambah Data Adopsi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (konfirmasi == DialogResult.No)
                        return;

                    // Eksekusi stored procedure
                    SqlCommand cmd = new SqlCommand("sp_TambahAdopsi", con, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@anak_id", (int)resultAnak);
                    cmd.Parameters.AddWithValue("@orang_tua_id", (int)resultOrtu);
                    cmd.Parameters.AddWithValue("@tanggal_adopsi", dateTime.Value);
                    cmd.Parameters.AddWithValue("@status_adopsi", comboStatus.Text);

                    cmd.ExecuteNonQuery();
                    transaction.Commit();

                    MessageBox.Show("Data adopsi berhasil ditambahkan", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _cache.Remove(_cacheKey);
                    ClearForm();
                    LoadData();
                }
                catch (Exception ex)
                {
                    try { transaction.Rollback(); } catch { /* rollback gagal? abaikan */ }
                    MessageBox.Show("Gagal menambahkan data adopsi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewAsuh.CurrentRow != null)
            {
                int adopsiId = Convert.ToInt32(dataGridViewAsuh.CurrentRow.Cells["adopsi_id"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlTransaction transaction = con.BeginTransaction();

                    // Validasi input awal
                    if (string.IsNullOrWhiteSpace(comboNamaAnak.Text) ||
                        string.IsNullOrWhiteSpace(comboNamaOrangtua.Text) ||
                        string.IsNullOrWhiteSpace(comboStatus.Text))
                    {
                        MessageBox.Show("Semua field harus diisi.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(comboNamaAnak.Text, @"^[a-zA-Z\s]+$"))
                    {
                        MessageBox.Show("Nama anak tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!System.Text.RegularExpressions.Regex.IsMatch(comboNamaOrangtua.Text, @"^[a-zA-Z\s]+$"))
                    {
                        MessageBox.Show("Nama orang tua tidak boleh mengandung karakter spesial atau angka.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Validasi tanggal tidak boleh lebih dari 7 hari ke depan atau ke belakang
                    if (dateTime.Value < DateTime.Today.AddDays(-7) || dateTime.Value > DateTime.Today)
                    {
                        MessageBox.Show("Tanggal adopsi hanya boleh antara 1 minggu yang lalu hingga 1 minggu ke depan.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Ambil ID anak berdasarkan nama
                    string queryAnak = "SELECT anak_id FROM Anak_Asuh WHERE nama = @namaAnak";
                    SqlCommand cmdAnak = new SqlCommand(queryAnak, con, transaction);
                    cmdAnak.Parameters.AddWithValue("@namaAnak", comboNamaAnak.Text);
                    object resultAnak = cmdAnak.ExecuteScalar();

                    if (resultAnak == null)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Nama anak tidak ditemukan.");
                        return;
                    }

                    // Ambil ID orang tua berdasarkan nama
                    string queryOrtu = "SELECT orang_tua_id FROM Orang_Tua_Asuh WHERE nama = @namaOrangtua";
                    SqlCommand cmdOrtu = new SqlCommand(queryOrtu, con, transaction);
                    cmdOrtu.Parameters.AddWithValue("@namaOrangtua", comboNamaOrangtua.Text);
                    object resultOrtu = cmdOrtu.ExecuteScalar();

                    if (resultOrtu == null)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Nama orang tua tidak ditemukan.");
                        return;
                    }

                    // Konfirmasi sebelum update
                    var konfirmasi = MessageBox.Show(
                        $"Apakah Anda yakin ingin memperbarui data adopsi ini?\n\n" +
                        $"Nama Anak: {comboNamaAnak.Text}\n" +
                        $"Nama Orang Tua: {comboNamaOrangtua.Text}\n" +
                        $"Tanggal Adopsi: {dateTime.Value:dd MMMM yyyy}\n" +
                        $"Status Adopsi: {comboStatus.Text}",
                        "Konfirmasi Update Data Adopsi",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (konfirmasi == DialogResult.No)
                    {
                        return;
                    }

                    SqlCommand cmd = new SqlCommand("sp_UpdateAdopsi", con, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adopsi_id", adopsiId);
                    cmd.Parameters.AddWithValue("@anak_id", (int)resultAnak);
                    cmd.Parameters.AddWithValue("@orang_tua_id", (int)resultOrtu);
                    cmd.Parameters.AddWithValue("@tanggal_adopsi", dateTime.Value);
                    cmd.Parameters.AddWithValue("@status_adopsi", comboStatus.Text);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                        MessageBox.Show("Data adopsi berhasil diperbarui", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _cache.Remove(_cacheKey);
                        ClearForm();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Gagal memperbarui data adopsi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewAsuh.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewAsuh.CurrentRow.Cells["adopsi_id"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data adopsi ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlTransaction transaction = con.BeginTransaction();

                        try
                        {
                            SqlCommand cmd = new SqlCommand("sp_DeleteAdopsi", con, transaction);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@adopsi_id", id);

                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                            MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _cache.Remove(_cacheKey);
                            ClearForm();
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Gagal menghapus data adopsi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
        }

        private void EnsureIndexesAdopsi()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string indexScript = @"
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_Adopsi_AnakId')
    BEGIN
        CREATE NONCLUSTERED INDEX idx_Adopsi_AnakId ON Adopsi(anak_id)
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'idx_Adopsi_OrangtuaId')
    BEGIN
        CREATE NONCLUSTERED INDEX idx_Adopsi_OrangtuaId ON Adopsi(orang_tua_id)
    END;
";

                using (SqlCommand cmd = new SqlCommand(indexScript, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }



        private void txtAnakid_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

    

