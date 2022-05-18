using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Dependencia
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }

        public virtual Proyecto Proyecto { get; set; }
        
    }
}