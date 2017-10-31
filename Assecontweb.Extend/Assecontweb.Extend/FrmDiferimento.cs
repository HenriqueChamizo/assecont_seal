using Assecontweb.Extend.CFechamento;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace Assecontweb.Extend
{
    public partial class FrmDiferimento : Form
    {
        Fechamento fechamento;
        List<Cliente> clientes;
        List<ContaReceber> contasReceber;
        List<string> linhasContribuinte;
        List<string> linhasNContribuinte;
        List<string> linhasLei;
        List<string> linhasNLei;
        List<string> linhasErro;

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
            openContasReceber.Filter = "Arquivos Excel (*.xlsx)|*.xlsx|Arquivos Excel (*.xls)|*.xls|Arquivos Excel (*.csv)|*.csv|Todos Arquivos (*.*)|*.*";
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
                    FileContaReceber file = new FileContaReceber(txtContasReceber.Text, "Original");
                    if (file.contas != null && file.contas.Count > 0)
                        contasReceber = file.contas;
                    else
                        contasReceber = file.GetContas();
                }
            }
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            OpenFileDialog openClientes = new OpenFileDialog();
            openClientes.InitialDirectory = @"C:\";
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
            }
        }

        public void Cliente()
        {
            string path = @"C:\Users\henriquec\Desktop\FechamentoSeal\";
            string Contribuinte = @"Cliente\Contribuinte.txt";
            string NContribuinte = @"Cliente\NContribuinte.txt";
            string Lei = @"Cliente\Lei.txt";
            string NLei = @"Cliente\NLei.txt";
            string Erro = @"Cliente\Erro.txt";

            linhasContribuinte = new List<string>();
            List<Cliente> clienteContribuinte = new List<Cliente>();
            linhasNContribuinte = new List<string>();
            List<Cliente> clienteNContribuinte = new List<Cliente>();
            linhasLei = new List<string>();
            List<Cliente> clienteLei = new List<Cliente>();
            linhasNLei = new List<string>();
            List<Cliente> clienteNLei = new List<Cliente>();
            linhasErro = new List<string>();
            List<Cliente> clienteErro = new List<Cliente>();

            foreach (Cliente cli in clientes)
            {
                if (cli.codigoGrupo == "CONTRIBUINTE")
                {
                    linhasContribuinte.Add(cli.codigo + "|" + cli.nome + "|" + cli.tipo + "|" + cli.codigoGrupo);
                    clienteContribuinte.Add(cli);
                }
                else if (cli.codigoGrupo == "NÃO CONTRIBUINTE")
                {
                    linhasNContribuinte.Add(cli.codigo + "|" + cli.nome + "|" + cli.tipo + "|" + cli.codigoGrupo);
                    clienteNContribuinte.Add(cli);
                }
                else if (cli.codigoGrupo == "CONTRIB - Lei 9718")
                {
                    linhasLei.Add(cli.codigo + "|" + cli.nome + "|" + cli.tipo + "|" + cli.codigoGrupo);
                    clienteLei.Add(cli);
                }
                else if (cli.codigoGrupo == "Ñ CONTRIB - Lei 9718")
                {
                    linhasNLei.Add(cli.codigo + "|" + cli.nome + "|" + cli.tipo + "|" + cli.codigoGrupo);
                    clienteNLei.Add(cli);
                }
                else
                    linhasErro.Add(cli.codigo + "|" + cli.nome + "|" + cli.tipo + "|" + cli.codigoGrupo);
            }

            if (fechamento == null)
                fechamento = new Fechamento(clientes, contasReceber);

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

            gravalinhasNoArquivo(path + Contribuinte, linhasContribuinte);
            gravalinhasNoArquivo(path + NContribuinte, linhasNContribuinte);
            gravalinhasNoArquivo(path + Lei, linhasLei);
            gravalinhasNoArquivo(path + NLei, linhasLei);
            gravalinhasNoArquivo(path + Erro, linhasErro);
        }

        public void Contas()
        {
            string path = @"C:\Users\henriquec\Desktop\FechamentoSeal\";
            string contaReceber = "ContasReceber.txt";
            List<string> linhasContas = new List<string>();
            foreach (ContaReceber cr in contasReceber)
            {
                linhasContas.Add(cr.cliente + "|" + cr.documentoOrigem + "|" + cr.codigo + "|" + cr.docOrig + "|" + cr.notaFiscal + "|" + cr.prest + "|" + cr.emissao + "|" + cr.vencimento + "|" + cr.valor + "|" + cr.desconto + "|" + cr.juros + "|" + cr.valorRecebido + "|" + cr.dataRecto + "|" + cr.forma + "|" + cr.descricao);
            }
            gravalinhasNoArquivo(path + contaReceber, linhasContas);
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
    }
}
