using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAG2.Models
{
    public class Variable
    {
        public int ID { get; set; }
        public string Descripcion { get; set; }
        public string Indicador { get; set; }
        public string Estandar {get;set; }
        public int Medicion { get; set; } 
    }
}
