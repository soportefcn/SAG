using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class ConciliacionRegistro
    {
        public int ID { get; set; }
        public int? MovimientoID { get; set; }
        public int? DetalleID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        
        public int? PersonaID { get; set; }
        public DateTime? Fecha { get; set; }
        
        public virtual Persona Persona { get; set; }

    }
}