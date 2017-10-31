using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FechamentoSeal.Controllers
{
    public class HController : Controller
    {
        public bool Logado()
        {
            string login = Session["Usuario"] as string;
            if (string.IsNullOrEmpty(login))
            {
                login = Response.Cookies["Usuario"].Value as string;
                Session["Usuario"] = login;
            }

            return !string.IsNullOrEmpty(login);
        }
    }
}