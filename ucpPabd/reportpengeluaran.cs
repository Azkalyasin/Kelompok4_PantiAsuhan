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
    public partial class reportpengeluaran : Form
    {
        public reportpengeluaran()
        {
            InitializeComponent();
        }

        private void reportpengeluaran_Load(object sender, EventArgs e)
        {
            //setup the report viewer
            SetupReportViewer();
            this.reportViewer1.RefreshReport();
        }

        private void SetupReportViewer()
        {
            // Set the report path and parameters if needed
            string connectionString = "Data Source=LAPTOP-PGU1KG1D\\AZKALADZKIA;Initial Catalog=panti_asuhan;Integrated Security=True;"; // Update with your connection string

            string query = @"
            SELECT Pengeluaran.pengeluaran_id, Pengeluaran.kategori, Pengeluaran.jumlah, Pengeluaran.tanggal, Saldo.saldo_id, Saldo.total_saldo, Saldo.tanggal_update
            FROM     Pengeluaran INNER JOIN
            Saldo ON Pengeluaran.pengeluaran_id = Saldo.pengeluaran_id";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            // Create a ReportDataSource
            ReportDataSource rds = new ReportDataSource("DataSetPengeluaran", dt); // Make sure "DataSet1" matches your RDLC dataset name

            // Clear any existing data sources and add the new data source
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);

            // Set the path to the report (.rdlc file)
            // Change this to the actual path of your .rdlc file
            reportViewer1.LocalReport.ReportPath = @"E:\C++\ucpPabd\ucpPabd\reportpengeluaran.rdlc";

            // Refresh the ReportViewer to show the updated report
            reportViewer1.RefreshReport();
            // Refresh the report viewer to apply changes
            this.reportViewer1.RefreshReport();
        }
    }
}
