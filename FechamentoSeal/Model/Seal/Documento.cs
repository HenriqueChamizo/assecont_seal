using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Seal
{
    public class Documento
    {
        public Documento()
        {
            doc_int_id = -1;
        }

        public int doc_int_id { get; set; }
        public string doc_des_descricao { get; set; }
        public string doc_des_nameclientes { get; set; }
        public byte[] doc_file_clientes { get; set; }
        public string doc_des_namecontasreceber { get; set; }
        public byte[] doc_file_contasreceber { get; set; }
        public DateTime doc_dt_data { get; set; }
        public bool doc_bit_erro { get; set; }
    }
}
