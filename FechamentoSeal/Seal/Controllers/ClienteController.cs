using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Assecontweb.Extend;
using System.Globalization;

namespace Seal.Controllers
{
    public class ClienteController : HController
    {
        public ActionResult Index()
        {
            if (!Logado())
                return RedirectToAction("Index", "Home");
            return View();
        }

        public ActionResult Detail(int id)
        {
            if (!Logado())
                return RedirectToAction("Index", "Cliente");

            //Pegar usuario logado
            string[] split = GetUser();
            string user = split[0].Split(new string[] { "@" }, StringSplitOptions.None)[0].Replace(".", "").Replace("-", "").Replace("_", "");

            Model.Seal.Documento doc = new Model.Seal.Documento();
            doc.doc_int_id = id;
            Model.Seal.DocumentoDados dados = new Model.Seal.DocumentoDados();
            DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
            DAO.GetDocumento(ref doc, ref dados);

            CheckPath();
            Models.File homefileCliente = new Models.File();
            Models.File homefileContas = new Models.File();
            DirectoryInfo originalDirectory = new DirectoryInfo(string.Format("{0}\\Files\\" + user + "\\View", Server.MapPath(".")).Replace("Cliente\\Detail\\Files", "Files"));
            homefileCliente.pathString = homefileContas.pathString = System.IO.Path.Combine(originalDirectory.ToString(), pathPage);

            homefileCliente.fileName = System.IO.Path.GetFileName(doc.doc_des_nameclientes);
            homefileContas.fileName = System.IO.Path.GetFileName(doc.doc_des_namecontasreceber);

            bool isExists = Directory.Exists(homefileCliente.pathString);

            if (!isExists)
                Directory.CreateDirectory(homefileCliente.pathString);

            IEnumerable<string> directories = Directory.EnumerateDirectories(originalDirectory.ToString());
            foreach (string path in directories)
            {
                if (!path.Contains(pathPage) && !path.Contains("View"))
                    Directory.Delete(path, true);
            }

            homefileCliente.path = string.Format("{0}\\{1}", homefileCliente.pathString, homefileCliente.fileName);
            homefileContas.path = string.Format("{0}\\{1}", homefileContas.pathString, homefileContas.fileName);

            FileStream streamClientes = System.IO.File.Create(homefileCliente.path);
            streamClientes.Write(doc.doc_file_clientes, 0, doc.doc_file_clientes.Length);
            streamClientes.Close();

            FileStream streamContas = System.IO.File.Create(homefileContas.path);
            streamContas.Write(doc.doc_file_contasreceber, 0, doc.doc_file_contasreceber.Length);
            streamContas.Close();

            Models.Cliente.DetailModel model = new Models.Cliente.DetailModel();
            model.urlCliente = Request.Url.ToString().Replace("/" + id.ToString(), "").Replace("Cliente/Detail", "") + "Files/" + user + "/View/" + pathPage + "/" + homefileCliente.fileName;
            model.urlContasReceber = Request.Url.ToString().Replace("/" + id.ToString(), "").Replace("Cliente/Detail", "") + "Files/" + user + "/View/" + pathPage + "/" + homefileContas.fileName;
            model.documento = doc;
            model.dados = dados;

            return View(model);
        }

        [HttpPost]
        public string SaveUploadedFileClientes()
        {
            bool isSavedSuccessfully = false;
            erro = null;
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        //Pegar usuario logado
                        string[] split = GetUser();
                        string user = split[0].Split(new string[] { "@" }, StringSplitOptions.None)[0].Replace(".", "").Replace("-", "").Replace("_", "");

                        Models.File homefile = new Models.File();
                        CheckPath();
                        DirectoryInfo originalDirectory = new DirectoryInfo(string.Format("{0}\\Files\\" + user, Server.MapPath(".")).Replace("Cliente\\Files", "Files"));
                        homefile.pathString = System.IO.Path.Combine(originalDirectory.ToString(), pathPage);
                        homefile.fileName = System.IO.Path.GetFileName(file.FileName);
                        bool isExists = Directory.Exists(homefile.pathString);

                        if (!isExists)
                            Directory.CreateDirectory(homefile.pathString);

                        IEnumerable<string> directories = Directory.EnumerateDirectories(originalDirectory.ToString());
                        foreach (string path in directories)
                        {
                            if (!path.Contains(pathPage) && !path.Contains("View"))
                                Directory.Delete(path, true);
                        }

                        homefile.path = string.Format("{0}\\{1}", homefile.pathString, file.FileName);
                        file.SaveAs(homefile.path);

                        Stream stream = new StreamReader(homefile.path).BaseStream;
                        homefile.buffer = new byte[stream.Length];
                        stream.Read(homefile.buffer, 0, homefile.buffer.Length);
                        stream.Close();
                        stream.Dispose();
                        
                        Session.Add("ClientesHomeFile", homefile);
                        isSavedSuccessfully = true;
                        erro = "OK";
                    }
                }

                //erro = "Sem arquivos";
            }
            catch (Exception e)
            {
                Models.SealException ex = new Models.SealException(this, e);
                ex.SendEmail();
                //Models.EmailSeal email = new Models.EmailSeal(emailExceptionSend, "Erro: Cliente, SaveUploadedFileClientes", Server.MapPath("") + "\\..\\Resource\\erro.html");
                //email.Send(emailExceptionReceive, ex.Message);
                isSavedSuccessfully = false;
                erro = e.Message;
            }

            if (isSavedSuccessfully)
                return "Upload Completo";
            else
                return erro;
        }

        public string SaveClientesSql(string sDoc)
        {
            int documento = Convert.ToInt32(sDoc);
            bool result;
            Models.File homefile = Session["ClientesHomeFile"] as Models.File;
            if (homefile != null)
            {
                try
                {
                    //Novo pega dados do arquivo de clientes
                    FileCliente fileCliente = new FileCliente(homefile.path, "Plan1");
                    //Cria lista de Clientes "local"
                    List<Model.Seal.Cliente> clientes = new List<Model.Seal.Cliente>();
                    //Passa clientes de dll para cliente "local"
                    Model.Seal.Cliente cliente;
                    foreach (Assecontweb.Extend.CFechamento.Cliente c in fileCliente.clientes)
                    {
                        cliente = new Model.Seal.Cliente();
                        cliente.codigo = c.codigo;
                        cliente.codigoGrupo = c.codigoGrupo;
                        cliente.nome = c.nome;
                        cliente.tipo = c.tipo;

                        clientes.Add(cliente);
                    }
                    DAO.ClienteDAO DAO = new DAO.ClienteDAO();
                    result = DAO.SetClientes(clientes, out erro);
                }
                catch (Exception e)
                {
                    DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
                    Model.Seal.Documento doc = new Model.Seal.Documento();
                    doc.doc_int_id = documento;
                    DAO.SetErrorDocumento(doc, out erro);
                    Exception enew = new Exception(e.Message + "<br>Documento Cliente: " + doc.doc_int_id.ToString());
                    Models.SealException ex = new Models.SealException(this, enew);
                    ex.SendEmail();
                    //Models.EmailSeal email = new Models.EmailSeal(emailExceptionSend, "Erro: Cliente, SaveClientesSql", Server.MapPath("") + "\\..\\Resource\\erro.html");
                    //email.Send(emailExceptionReceive, ex.Message);
                    result = false;
                    erro = e.Message;
                }
            }
            else
            {
                result = false;
                erro = "Session vazia";
            }
            return sDoc;
        }

        [HttpPost]
        public string SaveUploadedFileContasReceber()
        {
            bool isSavedSuccessfully = false;
            erro = "Sem arquivos";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    if (file != null && file.ContentLength > 0)
                    {
                        //Pegar usuario logado
                        string[] split = GetUser();
                        string user = split[0].Split(new string[] { "@" }, StringSplitOptions.None)[0].Replace(".", "").Replace("-", "").Replace("_", "");

                        Models.File homefile = new Models.File();
                        CheckPath();
                        DirectoryInfo originalDirectory = new DirectoryInfo(string.Format("{0}\\Files\\" + user, Server.MapPath(".")).Replace("Cliente\\Files", "Files"));
                        homefile.pathString = System.IO.Path.Combine(originalDirectory.ToString(), pathPage);
                        homefile.fileName = System.IO.Path.GetFileName(file.FileName);
                        bool isExists = Directory.Exists(homefile.pathString);

                        if (!isExists)
                            Directory.CreateDirectory(homefile.pathString);

                        IEnumerable<string> directories = Directory.EnumerateDirectories(originalDirectory.ToString());
                        foreach (string path in directories)
                        {
                            if (!path.Contains(pathPage) && !path.Contains("View"))
                                Directory.Delete(path, true);
                        }

                        homefile.path = string.Format("{0}\\{1}", homefile.pathString, file.FileName);
                        file.SaveAs(homefile.path);

                        Stream stream = new StreamReader(homefile.path).BaseStream;
                        homefile.buffer = new byte[stream.Length];
                        stream.Read(homefile.buffer, 0, homefile.buffer.Length);
                        stream.Close();
                        stream.Dispose();

                        Session.Add("ContasReceberHomeFile", homefile);
                        isSavedSuccessfully = true;
                        erro = "OK";
                    }
                }
            }
            catch (Exception e)
            {
                Models.SealException ex = new Models.SealException(this, e);
                ex.SendEmail();
                //Models.EmailSeal email = new Models.EmailSeal(emailExceptionSend, "Erro: Cliente, SaveUploadedFileContasReceber", Server.MapPath("") + "\\..\\Resource\\erro.html");
                //email.Send(emailExceptionReceive, ex.Message);
                isSavedSuccessfully = false;
                erro = e.Message;
            }

            if (isSavedSuccessfully)
                return "Upload Completo";
            else
                return erro;
        }

        public string SaveContasReceberSql(string sDoc)
        {
            int documento = Convert.ToInt32(sDoc);
            bool result;
            Models.File homefile = Session["ContasReceberHomeFile"] as Models.File;
            if (homefile != null)
            {
                try
                {
                    //Novo pega dados do arquivo de clientes
                    FileContaReceber fileContas = new FileContaReceber(homefile.path, "Original");
                    //Cria lista de Clientes "local"
                    List<Model.Seal.ContaReceber> contas = new List<Model.Seal.ContaReceber>();
                    //Passa clientes de dll para cliente "local"
                    Model.Seal.ContaReceber conta;
                    foreach (Assecontweb.Extend.CFechamento.ContaReceber cr in fileContas.contas)
                    {
                        if (!string.IsNullOrEmpty(cr.codigo))
                        {
                            conta = new Model.Seal.ContaReceber();
                            conta.cliente = cr.cliente;
                            conta.codigo = cr.codigo;
                            conta.dataRecto = cr.dataRecto;
                            conta.desconto = cr.desconto;
                            conta.descricao = cr.descricao;
                            conta.docOrig = cr.docOrig;
                            conta.documentoOrigem = cr.documentoOrigem.Replace(".", "").Replace("-", "").Replace("/", "");
                            conta.emissao = cr.emissao;
                            conta.forma = cr.forma;
                            conta.juros = cr.juros;
                            conta.notaFiscal = cr.notaFiscal;
                            conta.prest = cr.prest;
                            conta.valor = cr.valor;
                            conta.valorRecebido = cr.valorRecebido;
                            conta.vencimento = cr.vencimento;

                            contas.Add(conta);
                        }
                    }
                    DAO.ContaReceberDAO DAO = new DAO.ContaReceberDAO();
                    result = DAO.SetContaReceber(contas, documento, out erro);
                }
                catch (Exception e)
                {
                    DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
                    Model.Seal.Documento doc = new Model.Seal.Documento();
                    doc.doc_int_id = documento;
                    DAO.SetErrorDocumento(doc, out erro);
                    Exception enew = new Exception(e.Message + "<br>Documento Contas Receber: " + doc.doc_int_id.ToString());
                    Models.SealException ex = new Models.SealException(this, enew);
                    ex.SendEmail();
                    //Models.EmailSeal email = new Models.EmailSeal(emailExceptionSend, "Erro: Cliente, SaveContasReceberSql", Server.MapPath("") + "\\..\\Resource\\erro.html");
                    //email.Send(emailExceptionReceive, ex.Message);
                    result = false;
                    erro = e.Message;
                }
            }
            else
            {
                result = false;
                erro = "Session vazia";
            }
            return sDoc;
        }

        public string SalvarDocumento(string descricao)
        {
            Model.Seal.Documento documento = new Model.Seal.Documento();
            Model.Seal.DocumentoDados documentoDados = new Model.Seal.DocumentoDados();
            try
            {
                DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
                Models.File FileClientes;
                Models.File FileContas;

                if (CheckFileSession(out FileClientes, out FileContas))
                {
                    //Monta objeto documento
                    documento.doc_des_descricao = string.IsNullOrEmpty(descricao.Replace(" ", "")) ? null : descricao;
                    documento.doc_dt_data = DateTime.Now;
                    documento.doc_des_nameclientes = FileClientes.fileName;
                    documento.doc_file_clientes = FileClientes.buffer;
                    documento.doc_des_namecontasreceber = FileContas.fileName;
                    documento.doc_file_contasreceber = FileContas.buffer;
                    //Salva no banco e retorna o objeto com indice
                    documento.doc_int_id = documentoDados.doc_int_id = DAO.SetDocumento(documento, out erro);

                    FileCliente fileCliente = new FileCliente(FileClientes.path, "Plan1");
                    List<Model.Seal.Cliente> clientes = Models.Convert.Clientes(fileCliente.clientes);
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

                    FileContaReceber fileContas = new FileContaReceber(FileContas.path, "Original");
                    List<Model.Seal.ContaReceber> contas = Models.Convert.ContasReceber(fileContas.contas);


                    Fechamento fechamento = new Fechamento(Models.Convert.ClientesInverse(clientes), Models.Convert.ContasReceberInverse(contas));

                    NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;

                    List<Model.Seal.ContaReceber> crContribuinte = contas.FindAll(cr => clienteContribuinte.Exists(c => c.codigo == cr.codigo));
                    documentoDados.dcd_num_privadocontribuinte = Convert.ToDouble(fechamento.SumValorRecebido(Models.Convert.ContasReceberInverse(crContribuinte)));

                    List<Model.Seal.ContaReceber> crNContribuinte = contas.FindAll(cr => clienteNContribuinte.Exists(c => c.codigo == cr.codigo));
                    documentoDados.dcd_num_privadoncontribuinte = Convert.ToDouble(fechamento.SumValorRecebido(Models.Convert.ContasReceberInverse(crNContribuinte)));

                    List<Model.Seal.ContaReceber> crLei = contas.FindAll(cr => clienteLei.Exists(c => c.codigo == cr.codigo));
                    documentoDados.dcd_num_publicocontribuinte = Convert.ToDouble(fechamento.SumValorRecebido(Models.Convert.ContasReceberInverse(crLei)));

                    List<Model.Seal.ContaReceber> crNLei = contas.FindAll(cr => clienteNLei.Exists(c => c.codigo == cr.codigo));
                    documentoDados.dcd_num_publiconcontribuinte = Convert.ToDouble(fechamento.SumValorRecebido(Models.Convert.ContasReceberInverse(crNLei)));

                    DAO.SetDocumentoDados(documentoDados, out erro);
                }
            }
            catch (Exception e)
            {
                Models.SealException ex = new Models.SealException(this, e);
                ex.SendEmail();
            }
            return documento.doc_int_id.ToString();
        }

        public string ConfereDocumento(string dado, string aprova)
        {
            try
            {
                DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
                DAO.SetDocumentoDadosAprova(Convert.ToInt32(dado), Convert.ToInt32(aprova));
            }
            catch (Exception e)
            {
                Models.SealException ex = new Models.SealException(this, e);
                ex.SendEmail();
                //Models.EmailSeal email = new Models.EmailSeal(emailExceptionSend, "Erro: Cliente, Salvar", Server.MapPath("") + "\\..\\Resource\\erro.html");
                //email.Send(emailExceptionReceive, ex.Message);
            }
            return Url.Action("Inicio", "Home");
        }

        private bool CheckFileSession(out Models.File FileClientes, out Models.File FileContas)
        {
            bool result = false;

            if (Session["ClientesHomeFile"] != null && Session["ContasReceberHomeFile"] != null)
            {
                FileClientes = Session["ClientesHomeFile"] as Models.File;
                FileContas = Session["ContasReceberHomeFile"] as Models.File;
                result = true;
            }
            else
            {
                FileClientes = null;
                FileContas = null;
            }

            return result;
        }
        
        private void DeleteDiretorios(string path)
        {
            string[] diretories = Directory.GetDirectories(path);
            string[] subdiretories;
            string[] files = Directory.GetFiles(path);
            string[] subfiles;

            for (int x = 0; x < files.Length; x++)
            {
                System.IO.File.Delete(files[x]);
            }

            for (int x = 0; x < diretories.Length; x++)
            {
                subfiles = Directory.GetFiles(diretories[x]);
                for (int fx = 0; fx < subfiles.Length; fx++)
                {
                    System.IO.File.Delete(subfiles[fx]);
                }
                subdiretories = Directory.GetDirectories(diretories[x]);
                for (int y = 0; y < subdiretories.Length; y++)
                {
                    DeleteDiretorios(subdiretories[y]);
                }
                Directory.Delete(diretories[x]);
            }
        }

        //private int inseredadosNFe(int idNFE, Models.NFe dados, SqlTransaction transacao, bool eImportacao)
        //{
        //    //NOT_ENTRADASAIDA,NOT_EMITIDOEM,NOT_AMBIENTE,NOT_CHAVE,NOT_RECIBO,NOT_PROTOCOLO,NOT_DATA,NOT_XML,NOT_ASSINADA,NOT_ENVIADAEM,NOT_AUTORIZADAEM,NOT_ENVIADA_AO_CLIENTEEM,";
        //    //"NOT_CANCELADOEM,NOT_REJEITADAEM,NOT_INUTILIZADAEM,NOT_CODIGO_RETORNO,NOT_PROTOCOLO_CANCELAMENTO,";
        //    //"NOT_MOTIVO_CANCELAMENTO,NOT_MOTIVO_REJEICAO,NOT_ENVIAR_EMAIL,
        //    string idNota = "";

        //    string strsql = "select (max(Not_ind)+1)*-1 as id from notas where not_grupo=1";


        //    if (!eImportacao)
        //    {
        //        SqlDataReader drAux = dao.consultaDR(strsql, transacao);
        //        while (drAux.Read())
        //        {
        //            idNota = drAux["id"].ToString();
        //            dados.ide.nNF = Convert.ToInt32(idNota);
        //        }
        //        drAux.Close();
        //    }
        //    else
        //    {
        //        idNota = "0";
        //    }



        //    strsql = "insert into notas (NOT_GRUPO,NOT_CADASTRO,NOT_EMITIDOEM,NOT_NUMERO,NOT_TIPO_EMISSAO,";
        //    strsql += "NOT_DESTINATARIO,NOT_DESTINATARIO_EMAIL,";
        //    strsql += "NOT_ICMS_VALOR,NOT_ICMS_ST_VALOR,NOT_TOTAL_PRODUTOS,NOT_TOTAL_FRETE,NOT_TOTAL_OUTROS,NOT_IPI,NOT_PIS,NOT_COFINS,";
        //    strsql += "NOT_VALOR_TOTAL,NOT_CFOP_PRINCIPAL,NOT_EMISSOR_SIMPLES,";
        //    strsql += "NOT_CONSUMIDOR_FINAL,NOT_IPI_AGREGADO,NOT_VENDA,NOT_COMPRA,NOT_NOTA_REFERENCIADA) ";
        //    strsql += "values (@NOT_GRUPO,@NOT_CADASTRO,@NOT_EMITIDOEM,@NOT_NUMERO,@NOT_TIPO_EMISSAO,";
        //    strsql += "@NOT_DESTINATARIO,@NOT_DESTINATARIO_EMAIL,";
        //    strsql += "@NOT_ICMS_VALOR,@NOT_ICMS_ST_VALOR,@NOT_TOTAL_PRODUTOS,@NOT_TOTAL_FRETE,@NOT_TOTAL_OUTROS,@NOT_IPI,@NOT_PIS,@NOT_COFINS,";
        //    strsql += "@NOT_VALOR_TOTAL,@NOT_CFOP_PRINCIPAL,@NOT_EMISSOR_SIMPLES,";
        //    strsql += "@NOT_CONSUMIDOR_FINAL,@NOT_IPI_AGREGADO,@NOT_VENDA,@NOT_COMPRA,@NOT_NOTA_REFERENCIADA) ";

        //    dao.limparListadeParametros();
        //    dao.addListaParametros(new SqlParameter("@NOT_GRUPO", 1));
        //    dao.addListaParametros(new SqlParameter("@NOT_CADASTRO", dados.dest.cdDest));
        //    dao.addListaParametros(new SqlParameter("@NOT_EMITIDOEM", util.formatDataEhoraToSQL(DateTime.Now.ToShortDateString())));
        //    dao.addListaParametros(new SqlParameter("@NOT_NUMERO", dados.ide.nNF.ToString()));
        //    dao.addListaParametros(new SqlParameter("@NOT_TIPO_EMISSAO", dados.ide.tpEmis));
        //    dao.addListaParametros(new SqlParameter("@NOT_DESTINATARIO", dados.dest.xNome));
        //    dao.addListaParametros(new SqlParameter("@NOT_DESTINATARIO_EMAIL", dados.dest.email));

        //    dao.addListaParametros(new SqlParameter("@NOT_ICMS_VALOR", dados.total.IcmsTot.vICMS));
        //    dao.addListaParametros(new SqlParameter("@NOT_ICMS_ST_VALOR", dados.total.IcmsTot.vST));
        //    dao.addListaParametros(new SqlParameter("@NOT_TOTAL_PRODUTOS", dados.total.IcmsTot.vProd));
        //    dao.addListaParametros(new SqlParameter("@NOT_TOTAL_FRETE", dados.total.IcmsTot.vFrete));
        //    dao.addListaParametros(new SqlParameter("@NOT_TOTAL_OUTROS", dados.total.IcmsTot.vOutro));
        //    dao.addListaParametros(new SqlParameter("@NOT_PIS", dados.total.IcmsTot.vIPI));
        //    dao.addListaParametros(new SqlParameter("@NOT_IPI", dados.total.IcmsTot.vPIS));

        //    dao.addListaParametros(new SqlParameter("@NOT_COFINS", dados.total.IcmsTot.vCOFINS));
        //    dao.addListaParametros(new SqlParameter("@NOT_VALOR_TOTAL", dados.total.IcmsTot.vNF));
        //    var CFOPPrincipal = 0;
        //    dao.addListaParametros(new SqlParameter("@NOT_CFOP_PRINCIPAL", CFOPPrincipal));
        //    var varsimples = dados.emit.CRT == 1 ? 1 : 0;
        //    dao.addListaParametros(new SqlParameter("@NOT_EMISSOR_SIMPLES", varsimples));


        //    dao.addListaParametros(new SqlParameter("@NOT_CONSUMIDOR_FINAL", dados.ide.indFinal));
        //    var ipiAgregado = dados.total.IcmsTot.vIPI > 0 ? 1 : 0;
        //    dao.addListaParametros(new SqlParameter("@NOT_IPI_AGREGADO", ipiAgregado));
        //    dao.addListaParametros(new SqlParameter("@NOT_VENDA", 0));
        //    dao.addListaParametros(new SqlParameter("@NOT_COMPRA", 0));
        //    var nfReferencia = 0;
        //    dao.addListaParametros(new SqlParameter("@NOT_NOTA_REFERENCIADA", nfReferencia));

        //    try
        //    {
        //        // execução da consulta
        //        dao.executaComandoComPArametros(strsql, dao.getComandoDefinido(), transacao);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("NotaNegocio : inseredadosNFe - Mensagem : " + (char)13 + ex.Message);
        //    }

        //    // consultando a nota inserida


        //    // strsql = "select max(not_ind) from notas where not_cadastro=" + dados.dest.cdDest;
        //    strsql = "select max(not_ind) from notas";
        //    SqlDataReader dr = dao.consultaDR(strsql, transacao);
        //    var ultimoId = 0;
        //    while (dr.Read())
        //    {
        //        ultimoId = Convert.ToInt32(dr[0]);
        //    }
        //    dr.Close();

        //    return ultimoId;
        //}
    }
}