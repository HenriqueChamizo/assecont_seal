using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Assecontweb.Extend.Nfe.Danfe
{
    public class Duplicata
    {
        public int idCondicao { get; set; }
        public double valor_totalDuplicata { get; set; }
        public string  periodicidade { get; set; }
        public int qtParcelas { get; set; }
        public string  Fatura { get; set; }
        public DateTime Vencimento { get; set; }
        public Double  Valor { get; set; }
    }
}
