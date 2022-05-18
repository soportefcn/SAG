using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class ProgramaAnualAuditorias
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int PersonaID { get; set; }
        public int? InformeAuditoriaID { get; set; }
        public int Numero { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime? FechaReal { get; set; }
        public string Observaciones { get; set; }

        public virtual Proyecto Proyecto { get; set; }
        public virtual Persona Auditor { get; set; }
        public virtual InformeAuditoria Informe { get; set; }
        //public virtual Persona Persona { get; set; }
    }
}