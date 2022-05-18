using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class SistemaAsistencial
    {
        public int ID { get; set; }
        public int LineaAtencionID { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }

        // Listado de Establecimientos pertenecientes a un sistema asistencial
        public virtual List<Proyecto> Establecimientos { get; set; }
        public virtual LineasAtencion LineaAtencion { get; set; }

        public virtual string NombreLista
        {
            get { return Abreviatura + " - " + Nombre.Substring(0, 15) + "..."; }
        }
    }
}