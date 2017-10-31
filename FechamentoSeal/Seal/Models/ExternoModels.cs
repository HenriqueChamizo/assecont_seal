using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seal.Models
{
    public class ExternoIndexModels
    {
        public List<Model.Seal.Documento> documentos { get; set; }
        public List<Model.Seal.DocumentoDados> dados { get; set; }
    }
}