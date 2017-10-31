using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Assecontweb.Extend.Nfe.Danfe
{
    public class prodModel
    {
    public string cProd { get; set; }
    public string cEAN { get; set; }
    public string xProd {get; set;}
    public string NCM {get; set;}
    public int CFOP {get; set;}
    public string uCom {get; set;}
    public double qCom {get; set;}
    public double vUnCom {get; set;}
    public double vProd {get; set;}
    public string cEANTrib {get; set;}
    public string uTrib {get; set;}
    public double qTrib {get; set;}
    public double vUnTrib {get; set;}
    public int indTot {get; set;}
    public double vFrete { get; set; }
    public string lote { get; set; }
    public string fabricacao { get; set; }
    public string validade { get; set; }
        // nao pertence ao xml nota
    public double vOutros { get; set; }
    public double vTotal { get; set; }
        

    }
}
