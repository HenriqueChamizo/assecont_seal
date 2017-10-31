using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diferimento
{
    public class File
    {
        public File() { }

        public string pathString { get; set; }
        public string fileName { get; set; }
        public string path { get; set; }
        public byte[] buffer { get; set; }
    }
}
