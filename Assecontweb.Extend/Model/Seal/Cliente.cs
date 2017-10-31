using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Seal
{
    public class Cliente
    {
        public Cliente() { }

        public string codigo { get; set; }
        public string codigoGrupo { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }
        public string cnpj { get; set; }
    }
}
