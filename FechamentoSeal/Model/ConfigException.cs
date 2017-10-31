using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ConfigException
    {
        public int id { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public bool ssl { get; set; }
        public string email_from { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public DateTime data { get; set; }
        public bool ativo { get; set; }
        public Dictionary<string, string> emails;
    }
}
