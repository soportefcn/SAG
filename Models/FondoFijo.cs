using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class FondoFijo
    {
        // Clave primaria y foraneas
        //[ScaffoldColumn(false)]
        public int ID {get; set;}
        [Display(Name = "Cuenta")]
        public int FondoFijoGrupoID { get; set; }
        public int CuentaID { get; set; }
        public int ProyectoID {get; set;}
        
        // Campos
        public DateTime Fecha {get; set;}
        [Display(Name = "Nº Doc. Resp.")]
        public long? NumeroDocumento { get; set; }
        [Display(Name = "Monto $")]
        public int Monto {get; set;}
        public string Glosa {get; set;}
        public int Periodo {get; set;}
        public int Mes {get; set;}
        [ScaffoldColumn(false)]
        public int? EgresoID { get; set; }

        // Entidades foraneas
        public virtual Movimiento Egreso { get; set; }
        public virtual Cuenta Cuenta { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public virtual FondoFijoGrupo FondoFijoGrupo { get; set; }
    }
}