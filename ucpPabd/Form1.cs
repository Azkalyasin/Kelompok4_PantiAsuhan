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
            txtPassword.PasswordChar = '*';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // COLLATE SQL_Latin1_General_CP1_CS_AS agar perbandingan case-sensitive
                    string query = @"SELECT UserName FROM Kepala_Panti 
                             WHERE UserName COLLATE SQL_Latin1_General_CP1_CS_AS = @username 
                             AND Password COLLATE SQL_Latin1_General_CP1_CS_AS = @password";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    con.Open();

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        Main mn = new Main();
                        mn.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Username atau password salah", "Login Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
