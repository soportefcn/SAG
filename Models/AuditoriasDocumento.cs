using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class AuditoriasDocumento
    {
        public int ID { get; set; }
        public string Documento { get; set; }
        public int Orden { get; set; }
        public string Activo { get; set; }
    }
}