using Microsoft.Reporting.WinForms;
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
    public partial class ReportKeuangan : Form
    {
        public ReportKeuangan()
        {
            InitializeComponent();
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
            // Connection string to your database
            string connectionString = @"Data Source=LAPTOP-PGU1KG1D\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True";

            // SQL query to retrieve the required data from the database
            string query = @"
        SELECT
    LaporanGabungan.tanggal AS [Tanggal],
    LaporanGabungan.kategori AS [Keterangan],
    LaporanGabungan.Pemasukan,
    LaporanGabungan.Pengeluaran,
    LaporanGabungan.total_saldo AS [Saldo]
FROM
    (
        SELECT s.saldo_id, p.tanggal, p.kategori, p.jumlah AS Pemasukan, 0 AS Pengeluaran, s.total_saldo
        FROM Saldo s INNER JOIN Pemasukan p ON s.pemasukan_id = p.pemasukan_id
        UNION ALL
        SELECT s.saldo_id, e.tanggal, e.kategori, 0 AS Pemasukan, e.jumlah AS Pengeluaran, s.total_saldo
        FROM Saldo s INNER JOIN Pengeluaran e ON s.pengeluaran_id = e.pengeluaran_id
    ) AS LaporanGabungan
ORDER BY LaporanGabungan.saldo_id;";

            // Create a DataTable to store the data
            DataTable dt = new DataTable();

            // Use SqlDataAdapter to fill the DataTable with data from the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            // Create a ReportDataSource
            ReportDataSource rds = new ReportDataSource("DataSetKeuangan", dt); // Make sure "DataSet1" matches your RDLC dataset name

            // Clear any existing data sources and add the new data source
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            // Set the path to the report (.rdlc file)
            // Change this to the actual path of your .rdlc file
            reportViewer1.LocalReport.ReportPath = @"E:\C++\ucpPabd\ucpPabd\ReportKeuangan.rdlc";

            // Refresh the ReportViewer to show the updated report
            reportViewer1.RefreshReport();
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
