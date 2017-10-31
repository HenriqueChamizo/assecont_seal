namespace Assecontweb.Extend
{
    partial class FrmDiferimento
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
            this.txtContasReceber = new System.Windows.Forms.TextBox();
            this.btnContasReceber = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnServicos = new System.Windows.Forms.Button();
            this.txtServicos = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReceitaDiferida = new System.Windows.Forms.Button();
            this.txtReceitaDiferida = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtContasReceber
            // 
            this.txtContasReceber.Enabled = false;
            this.txtContasReceber.Location = new System.Drawing.Point(13, 33);
            this.txtContasReceber.Name = "txtContasReceber";
            this.txtContasReceber.Size = new System.Drawing.Size(452, 20);
            this.txtContasReceber.TabIndex = 0;
            // 
            // btnContasReceber
            // 
            this.btnContasReceber.Location = new System.Drawing.Point(471, 33);
            this.btnContasReceber.Name = "btnContasReceber";
            this.btnContasReceber.Size = new System.Drawing.Size(36, 20);
            this.btnContasReceber.TabIndex = 1;
            this.btnContasReceber.Text = "...";
            this.btnContasReceber.UseVisualStyleBackColor = true;
            this.btnContasReceber.Click += new System.EventHandler(this.btnContasReceber_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Contas Receber";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Serviço";
            // 
            // btnServicos
            // 
            this.btnServicos.Location = new System.Drawing.Point(471, 86);
            this.btnServicos.Name = "btnServicos";
            this.btnServicos.Size = new System.Drawing.Size(36, 20);
            this.btnServicos.TabIndex = 4;
            this.btnServicos.Text = "...";
            this.btnServicos.UseVisualStyleBackColor = true;
            this.btnServicos.Click += new System.EventHandler(this.btnServicos_Click);
            // 
            // txtServicos
            // 
            this.txtServicos.Enabled = false;
            this.txtServicos.Location = new System.Drawing.Point(13, 86);
            this.txtServicos.Name = "txtServicos";
            this.txtServicos.Size = new System.Drawing.Size(452, 20);
            this.txtServicos.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Receita Diferida";
            // 
            // btnReceitaDiferida
            // 
            this.btnReceitaDiferida.Location = new System.Drawing.Point(471, 141);
            this.btnReceitaDiferida.Name = "btnReceitaDiferida";
            this.btnReceitaDiferida.Size = new System.Drawing.Size(36, 20);
            this.btnReceitaDiferida.TabIndex = 7;
            this.btnReceitaDiferida.Text = "...";
            this.btnReceitaDiferida.UseVisualStyleBackColor = true;
            this.btnReceitaDiferida.Click += new System.EventHandler(this.btnReceitaDiferida_Click);
            // 
            // txtReceitaDiferida
            // 
            this.txtReceitaDiferida.Enabled = false;
            this.txtReceitaDiferida.Location = new System.Drawing.Point(13, 141);
            this.txtReceitaDiferida.Name = "txtReceitaDiferida";
            this.txtReceitaDiferida.Size = new System.Drawing.Size(452, 20);
            this.txtReceitaDiferida.TabIndex = 6;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(432, 226);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // FrmDiferimento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 261);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnReceitaDiferida);
            this.Controls.Add(this.txtReceitaDiferida);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnServicos);
            this.Controls.Add(this.txtServicos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnContasReceber);
            this.Controls.Add(this.txtContasReceber);
            this.Name = "FrmDiferimento";
            this.Text = "FrmDiferimento";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtContasReceber;
        private System.Windows.Forms.Button btnContasReceber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnServicos;
        private System.Windows.Forms.TextBox txtServicos;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnReceitaDiferida;
        private System.Windows.Forms.TextBox txtReceitaDiferida;
        private System.Windows.Forms.Button btnOk;
    }
}