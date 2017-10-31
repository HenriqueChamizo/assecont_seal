using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seal.Controllers
{
    public class NotaFiscalController : HController
    {
        // GET: NotaFiscal
        public ActionResult Index()
        {
            if (!Logado())
                return RedirectToAction("Index", "Home");

            CheckUser();

            //Cria conexão
            DAO.NfeDAO DAO = new DAO.NfeDAO();
            //Traz documentos
            List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes = new List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal>();
            string erro = "";
            if(DAO.GetDanfes(ref danfes, ref erro))
            {
                //Cria modelo p/ View
                Models.NotaFiscal.IndexModel model = new Models.NotaFiscal.IndexModel();
                //Atribui documentos no modelo
                model.title = "Notas Fiscais Importadas";

                model.danfes = danfes;
                return View(model);
            }
            else
            {
                Models.SealException ex = new Models.SealException(this, new Exception(erro));
                ex.SendEmail();
                return RedirectToAction("Index", "Home");
            }
        }
    }
}