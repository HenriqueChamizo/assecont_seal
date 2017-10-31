using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assecontweb.Extend.Nfe.Servico
{
    public class PrestadorServicoModel
    {
        public IdentificacaoModel IdentificacaoPrestador { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public EnderecoModel Endereco { get; set; }
        public ContatoModel Contato { get; set; }
    }
}
