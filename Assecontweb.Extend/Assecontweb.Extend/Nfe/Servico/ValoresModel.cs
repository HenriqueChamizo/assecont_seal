using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assecontweb.Extend.Nfe.Servico
{
    public class ValoresModel
    {
        public double ValorServicos { get; set; }
        public double ValorDeducoes { get; set; }
        public double ValorPis { get; set; }
        public double ValorCofins { get; set; }
        public double ValorInss { get; set; }
        public double ValorIr { get; set; }
        public double ValorCsll { get; set; }
        public double IssRetido { get; set; }
        public double OutrasRetencoes { get; set; }
        public double BaseCalculo { get; set; }
        public double Aliquota { get; set; }
        public double ValorLiquidoNfse { get; set; }
        public double ValorIssRetido { get; set; }
        public double ValorIss { get; set; }
        public double DescontoCondicionado { get; set; }
        public double DescontoIncondicionado { get; set; }
    }
}
