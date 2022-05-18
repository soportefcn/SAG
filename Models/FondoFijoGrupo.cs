using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class FondoFijoGrupo
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int? EgresoID { get; set; }
        public int Numero { get; set; }
        public int Periodo { get; set; }
        public int Monto { get; set; }
        public string Activo { get; set; }
        public string Descripcion { get; set; }
        public DateTime Creacion { get; set; }
        public DateTime Modificacion { get; set; }

        public virtual Movimiento Egreso { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public virtual List<FondoFijo> FondosFijos { get; set; }
    }
}