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
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btnAdopsi = new System.Windows.Forms.Button();
            this.btnAnak = new System.Windows.Forms.Button();
            this.btnOrtu = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.btnAdopsi);
            this.panel1.Controls.Add(this.btnAnak);
            this.panel1.Controls.Add(this.btnOrtu);
            this.panel1.Location = new System.Drawing.Point(-1, 54);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(282, 534);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(45, 437);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(166, 39);
            this.button6.TabIndex = 8;
            this.button6.Text = "Saldo";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(45, 358);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(166, 39);
            this.button5.TabIndex = 6;
            this.button5.Text = "Pengeluaran";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(45, 276);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(166, 39);
            this.button4.TabIndex = 5;
            this.button4.Text = "Pemasukan";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
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
            this.label1.Location = new System.Drawing.Point(602, 20);
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
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(-1, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1827, 55);
            this.panel2.TabIndex = 1;
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
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnAdopsi;
        private System.Windows.Forms.Button btnAnak;
        private System.Windows.Forms.Button btnOrtu;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Panel panel2;
    }
}