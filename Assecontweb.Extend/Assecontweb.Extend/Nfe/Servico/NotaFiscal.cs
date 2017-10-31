using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assecontweb.Extend.Nfe.Servico
{
    public class NotaFiscal
    {
        public string Cancelada { get; set; }
        public int Numero { get; set; }
        public string CodigoVerificacao { get; set; }
        /// <summary>
        /// exemplo 2016-10-06T10:37:29
        /// </summary>
        public string DataEmissao { get; set; }
        public int NaturezaOperacao { get; set; }
        public int RegimeEspecialTributacao { get; set; }
        public int OptanteSimplesNacional { get; set; }
        public int IncentivadorCultural { get; set; }
        public string Competencia { get; set; }
        public int NfseSubstituida { get; set; }
        public string OutrasInformacoes { get; set; }
        public ServicoModel Servico { get; set; }
        public PrestadorServicoModel PrestadorServico { get; set; }
        public TomadorServicoModel TomadorServico { get; set; }
        public OrgaoGeradorModel OrgaoGerador { get; set; }
        public double ValorCredito { get; set; }
        public int nse_int_tomador { get; set; }
        
        public string xmlImportado { get; set; }
        public string nomeArquivo { get; set; }

        public int nfd_int_id { get; set; }
        public string chave { get; set; }
    }
}
