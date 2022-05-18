using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class RegistroModificacionProyecto
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int PersonaID { get; set; }
        public string Descripcion { get; set; }

        public virtual Proyecto Proyecto { get; set; }
        public virtual Persona Persona { get; set; }
    } 
}