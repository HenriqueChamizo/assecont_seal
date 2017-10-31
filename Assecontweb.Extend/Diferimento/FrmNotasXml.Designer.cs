namespace Diferimento
{
    partial class FrmNotasXml
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
            this.lblProgress = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.btnOkNotas = new System.Windows.Forms.Button();
            this.txtNotas = new System.Windows.Forms.TextBox();
            this.btnNotas = new System.Windows.Forms.Button();
            this.lblNotasDanfe = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(9, 53);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(57, 13);
            this.lblProgress.TabIndex = 39;
            this.lblProgress.Text = "Progress...";
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(12, 69);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(413, 10);
            this.progress.TabIndex = 38;
            // 
            // btnOkNotas
            // 
            this.btnOkNotas.Location = new System.Drawing.Point(431, 56);
            this.btnOkNotas.Name = "btnOkNotas";
            this.btnOkNotas.Size = new System.Drawing.Size(75, 23);
            this.btnOkNotas.TabIndex = 37;
            this.btnOkNotas.Text = "Ok";
            this.btnOkNotas.UseVisualStyleBackColor = true;
            this.btnOkNotas.Click += new System.EventHandler(this.btnOkNotas_Click);
            // 
            // txtNotas
            // 
            this.txtNotas.Enabled = false;
            this.txtNotas.Location = new System.Drawing.Point(12, 29);
            this.txtNotas.Name = "txtNotas";
            this.txtNotas.Size = new System.Drawing.Size(452, 20);
            this.txtNotas.TabIndex = 34;
            // 
            // btnNotas
            // 
            this.btnNotas.Location = new System.Drawing.Point(470, 28);
            this.btnNotas.Name = "btnNotas";
            this.btnNotas.Size = new System.Drawing.Size(36, 20);
            this.btnNotas.TabIndex = 35;
            this.btnNotas.Text = "...";
            this.btnNotas.UseVisualStyleBackColor = true;
            this.btnNotas.Click += new System.EventHandler(this.btnNotas_Click);
            // 
            // lblNotasDanfe
            // 
            this.lblNotasDanfe.AutoSize = true;
            this.lblNotasDanfe.Location = new System.Drawing.Point(9, 9);
            this.lblNotasDanfe.Name = "lblNotasDanfe";
            this.lblNotasDanfe.Size = new System.Drawing.Size(70, 13);
            this.lblNotasDanfe.TabIndex = 36;
            this.lblNotasDanfe.Text = "Notas Fiscais";
            // 
            // FrmNotasXml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 106);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.btnOkNotas);
            this.Controls.Add(this.txtNotas);
            this.Controls.Add(this.btnNotas);
            this.Controls.Add(this.lblNotasDanfe);
            this.Name = "FrmNotasXml";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Importar Notas Fiscais (XML)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Button btnOkNotas;
        private System.Windows.Forms.TextBox txtNotas;
        private System.Windows.Forms.Button btnNotas;
        private System.Windows.Forms.Label lblNotasDanfe;
    }
}