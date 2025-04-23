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
    public partial class Anak_Asuh: Form
    {
        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";

        public Anak_Asuh()
        {
            InitializeComponent();
            LoadData();
            comboJenisKelamin.Items.AddRange(new string[] { "L", "P"});
            dataGridViewAnak.CellClick += DataGridViewAnak_CellClick;

        }

        private void btnTambahData_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text;
            string tempatTanggalLahir = txtTempatTglLahir.Text;
            string tanggalMasuk = dateTime.Value.ToString("yyyy-MM-dd");
            string statusPendidikan = txtStatusPendidikan.Text;
            string jenisKelamin = comboJenisKelamin.Text;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Anak_Asuh (Nama, Jenis_Kelamin, Tanggal_Masuk, Tempat_Tanggal_Lahir, Status_Pendidikan) " +
                                   "VALUES (@nama, @jenisKelamin, @tanggalMasuk, @tempatTanggalLahir, @statusPendidikan)";



                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@nama", nama);
                    cmd.Parameters.AddWithValue("@jenisKelamin", jenisKelamin);
                    cmd.Parameters.AddWithValue("@tanggalMasuk", tanggalMasuk);
                    cmd.Parameters.AddWithValue("@tempatTanggalLahir", tempatTanggalLahir);
                    cmd.Parameters.AddWithValue("@statusPendidikan", statusPendidikan);

                    con.Open();
                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Data berhasil ditambahkan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Data gagal ditambahkan.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            txtStatusPendidikan.Clear();
            txtTempatTglLahir.Clear();
            dateTime.Value = DateTime.Now;
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Anak_Asuh";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridViewAnak.DataSource = dt;
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
                dateTime.Value = Convert.ToDateTime(row.Cells["Tanggal_Masuk"].Value);
                txtTempatTglLahir.Text = row.Cells["Tempat_Tanggal_Lahir"].Value.ToString();
                txtStatusPendidikan.Text = row.Cells["Status_Pendidikan"].Value.ToString();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridViewAnak.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewAnak.CurrentRow.Cells["anak_id"].Value);
                string nama = txtNama.Text;
                string jenisKelamin = comboJenisKelamin.Text;
                string tanggalMasuk = dateTime.Value.ToString("yyyy-MM-dd");
                string tempatTanggalLahir = txtTempatTglLahir.Text;
                string statusPendidikan = txtStatusPendidikan.Text;

                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Anak_Asuh SET Nama = @nama, Jenis_Kelamin = @jenisKelamin, Tanggal_Masuk = @tanggalMasuk, Tempat_Tanggal_Lahir = @tempatTanggalLahir, Status_Pendidikan = @statusPendidikan WHERE anak_id = @id";

                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@nama", nama);
                        cmd.Parameters.AddWithValue("@jenisKelamin", jenisKelamin);
                        cmd.Parameters.AddWithValue("@tanggalMasuk", tanggalMasuk);
                        cmd.Parameters.AddWithValue("@tempatTanggalLahir", tempatTanggalLahir);
                        cmd.Parameters.AddWithValue("@statusPendidikan", statusPendidikan);
                        cmd.Parameters.AddWithValue("@id", id);

                        con.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            MessageBox.Show("Data berhasil diperbarui.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Update gagal.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewAnak.CurrentRow != null)
            {
                int id = Convert.ToInt32(dataGridViewAnak.CurrentRow.Cells["anak_id"].Value);

                var confirm = MessageBox.Show("Yakin ingin menghapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection con = new SqlConnection(connectionString))
                        {
                            string query = "DELETE FROM Anak_Asuh WHERE anak_id = @id";
                            SqlCommand cmd = new SqlCommand(query, con);
                            cmd.Parameters.AddWithValue("@id", id);

                            con.Open();
                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                MessageBox.Show("Data berhasil dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ClearForm();
                                LoadData();
                            }
                            else
                            {
                                MessageBox.Show("Hapus gagal.", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Anak_Asuh_Load(object sender, EventArgs e)
        {

        }

        private void txtTempatTglLahir_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
