using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
//using AsseFincSharp.Model.NFe;
namespace Assecontweb.Extend.Nfe.Danfe
{
    public class NotaFiscal
    {
        public int idGrupo  { get; set; }
        public int idFornecedor { get; set; }
        public IdeModel Ide { get; set; }
        public EmitModel Emit { get; set; }
        public DestModel Dest { get; set; }
        public List<detModel> Itens { get; set; }
        public ICMSTotModel Total { get; set; }
        public List<Duplicata> Cobranca { get; set; }
        public TranspModel Transp { get; set; }
        public InfAdicModel InfAdic { get; set; }
        public InfProt Prot { get; set; }
        public int nde_int_dest { get; set; }

        public string nrPedidoCompra { get; set; }
        public string xmlImportado { get; set; }
        public string xmlConteudo { get; set; }
        public string nomeArquivo { get; set; }
        public int nfd_int_id { get; set; }
        public string chave { get; set; }
    }

    
}
