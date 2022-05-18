using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class TipoRol
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public virtual List<Rol> Roles { get; set; }
    }
}