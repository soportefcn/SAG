using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Proveedor
    {
        public int ID { get; set; }
        public int DireccionID { get; set; }
        public string Nombre { get; set; }
        public string Rut { get; set; }
        public string DV { get; set; }
        public string Fono { get; set; }
        public string Fax { get; set; }
        public string CorreoElectronico { get; set; }
        public string PersonaContacto { get; set; }
        public string PaginaWeb { get; set; }

        public virtual string RutDV 
        {
            get 
            {
                return Rut + "-" + DV;
            }
        }

        public virtual string NombreLista
        {
            get { return RutDV + " " + Nombre; }
        }

        public virtual Direccion Direccion { get; set; }
    }
}