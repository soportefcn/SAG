using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class PlanTrabajoAuditoria
    {
        public int ID { get; set; }
        public int PersonaID { get; set; }
        public DateTime Fecha { get; set; }
        public string Documentos { get; set; }

        public virtual Persona Auditor { get; set; }
    }
}