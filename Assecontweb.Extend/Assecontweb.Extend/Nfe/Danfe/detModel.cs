using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
////using System.Threading.Tasks;

namespace Assecontweb.Extend.Nfe.Danfe
{
    public class detModel
    {
        public detModel()
        {
            prod = new prodModel();
            imposto = new impostoModel();
        }
        public prodModel prod { get; set; }
        public impostoModel imposto { get; set; }
    }
}
