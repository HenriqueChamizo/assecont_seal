using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wr.Classes;
using System.Net.Mail;

namespace Seal.Models
{
    public class EmailSeal : Email
    {
        private EmailDados emaildados;
        private string assunto;

        public EmailSeal(string EmailFrom, string Subject, string TemplateContent)
            : base(new EmailAuthentication("smtplw.com.br", 587, "assecont", "BwpGCkuA7951", true), EmailFrom, Subject, TemplateContent)
        {
//<<<<<<< HEAD
//=======
//            assunto = Subject;
//>>>>>>> ac2a542a433d281f9d2a33e9e476edfdf581f40d
        }

        public EmailSeal(string host, int port, bool ssl, string from, string user, string pass, string Subject, string TemplateContent)
            : base(new EmailAuthentication(host, port, user, pass, ssl), from, Subject, TemplateContent)
        {
//<<<<<<< HEAD
//=======
//            assunto = Subject;
//>>>>>>> ac2a542a433d281f9d2a33e9e476edfdf581f40d
        }

        public SmtpStatusCode Send(string email, Dictionary<string, string> variaveis)
        {
            emaildados = new EmailDados();

            Variaveis = variaveis;
            emaildados.Body = getBody();
//            emaildados.Subject = assunto;
//<<<<<<< HEAD
//            emaildados.Body = templatecontent;
//=======
//            emaildados.Body = this.templatecontent;
//>>>>>>> ac2a542a433d281f9d2a33e9e476edfdf581f40d
            emaildados.Emails = new List<string>();
            emaildados.Emails.Add(email);

            SmtpStatusCode Result = base.Send(emaildados);
            return Result;
        }

        public SmtpStatusCode Send(List<string> emails, Dictionary<string, string> variaveis)
        {
            emaildados = new EmailDados();

            Variaveis = variaveis;
            emaildados.Body = getBody();
//            emaildados.Subject = assunto;
//<<<<<<< HEAD
//            emaildados.Body = templatecontent;
//=======
//            emaildados.Body = this.templatecontent;
//>>>>>>> ac2a542a433d281f9d2a33e9e476edfdf581f40d
            emaildados.Emails = new List<string>();
            emaildados.Emails = emails;

            SmtpStatusCode Result = base.Send(emaildados);
            return Result;
        }
    }
}