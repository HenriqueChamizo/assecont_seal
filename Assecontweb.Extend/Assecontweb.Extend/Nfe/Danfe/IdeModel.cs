using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Assecontweb.Extend.Nfe.Danfe
{
   public class IdeModel
    {
       public string cUF { get; set; }
       public Int32 cNF { get; set; }
       public string natOp { get; set; }
       public int indPag { get; set; }
       public int mod { get; set; } 
       public int serie { get; set; } 
       public int nNf { get; set; } 
       /// <summary>
       /// exemplo 2016-10-06T10:31:00-03:00 
       /// </summary>
       public string dhEmi { get; set; }
       /// <summary>
       /// 2016-10-06T10:31:00-03:00
       /// </summary>
       public string dhSaiEnt { get; set; } 
       public int tpNF { get; set; } 
       public int idDest { get; set; }         
       public Int32 cMunFG { get; set; } 
       public int tpImp { get; set; } 
       public int tpEmis { get; set; } 
       public int cDV { get; set; } 
       public int tpAmb { get; set; } 
       public int finNFe { get; set; } 
       public int indFinal { get; set; }
       public int indPres { get; set; } 
       public int procEmi { get; set; } 
       public string veProc { get; set; } 
    }
}
