using Assecontweb.Extend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diferimento
{
    public partial class FrmNotasTxt : Form
    {
        public Fechamento fechamento;
        public List<Model.Seal.Cliente> clientes;
        public List<Model.Seal.ContaReceber> contasReceber;
        public List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes;
        public List<Assecontweb.Extend.Nfe.Servico.NotaFiscal> servicos;

        public FrmNotasTxt()
        {
            InitializeComponent();
        }

        private void btnNotas_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog open = new FolderBrowserDialog();
            open.Description = "Localizar Notas";

            DialogResult dr = open.ShowDialog();
            if (dr == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(open.SelectedPath))
                {
                    txtNotas.Text = open.SelectedPath;
                    string[] files = System.IO.Directory.GetFiles(txtNotas.Text);

                    if (danfes == null || danfes.Count() == 0)
                        danfes = new List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal>();
                    if (servicos == null || servicos.Count() == 0)
                        servicos = new List<Assecontweb.Extend.Nfe.Servico.NotaFiscal>();

                    string file;
                    string[] split;
                    string filename;
                    progress.Maximum = files.Length;
                    lblProgress.Text = "Lendo 0 de " + files.Length.ToString() + "...";
                    for (int i = 0; i < files.Length; i++)
                    {
                        file = files[i];
                        split = file.Split(new string[] { "\\" }, StringSplitOptions.None);
                        filename = split[split.Length - 1];
                        string nome;
                        if (filename.Length > 40)
                        {
                            string comeco = filename.Substring(0, 17);
                            string meio = "...";
                            string fim = filename.Substring(filename.Length - 20, 20);
                            nome = comeco + meio + fim;
                        }
                        else
                            nome = filename;
                        lblProgress.Text = "Lendo " + i.ToString() + " de " + files.Length.ToString() + "... " + "Arquivo: " + nome;
                        lblProgress.Refresh();
                        if (filename.Contains(".txt"))//&& filename.Contains("procNFE"))
                        {
                            FileNfeServico nfes = new FileNfeServico(file);
                            if (nfes.notafiscal.Numero != -1)
                            {
                                foreach (Assecontweb.Extend.Nfe.Servico.NotaFiscal nota in nfes.notasfiscais)
                                {
                                    servicos.Add(nota);
                                }
                            }
                            else
                            {
                                FileNfeDanfe nfed = new FileNfeDanfe(file);
                                if (nfed.notafiscal.idFornecedor != -1)
                                {
                                    foreach (Assecontweb.Extend.Nfe.Danfe.NotaFiscal nota in nfed.notasfiscais)
                                    {
                                        danfes.Add(nota);
                                    }
                                }
                            }
                        }
                        progress.Increment(1);
                    }
                    progress.Value = 0;
                    lblProgress.Text = "Notas Fiscais carregadas...";
                }
            }
            else
                MessageBox.Show("Por favor, selecione um local onde está as notas de vendas.");
        }

        private void btnOkNotas_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            btnNotas.Enabled = false;
            btnOkNotas.Enabled = false;
            string erro = "";
            DAO.NfeDAO DAO = new DAO.NfeDAO();
            if (!DAO.SetServicos(servicos, out erro))
                MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Cursor = Cursors.Default;
            btnNotas.Enabled = true;
            btnOkNotas.Enabled = true;
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
