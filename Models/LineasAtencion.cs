using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class LineasAtencion
    {
        public int ID { get; set; }
        public string Sigla { get; set; }
        public string Nombre { get; set; }

        public virtual string NombreLista 
        {
            get
            {
                return Sigla + " - " + Nombre;
            }
        }
    }
}