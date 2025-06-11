namespace ucpPabd
{
    partial class Saldo
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
            this.dataGridViewSaldo = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSaldo)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewSaldo
            // 
            this.dataGridViewSaldo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSaldo.Location = new System.Drawing.Point(3, 192);
            this.dataGridViewSaldo.Name = "dataGridViewSaldo";
            this.dataGridViewSaldo.RowHeadersWidth = 51;
            this.dataGridViewSaldo.RowTemplate.Height = 24;
            this.dataGridViewSaldo.Size = new System.Drawing.Size(796, 255);
            this.dataGridViewSaldo.TabIndex = 0;
            this.dataGridViewSaldo.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewSaldo_CellContentClick_1);
            // 
            // Saldo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridViewSaldo);
            this.Name = "Saldo";
            this.Text = "Saldo";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSaldo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewSaldo;
    }
}