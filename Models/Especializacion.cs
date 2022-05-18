using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Especializacion
    {
        public int ID { get; set; }
        public int ProfesionID { get; set; }
        public string Nombre { get; set; }

        public virtual Profesion Profesion { get; set; }
    }
}