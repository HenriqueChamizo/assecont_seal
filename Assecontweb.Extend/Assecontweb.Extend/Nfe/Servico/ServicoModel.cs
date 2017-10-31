using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assecontweb.Extend.Nfe.Servico
{
    public class ServicoModel
    {
        public int ItemListaServico { get; set; }
        public string CodigoCnae { get; set; }
        public int CodigoTributacaoMunicipio { get; set; }
        public string Discriminacao { get; set; }
        public int MunicipioPrestacaoServico { get; set; }
        public ValoresModel Valores { get; set; }
    }
}
