using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Assecontweb.Extend.Nfe.Danfe
{
    public class impostoModel
    {
        public double vTotal { get; set; }
        public IcmsModel Icms { get; set; }
        public PISModel Pis { get; set; }
        public COFINSModel Cofins { get; set; }
    }
}
