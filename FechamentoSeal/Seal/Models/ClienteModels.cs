using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seal.Models.Cliente
{
    public class IndexModel
    {
        public Model.Seal.Documento documento { get; set; }
        public Model.Seal.DocumentoDados dados { get; set; }
        public string urlCliente { get; set; }
        public string urlContasReceber { get; set; }
    }

    public class DetailModel
    {
        public Model.Seal.Documento documento { get; set; }
        public Model.Seal.DocumentoDados dados { get; set; }
        public string urlCliente { get; set; }
        public string urlContasReceber { get; set; }
    }
}