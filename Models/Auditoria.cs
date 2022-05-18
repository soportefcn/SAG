using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Auditoria
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int RolID { get; set; }
        public DateTime Fecha { get; set; }
        public string Respuesta { get; set; }

        [Display(Name = "Observaciones")]
        public string ObsPendientes { get; set; }
        public string Seguimiento { get; set; }

        public virtual Rol Rol { get; set; }
    }
}