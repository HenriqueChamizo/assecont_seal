using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Seal.Controllers
{
    public class HController : Controller
    {
        public string erro = "";
        public string pathPage = "";
        public string emailExceptionSend = "notificacao@assecont.com.br";
        public string emailExceptionReceive = "henrique@assecont.com.br";
        
        public void UploadOk()
        {
            DAO.DocumentosDAO DAO = new DAO.DocumentosDAO();
            if (DAO.GetExistDocPeriodo())
                ViewBag.btnUpload = false;
            else
                ViewBag.btnUpload = true;
        }

        protected void CheckUser()
        {
            ViewBag.Interno = (GetUserType() == "INTERNO");
        }

        protected void Permission()
        {
            ViewBag.Permission = GetUserType();
        }

        protected bool Logado()
        {
            ViewBag.Interno = false;
            UploadOk();
            string login = Session["Usuario"] as string;
            if (string.IsNullOrEmpty(login))
            {
                login = Response.Cookies["Usuario"].Value as string;
                Session["Usuario"] = login;
            }
            else
                CheckUser();
            CheckPath();
            return !string.IsNullOrEmpty(login);
        }

        protected void CheckPath()
        {
            string session = Session["pathPage"] as string;
            if (string.IsNullOrEmpty(session))
            {
                session = pathPage = DateTime.Now.ToString("yyyyMMddHHmmss");
                Session.Remove("pathPage");
                Session.Add("pathPage", session);
            }
            else
                pathPage = session;
        }

        protected string[] GetUser()
        {
            string[] split;
            string user = Session["Usuario"] as string;
            if (string.IsNullOrEmpty(user))
                throw new Exception("Sem usuario logado!");
            else
                split = user.Split(new string[] { "%||%" }, StringSplitOptions.None);

            return split;
        }

        protected string GetUserName()
        {
            string[] split = GetUser();
            return split[0];
        }

        protected string GetUserPass()
        {
            string[] split = GetUser();
            return split[1];
        }

        protected string GetUserType()
        {
            string[] split = GetUser();
            return split[2];
        }
    }
}