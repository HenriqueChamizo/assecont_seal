using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assecontweb.Extend;
using Assecontweb.Extend.CFechamento;
using System.IO;
using System.Globalization;

namespace Diferimento
{
    public partial class FrmClientes : Form
    {
        public Fechamento fechamento;
        public List<Cliente> clientes;
        public List<ContaReceber> contasReceber;
        public Model.Seal.Documento documento;

        File clientesFile;
        File contasFile;

        public FrmClientes()
        {
            InitializeComponent();
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

            clientesFile = null;
            DialogResult dr = openClientes.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtCliente.Text = openClientes.FileName;
                if (!string.IsNullOrEmpty(txtCliente.Text))
                {
                    FileCliente file = new FileCliente(txtCliente.Text, "Atualização de Parceiros de Neg");//"Plan1");
                    if (file.clientes != null && file.clientes.Count > 0)
                        clientes = file.clientes;
                    else
                        clientes = file.GetClientes();
                    
                    File homefile = new File();
                    homefile.path = file.path;
                    homefile.fileName = file.filename;
                    Stream stream = new StreamReader(homefile.path).BaseStream;
                    homefile.buffer = new byte[stream.Length];
                    stream.Read(homefile.buffer, 0, homefile.buffer.Length);
                    stream.Close();
                    stream.Dispose();

                    clientesFile = homefile;
                }
            }
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

            contasFile = null;
            DialogResult dr = openContasReceber.ShowDialog();
            if (dr == DialogResult.OK)
            {
                txtContasReceber.Text = openContasReceber.FileName;
                if (!string.IsNullOrEmpty(txtContasReceber.Text))
                {
                    cbAntigo.Enabled = false;
                    FileContaReceber file = new FileContaReceber(txtContasReceber.Text, cbAntigo.Checked, "Original");
                    if (file.contas != null && file.contas.Count > 0)
                        contasReceber = file.contas;
                    else
                        contasReceber = file.GetContas(cbAntigo.Checked);

                    File homefile = new File();
                    homefile.path = file.path;
                    homefile.fileName = file.filename;
                    Stream stream = new StreamReader(homefile.path).BaseStream;
                    homefile.buffer = new byte[stream.Length];
                    stream.Read(homefile.buffer, 0, homefile.buffer.Length);
                    stream.Close();
                    stream.Dispose();

                    contasFile = homefile;
                }
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            btnCliente.Enabled = false;
            btnSalvar.Enabled = false;
            string erro = "";
            //Model.Seal.Documento documento = new Model.Seal.Documento();
            Model.Seal.DocumentoDados documentoDados = new Model.Seal.DocumentoDados();
            documento = new Model.Seal.Documento();
            try
            {
                List<Model.Seal.Cliente> clientes;
                List<Model.Seal.ContaReceber> contas;

                DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
                File FileClientes = clientesFile;
                File FileContas = contasFile;

                if (FileClientes == null)
                {
                    DAO.ClienteDAO cDAO = new DAO.ClienteDAO();
                    clientes = new List<Model.Seal.Cliente>();
                    cDAO.GetClientes(ref clientes, out erro);
                }
                else
                {
                    documento.doc_des_nameclientes = FileClientes.fileName;
                    documento.doc_file_clientes = FileClientes.buffer;

                    FileCliente fileCliente = new FileCliente(FileClientes.path, "Plan1");
                    clientes = Convert.Clientes(fileCliente.clientes);
                }

                if (FileContas == null)
                {
                    DAO.ContaReceberDAO dDAO = new DAO.ContaReceberDAO();
                    contas = new List<Model.Seal.ContaReceber>();
                    dDAO.GetContaReceberByDateRec(ref contas, out erro);
                }
                else
                {
                    documento.doc_des_namecontasreceber = FileContas.fileName;
                    documento.doc_file_contasreceber = FileContas.buffer;

                    FileContaReceber fileContas = new FileContaReceber(FileContas.path, cbAntigo.Checked, "Original");
                    contas = Convert.ContasReceber(fileContas.contas);
                }
                
                //Monta objeto documento
                documento.doc_des_descricao = null;
                documento.doc_dt_data = DateTime.Now;
                //Salva no banco e retorna o objeto com indice
                documento.doc_int_id = documentoDados.doc_int_id = DAO.SetDocumento(documento, out erro);

                List<Model.Seal.Cliente> clienteContribuinte = new List<Model.Seal.Cliente>();
                List<Model.Seal.Cliente> clienteNContribuinte = new List<Model.Seal.Cliente>();
                List<Model.Seal.Cliente> clienteLei = new List<Model.Seal.Cliente>();
                List<Model.Seal.Cliente> clienteNLei = new List<Model.Seal.Cliente>();
                List<Model.Seal.Cliente> clienteErro = new List<Model.Seal.Cliente>();

                foreach (Model.Seal.Cliente cli in clientes)
                {
                    if (cli.codigoGrupo == "CONTRIBUINTE")
                        clienteContribuinte.Add(cli);
                    else if (cli.codigoGrupo == "NÃO CONTRIBUINTE")
                        clienteNContribuinte.Add(cli);
                    else if (cli.codigoGrupo == "CONTRIB - Lei 9718")
                        clienteLei.Add(cli);
                    else if (cli.codigoGrupo == "Ñ CONTRIB - Lei 9718")
                        clienteNLei.Add(cli);
                }

                Fechamento fechamento = new Fechamento(Convert.ClientesInverse(clientes), Convert.ContasReceberInverse(contas));

                NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;

                List<Model.Seal.ContaReceber> crContribuinte = contas.FindAll(cr => clienteContribuinte.Exists(c => c.codigo == cr.codigo));
                documentoDados.dcd_num_privadocontribuinte = System.Convert.ToDouble(fechamento.SumValorRecebido(Convert.ContasReceberInverse(crContribuinte)));

                List<Model.Seal.ContaReceber> crNContribuinte = contas.FindAll(cr => clienteNContribuinte.Exists(c => c.codigo == cr.codigo));
                documentoDados.dcd_num_privadoncontribuinte = System.Convert.ToDouble(fechamento.SumValorRecebido(Convert.ContasReceberInverse(crNContribuinte)));

                List<Model.Seal.ContaReceber> crLei = contas.FindAll(cr => clienteLei.Exists(c => c.codigo == cr.codigo));
                documentoDados.dcd_num_publicocontribuinte = System.Convert.ToDouble(fechamento.SumValorRecebido(Convert.ContasReceberInverse(crLei)));

                List<Model.Seal.ContaReceber> crNLei = contas.FindAll(cr => clienteNLei.Exists(c => c.codigo == cr.codigo));
                documentoDados.dcd_num_publiconcontribuinte = System.Convert.ToDouble(fechamento.SumValorRecebido(Convert.ContasReceberInverse(crNLei)));

                DAO.SetDocumentoDados(documentoDados, out erro);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Models.SealException ex = new Models.SealException(this, e);
                //ex.SendEmail();
            }

            if (clientes != null)
            {
                DAO.ClienteDAO cliDAO = new DAO.ClienteDAO();
                if (!cliDAO.SetClientes(Diferimento.Convert.Clientes(clientes), out erro))
                    MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (contasReceber != null)
            {
                DAO.ContaReceberDAO conDAO = new DAO.ContaReceberDAO();
                if (!conDAO.SetContaReceber(Diferimento.Convert.ContasReceber(contasReceber), documento.doc_int_id, out erro))
                    MessageBox.Show(erro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Cursor = Cursors.Default;
            btnCliente.Enabled = true;
            btnSalvar.Enabled = true;
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
