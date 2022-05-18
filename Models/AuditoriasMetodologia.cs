using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class AuditoriasMetodologia
    {
        public int ID { get; set; }
        public string Metodologia { get; set; }
        public int Orden { get; set; }
        public string Activo { get; set; }
    }
}