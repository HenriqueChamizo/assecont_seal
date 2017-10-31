using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Assecontweb.Extend.Nfe.Danfe
{
   public class EmitModel
    {
       enderEmitModel endereco;
        public EmitModel()
        {
            enderEmit= new enderEmitModel();
            enderEmit = endereco; ;
        }
        public string CNPJ { get; set; }
        public string xNome { get; set; }
        public string xFant { get; set; }
        public enderEmitModel enderEmit {get; set;} 
    
        public string IE { get; set; }
        public string IM { get; set; }
        public string CNAE { get; set;} 
        public int CRT { get; set; }

        public object CPF { get; set; }
    }
}
