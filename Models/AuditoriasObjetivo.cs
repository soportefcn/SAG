using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class AuditoriasObjetivo
    {
        public int ID { get; set; }
        public string Objetivo { get; set; }
        public int Orden { get; set; }
        public string Activo { get; set; }
    }
}