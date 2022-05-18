using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Seccion
    {
        public int ID { get; set; }
        public int? PadreID { get; set; }
        public string Nombre { get; set; }
        public string URL { get; set; }
        public string Menu { get; set; }
        public int Orden { get; set; }

        public virtual Seccion Padre { get; set; }
        public virtual List<Seccion> Hijos { get; set; }
    }
}