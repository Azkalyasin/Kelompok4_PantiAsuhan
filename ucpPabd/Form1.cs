using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ucpPabd
{
    public partial class Form1 : Form
    {
        // Connection string yang benar
        static string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Ambil nilai username dan password dari TextBox
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            try
            {
                // Gunakan parameterized query untuk mencegah SQL Injection
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserName, Password FROM Kepala_Panti WHERE UserName = @username AND Password = @password";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    // Buka koneksi ke database
                    con.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Cek apakah data ditemukan
                    if (dt.Rows.Count > 0)
                    {
                        // Jika login berhasil, buka form utama
                        Main mn = new Main();
                        mn.Show();
                        this.Hide();  // Sembunyikan form login setelah login berhasil
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Tangani kesalahan yang terjadi selama proses login
                MessageBox.Show("Error: " + ex.Message, "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
