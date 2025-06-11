namespace ucpPabd
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaldo = new System.Windows.Forms.Button();
            this.btnPengeluaran = new System.Windows.Forms.Button();
            this.btnPemasukan = new System.Windows.Forms.Button();
            this.btnAdopsi = new System.Windows.Forms.Button();
            this.btnAnak = new System.Windows.Forms.Button();
            this.btnOrtu = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnLaporanKeuangan = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.btnLaporanKeuangan);
            this.panel1.Controls.Add(this.btnSaldo);
            this.panel1.Controls.Add(this.btnPengeluaran);
            this.panel1.Controls.Add(this.btnPemasukan);
            this.panel1.Controls.Add(this.btnAdopsi);
            this.panel1.Controls.Add(this.btnAnak);
            this.panel1.Controls.Add(this.btnOrtu);
            this.panel1.Location = new System.Drawing.Point(-1, 54);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(513, 534);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // btnSaldo
            // 
            this.btnSaldo.Location = new System.Drawing.Point(45, 443);
            this.btnSaldo.Name = "btnSaldo";
            this.btnSaldo.Size = new System.Drawing.Size(166, 39);
            this.btnSaldo.TabIndex = 7;
            this.btnSaldo.Text = "saldo";
            this.btnSaldo.UseVisualStyleBackColor = true;
            this.btnSaldo.Click += new System.EventHandler(this.btnSaldo_Click);
            // 
            // btnPengeluaran
            // 
            this.btnPengeluaran.Location = new System.Drawing.Point(45, 346);
            this.btnPengeluaran.Name = "btnPengeluaran";
            this.btnPengeluaran.Size = new System.Drawing.Size(166, 39);
            this.btnPengeluaran.TabIndex = 6;
            this.btnPengeluaran.Text = "Pengeluaran";
            this.btnPengeluaran.UseVisualStyleBackColor = true;
            this.btnPengeluaran.Click += new System.EventHandler(this.btnPengeluaran_Click);
            // 
            // btnPemasukan
            // 
            this.btnPemasukan.Location = new System.Drawing.Point(45, 269);
            this.btnPemasukan.Name = "btnPemasukan";
            this.btnPemasukan.Size = new System.Drawing.Size(166, 39);
            this.btnPemasukan.TabIndex = 5;
            this.btnPemasukan.Text = "Pemasukan";
            this.btnPemasukan.UseVisualStyleBackColor = true;
            this.btnPemasukan.Click += new System.EventHandler(this.btnPemasukan_Click);
            // 
            // btnAdopsi
            // 
            this.btnAdopsi.Location = new System.Drawing.Point(45, 189);
            this.btnAdopsi.Name = "btnAdopsi";
            this.btnAdopsi.Size = new System.Drawing.Size(166, 39);
            this.btnAdopsi.TabIndex = 4;
            this.btnAdopsi.Text = "Adopsi";
            this.btnAdopsi.UseVisualStyleBackColor = true;
            this.btnAdopsi.Click += new System.EventHandler(this.btnAdopsi_Click);
            // 
            // btnAnak
            // 
            this.btnAnak.Location = new System.Drawing.Point(45, 31);
            this.btnAnak.Name = "btnAnak";
            this.btnAnak.Size = new System.Drawing.Size(166, 39);
            this.btnAnak.TabIndex = 3;
            this.btnAnak.Text = "Anak Asuh";
            this.btnAnak.UseVisualStyleBackColor = true;
            this.btnAnak.Click += new System.EventHandler(this.btnAnak_Click);
            // 
            // btnOrtu
            // 
            this.btnOrtu.Location = new System.Drawing.Point(45, 105);
            this.btnOrtu.Name = "btnOrtu";
            this.btnOrtu.Size = new System.Drawing.Size(166, 39);
            this.btnOrtu.TabIndex = 2;
            this.btnOrtu.Text = "Orang Tua Asuh";
            this.btnOrtu.UseVisualStyleBackColor = true;
            this.btnOrtu.Click += new System.EventHandler(this.btnOrtu_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(456, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(310, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Panti Asuhan Management Systems";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel2.Controls.Add(this.btnLogout);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(-1, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1827, 69);
            this.panel2.TabIndex = 1;
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(977, 15);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(85, 37);
            this.btnLogout.TabIndex = 8;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnLaporanKeuangan
            // 
            this.btnLaporanKeuangan.Location = new System.Drawing.Point(296, 46);
            this.btnLaporanKeuangan.Name = "btnLaporanKeuangan";
            this.btnLaporanKeuangan.Size = new System.Drawing.Size(162, 436);
            this.btnLaporanKeuangan.TabIndex = 8;
            this.btnLaporanKeuangan.Text = "Laporan Keuangan";
            this.btnLaporanKeuangan.UseVisualStyleBackColor = true;
            this.btnLaporanKeuangan.Click += new System.EventHandler(this.btnLaporanKeuangan_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1088, 588);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Main";
            this.Text = "Main";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAdopsi;
        private System.Windows.Forms.Button btnAnak;
        private System.Windows.Forms.Button btnOrtu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnSaldo;
        private System.Windows.Forms.Button btnPengeluaran;
        private System.Windows.Forms.Button btnPemasukan;
        private System.Windows.Forms.Button btnLaporanKeuangan;
    }
}