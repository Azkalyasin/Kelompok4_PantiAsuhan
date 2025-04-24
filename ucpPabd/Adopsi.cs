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
    public partial class Adopsi: Form
    {
        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";
        public Adopsi()
        {
            InitializeComponent();
            comboStatus.Items.AddRange(new string[] { "Proses", "Selesai", "Dibatalkan" });
            LoadData();
            dataGridViewAsuh.CellClick += DataGridViewAdopsi_CellClick;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Adopsi";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewAsuh.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            txtNamaAnak.Clear();
            txtNamaOrangtua.Clear();
            dateTime.Value = DateTime.Now;
            comboStatus.SelectedIndex = -1;
        }


        private void DataGridViewAdopsi_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewAsuh.Rows[e.RowIndex];

                int anakId = Convert.ToInt32(row.Cells["anak_id"].Value);
                int ortuId = Convert.ToInt32(row.Cells["orang_tua_id"].Value);

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    SqlCommand cmdAnak = new SqlCommand("SELECT nama FROM Anak_Asuh WHERE anak_id = @id", con);
                    cmdAnak.Parameters.AddWithValue("@id", anakId);
                    txtNamaAnak.Text = cmdAnak.ExecuteScalar()?.ToString();

                    SqlCommand cmdOrtu = new SqlCommand("SELECT nama FROM Orang_Tua_Asuh WHERE orang_tua_id = @id", con);
                    cmdOrtu.Parameters.AddWithValue("@id", ortuId);
                    txtNamaOrangtua.Text = cmdOrtu.ExecuteScalar()?.ToString();
                }

                if (row.Cells["tanggal_adopsi"].Value != DBNull.Value)
                {
                    dateTime.Value = Convert.ToDateTime(row.Cells["tanggal_adopsi"].Value);
                }

                comboStatus.Text = row.Cells["status_adopsi"].Value.ToString();
            }
        }


        private void btnTambah_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Cari anak_id berdasarkan nama anak
                string queryAnak = "SELECT anak_id FROM Anak_Asuh WHERE nama = @namaAnak";
                SqlCommand cmdAnak = new SqlCommand(queryAnak, con);
                cmdAnak.Parameters.AddWithValue("@namaAnak", txtNamaAnak.Text);
                object resultAnak = cmdAnak.ExecuteScalar();

                if (resultAnak == null)
                {
                    MessageBox.Show("Nama anak tidak ditemukan.");
                    return;
                }

                // Cari orang_tua_id berdasarkan nama orang tua
                string queryOrtu = "SELECT orang_tua_id FROM Orang_Tua_Asuh WHERE nama = @namaOrangtua";
                SqlCommand cmdOrtu = new SqlCommand(queryOrtu, con);
                cmdOrtu.Parameters.AddWithValue("@namaOrangtua", txtNamaOrangtua.Text);
                object resultOrtu = cmdOrtu.ExecuteScalar();

                if (resultOrtu == null)
                {
                    MessageBox.Show("Nama orang tua tidak ditemukan.");
                    return;
                }

                // Insert ke tabel Adopsi
                string query = "INSERT INTO Adopsi (anak_id, orang_tua_id, tanggal_adopsi, status_adopsi) VALUES (@anak_id, @orang_tua_id, @tanggal_adopsi, @status_adopsi)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@anak_id", (int)resultAnak);
                cmd.Parameters.AddWithValue("@orang_tua_id", (int)resultOrtu);
                cmd.Parameters.AddWithValue("@tanggal_adopsi", dateTime.Value);
                cmd.Parameters.AddWithValue("@status_adopsi", comboStatus.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Data berhasil ditambahkan");
                ClearForm();
                LoadData();
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

                    // Ambil ID anak berdasarkan nama
                    string queryAnak = "SELECT anak_id FROM Anak_Asuh WHERE nama = @namaAnak";
                    SqlCommand cmdAnak = new SqlCommand(queryAnak, con);
                    cmdAnak.Parameters.AddWithValue("@namaAnak", txtNamaAnak.Text);
                    object resultAnak = cmdAnak.ExecuteScalar();

                    if (resultAnak == null)
                    {
                        MessageBox.Show("Nama anak tidak ditemukan.");
                        return;
                    }

                    // Ambil ID orang tua berdasarkan nama
                    string queryOrtu = "SELECT orang_tua_id FROM Orang_Tua_Asuh WHERE nama = @namaOrangtua";
                    SqlCommand cmdOrtu = new SqlCommand(queryOrtu, con);
                    cmdOrtu.Parameters.AddWithValue("@namaOrangtua", txtNamaOrangtua.Text);
                    object resultOrtu = cmdOrtu.ExecuteScalar();

                    if (resultOrtu == null)
                    {
                        MessageBox.Show("Nama orang tua tidak ditemukan.");
                        return;
                    }

                    // Update data
                    string queryUpdate = "UPDATE Adopsi SET anak_id=@anak_id, orang_tua_id=@orang_tua_id, tanggal_adopsi=@tanggal_adopsi, status_adopsi=@status_adopsi WHERE adopsi_id=@id";
                    SqlCommand cmd = new SqlCommand(queryUpdate, con);
                    cmd.Parameters.AddWithValue("@anak_id", (int)resultAnak);
                    cmd.Parameters.AddWithValue("@orang_tua_id", (int)resultOrtu);
                    cmd.Parameters.AddWithValue("@tanggal_adopsi", dateTime.Value);
                    cmd.Parameters.AddWithValue("@status_adopsi", comboStatus.Text);
                    cmd.Parameters.AddWithValue("@id", adopsiId);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Data berhasil diperbarui");
                    ClearForm();
                    LoadData();
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
                        string query = "DELETE FROM Adopsi WHERE adopsi_id = @id";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@id", id);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Data berhasil dihapus", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadData();
                    }
                }
            }
        }

        private void txtAnakid_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

    

