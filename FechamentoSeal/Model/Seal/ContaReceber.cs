using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Seal
{
    public class ContaReceber
    {
        public string cliente { get; set; }
        public string documentoOrigem { get; set; }
        public string codigo { get; set; }
        public string docOrig { get; set; }
        public string notaFiscal { get; set; }
        public string prest { get; set; }
        public DateTime emissao { get; set; }
        public DateTime vencimento { get; set; }
        public double valor { get; set; }
        public double desconto { get; set; }
        public double juros { get; set; }
        public double valorRecebido { get; set; }
        public DateTime dataRecto { get; set; }
        public string forma { get; set; }
        public string descricao { get; set; }

        public ContaReceber()
        {

        }
    }
}
