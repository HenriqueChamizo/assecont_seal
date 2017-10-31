using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Assecontweb.Extend
{
    public partial class FrmDiferimento : Form
    {
        byte[] byteContasReceber;
        FileTypes typeContasReceber;
        byte[] byteServicos;
        FileTypes typeServicos;
        byte[] byteReceitaDiferida;
        FileTypes typeReceitaDiferida;

        public FrmDiferimento()
        {
            InitializeComponent();
        }

        private void btnContasReceber_Click(object sender, EventArgs e)
        {
            OpenFileDialog openContasReceber = new OpenFileDialog();
            openContasReceber.InitialDirectory = @"C:\";
            openContasReceber.RestoreDirectory = true;
            openContasReceber.Title = "Localizar Contas Receber";
            openContasReceber.DefaultExt = "xlsx";
            openContasReceber.Filter = "Arquivos Excel (*.xls|*.xlsx|*.csv)|Todos Arquivos (*.*)|*.*";
            openContasReceber.FilterIndex = 0;
            openContasReceber.CheckFileExists = true;
            openContasReceber.CheckPathExists = true;
            openContasReceber.Multiselect = false;

            DialogResult dr = openContasReceber.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtContasReceber.Text = openContasReceber.FileName;
                if(!string.IsNullOrEmpty(txtContasReceber.Text))
                {
                    string[] split = txtContasReceber.Text.Split(new string[] { "." }, StringSplitOptions.None);
                    string extensao = split[split.Length - 1];

                    switch (extensao)
                    {
                        case "xls":
                            typeContasReceber = FileTypes.xls;
                            break;
                        case "xlsx":
                            typeContasReceber = FileTypes.xlsx;
                            break;
                        case "csv":
                            typeContasReceber = FileTypes.csv;
                            break;
                        default:
                            typeContasReceber = FileTypes.xlsx;
                            break;
                    }

                    using (FileStream fileStream = new FileStream(txtContasReceber.Text, FileMode.Open))
                    {
                        byte[] bytes = new byte[fileStream.Length];
                        fileStream.Read(bytes, 0, Convert.ToInt32(fileStream.Length));

                        byteContasReceber = bytes;
                    }
                }
            }
        }

        private void btnServicos_Click(object sender, EventArgs e)
        {
            OpenFileDialog openServicos = new OpenFileDialog();
            openServicos.InitialDirectory = @"C:\";
            openServicos.RestoreDirectory = true;
            openServicos.Title = "Localizar Contas Receber";
            openServicos.DefaultExt = "xlsx";
            openServicos.Filter = "Arquivos Excel (*.xls|*.xlsx|*.csv)|Todos Arquivos (*.*)|*.*";
            openServicos.FilterIndex = 0;
            openServicos.CheckFileExists = true;
            openServicos.CheckPathExists = true;
            openServicos.Multiselect = false;

            DialogResult dr = openServicos.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtServicos.Text = openServicos.FileName;
                if (!string.IsNullOrEmpty(txtServicos.Text))
                {
                    string[] split = txtServicos.Text.Split(new string[] { "." }, StringSplitOptions.None);
                    string extensao = split[split.Length - 1];

                    switch (extensao)
                    {
                        case "xls":
                            typeServicos = FileTypes.xls;
                            break;
                        case "xlsx":
                            typeServicos = FileTypes.xlsx;
                            break;
                        case "csv":
                            typeServicos = FileTypes.csv;
                            break;
                        default:
                            typeServicos = FileTypes.xlsx;
                            break;
                    }

                    using (FileStream fileStream = new FileStream(txtServicos.Text, FileMode.Open))
                    {
                        byte[] bytes = new byte[fileStream.Length];
                        fileStream.Read(bytes, 0, Convert.ToInt32(fileStream.Length));

                        byteServicos = bytes;
                    }
                }
            }
        }

        private void btnReceitaDiferida_Click(object sender, EventArgs e)
        {

            OpenFileDialog openReceitaDiferida = new OpenFileDialog();
            openReceitaDiferida.InitialDirectory = @"C:\";
            openReceitaDiferida.RestoreDirectory = true;
            openReceitaDiferida.Title = "Localizar Contas Receber";
            openReceitaDiferida.DefaultExt = "xlsx";
            openReceitaDiferida.Filter = "Arquivos Excel (*.xls|*.xlsx|*.csv)|Todos Arquivos (*.*)|*.*";
            openReceitaDiferida.FilterIndex = 0;
            openReceitaDiferida.CheckFileExists = true;
            openReceitaDiferida.CheckPathExists = true;
            openReceitaDiferida.Multiselect = false;

            DialogResult dr = openReceitaDiferida.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtReceitaDiferida.Text = openReceitaDiferida.FileName;
                if (!string.IsNullOrEmpty(txtReceitaDiferida.Text))
                {
                    string[] split = txtReceitaDiferida.Text.Split(new string[] { "." }, StringSplitOptions.None);
                    string extensao = split[split.Length - 1];

                    switch (extensao)
                    {
                        case "xls":
                            typeReceitaDiferida = FileTypes.xls;
                            break;
                        case "xlsx":
                            typeReceitaDiferida = FileTypes.xlsx;
                            break;
                        case "csv":
                            typeReceitaDiferida = FileTypes.csv;
                            break;
                        default:
                            typeReceitaDiferida = FileTypes.xlsx;
                            break;
                    }

                    using (FileStream fileStream = new FileStream(txtReceitaDiferida.Text, FileMode.Open))
                    {
                        byte[] bytes = new byte[fileStream.Length];
                        fileStream.Read(bytes, 0, Convert.ToInt32(fileStream.Length));

                        byteReceitaDiferida = bytes;
                    }
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }
    }
}
