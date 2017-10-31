using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Seal
{
    public class DocumentoDados
    {
        public DocumentoDados()
        {
            dcd_int_id = 0;
            dcd_num_privadocontribuinte = 0;
            dcd_num_privadoncontribuinte = 0;
            dcd_num_publicocontribuinte = 0;
            dcd_num_publiconcontribuinte = 0;
        }

        public int dcd_int_id { get; set; }
        public int doc_int_id { get; set; }
        public int dcd_int_aprovacao { get; set; }
        public double dcd_num_privadocontribuinte { get; set; }
        public double dcd_num_privadoncontribuinte { get; set; }

        public double privado_total {
            get
            {
                return dcd_num_privadocontribuinte + dcd_num_privadoncontribuinte;
            }
        }

        public double dcd_num_publicocontribuinte { get; set; }
        public double dcd_num_publiconcontribuinte { get; set; }

        public double publico_total
        {
            get
            {
                return dcd_num_publicocontribuinte + dcd_num_publiconcontribuinte;
            }
        }

        public double total
        {
            get
            {
                return privado_total + publico_total;
            }
        }
    }
}
