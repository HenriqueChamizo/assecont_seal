using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Seal.Controllers
{
    public class UploadController : HController
    {
        // GET: Upload
        public ActionResult Index()
        {
            if (!Logado() && !ViewBag.Interno || ViewBag.Interno == null)
                return RedirectToAction("Inicio", "Home");

            return View();
        }

        public ActionResult Cliente(int id)
        {
            if (!Logado() && !ViewBag.Interno || ViewBag.Interno == null)
                return RedirectToAction("Inicio", "Home");

            return View();
        }
        
        [HttpPost]
        public string SaveUploadedFileXml()
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

                        Models.File uploadfile = new Models.Upload.UploadFile();
                        CheckPath();
                        DirectoryInfo originalDirectory = new DirectoryInfo(string.Format("{0}\\Files\\" + user, Server.MapPath(".")).Replace("Upload\\Files", "Files"));
                        uploadfile.pathString = Path.Combine(originalDirectory.ToString(), pathPage);
                        uploadfile.fileName = Path.GetFileName(file.FileName);
                        bool isExists = Directory.Exists(uploadfile.pathString);

                        if (!isExists)
                            Directory.CreateDirectory(uploadfile.pathString);

                        IEnumerable<string> directories = Directory.EnumerateDirectories(originalDirectory.ToString());
                        foreach (string path in directories)
                        {
                            if (!path.Contains(pathPage) && !path.Contains("View"))
                                Directory.Delete(path, true);
                        }

                        uploadfile.path = string.Format("{0}\\{1}", uploadfile.pathString, file.FileName);
                        file.SaveAs(uploadfile.path);

                        Stream stream = new StreamReader(uploadfile.path).BaseStream;
                        uploadfile.buffer = new byte[stream.Length];
                        stream.Read(uploadfile.buffer, 0, uploadfile.buffer.Length);
                        stream.Close();
                        stream.Dispose();

                        Session.Add("UploadFileXml", uploadfile);
                        isSavedSuccessfully = true;
                        erro = "OK";
                    }
                }

                erro = "Sem arquivos";
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

        [HttpPost]
        public string SaveUploadedFileTxt()
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
                        DirectoryInfo originalDirectory = new DirectoryInfo(string.Format("{0}\\Files\\" + user, Server.MapPath(".")).Replace("Upload\\Files", "Files"));
                        homefile.pathString = Path.Combine(originalDirectory.ToString(), pathPage);
                        homefile.fileName = Path.GetFileName(file.FileName);
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

                        Session.Add("UploadFileTxt", homefile);
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

        public JsonResult ExtractAllXml()
        {
            string retorno = "OK";
            try
            {
                Models.File xml;
                if (CheckFileSessionXml(out xml))
                {
                    using (ZipFile zip = new ZipFile(xml.path))
                    {
                        zip.ExtractAll(xml.pathString);
                    }
                }
            }
            catch (Exception e)
            {
                retorno = e.Message;
                Models.SealException ex = new Models.SealException(this, e);
                ex.SendEmail();
            }
            return Json(new { retorno }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveNotasFiscaisXml()
        {
            string retorno = "OK";
            try
            {
                Models.File xml;
                if (CheckFileSessionXml(out xml))
                {
                    using (Assecontweb.Extend.PathNfe path = new Assecontweb.Extend.PathNfe(xml.pathString))
                    {
                        DAO.NfeDAO DAO = new DAO.NfeDAO();
                        string erro = "";

                        List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes = new List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal>();
                        foreach(Assecontweb.Extend.FileNfeDanfe danfe in path.FilesDanfes)
                        {
                            danfes.Add(danfe.notafiscal);
                        }

                        if (!DAO.SetDanfes(danfes, out erro))
                            throw new Exception(erro);
                    }
                }
            }
            catch (Exception e)
            {
                retorno = e.Message;
                Models.SealException ex = new Models.SealException(this, e);
                ex.SendEmail();
            }
            return Json(new { retorno }, JsonRequestBehavior.AllowGet);
        }
        
        public string SaveNotasFiscaisTxt(string sDoc)
        {
            return "";
        }
        public string SalvarDocumento(string descricao)
        {
            Model.Seal.Documento documento = new Model.Seal.Documento();
            Model.Seal.DocumentoDados documentoDados = new Model.Seal.DocumentoDados();
            try
            {
                //DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
                //Models.File FileClientes;
                //Models.File FileContas;

                //if (CheckFileSession(out FileClientes, out FileContas))
                //{
                //    //Monta objeto documento
                //    documento.doc_des_descricao = string.IsNullOrEmpty(descricao.Replace(" ", "")) ? null : descricao;
                //    documento.doc_dt_data = DateTime.Now;
                //    documento.doc_des_nameclientes = FileClientes.fileName;
                //    documento.doc_file_clientes = FileClientes.buffer;
                //    documento.doc_des_namecontasreceber = FileContas.fileName;
                //    documento.doc_file_contasreceber = FileContas.buffer;
                //    //Salva no banco e retorna o objeto com indice
                //    documento.doc_int_id = documentoDados.doc_int_id = DAO.SetDocumento(documento, out erro);

                //    FileCliente fileCliente = new FileCliente(FileClientes.path, "Plan1");
                //    List<Model.Seal.Cliente> clientes = Models.Convert.Clientes(fileCliente.clientes);
                //    List<Model.Seal.Cliente> clienteContribuinte = new List<Model.Seal.Cliente>();
                //    List<Model.Seal.Cliente> clienteNContribuinte = new List<Model.Seal.Cliente>();
                //    List<Model.Seal.Cliente> clienteLei = new List<Model.Seal.Cliente>();
                //    List<Model.Seal.Cliente> clienteNLei = new List<Model.Seal.Cliente>();
                //    List<Model.Seal.Cliente> clienteErro = new List<Model.Seal.Cliente>();

                //    foreach (Model.Seal.Cliente cli in clientes)
                //    {
                //        if (cli.codigoGrupo == "CONTRIBUINTE")
                //            clienteContribuinte.Add(cli);
                //        else if (cli.codigoGrupo == "NÃO CONTRIBUINTE")
                //            clienteNContribuinte.Add(cli);
                //        else if (cli.codigoGrupo == "CONTRIB - Lei 9718")
                //            clienteLei.Add(cli);
                //        else if (cli.codigoGrupo == "Ñ CONTRIB - Lei 9718")
                //            clienteNLei.Add(cli);
                //    }

                //    FileContaReceber fileContas = new FileContaReceber(FileContas.path, "Original");
                //    List<Model.Seal.ContaReceber> contas = Models.Convert.ContasReceber(fileContas.contas);


                //    Fechamento fechamento = new Fechamento(Models.Convert.ClientesInverse(clientes), Models.Convert.ContasReceberInverse(contas));

                //    NumberFormatInfo nfi = new CultureInfo("pt-BR", false).NumberFormat;

                //    List<Model.Seal.ContaReceber> crContribuinte = contas.FindAll(cr => clienteContribuinte.Exists(c => c.codigo == cr.codigo));
                //    documentoDados.dcd_num_privadocontribuinte = Convert.ToDouble(fechamento.SumValorRecebido(Models.Convert.ContasReceberInverse(crContribuinte)));

                //    List<Model.Seal.ContaReceber> crNContribuinte = contas.FindAll(cr => clienteNContribuinte.Exists(c => c.codigo == cr.codigo));
                //    documentoDados.dcd_num_privadoncontribuinte = Convert.ToDouble(fechamento.SumValorRecebido(Models.Convert.ContasReceberInverse(crNContribuinte)));

                //    List<Model.Seal.ContaReceber> crLei = contas.FindAll(cr => clienteLei.Exists(c => c.codigo == cr.codigo));
                //    documentoDados.dcd_num_publicocontribuinte = Convert.ToDouble(fechamento.SumValorRecebido(Models.Convert.ContasReceberInverse(crLei)));

                //    List<Model.Seal.ContaReceber> crNLei = contas.FindAll(cr => clienteNLei.Exists(c => c.codigo == cr.codigo));
                //    documentoDados.dcd_num_publiconcontribuinte = Convert.ToDouble(fechamento.SumValorRecebido(Models.Convert.ContasReceberInverse(crNLei)));

                //    DAO.SetDocumentoDados(documentoDados, out erro);
                //}
            }
            catch (Exception e)
            {
                Models.SealException ex = new Models.SealException(this, e);
                ex.SendEmail();
            }
            return documento.doc_int_id.ToString();
        }

        private bool CheckFileSessionXml(out Models.File FileXml)
        {
            bool result = false;

            if (Session["UploadFileXml"] != null)
            {
                FileXml = Session["UploadFileXml"] as Models.Upload.UploadFile;
                result = true;
            }
            else
                FileXml = null;

            return result;
        }

        private bool CheckFileSession(out Models.File FileXml, out Models.File FileTxt)
        {
            bool result = false;

            if (Session["UploadFileXml"] != null && Session["UploadFileTxt"] != null)
            {
                FileXml = Session["UploadFileXml"] as Models.Upload.UploadFile;
                FileTxt = Session["UploadFileTxt"] as Models.Upload.UploadFile;
                result = true;
            }
            else
            {
                FileXml = null;
                FileTxt = null;
            }

            return result;
        }
    }
}