using Seal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seal.Controllers
{
    public class HomeController : HController
    {
        public ActionResult Index()
        {
            if (Logado())
                return RedirectToAction("Inicio", "Home");

            return View();
        }

        public ActionResult Login(string user, string pass, bool? lembrar)
        {
            DAO.LoginDAO login = new DAO.LoginDAO();
            string retorno = login.Logar(user, pass);
            if (!string.IsNullOrEmpty(retorno))
            {
                string[] split = retorno.Split(new string[] { "%||%" }, StringSplitOptions.None);

                string UserOrError = split[0];
                if (UserOrError == user)
                {
                    if (lembrar == true)
                    {
                        HttpCookie usuario = new HttpCookie("Usuario", retorno);
                        Response.Cookies.Add(usuario);
                    }
                    Session.Add("Usuario", retorno);
                    return RedirectToAction("Inicio", "Home");
                    //return CheckUser();
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            else
                return RedirectToAction("Index", "Home");
        }

        public ActionResult SaveUploadedFile()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                        string pathString = Path.Combine(originalDirectory.ToString(), "imagepath");

                        var fileName1 = Path.GetFileName(file.FileName);

                        bool isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);
                        fPath = path;
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }


            if (isSavedSuccessfully)
            {
                //Teste(fPath);
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        public ActionResult SaveUploadedFileContasReceber()
        {
            bool isSavedSuccessfully = true;
            string fName = "";
            string fPath = "";
            try
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    //Save file content goes here
                    fName = file.FileName;
                    if (file != null && file.ContentLength > 0)
                    {

                        var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\WallImages", Server.MapPath(@"\")));

                        string pathString = Path.Combine(originalDirectory.ToString(), "imagepath");

                        var fileName1 = Path.GetFileName(file.FileName);

                        bool isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);

                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);
                        fPath = path;
                    }

                }

            }
            catch (Exception ex)
            {
                isSavedSuccessfully = false;
            }


            if (isSavedSuccessfully)
            {
                //Teste(fPath);
                return Json(new { Message = fName });
            }
            else
            {
                return Json(new { Message = "Error in saving file" });
            }
        }

        public ActionResult Inicio()
        {
            if (!Logado())
                return RedirectToAction("Index", "Home");
            
            CheckUser();

            //Cria conexão
            DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
            //Traz documentos
            List<Model.Seal.Documento> documentos = new List<Model.Seal.Documento>();
            List<Model.Seal.DocumentoDados> dados = new List<Model.Seal.DocumentoDados>();
            DAO.GetDocumentos(ref documentos, ref dados);
            //Cria modelo p/ View
            Models.Home.InicioModel model = new Models.Home.InicioModel();
            //Remove da lista o Documento "DOCUMENTO NÃO EXISTENTE"
            //documentos.Remove(documentos.Find(f => f.doc_int_id == 1));
            //Atribui documentos no modelo
            if (ViewBag.Interno)
                model.title = "Histórico de Uploads do Cliente";
            else
                model.title = "Histórico de Uploads";

            model.documentos = documentos;
            model.dados = dados;
            //model.documentos = new List<Model.Seal.Documento>();

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload()
        {
            var file = this.Request.Files[0];
            string savedFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
            savedFileName = System.IO.Path.Combine(savedFileName, System.IO.Path.GetFileName(file.FileName));
            file.SaveAs(savedFileName);
            return View();
        }

        public DataSet OpenFileExcel()
        {
            string arquivo = @"C:\Users\edvaldob\Documents\Visual Studio 2012\Projects\ProgramasAcessorios\importadorArquivoXLS\importadorArquivoXLS\bin\Debug\Inventario_2016_PA.xlsx";
            string strConexao = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"", arquivo);
            OleDbConnection conn = new OleDbConnection(strConexao);
            conn.Open();
            DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            //Cria o objeto dataset para receber o conteúdo do arquivo Excel
            DataSet output = new DataSet();
            DataTable outputTable = null;
            foreach (DataRow row in dt.Rows)
            {
                // obtem o noma da planilha corrente
                // string sheet = row["Inventario_2016"].ToString();
                string sheet = "Inventario_2016$";
                // obtem todos as linhas da planilha corrente
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                cmd.CommandType = CommandType.Text;
                // copia os dados da planilha para o datatable
                outputTable = new DataTable(sheet);
                output.Tables.Add(outputTable);
                new OleDbDataAdapter(cmd).Fill(outputTable);
            }

            return output;
        }

        public ActionResult Sair()
        {
            Session.Remove("Usuario");
            Response.Cookies.Remove("Usuario");

            return View("Index");
        }
    }
}