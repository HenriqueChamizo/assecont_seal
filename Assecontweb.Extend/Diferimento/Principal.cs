using Assecontweb.Extend;
using Assecontweb.Extend.CFechamento;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diferimento
{
    public enum StatusPrincipal
    {
        [Description("Sistema Ok")]
        SistemaOK = 0,
        [Description("Carregando Informações")]
        Carregando = 1,
        [Description("Processando Informações")]
        Processando = 2,
        [Description("Reprocessando dados..")]
        Reprocessando = 3
    }
    public enum StatusDadosPrincipal
    {
        [Description("Detail")]
        Detail = 0,
        [Description("Consolidado")]
        Consolidado = 1
    }

    public partial class Principal : Form
    {
        //Fechamento fechamento;
        List<Cliente> clientes;
        List<ContaReceber> contasReceber;
        public List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes;
        public List<Assecontweb.Extend.Nfe.Servico.NotaFiscal> servicos;
        StatusPrincipal status;
        StatusDadosPrincipal statusdados;
        NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;
        DAO.Detail detail;
        DAO.Consolidado conso;
        string erro;
        bool periodoFinalizado = false;

        public Principal()
        {
            InitializeComponent();
            status = StatusPrincipal.SistemaOK;
        }
        
        private void Principal_Load(object sender, EventArgs e)
        {
            DateTime inicio = System.Convert.ToDateTime("1/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString());
            DateTime fim = System.Convert.ToDateTime(DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString());
            dateTimeInicio.Value = inicio;
            dateTimeFim.Value = fim;
            

            timer1.Start();
            timer1.Tick += Timer1_Tick;
        }

        private void importarClientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmClientes frm = new FrmClientes();
            this.Cursor = Cursors.WaitCursor;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                clientes = frm.clientes;
                contasReceber = frm.contasReceber;
            }
            this.Cursor = Cursors.Default;
        }

        private void importarXmlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmNotasXml frm = new FrmNotasXml();
            if (clientes == null)
            {
                string erro = "";
                List<Model.Seal.Cliente> clientesmodel = new List<Model.Seal.Cliente>();
                DAO.ClienteDAO cliDAO = new DAO.ClienteDAO();
                cliDAO.GetClientes(ref clientesmodel, out erro);
                clientes = Convert.ClientesInverse(clientesmodel);
                frm.clientes = clientesmodel;
            }
            else
                frm.clientes = Convert.Clientes(clientes);

            if (contasReceber == null)
            {
                string erro = "";
                List<Model.Seal.ContaReceber> contasRecebermodel = new List<Model.Seal.ContaReceber>();
                DAO.ContaReceberDAO ctDAO = new DAO.ContaReceberDAO();
                ctDAO.GetContaReceber(ref contasRecebermodel, out erro);
                contasReceber = Convert.ContasReceberInverse(contasRecebermodel);
                frm.contasReceber = contasRecebermodel;
            }
            else
                frm.contasReceber = Convert.ContasReceber(contasReceber);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                string erro = "";

                danfes = frm.danfes;
                servicos = frm.servicos;
            }
        }

        private void importarTxtsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmNotasTxt frm = new FrmNotasTxt();
            this.Cursor = Cursors.WaitCursor;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                string erro = "";
                //danfes = frm.danfes;
                servicos = frm.servicos;
            }
            this.Cursor = Cursors.Default;
        }

        private void tabControlSelectedIndexChanged_Click(object sender, EventArgs e)
        {
            if (tabControlPrincipal.SelectedIndex == 0)
            {
                //Matrix
                Task task = new Task(CarregarDetail);
                task.Start();
                //CarregarDetail();
            }
            else if (tabControlPrincipal.SelectedIndex == 1)
            {
                //Consolidado
                Task task = new Task(CarregarConsolidado);
                task.Start();
                //CarregarConsolidado();
            }
            else if (tabControlPrincipal.SelectedIndex == 2)
            {
                //Diferimento
                //CarregarConsolidado();
            }
            else
                MessageBox.Show("Erro na lista de Tabs", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnReprocessar_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            Reprocessar();
            Cursor = Cursors.Default;
        }

        private void Reprocessar()
        {
            status = StatusPrincipal.Reprocessando;
            string erro = "";
            List<Model.Seal.ContaReceber> contasbd = new List<Model.Seal.ContaReceber>();
            List<Model.Seal.ContaReceber> contasdanfes = new List<Model.Seal.ContaReceber>();
            List<Model.Seal.ContaReceber> contasservicos = new List<Model.Seal.ContaReceber>();
            DAO.ContaReceberDAO cDAO = new DAO.ContaReceberDAO();
            if (cDAO.GetContasReceberNaoProcessadas(ref contasbd, out erro))
            {
                List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes = new List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal>();
                List<Assecontweb.Extend.Nfe.Servico.NotaFiscal> servicos = new List<Assecontweb.Extend.Nfe.Servico.NotaFiscal>();

                DAO.NfeDAO nDAO = new DAO.NfeDAO();
                if (nDAO.GetServicosNaoProcessados(ref servicos, out erro))
                {
                    Model.Seal.NotaFiscalRecebidas recebida;
                    foreach (Model.Seal.ContaReceber conta in contasbd)
                    {
                        Assecontweb.Extend.Nfe.Servico.NotaFiscal servico = servicos.Find(f => (f.Numero == System.Convert.ToInt32(conta.notaFiscal)) &&
                                                                                               //(f.TomadorServico.IdentificadorTomador.CpfCnpj.Cnpj == conta.docOrig) && 
                                                                                               (System.Convert.ToDateTime(f.DataEmissao).ToString("dd/MM/yyyy") == conta.emissao.ToString("dd/MM/yyyy")));
                        if (servico != null)
                        {
                            recebida = new Model.Seal.NotaFiscalRecebidas();
                            recebida.nfd_int_id = 0;
                            recebida.nfs_int_id = servico.nfd_int_id;
                            conta.nfr_int_id = recebida;
                            contasservicos.Add(conta);
                        }
                    }

                    //contasservicos = contasbd.FindAll(f => f.nfr_int_id != null);
                    if (contasservicos.Count > 0)
                    {
                        if (!cDAO.SetRecebidas(contasservicos, out erro))
                            MessageBox.Show(erro, "Erro em Reprocessamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Reprocessamento concluído com sucesso", "SERVIÇOS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                        MessageBox.Show("Sem Contas (SERVIÇOS) a serem processadas", "Reprocessamento SERVIÇOS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                    MessageBox.Show(erro, "Erro em Notas Fiscais (SERVICOS)", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //contasbd = contasbd.FindAll(f => f.nfr_int_id == null);
                contasbd.RemoveAll(r => contasservicos.Contains(r));

                if (nDAO.GetDanfesNaoProcessadas(ref danfes, out erro))
                {
                    Model.Seal.NotaFiscalRecebidas recebida;
                    foreach (Model.Seal.ContaReceber conta in contasbd)
                    {
                        Assecontweb.Extend.Nfe.Danfe.NotaFiscal danfe = danfes.Find(f => (f.Ide.nNf == System.Convert.ToInt32(conta.notaFiscal)) &&
                                                                                         //(f.Dest.CNPJ == conta.docOrig || f.Dest.CPF == conta.docOrig) && 
                                                                                         (System.Convert.ToDateTime(f.Ide.dhEmi).ToString("dd/MM/yyyy") == conta.emissao.ToString("dd/MM/yyyy")));
                        if (danfe != null)
                        {
                            recebida = new Model.Seal.NotaFiscalRecebidas();
                            recebida.nfd_int_id = danfe.nfd_int_id;
                            recebida.nfs_int_id = 0;
                            conta.nfr_int_id = recebida;
                            contasdanfes.Add(conta);
                        }
                    }

                    //contasdanfes = contasbd.FindAll(f => f.nfr_int_id != null);
                    if (contasdanfes.Count > 0)
                    {
                        if (!cDAO.SetRecebidas(contasdanfes, out erro))
                            MessageBox.Show(erro, "Erro em Reprocessamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show("Reprocessamento concluído com sucesso", "DANFES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                        MessageBox.Show("Sem Contas (DANFES) a serem processadas", "Reprocessamento DANFES", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                    MessageBox.Show(erro, "Erro em Notas Fiscais (DANFES)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show(erro, "Erro em Contas a Receber", MessageBoxButtons.OK, MessageBoxIcon.Error);
            status = StatusPrincipal.SistemaOK;
        }

        private void ReprocessarCalculo()
        {
            status = StatusPrincipal.Reprocessando;
            DAO.CalculoDAO cDAO = new DAO.CalculoDAO();
            if (cDAO.SetCalculoByPeriodo(dateTimeInicio.Value, dateTimeFim.Value, out string erro))
            {
                Task task = new Task(CarregarDetail);
                task.Start();
            }
            else
                MessageBox.Show("Erro no reprocessamento do calculo", "Erro Calculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            status = StatusPrincipal.SistemaOK;
        }

        private void CarregarDetail()
        {
            statusdados = StatusDadosPrincipal.Detail;
            status = StatusPrincipal.Processando;
            erro = "";
            //DAO.NfeDAO cDao = new DAO.NfeDAO();
            DAO.CalculoDAO cDao = new DAO.CalculoDAO();
            detail = new DAO.Detail();
            
            CarregarFiles();
            //if (!cDao.GetFechamentoDetail(dateTimeInicio.Value, dateTimeFim.Value, ref detail, out erro))
            if (!cDao.GetCalculoDetail(ref detail, dateTimeInicio.Value, dateTimeFim.Value, out erro))
                MessageBox.Show("Erro ao calcular fechamento", "Erro de Calculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            status = StatusPrincipal.Carregando;

            #region Comments
            //double ItauPrivado = 0;
            //double BBPrivado = 0;
            //double CaixaPrivado = 0;
            //double BradescoPrivado = 0;
            //double DaycovalPrivado = 0;
            //double DescontosPrivado = 0;
            //double PISPrivado = 0;
            //double CSSLPrivado = 0;
            //double COFINSPrivado = 0;
            //double IRPrivado = 0;
            //double ISSPrivado = 0;
            //double TotalPrivado = 0;

            //double ItauPublico = 0;
            //double BBPublico = 0;
            //double CaixaPublico = 0;
            //double BradescoPublico = 0;
            //double DaycovalPublico = 0;
            //double DescontosPublico = 0;
            //double PISPublico = 0;
            //double CSSLPublico = 0;
            //double COFINSPublico = 0;
            //double IRPublico = 0;
            //double ISSPublico = 0;
            //double TotalPublico = 0;

            //if (!cDao.GetRelatorioMatrix(dateTimeInicio.Value, dateTimeFim.Value, ref ItauPrivado, ref BBPrivado, ref CaixaPrivado, ref BradescoPrivado, ref DaycovalPrivado, ref DescontosPrivado,
            //                      ref PISPrivado, ref CSSLPrivado, ref COFINSPrivado, ref IRPrivado, ref ISSPrivado, ref TotalPrivado, ref ItauPublico, ref BBPublico, ref CaixaPublico, ref BradescoPublico, 
            //                      ref DaycovalPublico, ref DescontosPublico, ref PISPublico, ref CSSLPublico, ref COFINSPublico, ref IRPublico, ref ISSPublico, ref TotalPublico, out erro))
            //    MessageBox.Show("Erro ao calcular fechamento", "Erro de Calculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //else
            //{
            //    //Privado
            //    lblMatrixRelPrivadoItau.Text = ItauPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoBB.Text = BBPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoCaixa.Text = CaixaPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoBradesco.Text = BradescoPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoDaycoval.Text = DaycovalPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoDesconto.Text = DescontosPrivado.ToString("C", nfi);

            //    lblMatrixRelPrivadoPis.Text = PISPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoCSSL.Text = CSSLPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoCofins.Text = COFINSPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoIr.Text = IRPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoIss.Text = ISSPrivado.ToString("C", nfi);

            //    //Publico
            //    lblMatrixRelPublicoItau.Text = ItauPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoBB.Text = BBPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoCaixa.Text = CaixaPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoBradesco.Text = BradescoPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoDaycoval.Text = DaycovalPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoDesconto.Text = DescontosPublico.ToString("C", nfi);

            //    lblMatrixRelPublicoPis.Text = PISPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoCSSL.Text = CSSLPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoCofins.Text = COFINSPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoIr.Text = IRPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoIss.Text = ISSPublico.ToString("C", nfi);
            //}
            #endregion
        }

        private void CarregarConsolidado()
        {
            statusdados = StatusDadosPrincipal.Consolidado;
            status = StatusPrincipal.Processando;
            erro = "";
            //DAO.NfeDAO cDao = new DAO.NfeDAO();
            DAO.CalculoDAO cDao = new DAO.CalculoDAO();
            conso = new DAO.Consolidado();

            CarregarFiles();
            //if (!cDao.GetFechamentoConsolidado(dateTimeInicio.Value, dateTimeFim.Value, ref conso, out erro))
            if (!cDao.GetCalculoConsolidado(ref conso, dateTimeInicio.Value, dateTimeFim.Value, out erro))
                MessageBox.Show("Erro ao calcular fechamento", "Erro de Calculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            status = StatusPrincipal.Carregando;

            #region Comments
            //double ItauPrivado = 0;
            //double BBPrivado = 0;
            //double CaixaPrivado = 0;
            //double BradescoPrivado = 0;
            //double DaycovalPrivado = 0;
            //double DescontosPrivado = 0;
            //double PISPrivado = 0;
            //double CSSLPrivado = 0;
            //double COFINSPrivado = 0;
            //double IRPrivado = 0;
            //double ISSPrivado = 0;
            //double TotalPrivado = 0;

            //double ItauPublico = 0;
            //double BBPublico = 0;
            //double CaixaPublico = 0;
            //double BradescoPublico = 0;
            //double DaycovalPublico = 0;
            //double DescontosPublico = 0;
            //double PISPublico = 0;
            //double CSSLPublico = 0;
            //double COFINSPublico = 0;
            //double IRPublico = 0;
            //double ISSPublico = 0;
            //double TotalPublico = 0;

            //if (!cDao.GetRelatorioMatrix(dateTimeInicio.Value, dateTimeFim.Value, ref ItauPrivado, ref BBPrivado, ref CaixaPrivado, ref BradescoPrivado, ref DaycovalPrivado, ref DescontosPrivado,
            //                      ref PISPrivado, ref CSSLPrivado, ref COFINSPrivado, ref IRPrivado, ref ISSPrivado, ref TotalPrivado, ref ItauPublico, ref BBPublico, ref CaixaPublico, ref BradescoPublico, 
            //                      ref DaycovalPublico, ref DescontosPublico, ref PISPublico, ref CSSLPublico, ref COFINSPublico, ref IRPublico, ref ISSPublico, ref TotalPublico, out erro))
            //    MessageBox.Show("Erro ao calcular fechamento", "Erro de Calculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //else
            //{
            //    //Privado
            //    lblMatrixRelPrivadoItau.Text = ItauPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoBB.Text = BBPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoCaixa.Text = CaixaPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoBradesco.Text = BradescoPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoDaycoval.Text = DaycovalPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoDesconto.Text = DescontosPrivado.ToString("C", nfi);

            //    lblMatrixRelPrivadoPis.Text = PISPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoCSSL.Text = CSSLPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoCofins.Text = COFINSPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoIr.Text = IRPrivado.ToString("C", nfi);
            //    lblMatrixRelPrivadoIss.Text = ISSPrivado.ToString("C", nfi);

            //    //Publico
            //    lblMatrixRelPublicoItau.Text = ItauPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoBB.Text = BBPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoCaixa.Text = CaixaPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoBradesco.Text = BradescoPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoDaycoval.Text = DaycovalPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoDesconto.Text = DescontosPublico.ToString("C", nfi);

            //    lblMatrixRelPublicoPis.Text = PISPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoCSSL.Text = CSSLPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoCofins.Text = COFINSPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoIr.Text = IRPublico.ToString("C", nfi);
            //    lblMatrixRelPublicoIss.Text = ISSPublico.ToString("C", nfi);
            //}
            #endregion
        }

        private void CarregarFiles()
        {
            //string erro = "";
            //int clientes = 0;
            //int recebidas = 0;
            //int danfes = 0;
            //int servicos = 0;
            //DAO.DocumentosDAO dDao = new DAO.DocumentosDAO();

            //if (!dDao.GetFilesCount(dateTimeInicio.Value, dateTimeFim.Value, ref clientes, ref recebidas, ref danfes, ref servicos, ref erro))
            //    MessageBox.Show("Erro ao calcular fechamento", "Erro de Calculo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //else
            //{
            //    string[] split = lblCliImport.Text.Split(new string[] { " " }, StringSplitOptions.None);
            //    lblCliImport.Text = clientes.ToString() + " " + split[1];

            //    split = lblContRecImport.Text.Split(new string[] { " " }, StringSplitOptions.None);
            //    lblContRecImport.Text = recebidas.ToString() + " " + split[1];

            //    split = lblNFDImport.Text.Split(new string[] { " " }, StringSplitOptions.None);
            //    lblNFDImport.Text = danfes.ToString() + " " + split[1];

            //    split = lblNFSImport.Text.Split(new string[] { " " }, StringSplitOptions.None);
            //    lblNFSImport.Text = servicos.ToString() + " " + split[1];
            //}
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            finalizarPeriodoToolStripMenuItem.Visible = !periodoFinalizado;
            switch (status)
            {
                case StatusPrincipal.SistemaOK:
                    lblStatus.Text = StatusPrincipal.SistemaOK.ToString();
                    lblStatus.ForeColor = Color.Blue;
                    break;
                case StatusPrincipal.Carregando:
                    lblStatus.Text = StatusPrincipal.Carregando.ToString();
                    lblStatus.ForeColor = Color.Green;
                    if (statusdados == StatusDadosPrincipal.Detail)
                    {
                        if (detail != null)
                        {
                            periodoFinalizado = detail.finalizado;
                            if (detail.finalizado) {
                                lblStatusPeriodo.Text = "Finalizado";
                                lblStatusPeriodo.ForeColor = Color.Red;
                            }
                            else
                            {
                                lblStatusPeriodo.Text = "Em Aberto";
                                lblStatusPeriodo.ForeColor = Color.LimeGreen;
                            }
                            #region Danfes
                            //MSMSMSMSMSMSMS
                            lblDetailMSDanfPrivado.Text = detail.DanfeFaturada.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfLei.Text = detail.DanfeFaturada.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfTot.Text = detail.DanfeFaturada.MSFatu.ToString("C", nfi);

                            lblDetailMSDanfPrivadoRec.Text = detail.DanfeFaturada.Recebidas.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfLeiRec.Text = detail.DanfeFaturada.Recebidas.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfTotRec.Text = detail.DanfeFaturada.Recebidas.MSFatu.ToString("C", nfi);

                            lblDetailMSDanfPrivadoNRec.Text = detail.DanfeFaturada.NRecebidas.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfLeiNRec.Text = detail.DanfeFaturada.NRecebidas.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfTotNRec.Text = detail.DanfeFaturada.NRecebidas.MSFatu.ToString("C", nfi);

                            lblDetailMSDanfPrivadoContri.Text = detail.DanfeFaturada.Contribuinte.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfLeiContri.Text = detail.DanfeFaturada.Contribuinte.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfTotContri.Text = detail.DanfeFaturada.Contribuinte.MSFatu.ToString("C", nfi);

                            lblDetailMSDanfPrivadoNContri.Text = detail.DanfeFaturada.NContribuinte.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfLeiNContri.Text = detail.DanfeFaturada.NContribuinte.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSDanfTotNContri.Text = detail.DanfeFaturada.NContribuinte.MSFatu.ToString("C", nfi);

                            //SPSPSPSPSPSPSP
                            lblDetailSPDanfPrivado.Text = detail.DanfeFaturada.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfLei.Text = detail.DanfeFaturada.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfTot.Text = detail.DanfeFaturada.SPFatu.ToString("C", nfi);

                            lblDetailSPDanfPrivadoRec.Text = detail.DanfeFaturada.Recebidas.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfLeiRec.Text = detail.DanfeFaturada.Recebidas.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfTotRec.Text = detail.DanfeFaturada.Recebidas.SPFatu.ToString("C", nfi);

                            lblDetailSPDanfPrivadoNRec.Text = detail.DanfeFaturada.NRecebidas.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfLeiNRec.Text = detail.DanfeFaturada.NRecebidas.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfTotNRec.Text = detail.DanfeFaturada.NRecebidas.SPFatu.ToString("C", nfi);

                            lblDetailSPDanfPrivadoContri.Text = detail.DanfeFaturada.Contribuinte.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfLeiContri.Text = detail.DanfeFaturada.Contribuinte.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfTotContri.Text = detail.DanfeFaturada.Contribuinte.SPFatu.ToString("C", nfi);

                            lblDetailSPDanfPrivadoNContri.Text = detail.DanfeFaturada.NContribuinte.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfLeiNContri.Text = detail.DanfeFaturada.NContribuinte.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPDanfTotNContri.Text = detail.DanfeFaturada.NContribuinte.SPFatu.ToString("C", nfi);

                            //Tptais
                            lblDetailDanfTot.Text = (detail.DanfeFaturada.MSFatu + detail.DanfeFaturada.SPFatu).ToString("C", nfi);
                            lblDetailDanfTotRec.Text = (detail.DanfeFaturada.Recebidas.MSFatu + detail.DanfeFaturada.Recebidas.SPFatu).ToString("C", nfi);
                            lblDetailDanfTotNRec.Text = (detail.DanfeFaturada.NRecebidas.MSFatu + detail.DanfeFaturada.NRecebidas.SPFatu).ToString("C", nfi);
                            lblDetailDanfTotContri.Text = (detail.DanfeFaturada.Contribuinte.MSFatu + detail.DanfeFaturada.Contribuinte.SPFatu).ToString("C", nfi);
                            lblDetailDanfTotNContri.Text = (detail.DanfeFaturada.NContribuinte.MSFatu + detail.DanfeFaturada.NContribuinte.SPFatu).ToString("C", nfi);
                            #endregion

                            #region Servicos
                            //MSMSMSMSMSMSMS
                            lblDetailMSServPrivado.Text = detail.ServicoFaturado.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServLei.Text = detail.ServicoFaturado.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServTot.Text = detail.ServicoFaturado.MSFatu.ToString("C", nfi);

                            lblDetailMSServPrivadoRec.Text = detail.ServicoFaturado.Recebidas.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServLeiRec.Text = detail.ServicoFaturado.Recebidas.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServTotRec.Text = detail.ServicoFaturado.Recebidas.MSFatu.ToString("C", nfi);

                            lblDetailMSServPrivadoNRec.Text = detail.ServicoFaturado.NRecebidas.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServLeiNRec.Text = detail.ServicoFaturado.NRecebidas.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServTotNRec.Text = detail.ServicoFaturado.NRecebidas.MSFatu.ToString("C", nfi);

                            lblDetailMSServPrivadoCum.Text = detail.ServicoFaturado.Cumulativo.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServLeiCum.Text = detail.ServicoFaturado.Cumulativo.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServTotCum.Text = detail.ServicoFaturado.Cumulativo.MSFatu.ToString("C", nfi);

                            lblDetailMSServPrivadoNCum.Text = detail.ServicoFaturado.NCumulativo.MSPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServLeiNCum.Text = detail.ServicoFaturado.NCumulativo.MSLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailMSServTotNCum.Text = detail.ServicoFaturado.NCumulativo.MSFatu.ToString("C", nfi);

                            //SPSPSPSPSPSPSP
                            lblDetailSPServPrivado.Text = detail.ServicoFaturado.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServLei.Text = detail.ServicoFaturado.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServTot.Text = detail.ServicoFaturado.SPFatu.ToString("C", nfi);

                            lblDetailSPServPrivadoRec.Text = detail.ServicoFaturado.Recebidas.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServLeiRec.Text = detail.ServicoFaturado.Recebidas.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServTotRec.Text = detail.ServicoFaturado.Recebidas.SPFatu.ToString("C", nfi);

                            lblDetailSPServPrivadoNRec.Text = detail.ServicoFaturado.NRecebidas.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServLeiNRec.Text = detail.ServicoFaturado.NRecebidas.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServTotNRec.Text = detail.ServicoFaturado.NRecebidas.SPFatu.ToString("C", nfi);

                            lblDetailSPServPrivadoCum.Text = detail.ServicoFaturado.Cumulativo.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServLeiCum.Text = detail.ServicoFaturado.Cumulativo.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServTotCum.Text = detail.ServicoFaturado.Cumulativo.SPFatu.ToString("C", nfi);

                            lblDetailSPServPrivadoNCum.Text = detail.ServicoFaturado.NCumulativo.SPPrivado.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServLeiNCum.Text = detail.ServicoFaturado.NCumulativo.SPLei.ToString("C", nfi).Replace("R$ ", "");
                            lblDetailSPServTotNCum.Text = detail.ServicoFaturado.NCumulativo.SPFatu.ToString("C", nfi);

                            //Tptais
                            lblDetailServTot.Text = (detail.ServicoFaturado.MSFatu + detail.ServicoFaturado.SPFatu).ToString("C", nfi);
                            lblDetailServTotRec.Text = (detail.ServicoFaturado.Recebidas.MSFatu + detail.ServicoFaturado.Recebidas.SPFatu).ToString("C", nfi);
                            lblDetailServTotNRec.Text = (detail.ServicoFaturado.NRecebidas.MSFatu + detail.ServicoFaturado.NRecebidas.SPFatu).ToString("C", nfi);
                            lblDetailServTotCum.Text = (detail.ServicoFaturado.Cumulativo.MSFatu + detail.ServicoFaturado.Cumulativo.SPFatu).ToString("C", nfi);
                            lblDetailServTotNCum.Text = (detail.ServicoFaturado.NCumulativo.MSFatu + detail.ServicoFaturado.NCumulativo.SPFatu).ToString("C", nfi);
                            #endregion
                            detail = null;
                            //status = StatusPrincipal.SistemaOK;
                        }
                        else
                            status = StatusPrincipal.SistemaOK;
                    }
                    else if (statusdados == StatusDadosPrincipal.Consolidado)
                    {
                        if (conso != null)
                        {
                            periodoFinalizado = conso.finalizado;
                            if (conso.finalizado)
                            {
                                lblStatusPeriodo.Text = "Finalizado";
                                lblStatusPeriodo.ForeColor = Color.Red;
                            }
                            else
                            {
                                lblStatusPeriodo.Text = "Em Aberto";
                                lblStatusPeriodo.ForeColor = Color.LimeGreen;
                            }
                            #region Danfes
                            lblConsulSPDanfNorm.Text = conso.DanfeConsolidado.Normais.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSDanfNorm.Text = conso.DanfeConsolidado.Normais.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotDanfNorm.Text = conso.DanfeConsolidado.Normais.Tot.ToString("C", nfi);

                            lblConsulSPDanfRecDif.Text = conso.DanfeConsolidado.Exclusao.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSDanfRecDif.Text = conso.DanfeConsolidado.Exclusao.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotDanfRecDif.Text = conso.DanfeConsolidado.Exclusao.Tot.ToString("C", nfi);

                            lblConsulSPDanfRbRecDif.Text = conso.DanfeConsolidado.Adicao.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSDanfRbRecDif.Text = conso.DanfeConsolidado.Adicao.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotDanfRbRecDif.Text = conso.DanfeConsolidado.Adicao.Tot.ToString("C", nfi);

                            //SP Total
                            lblConsulSPDanfSaldo.Text = (conso.DanfeConsolidado.Normais.SP - conso.DanfeConsolidado.Exclusao.SP + conso.DanfeConsolidado.Adicao.SP).ToString("C", nfi);
                            //MS Total
                            lblConsulMSDanfSaldo.Text = (conso.DanfeConsolidado.Normais.MS - conso.DanfeConsolidado.Exclusao.MS + conso.DanfeConsolidado.Adicao.MS).ToString("C", nfi);
                            //Total
                            lblConsulTotDanfSaldo.Text = (conso.DanfeConsolidado.Normais.Tot - conso.DanfeConsolidado.Exclusao.Tot + conso.DanfeConsolidado.Adicao.Tot).ToString("C", nfi);
                            #endregion

                            #region Servicos Não Cumulativos
                            lblConsulSPSNCumNorm.Text = conso.ServicoConsuNCum.Normais.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSSNCumNorm.Text = conso.ServicoConsuNCum.Normais.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotSNCumNorm.Text = conso.ServicoConsuNCum.Normais.Tot.ToString("C", nfi);

                            lblConsulSPSNCumRecDif.Text = conso.ServicoConsuNCum.Exclusao.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSSNCumRecDif.Text = conso.ServicoConsuNCum.Exclusao.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotSNCumRecDif.Text = conso.ServicoConsuNCum.Exclusao.Tot.ToString("C", nfi);

                            lblConsulSPSNCumRbRecDif.Text = conso.ServicoConsuNCum.Adicao.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSSNCumRbRecDif.Text = conso.ServicoConsuNCum.Adicao.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotSNCumRbRecDif.Text = conso.ServicoConsuNCum.Adicao.Tot.ToString("C", nfi);

                            //SP Total
                            lblConsulSPSNCumSaldo.Text = (conso.ServicoConsuNCum.Normais.SP - conso.ServicoConsuNCum.Exclusao.SP + conso.ServicoConsuNCum.Adicao.SP).ToString("C", nfi);
                            //MS Total
                            lblConsulMSSNCumSaldo.Text = (conso.ServicoConsuNCum.Normais.MS - conso.ServicoConsuNCum.Exclusao.MS + conso.ServicoConsuNCum.Adicao.MS).ToString("C", nfi);
                            //Total
                            lblConsulTotSNCumSaldo.Text = (conso.ServicoConsuNCum.Normais.Tot - conso.ServicoConsuNCum.Exclusao.Tot + conso.ServicoConsuNCum.Adicao.Tot).ToString("C", nfi);
                            #endregion

                            #region Servicos Cumulativos
                            lblConsulSPSCumNorm.Text = conso.ServicoConsuCum.Normais.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSSCumNorm.Text = conso.ServicoConsuCum.Normais.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotSCumNorm.Text = conso.ServicoConsuCum.Normais.Tot.ToString("C", nfi);

                            lblConsulSPSCumRecDif.Text = conso.ServicoConsuCum.Exclusao.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSSCumRecDif.Text = conso.ServicoConsuCum.Exclusao.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotSCumRecDif.Text = conso.ServicoConsuCum.Exclusao.Tot.ToString("C", nfi);

                            lblConsulSPSCumRbRecDif.Text = conso.ServicoConsuCum.Adicao.SP.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulMSSCumRbRecDif.Text = conso.ServicoConsuCum.Adicao.MS.ToString("C", nfi).Replace("R$ ", "");
                            lblConsulTotSCumRbRecDif.Text = conso.ServicoConsuCum.Adicao.Tot.ToString("C", nfi);

                            //SP Total
                            lblConsulSPSCumSaldo.Text = (conso.ServicoConsuCum.Normais.SP - conso.ServicoConsuCum.Exclusao.SP + conso.ServicoConsuCum.Adicao.SP).ToString("C", nfi);
                            //MS Total
                            lblConsulMSSCumSaldo.Text = (conso.ServicoConsuCum.Normais.MS - conso.ServicoConsuCum.Exclusao.MS + conso.ServicoConsuCum.Adicao.MS).ToString("C", nfi);
                            //Total
                            lblConsulTotSCumSaldo.Text = (conso.ServicoConsuCum.Normais.Tot - conso.ServicoConsuCum.Exclusao.Tot + conso.ServicoConsuCum.Adicao.Tot).ToString("C", nfi);
                            #endregion

                            #region Retenções Não Cumulativos
                            lblConsulNCumPisPrivado.Text = conso.RetencaoConsuNCum.PisPrivado.ToString("C", nfi);
                            lblConsulNCumPisPublico.Text = conso.RetencaoConsuNCum.PisPublico.ToString("C", nfi);
                            lblConsulNCumCsslPrivado.Text = conso.RetencaoConsuNCum.CsslPrivado.ToString("C", nfi);
                            lblConsulNCumCsslPublico.Text = conso.RetencaoConsuNCum.CsslPublico.ToString("C", nfi);
                            lblConsulNCumCofinsPrivado.Text = conso.RetencaoConsuNCum.CofinsPrivado.ToString("C", nfi);
                            lblConsulNCumCofinsPublico.Text = conso.RetencaoConsuNCum.CofinsPublico.ToString("C", nfi);
                            lblConsulNCumIrPrivado.Text = conso.RetencaoConsuNCum.IrPrivado.ToString("C", nfi);
                            lblConsulNCumIrPublico.Text = conso.RetencaoConsuNCum.IrPublico.ToString("C", nfi);
                            lblConsulNCumInssPrivado.Text = conso.RetencaoConsuNCum.InssPrivado.ToString("C", nfi);
                            lblConsulNCumInssPublico.Text = conso.RetencaoConsuNCum.InssPublico.ToString("C", nfi);
                            lblConsulNCumIssPrivado.Text = conso.RetencaoConsuNCum.IssPrivado.ToString("C", nfi);
                            lblConsulNCumIssPublico.Text = conso.RetencaoConsuNCum.IssPublico.ToString("C", nfi);
                            #endregion

                            #region Retenções Cumulativos
                            lblConsulCumPisPrivado.Text = conso.RetencaoConsuCum.PisPrivado.ToString("C", nfi);
                            lblConsulCumPisPublico.Text = conso.RetencaoConsuCum.PisPublico.ToString("C", nfi);
                            lblConsulCumCsslPrivado.Text = conso.RetencaoConsuCum.CsslPrivado.ToString("C", nfi);
                            lblConsulCumCsslPublico.Text = conso.RetencaoConsuCum.CsslPublico.ToString("C", nfi);
                            lblConsulCumCofinsPrivado.Text = conso.RetencaoConsuCum.CofinsPrivado.ToString("C", nfi);
                            lblConsulCumCofinsPublico.Text = conso.RetencaoConsuCum.CofinsPublico.ToString("C", nfi);
                            lblConsulCumIrPrivado.Text = conso.RetencaoConsuCum.IrPrivado.ToString("C", nfi);
                            lblConsulCumIrPublico.Text = conso.RetencaoConsuCum.IrPublico.ToString("C", nfi);
                            lblConsulCumInssPrivado.Text = conso.RetencaoConsuCum.InssPrivado.ToString("C", nfi);
                            lblConsulCumInssPublico.Text = conso.RetencaoConsuCum.InssPublico.ToString("C", nfi);
                            lblConsulCumIssPrivado.Text = conso.RetencaoConsuCum.IssPrivado.ToString("C", nfi);
                            lblConsulCumIssPublico.Text = conso.RetencaoConsuCum.IssPublico.ToString("C", nfi);
                            #endregion
                            conso = null;
                            //status = StatusPrincipal.SistemaOK;
                        }
                        else
                            status = StatusPrincipal.SistemaOK;
                    }
                    break;
                case StatusPrincipal.Processando:
                    lblStatus.Text = StatusPrincipal.Processando.ToString();
                    lblStatus.ForeColor = Color.YellowGreen;
                    break;
                case StatusPrincipal.Reprocessando:
                    lblStatus.Text = StatusPrincipal.Reprocessando.ToString();
                    lblStatus.ForeColor = Color.Red;
                    break;
                default:
                    lblStatus.Text = StatusPrincipal.SistemaOK.ToString();
                    lblStatus.ForeColor = Color.Blue;
                    break;
            }
        }

        private void dateTimeInicio_ValueChanged(object sender, EventArgs e)
        {
            DataPeriodoValueChanged();
        }

        private void dateTimeFim_ValueChanged(object sender, EventArgs e)
        {
            DataPeriodoValueChanged();
        }

        private void DataPeriodoValueChanged()
        {
            if (status != StatusPrincipal.Processando)
            {
                if (tabControlPrincipal.SelectedIndex == 0)
                {
                    //Matrix
                    Task task = new Task(CarregarDetail);
                    task.Start();
                    //CarregarDetail();
                }
                else if (tabControlPrincipal.SelectedIndex == 1)
                {
                    //Consolidado
                    Task task = new Task(CarregarConsolidado);
                    task.Start();
                    //CarregarConsolidado();
                }
                else if (tabControlPrincipal.SelectedIndex == 2)
                {
                    //Diferimento (Criando)
                    //CarregarConsolidado();
                }
                else
                    MessageBox.Show("Erro na lista de Tabs", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbl_MouseEnter(object sender, EventArgs e)
        {
            Label label = sender as Label;
            label.ForeColor = Color.Blue;
        }

        private void lbl_MouseLeave(object sender, EventArgs e)
        {
            Label label = sender as Label;
            label.ForeColor = Color.Black;
        }

        private void lbl_DoubleClick(object sender, EventArgs e)
        {
            if (!periodoFinalizado)
            {
                Label l = sender as Label;
                DAO.NfeDAO nDAO = new DAO.NfeDAO();
                DataTable table;
                FrmRelatorio frm;
                string erro = "";
                switch (l.Name)
                {
                    #region Detalhado -- MS -- Danfes
                    //Privado
                    case "lblDetailMSDanfPrivado":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfPrivadoRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfPrivadoNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfPrivadoContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfPrivadoNContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Lei
                    case "lblDetailMSDanfLei":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfLeiRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfLeiNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfLeiContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfLeiNContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Tot
                    case "lblDetailMSDanfTot":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfTotRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfTotNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfTotContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSDanfTotNContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    #endregion
                    #region Detalhado -- SP -- Danfes
                    //Privado
                    case "lblDetailSPDanfPrivado":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfPrivadoRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfPrivadoNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfPrivadoContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfPrivadoNContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Lei
                    case "lblDetailSPDanfLei":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfLeiRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfLeiNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfLeiContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfLeiNContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Tot
                    case "lblDetailSPDanfTot":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfTotRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfTotNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfTotContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPDanfTotNContri":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    #endregion

                    #region Detalhado -- MS -- Servicos
                    //Privado
                    case "lblDetailMSServPrivado":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServPrivadoRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 0, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServPrivadoNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 0, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServPrivadoCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServPrivadoNCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Lei
                    case "lblDetailMSServLei":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServLeiRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 1, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServLeiNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 1, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServLeiCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServLeiNCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Tot
                    case "lblDetailMSServTot":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServTotRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServTotNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServTotCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailMSServTotNCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    #endregion
                    #region Detalhado -- SP -- Servicos
                    //Privado
                    case "lblDetailSPServPrivado":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServPrivadoRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 0, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServPrivadoNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 0, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServPrivadoCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServPrivadoNCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Lei
                    case "lblDetailSPServLei":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServLeiRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 1, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServLeiNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 1, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServLeiCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServLeiNCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Tot
                    case "lblDetailSPServTot":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServTotRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServTotNRec":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServTotCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblDetailSPServTotNCum":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    #endregion


                    #region Consolidado -- Danfes
                    //SP
                    case "lblConsulSPDanfNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulSPDanfRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulSPDanfRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //MS
                    case "lblConsulMSDanfNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulMSDanfRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulMSDanfRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Tot
                    case "lblConsulTotDanfNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 2, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulTotDanfRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 2, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulSPTotDanfRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 2, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    #endregion
                    #region Consolidado -- Serviços -- Não Cumulativo
                    //SP
                    case "lblConsulSPSNCumNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulSPSNCumRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulSPSNCumRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //MS
                    case "lblConsulMSSNCumNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulMSSNCumRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulMSSNCumRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Tot
                    case "lblConsulTotSNCumNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 0, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulTotSNCumRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 0, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulTotSNCumRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 0, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    #endregion
                    #region Consolidado -- Serviços -- Cumulativo
                    //SP
                    case "lblConsulSPSCumNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulSPSCumRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulSPSCumRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //MS
                    case "lblConsulMSSCumNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulMSSCumRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulMSSCumRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    //Tot
                    case "lblConsulTotSCumNorm":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 1, 0, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulTotSCumRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 1, 1, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                    case "lblConsulTotSCumRbRecDif":
                        this.Cursor = Cursors.WaitCursor;
                        table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 1, 2, out erro);
                        frm = new FrmRelatorio(table);
                        this.Cursor = Cursors.Default;
                        frm.ShowDialog();
                        break;
                        #endregion
                }
            }
        }

        private void excluirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmExcluirNota frmExcluirNota = new FrmExcluirNota();
            frmExcluirNota.ShowDialog();
            
            Task task = new Task(Reprocessar);
            task.Start();

        }

        private void reprocessarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task task = new Task(Reprocessar);
            task.Start();
        }

        private void reprocessarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Task task = new Task(ReprocessarCalculo);
            task.Start();
        }

        private void finalizarPeriodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!periodoFinalizado)
            {
                FolderBrowserDialog open = new FolderBrowserDialog();
                open.Description = "Salvar Relatórios";

                DialogResult dr = open.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    status = StatusPrincipal.Processando;
                    if (!string.IsNullOrEmpty(open.SelectedPath))
                    {
                        CreateFilesRelatorios(open.SelectedPath);
                        DAO.CalculoDAO cDao = new DAO.CalculoDAO();
                        if (!cDao.SetCloseCalculoByPeriodo(dateTimeInicio.Value, dateTimeFim.Value, out string erro))
                            MessageBox.Show("Erro ao fechar periodo", "Erro Periodo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                    MessageBox.Show("Por favor, selecione um local onde será salvo os relatórios.");

                if (tabControlPrincipal.SelectedIndex == 0)
                {
                    //Matrix
                    Task t = new Task(CarregarDetail);
                    t.Start();
                    //CarregarDetail();
                }
                else if (tabControlPrincipal.SelectedIndex == 1)
                {
                    //Consolidado
                    Task t = new Task(CarregarConsolidado);
                    t.Start();
                    //CarregarConsolidado();
                }
            }
        }

        public void CreateFilesRelatorios(string path)
        {
            DAO.NfeDAO nDAO = new DAO.NfeDAO();
            DataTable table;

            #region Detail
            DataSet DetailMSDanfe = new DataSet();
            #region Privado
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 2, 2, out erro);
            table.TableName = "Privadas Total";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 1, 2, out erro);
            table.TableName = "Privadas Recebidas";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 0, 2, out erro);
            table.TableName = "Privadas Não Recebidas";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 2, 1, out erro);
            table.TableName = "Privadas Contribuinte";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 0, 2, 0, out erro);
            table.TableName = "Privadas Não Contribuinte";
            DetailMSDanfe.Tables.Add(table);
            #endregion
            #region Lei
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 2, 2, out erro);
            table.TableName = "Lei Total";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 1, 2, out erro);
            table.TableName = "Lei Recebidas";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 0, 2, out erro);
            table.TableName = "Lei Não Recebidas";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 2, 1, out erro);
            table.TableName = "Lei Contribuinte";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 1, 2, 0, out erro);
            table.TableName = "Lei Não Contribuinte";
            DetailMSDanfe.Tables.Add(table);
            #endregion
            #region Total
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 2, 2, out erro);
            table.TableName = "Total Geral";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 1, 2, out erro);
            table.TableName = "Total Recebidas";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 0, 2, out erro);
            table.TableName = "Total Não Recebidas";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 2, 1, out erro);
            table.TableName = "Total Contribuinte";
            DetailMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "MS", 2, 2, 0, out erro);
            table.TableName = "Total Não Contribuinte";
            DetailMSDanfe.Tables.Add(table);
            #endregion
            CreateExcelByDataSet(path, "Detalhes-Danfes-MS.xlsx", DetailMSDanfe);

            DataSet DetailSPDanfe = new DataSet();
            #region Privado
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 2, 2, out erro);
            table.TableName = "Privadas Total";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 1, 2, out erro);
            table.TableName = "Privadas Recebidas";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 0, 2, out erro);
            table.TableName = "Privadas Não Recebidas";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 2, 1, out erro);
            table.TableName = "Privadas Contribuinte";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 0, 2, 0, out erro);
            table.TableName = "Privadas Não Contribuinte";
            DetailSPDanfe.Tables.Add(table);
            #endregion
            #region Lei
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 2, 2, out erro);
            table.TableName = "Lei Total";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 1, 2, out erro);
            table.TableName = "Lei Recebidas";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 0, 2, out erro);
            table.TableName = "Lei Não Recebidas";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 2, 1, out erro);
            table.TableName = "Lei Contribuinte";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 1, 2, 0, out erro);
            table.TableName = "Lei Não Contribuinte";
            DetailSPDanfe.Tables.Add(table);
            #endregion
            #region Total
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 2, 2, out erro);
            table.TableName = "Total Geral";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 1, 2, out erro);
            table.TableName = "Total Recebidas";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 0, 2, out erro);
            table.TableName = "Total Não Recebidas";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 2, 1, out erro);
            table.TableName = "Total Contribuinte";
            DetailSPDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailDanfes(dateTimeInicio.Value, dateTimeFim.Value, "SP", 2, 2, 0, out erro);
            table.TableName = "Total Não Contribuinte";
            DetailSPDanfe.Tables.Add(table);
            #endregion
            CreateExcelByDataSet(path, "Detalhes-Danfes-SP.xlsx", DetailSPDanfe);

            DataSet DetailMSServico = new DataSet();
            #region Privado
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 0, 2, out erro);
            table.TableName = "Privadas Total";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 0, 1, out erro);
            table.TableName = "Privadas Recebidas";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 0, 0, out erro);
            table.TableName = "Privadas Não Recebidas";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 0, 2, out erro);
            table.TableName = "Privadas Cumulativo";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 0, 2, out erro);
            table.TableName = "Privadas Não Cumulativo";
            DetailMSServico.Tables.Add(table);
            #endregion
            #region Lei
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 1, 2, out erro);
            table.TableName = "Lei Total";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 1, 1, out erro);
            table.TableName = "Lei Recebidas";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 1, 0, out erro);
            table.TableName = "Lei Não Recebidas";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 1, 2, out erro);
            table.TableName = "Lei Cumulativo";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 1, 2, out erro);
            table.TableName = "Lei Não Cumulativo";
            DetailMSServico.Tables.Add(table);
            #endregion
            #region Total
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 2, 2, out erro);
            table.TableName = "Total Geral";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 2, 1, out erro);
            table.TableName = "Total Recebidas";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, 2, 0, out erro);
            table.TableName = "Total Não Recebidas";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 2, 2, out erro);
            table.TableName = "Total Cumulativo";
            DetailMSServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 2, 2, out erro);
            table.TableName = "Total Não Cumulativo";
            DetailMSServico.Tables.Add(table);
            #endregion
            CreateExcelByDataSet(path, "Detalhes-Serviços-MS.xlsx", DetailMSServico);

            DataSet DetailSPServico = new DataSet();
            #region Privado
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 0, 2, out erro);
            table.TableName = "Privadas Total";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 0, 1, out erro);
            table.TableName = "Privadas Recebidas";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 0, 0, out erro);
            table.TableName = "Privadas Não Recebidas";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 0, 2, out erro);
            table.TableName = "Privadas Cumulativo";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 0, 2, out erro);
            table.TableName = "Privadas Não Cumulativo";
            DetailSPServico.Tables.Add(table);
            #endregion
            #region Lei
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 1, 2, out erro);
            table.TableName = "Lei Total";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 1, 1, out erro);
            table.TableName = "Lei Recebidas";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 1, 0, out erro);
            table.TableName = "Lei Não Recebidas";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 1, 2, out erro);
            table.TableName = "Lei Cumulativo";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 1, 2, out erro);
            table.TableName = "Lei Não Cumulativo";
            DetailSPServico.Tables.Add(table);
            #endregion
            #region Total
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 2, 2, out erro);
            table.TableName = "Total Geral";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 2, 1, out erro);
            table.TableName = "Total Recebidas";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, 2, 0, out erro);
            table.TableName = "Total Não Recebidas";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 2, 2, out erro);
            table.TableName = "Total Contribuinte";
            DetailSPServico.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetDetailServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 2, 2, out erro);
            table.TableName = "Total Não Contribuinte";
            DetailSPServico.Tables.Add(table);
            #endregion
            CreateExcelByDataSet(path, "Detalhes-Serviços-SP.xlsx", DetailSPServico);
            #endregion

            #region Consolidado
            DataSet ConsolMSDanfe = new DataSet();
            #region MS
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 0, 2, out erro);
            table.TableName = "Normais MS";
            ConsolMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, out erro);
            table.TableName = "Excluisão MS";
            ConsolMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, out erro);
            table.TableName = "Adição MS";
            ConsolMSDanfe.Tables.Add(table);
            #endregion
            #region SP
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 1, 2, out erro);
            table.TableName = "Normais SP";
            ConsolMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, out erro);
            table.TableName = "Excluisão SP";
            ConsolMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, out erro);
            table.TableName = "Adição SP";
            ConsolMSDanfe.Tables.Add(table);
            #endregion
            #region Total
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 2, 0, out erro);
            table.TableName = "Normais Total";
            ConsolMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 2, 1, out erro);
            table.TableName = "Excluisão Total";
            ConsolMSDanfe.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoDanfes(dateTimeInicio.Value, dateTimeFim.Value, 2, 2, out erro);
            table.TableName = "Adição Total";
            ConsolMSDanfe.Tables.Add(table);
            #endregion
            CreateExcelByDataSet(path, "Consolidado-Danfes.xlsx", ConsolMSDanfe);

            DataSet ConsolMSServicoNCum = new DataSet();
            #region MS
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 2, out erro);
            table.TableName = "Normais MS";
            ConsolMSServicoNCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 1, out erro);
            table.TableName = "Excluisão MS";
            ConsolMSServicoNCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 0, 0, out erro);
            table.TableName = "Adição MS";
            ConsolMSServicoNCum.Tables.Add(table);
            #endregion
            #region SP
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 2, out erro);
            table.TableName = "Normais SP";
            ConsolMSServicoNCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 1, out erro);
            table.TableName = "Excluisão SP";
            ConsolMSServicoNCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 0, 0, out erro);
            table.TableName = "Adição SP";
            ConsolMSServicoNCum.Tables.Add(table);
            #endregion
            #region Total
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 0, 2, out erro);
            table.TableName = "Normais Total";
            ConsolMSServicoNCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 0, 1, out erro);
            table.TableName = "Excluisão Total";
            ConsolMSServicoNCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 0, 0, out erro);
            table.TableName = "Adição Total";
            ConsolMSServicoNCum.Tables.Add(table);
            #endregion
            CreateExcelByDataSet(path, "Consolidado-Serviços-Cumulativos.xlsx", ConsolMSServicoNCum);

            DataSet ConsolMSServicoCum = new DataSet();
            #region MS
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 2, out erro);
            table.TableName = "Normais MS";
            ConsolMSServicoCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 1, out erro);
            table.TableName = "Excluisão MS";
            ConsolMSServicoCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 0, 1, 0, out erro);
            table.TableName = "Adição MS";
            ConsolMSServicoCum.Tables.Add(table);
            #endregion
            #region SP
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 2, out erro);
            table.TableName = "Normais SP";
            ConsolMSServicoCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 1, out erro);
            table.TableName = "Excluisão SP";
            ConsolMSServicoCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 1, 1, 0, out erro);
            table.TableName = "Adição SP";
            ConsolMSServicoCum.Tables.Add(table);
            #endregion
            #region Total
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 1, 2, out erro);
            table.TableName = "Normais Total";
            ConsolMSServicoCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 1, 1, out erro);
            table.TableName = "Excluisão Total";
            ConsolMSServicoCum.Tables.Add(table);
            table = new DataTable();
            table = nDAO.GetConsolidadoServicos(dateTimeInicio.Value, dateTimeFim.Value, 2, 1, 0, out erro);
            table.TableName = "Adição Total";
            ConsolMSServicoCum.Tables.Add(table);
            #endregion
            CreateExcelByDataSet(path, "Consolidado-Serviços-Não_Cumulativos.xlsx", ConsolMSServicoCum);
            #endregion
        }

        public void CreateExcelByDataSet(string Path, string FileName, DataSet dataSet)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Author = "Assecont";
                excelPackage.Workbook.Properties.Title = "Relatórios periodo " + dateTimeInicio.Value.ToString("MM/yyyy");
                Path = System.IO.Path.Combine(Path, "Relatorios_periodo_" + dateTimeInicio.Value.ToString("MM-yyyy"));

                if (!System.IO.Directory.Exists(Path))
                    System.IO.Directory.CreateDirectory(Path);

                foreach (DataTable table in dataSet.Tables)
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(table.TableName);
                    sheet.Name = table.TableName;

                    // Títulos
                    var i = 1;
                    foreach (DataColumn column in table.Columns)
                    {
                        sheet.Cells[1, i++].Value = column;
                    }

                    var rowIndex = 2;
                    foreach (DataRow row in table.Rows)
                    {
                        var col = 1;
                        foreach (DataColumn column in table.Columns)
                        {
                            sheet.Cells[rowIndex, col++].Value = row[column].ToString();
                        }

                        rowIndex++;
                    }
                }

                string path = System.IO.Path.Combine(Path, FileName);
                System.IO.File.WriteAllBytes(path, excelPackage.GetAsByteArray());
            }
        }
    }
}
