using Seal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Seal.Models
{
    public class SealException 
    {
        public HController hController { get; set; }
        public Exception ex { get; set; }

        public SealException()
        {
        }
        public SealException(Exception e)
        {
            ex = e;
        }

        public SealException(HController c)
        {
            hController = c;
        }

        public SealException(HController c, Exception e)
        {
            hController = c;
            ex = e;
        }

        public SmtpStatusCode SendEmail()
        {
            DAO.ExceptionDAO DAO;
            Model.ConfigException config = null;
            EmailSeal email;
            List<string> emails;
            try
            {
                DAO = new DAO.ExceptionDAO();
                config = DAO.GetEmailException(out hController.erro);
                email = new EmailSeal(config.host, config.port, config.ssl, config.email_from, config.user, config.pass, "Exception in 'Seal' Application", GetHtmlException());
            }
            catch
            {
                Wr.Classes.Email.EmailAuthentication auth = new Wr.Classes.Email.EmailAuthentication("", 0, "", "", true);
                email = new EmailSeal(hController.emailExceptionSend, "Exception in 'Seal' Application", GetHtmlException());
            }


            Dictionary<string, string> Variaveis = new Dictionary<string, string>();

            Variaveis.Add("ERRO", ex.Message);
            Variaveis.Add("URL", hController.Request.Url.ToString());
            Variaveis.Add("DATAHORA", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            Variaveis.Add("USUARIO", hController.Request.Cookies["Usuario"].Value.ToString());

            if (config != null)
                emails = config.emails.Keys.ToList<string>();
            else
            {
                emails = new List<string>();
                emails.Add(hController.emailExceptionReceive);
            }

            SmtpStatusCode retorno = email.Send(emails, Variaveis);
            return retorno;
        }

        private static string GetHtmlException()
        {
            return @"<!DOCTYPE html>
                     <html>
                     <head>
                         <title>ERRO</title>
	                     <meta charset='utf-8' />
                     </head>
                     <body>
                         Erro: %ERRO% 
                         <br>
                         Aconteceu em: %URL%
                         <br>
                         Data e hora: %DATAHORA%
                         <br>
                         Aconteceu com: %USUARIO%
                     </body>
                     </html>";
        }
    }
}