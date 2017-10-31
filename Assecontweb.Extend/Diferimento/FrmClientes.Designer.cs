namespace Diferimento
{
    partial class FrmClientes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmClientes));
            this.lblCliente = new System.Windows.Forms.Label();
            this.btnCliente = new System.Windows.Forms.Button();
            this.txtCliente = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnContasReceber = new System.Windows.Forms.Button();
            this.txtContasReceber = new System.Windows.Forms.TextBox();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCliente
            // 
            resources.ApplyResources(this.lblCliente, "lblCliente");
            this.lblCliente.Name = "lblCliente";
            // 
            // btnCliente
            // 
            resources.ApplyResources(this.btnCliente, "btnCliente");
            this.btnCliente.Name = "btnCliente";
            this.btnCliente.UseVisualStyleBackColor = true;
            this.btnCliente.Click += new System.EventHandler(this.btnCliente_Click);
            // 
            // txtCliente
            // 
            resources.ApplyResources(this.txtCliente, "txtCliente");
            this.txtCliente.Name = "txtCliente";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnContasReceber
            // 
            resources.ApplyResources(this.btnContasReceber, "btnContasReceber");
            this.btnContasReceber.Name = "btnContasReceber";
            this.btnContasReceber.UseVisualStyleBackColor = true;
            this.btnContasReceber.Click += new System.EventHandler(this.btnContasReceber_Click);
            // 
            // txtContasReceber
            // 
            resources.ApplyResources(this.txtContasReceber, "txtContasReceber");
            this.txtContasReceber.Name = "txtContasReceber";
            // 
            // btnSalvar
            // 
            resources.ApplyResources(this.btnSalvar, "btnSalvar");
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            // 
            // FrmClientes
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.btnCliente);
            this.Controls.Add(this.txtCliente);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnContasReceber);
            this.Controls.Add(this.txtContasReceber);
            this.Name = "FrmClientes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.Button btnCliente;
        private System.Windows.Forms.TextBox txtCliente;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnContasReceber;
        private System.Windows.Forms.TextBox txtContasReceber;
        private System.Windows.Forms.Button btnSalvar;
    }
}