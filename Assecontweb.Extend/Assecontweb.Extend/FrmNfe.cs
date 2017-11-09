using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Assecontweb.Extend.CFechamento;
using System.Globalization;
using System.Web.Script.Serialization;

namespace Assecontweb.Extend
{
    public partial class FrmNfe : Form
    {
        List<Nfe.Danfe.NotaFiscal> danfes;
        List<Nfe.Servico.NotaFiscal> servicos;

        Fechamento fechamento;
        List<Cliente> clientes;
        List<ContaReceber> contasReceber;

        List<string> linhasContribuinte;
        List<string> linhasNContribuinte;
        List<string> linhasLei;
        List<string> linhasNLei;

        public FrmNfe()
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
                        danfes = new List<Nfe.Danfe.NotaFiscal>();
                    if (servicos == null || servicos.Count() == 0)
                        servicos = new List<Nfe.Servico.NotaFiscal>();

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
                        if (filename.Contains(".xml"))//&& filename.Contains("procNFE"))
                        {
                            FileNfeDanfe nfed = new FileNfeDanfe(file);
                            if (nfed.notafiscal.idFornecedor != -1)
                            {
                                //if (nfed.notafiscal.Ide.nNf == 1441)
                                //{
                                //    FileNfeDanfe nfednew = new FileNfeDanfe(file);
                                //}
                                danfes.Add(nfed.notafiscal);
                            }
                            else
                            {
                                FileNfeServico nfes = new FileNfeServico(file);
                                if (nfes.notafiscal.Numero != -1)
                                    servicos.Add(nfes.notafiscal);
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

        //private void btnNotaServico_Click(object sender, EventArgs e)
        //{
        //    FolderBrowserDialog openServico = new FolderBrowserDialog();
        //    openServico.Description = "Localizar Notas";

        //    DialogResult dr = openServico.ShowDialog();
        //    if (dr == DialogResult.OK)
        //    {
        //        if (!string.IsNullOrEmpty(openServico.SelectedPath))
        //        {
        //            txtNotaServico.Text = openServico.SelectedPath;
        //            string[] files = System.IO.Directory.GetFiles(txtNotaServico.Text);

        //            if (servicos == null || servicos.Count() == 0)
        //                servicos = new List<Nfe.Servico.NotaFiscal>();

        //            string file;
        //            string[] split;
        //            string filename;
        //            progress.Maximum = files.Length;
        //            lblProgress.Text = "Lendo 0 de " + files.Length.ToString() + "...";
        //            for (int i = 0; i < files.Length; i++)
        //            {
        //                file = files[i];
        //                split = file.Split(new string[] { "\\" }, StringSplitOptions.None);
        //                filename = split[split.Length - 1];
        //                string nome;
        //                if (filename.Length > 40)
        //                {
        //                    string comeco = filename.Substring(0, 17);
        //                    string meio = "...";
        //                    string fim = filename.Substring(filename.Length - 20, 20);
        //                    nome = comeco + meio + fim;
        //                }
        //                else
        //                    nome = filename;
        //                lblProgress.Text = "Lendo " + i.ToString() + " de " + files.Length.ToString() + "... " + "Arquivo: " + nome;
        //                lblProgress.Refresh();
        //                //7234_NFSeNotaFiscaldeServiþosEletr¶nica_000900.xml
        //                if (filename.Contains(".xml"))
        //                {
        //                    FileNfeServico nfe = new FileNfeServico(file);
        //                    if (nfe.notafiscal.Numero != -1)
        //                    {
        //                        lstServicos.Items.Add(nfe.notafiscal.nomeArquivo);
        //                        lstServicos.Refresh();
        //                        servicos.Add(nfe.notafiscal);
        //                    }
        //                }
        //                progress.Increment(1);
        //            }
        //            progress.Value = 0;
        //            lblProgress.Text = "Serviços carregados...";
        //        }
        //    }
        //    else
        //        MessageBox.Show("Por favor, selecione um local onde está as notas de vendas.");
        //}

        private void btnContasReceber_Click(object sender, EventArgs e)
        {
            OpenFileDialog openContasReceber = new OpenFileDialog();
            openContasReceber.InitialDirectory = @"C:\Users\henriquec\Desktop";
            openContasReceber.RestoreDirectory = true;
            openContasReceber.Title = "Localizar Contas Receber";
            openContasReceber.DefaultExt = "xlsx";
            openContasReceber.Filter = "Arquivos Excel (*.xlsx)|*.xlsx|Arquivos Excel (*.xls)|*.xls|Arquivos Excel (*.csv)|*.csv|Todos Arquivos (*.*)|*.*";
            openContasReceber.FilterIndex = 0;
            openContasReceber.CheckFileExists = true;
            openContasReceber.CheckPathExists = true;
            openContasReceber.Multiselect = false;

            DialogResult dr = openContasReceber.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtContasReceber.Text = openContasReceber.FileName;
                if (!string.IsNullOrEmpty(txtContasReceber.Text))
                {
                    FileContaReceber file = new FileContaReceber(txtContasReceber.Text, true, "Original");
                    if (file.contas != null && file.contas.Count > 0)
                        contasReceber = file.contas;
                    else
                        contasReceber = file.GetContas(true);
                }
            }
        }

        private void btnCliente_Click(object sender, EventArgs e)
        {
            OpenFileDialog openClientes = new OpenFileDialog();
            openClientes.InitialDirectory = @"C:\Users\henriquec\Desktop";
            openClientes.RestoreDirectory = true;
            openClientes.Title = "Localizar Clientes";
            openClientes.DefaultExt = "xlsx";
            openClientes.Filter = "Arquivos Excel (*.xlsx)|*.xlsx|Arquivos Excel (*.xls)|*.xls|Arquivos Excel (*.csv)|*.csv|Todos Arquivos (*.*)|*.*";
            openClientes.FilterIndex = 0;
            openClientes.CheckFileExists = true;
            openClientes.CheckPathExists = true;
            openClientes.Multiselect = false;

            DialogResult dr = openClientes.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtCliente.Text = openClientes.FileName;
                if (!string.IsNullOrEmpty(txtCliente.Text))
                {
                    FileCliente file = new FileCliente(txtCliente.Text, "Plan1");
                    if (file.clientes != null && file.clientes.Count > 0)
                        clientes = file.clientes;
                    else
                        clientes = file.GetClientes();
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (clientes != null && clientes.Count > 0 &&
                contasReceber != null && contasReceber.Count > 0)
            {
                fechamento = new Fechamento(clientes, contasReceber);
                Cliente();
                Contas();
                panelNotas.Enabled = true;
            }
        }

        public void Cliente()
        {
            if (fechamento == null)
                fechamento = new Fechamento(clientes, contasReceber);

            linhasContribuinte = new List<string>();
            List<Cliente> clienteContribuinte = fechamento.GetClientesContribuintes();
            linhasNContribuinte = new List<string>();
            List<Cliente> clienteNContribuinte = fechamento.GetClientesNContribuintes();
            linhasLei = new List<string>();
            List<Cliente> clienteLei = fechamento.GetClientesLei();
            linhasNLei = new List<string>();
            List<Cliente> clienteNLei = fechamento.GetClientesNLei();

            NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;

            List<ContaReceber> crContribuinte = contasReceber.FindAll(cr => clienteContribuinte.Exists(c => c.codigo == cr.codigo));
            txtContribuinte.Text = fechamento.SumValorRecebido(crContribuinte).ToString("C", nfi);

            List<ContaReceber> crNContribuinte = contasReceber.FindAll(cr => clienteNContribuinte.Exists(c => c.codigo == cr.codigo));
            txtNContribuinte.Text = fechamento.SumValorRecebido(crNContribuinte).ToString("C", nfi);

            txtPrivado.Text = (fechamento.SumValorRecebido(crContribuinte) + fechamento.SumValorRecebido(crNContribuinte)).ToString("C", nfi);

            List<ContaReceber> crLei = contasReceber.FindAll(cr => clienteLei.Exists(c => c.codigo == cr.codigo));
            txtLei.Text = fechamento.SumValorRecebido(crLei).ToString("C", nfi);

            List<ContaReceber> crNLei = contasReceber.FindAll(cr => clienteNLei.Exists(c => c.codigo == cr.codigo));
            txtNLei.Text = fechamento.SumValorRecebido(crNLei).ToString("C", nfi);

            txtPublico.Text = (fechamento.SumValorRecebido(crLei) + fechamento.SumValorRecebido(crNLei)).ToString("C", nfi);

            txtTotal.Text = fechamento.SumValorRecebido(contasReceber).ToString("C", nfi);
        }

        public void Contas()
        {
            List<string> linhasContas = new List<string>();
            foreach (ContaReceber cr in contasReceber)
            {
                linhasContas.Add(cr.cliente + "|" + cr.documentoOrigem + "|" + cr.codigo + "|" + cr.docOrig + "|" + cr.notaFiscal + "|" + cr.prest + "|" + cr.emissao + "|" + cr.vencimento + "|" + cr.valor + "|" + cr.desconto + "|" + cr.juros + "|" + cr.valorRecebido + "|" + cr.dataRecto + "|" + cr.forma + "|" + cr.descricao);
            }
        }

        private void gravalinhasNoArquivo(string path, List<string> linhas)
        {
            // This text is added only once to the file.
            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to.

                // File.WriteAllLines(path, linhas, Encoding.UTF8);
                System.IO.File.WriteAllLines(path, linhas, Encoding.Unicode);
            }
        }

        private void btnOkNotas_Click(object sender, EventArgs e)
        {
            dgvNotasRecebidas.Rows.Clear();
            dgvNotasNRecebidas.Rows.Clear();

            if (danfes != null && danfes.Count > 0 &&
                servicos != null && servicos.Count > 0)
            {
                NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;

                fechamento = new Fechamento(clientes, contasReceber, danfes, servicos);
                List<Cliente> clienteContribuinte = fechamento.GetClientesContribuintes();
                List<Cliente> clienteNContribuinte = fechamento.GetClientesNContribuintes();
                List<Cliente> clienteLei = fechamento.GetClientesLei();
                List<Cliente> clienteNLei = fechamento.GetClientesNLei();

                List<Nfe.Danfe.NotaFiscal> danfesrecebidas = new List<Nfe.Danfe.NotaFiscal>();
                List<Nfe.Servico.NotaFiscal> servicosrecebidos = new List<Nfe.Servico.NotaFiscal>();
                List<Nfe.Danfe.NotaFiscal> danfesNRecebidas = new List<Nfe.Danfe.NotaFiscal>();
                List<Nfe.Servico.NotaFiscal> servicosNRecebidos = new List<Nfe.Servico.NotaFiscal>();

                fechamento.GetDanfesNotas(ref danfesrecebidas, ref danfesNRecebidas);
                fechamento.GetServicosNotas(ref servicosrecebidos, ref servicosNRecebidos);

                double contri = 0;
                double ncontri = 0;
                double lei = 0;
                double nlei = 0;
                dgvNotasRecebidas.Columns.Add("Numero", "Numero");
                dgvNotasRecebidas.Columns.Add("Valor", "Valor");
                dgvNotasRecebidas.Columns.Add("Tributos", "Tributos");
                dgvNotasRecebidas.Columns.Add("Tipo", "Tipo");
                dgvNotasRecebidas.Columns.Add("Cliente", "Cliente");
                foreach (Nfe.Danfe.NotaFiscal nt in danfesrecebidas)
                {
                    List<ContaReceber> contas = contasReceber.FindAll(f => Convert.ToInt32(f.notaFiscal.Split(new string[] { " " }, StringSplitOptions.None)[1]) == nt.Ide.nNf);
                    Cliente cliente = clientes.Find(f => f.codigo == contas[0].codigo);
                    if (cliente.codigoGrupo == "CONTRIBUINTE")
                        contri = contri + nt.Total.vNF;
                    else if (cliente.codigoGrupo == "NÃO CONTRIBUINTE")
                        ncontri = ncontri + nt.Total.vNF;
                    else if (cliente.codigoGrupo == "CONTRIB - Lei 9718")
                        lei = lei + nt.Total.vNF;
                    else if (cliente.codigoGrupo == "Ñ CONTRIB - Lei 9718")
                        nlei = nlei + nt.Total.vNF;

                    if (nt.Total.vTotTrib == -1)
                        dgvNotasRecebidas.Rows.Add(nt.Ide.nNf, nt.Total.vNF.ToString("C", nfi), nt.Total.vICMS.ToString("C", nfi), "Danfe", cliente.codigoGrupo);
                    else
                        dgvNotasRecebidas.Rows.Add(nt.Ide.nNf, nt.Total.vNF.ToString("C", nfi), nt.Total.vTotTrib.ToString("C", nfi), "Danfe", cliente.codigoGrupo);
                }
                foreach (Nfe.Servico.NotaFiscal nt in servicosrecebidos)
                {
                    List<ContaReceber> contas = contasReceber.FindAll(f => Convert.ToInt32(f.notaFiscal.Split(new string[] { " " }, StringSplitOptions.None)[1]) == nt.Numero);
                    Cliente cliente = clientes.Find(f => f.codigo == contas[0].codigo);
                    if (cliente.codigoGrupo == "CONTRIBUINTE")
                        contri = contri + nt.Servico.Valores.ValorServicos;
                    else if (cliente.codigoGrupo == "NÃO CONTRIBUINTE")
                        ncontri = ncontri + nt.Servico.Valores.ValorServicos;
                    else if (cliente.codigoGrupo == "CONTRIB - Lei 9718")
                        lei = lei + nt.Servico.Valores.ValorServicos;
                    else if (cliente.codigoGrupo == "Ñ CONTRIB - Lei 9718")
                        nlei = nlei + nt.Servico.Valores.ValorServicos;

                    dgvNotasRecebidas.Rows.Add(nt.Numero, nt.Servico.Valores.ValorServicos.ToString("C", nfi), nt.Servico.Valores.ValorIssRetido.ToString("C", nfi), "Serviço", cliente.codigoGrupo);
                }
                txtContribuinteRec.Text = contri.ToString("C", nfi);
                txtNContribuinteRec.Text = ncontri.ToString("C", nfi);
                txtLeiRec.Text = lei.ToString("C", nfi);
                txtNLeiRec.Text = nlei.ToString("C", nfi);
                txtPrivadoRec.Text = (contri + ncontri).ToString("C", nfi);
                txtPublicoRec.Text = (lei + nlei).ToString("C", nfi);
                txtTotalRec.Text = (contri + ncontri + lei + nlei).ToString("C", nfi);
                
                contri = 0;
                ncontri = 0;
                lei = 0;
                nlei = 0;
                double total = 0;
                dgvNotasNRecebidas.Columns.Add("Numero", "Numero");
                dgvNotasNRecebidas.Columns.Add("Valor", "Valor");
                dgvNotasNRecebidas.Columns.Add("Tributos", "Tributos");
                dgvNotasNRecebidas.Columns.Add("Tipo", "Tipo");
                foreach (Nfe.Danfe.NotaFiscal nt in danfesNRecebidas)
                {
                    List<ContaReceber> contas = contasReceber.FindAll(f => Convert.ToInt32(f.notaFiscal.Split(new string[] { " " }, StringSplitOptions.None)[1]) == nt.Ide.nNf);
                    if (contas.Count > 0)
                    {
                        Cliente cliente = clientes.Find(f => f.codigo == contas[0].codigo);
                        if (cliente.codigoGrupo == "CONTRIBUINTE")
                            contri = contri + nt.Total.vNF;
                        else if (cliente.codigoGrupo == "NÃO CONTRIBUINTE")
                            ncontri = ncontri + nt.Total.vNF;
                        else if (cliente.codigoGrupo == "CONTRIB - Lei 9718")
                            lei = lei + nt.Total.vNF;
                        else if (cliente.codigoGrupo == "Ñ CONTRIB - Lei 9718")
                            nlei = nlei + nt.Total.vNF;
                    }
                    total = total + nt.Total.vNF;

                    if (nt.Total.vTotTrib == -1)
                        dgvNotasNRecebidas.Rows.Add(nt.Ide.nNf, nt.Total.vNF.ToString("C", nfi), nt.Total.vICMS.ToString("C", nfi), "Danfe");
                    else
                        dgvNotasNRecebidas.Rows.Add(nt.Ide.nNf, nt.Total.vNF.ToString("C", nfi), nt.Total.vTotTrib.ToString("C", nfi), "Danfe");
                }
                foreach (Nfe.Servico.NotaFiscal nt in servicosNRecebidos)
                {
                    List<ContaReceber> contas = contasReceber.FindAll(f => Convert.ToInt32(f.notaFiscal.Split(new string[] { " " }, StringSplitOptions.None)[1]) == nt.Numero);
                    if (contas.Count > 0)
                    {
                        Cliente cliente = clientes.Find(f => f.codigo == contas[0].codigo);
                        if (cliente.codigoGrupo == "CONTRIBUINTE")
                            contri = contri + nt.Servico.Valores.ValorServicos;
                        else if (cliente.codigoGrupo == "NÃO CONTRIBUINTE")
                            ncontri = ncontri + nt.Servico.Valores.ValorServicos;
                        else if (cliente.codigoGrupo == "CONTRIB - Lei 9718")
                            lei = lei + nt.Servico.Valores.ValorServicos;
                        else if (cliente.codigoGrupo == "Ñ CONTRIB - Lei 9718")
                            nlei = nlei + nt.Servico.Valores.ValorServicos;
                    }
                    total = total + nt.Servico.Valores.ValorServicos;
                    dgvNotasNRecebidas.Rows.Add(nt.Numero, nt.Servico.Valores.ValorServicos.ToString("C", nfi), nt.Servico.Valores.ValorIss.ToString("C", nfi), "Serviço");
                }

                txtContribuinteDif.Text = (Convert.ToDouble(txtContribuinte.Text.Replace("R", "").Replace("$", "").Replace(" ", "")) -
                    Convert.ToDouble(txtContribuinteRec.Text.Replace("R", "").Replace("$", "").Replace(" ", ""))).ToString("C", nfi);
                txtNContribuinteDif.Text = (Convert.ToDouble(txtNContribuinte.Text.Replace("R", "").Replace("$", "").Replace(" ", "")) -
                    Convert.ToDouble(txtNContribuinteRec.Text.Replace("R", "").Replace("$", "").Replace(" ", ""))).ToString("C", nfi);
                txtLeiDif.Text = (Convert.ToDouble(txtLei.Text.Replace("R", "").Replace("$", "").Replace(" ", "")) -
                    Convert.ToDouble(txtLeiRec.Text.Replace("R", "").Replace("$", "").Replace(" ", ""))).ToString("C", nfi);
                txtNLeiDif.Text = (Convert.ToDouble(txtNLei.Text.Replace("R", "").Replace("$", "").Replace(" ", "")) -
                    Convert.ToDouble(txtNLeiRec.Text.Replace("R", "").Replace("$", "").Replace(" ", ""))).ToString("C", nfi);
                txtPrivadoDif.Text = (Convert.ToDouble(txtPrivado.Text.Replace("R", "").Replace("$", "").Replace(" ", "")) -
                    Convert.ToDouble(txtPrivadoRec.Text.Replace("R", "").Replace("$", "").Replace(" ", ""))).ToString("C", nfi);
                txtPublicoDif.Text = (Convert.ToDouble(txtPublico.Text.Replace("R", "").Replace("$", "").Replace(" ", "")) -
                    Convert.ToDouble(txtPublicoRec.Text.Replace("R", "").Replace("$", "").Replace(" ", ""))).ToString("C", nfi);
                txtTotalDif.Text = (Convert.ToDouble(txtTotal.Text.Replace("R", "").Replace("$", "").Replace(" ", "")) -
                    Convert.ToDouble(txtTotalRec.Text.Replace("R", "").Replace("$", "").Replace(" ", ""))).ToString("C", nfi);

                JavaScriptSerializer jss = new JavaScriptSerializer();
                string res = jss.Serialize(danfesrecebidas);

                txtTotalAllNRec.Text = total.ToString("C", nfi);
            }
        }

        private void txtNotaServico_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblNotaServico_Click(object sender, EventArgs e)
        {

        }

        private void lstDanfes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblDanfes_Click(object sender, EventArgs e)
        {

        }

        private void lstServicos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtLei_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblLei_Click(object sender, EventArgs e)
        {

        }

        private void FrmNfe_Load(object sender, EventArgs e)
        {
            //Assecontweb.Extend.Path path = new Assecontweb.Extend.Path("D:\\BitBucket\\Assecont\\Projetos\\Assecontweb.Extend");
            //Assecontweb.Extend.PathNfeDanfe pdanfe = new PathNfeDanfe("C:\\Users\\henriquec\\Desktop\\NotasFiscais");
        }

        private void lblCliente_Click(object sender, EventArgs e)
        {

        }

        private void txtCliente_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void txtContasReceber_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtContribuinte_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
