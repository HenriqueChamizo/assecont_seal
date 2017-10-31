using Seal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seal.Controllers
{
    public class ExternoController : HController
    {
        // GET: Externo
        public ActionResult Index()
        {
            if (!Logado())
                return RedirectToAction("Index", "Home");

            //Cria conexão
            DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
            //Traz documentos
            List<Model.Seal.Documento> documentos = new List<Model.Seal.Documento>();
            List<Model.Seal.DocumentoDados> dados = new List<Model.Seal.DocumentoDados>();
            DAO.GetDocumentos(ref documentos, ref dados);
            //Cria modelo p/ View
            ExternoIndexModels model = new ExternoIndexModels();
            //Remove da lista o Documento "DOCUMENTO NÃO EXISTENTE"
            //documentos.Remove(documentos.Find(f => f.doc_int_id == 1));
            //Atribui documentos no modelo
            model.documentos = documentos;
            model.dados = dados;
            //model.documentos = new List<Model.Seal.Documento>();

            return View(model);
        }

        // GET: Externo/ClienteDetail/id
        public ActionResult ClienteDetail(int id)
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
            HomeFile homefileCliente = new HomeFile();
            HomeFile homefileContas = new HomeFile();
            DirectoryInfo originalDirectory = new DirectoryInfo(string.Format("{0}\\Files\\" + user + "\\View", Server.MapPath(".")).Replace("Cliente\\Detail\\Files", "Files"));
            homefileCliente.pathString = homefileContas.pathString = Path.Combine(originalDirectory.ToString(), pathPage);

            homefileCliente.fileName = Path.GetFileName(doc.doc_des_nameclientes);
            homefileContas.fileName = Path.GetFileName(doc.doc_des_namecontasreceber);

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

            ClienteIndexModels model = new ClienteIndexModels();
            model.urlCliente = Request.Url.ToString().Replace("/" + id.ToString(), "").Replace("Cliente/Detail", "") + "Files/" + user + "/View/" + pathPage + "/" + homefileCliente.fileName;
            model.urlContasReceber = Request.Url.ToString().Replace("/" + id.ToString(), "").Replace("Cliente/Detail", "") + "Files/" + user + "/View/" + pathPage + "/" + homefileContas.fileName;
            model.documento = doc;
            model.dados = dados;

            return View(model);
        }
    }
}