using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Rol
    {
        public int ID { get; set; }
        public int PersonaID { get; set; }
        public int ProyectoID { get; set; }
        public int TipoRolID { get; set; }
        public string Comentarios { get; set; }

        public virtual Persona Persona { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public virtual TipoRol TipoRol { get; set; }
    }
}