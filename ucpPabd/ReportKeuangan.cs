using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ucpPabd
{
    public partial class ReportKeuangan : Form
    {
        Koneksi kn = new Koneksi();
        string connectionString = "";
        public ReportKeuangan()
        {
            InitializeComponent();
            connectionString = kn.connectionString();
        }

        private void ReportKeuangan_Load(object sender, EventArgs e)
        {
            SetupReportViewer();
            this.reportViewer1.RefreshReport();
        }

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void SetupReportViewer()
        {

            // SQL query to retrieve the required data from the database
            string query = @"
        SELECT tanggal AS Tanggal, kategori AS Keterangan, Pemasukan, Pengeluaran, total_saldo AS Saldo
        FROM     (SELECT s.saldo_id, p.tanggal, p.kategori, p.jumlah AS Pemasukan, 0 AS Pengeluaran, s.total_saldo
                          FROM      Saldo AS s INNER JOIN
                                            Pemasukan AS p ON s.pemasukan_id = p.pemasukan_id
                          UNION ALL
                          SELECT s.saldo_id, e.tanggal, e.kategori, 0 AS Pemasukan, e.jumlah AS Pengeluaran, s.total_saldo
                          FROM     Saldo AS s INNER JOIN
                                            Pengeluaran AS e ON s.pengeluaran_id = e.pengeluaran_id) AS LaporanGabungan
        ORDER BY saldo_id";

            // Create a DataTable to store the data
            DataTable dt = new DataTable();

            // Use SqlDataAdapter to fill the DataTable with data from the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                da.Fill(dt); // <--- Tambahkan Breakpoint di sini
            }

            // Create a ReportDataSource
            ReportDataSource rds = new ReportDataSource("DataSetKeuangan", dt); // Make sure "DataSet1" matches your RDLC dataset name

            // Clear any existing data sources and add the new data source
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            // Set the path to the report (.rdlc file)
            // Change this to the actual path of your .rdlc file
            //reportViewer1.LocalReport.ReportPath = @"E:\C++\ucpPabd\ucpPabd\ReportKeuangan.rdlc";
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReportKeuangan.rdlc");
            reportViewer1.LocalReport.ReportPath = reportPath;

            // Refresh the ReportViewer to show the updated report
            reportViewer1.RefreshReport();
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
