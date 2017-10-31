using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Assecontweb.Extend.Nfe.Danfe
{
    public class DestModel
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string xNome { get; set; }
        public enderDestModel enderDest { get; set; } 
        public int indIEDest { get; set; }

        public string IE { get; set; }

        public string xFant { get; set; }

        public string IM { get; set; }
    }
}
