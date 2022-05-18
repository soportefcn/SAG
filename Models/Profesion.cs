using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Profesion
    {
        public int ID { get; set; }
        public string Nombre { get; set; }

        public virtual List<Especializacion> Especializaciones { get; set; }
    }
}