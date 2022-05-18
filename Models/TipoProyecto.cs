using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class TipoProyecto
    {
        public int ID { get; set; }
        public int LineasAtencionID { get; set; }
        public string Sigla { get; set; }
        public string Nombre { get; set; }

        public virtual string NombreLista
        {
           
            get { return Sigla + " - " + Nombre.Substring(0, 15) + "..."; }
        }

        public virtual string NombreListaCompleto
        {
            get { return Sigla + " - " + Nombre; }
        }

        public virtual LineasAtencion LineaAtencion { get; set; }
    }
}