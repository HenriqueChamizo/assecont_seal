using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

<<<<<<< HEAD
namespace Seal.Models.Interno
{
    public class IndexModel
=======
namespace Seal.Models
{
    public class InternoIndexModels
>>>>>>> ac2a542a433d281f9d2a33e9e476edfdf581f40d
    {
        public List<Model.Seal.Documento> documentos { get; set; }
        public List<Model.Seal.DocumentoDados> dados { get; set; }
    }
<<<<<<< HEAD

    public class ClienteDetailModel
    {
        public ClienteDetailModel() { }

        public Model.Seal.Documento documento { get; set; }
        public Model.Seal.DocumentoDados dados { get; set; }
        public string urlCliente { get; set; }
        public string urlContasReceber { get; set; }
    }
=======
>>>>>>> ac2a542a433d281f9d2a33e9e476edfdf581f40d
}