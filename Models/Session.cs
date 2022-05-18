using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Session
    {
        public Persona Persona { get; set; }
        public Proyecto Establecimiento { get; set; }
        public Rol Rol { get; set; }

        public string mensaje {
            get {
                return Establecimiento.NombreLista + " | " + Persona.NombreCompleto + " | ";
            }
        }

    }
}