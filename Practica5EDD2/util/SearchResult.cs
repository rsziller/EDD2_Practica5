using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica5EDD2.util
{
    public class SearchResult
    {
        public string Nombre { get; set; }
        public long DPI { get; set; }
        public String FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public List<string> Company { get; set; }
        public List<string> Letter { get; set; }
        public List<string> Conversation { get; set; }


    }
}
