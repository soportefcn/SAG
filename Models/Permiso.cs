using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Permiso
    {
        public int ID { get; set; }
        public string TipoUsuario { get; set; }
        public int SeccionID { get; set; }

        public virtual Seccion Seccion { get; set; }
    }
}