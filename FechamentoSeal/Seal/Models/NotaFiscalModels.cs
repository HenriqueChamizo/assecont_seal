using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seal.Models.NotaFiscal
{
    public class IndexModel
    {
        public string title { get; set; }
        public List<Assecontweb.Extend.Nfe.Danfe.NotaFiscal> danfes { get; set; }
    }
}