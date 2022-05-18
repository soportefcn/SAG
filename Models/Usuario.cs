using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Usuario
    {
        public int ID {get; set;}
        public int PersonaID {get; set;}
        [StringLength(50)]
        public string Nombre {get; set;}
        [StringLength(1)]
        public string Estado {get; set;}
        [StringLength(1)]
        public string Administrador { get; set; }
        [StringLength(1)]
        public string Supervisor { get; set; }
        [StringLength(33)]
        public string Password {get; set;}
        public string Comentarios {get; set;}

        public bool esAdministrador {
            get
            {
                return Administrador.Equals("S");
            }
        }

        public bool esSupervisor
        {
            get
            {
                return Supervisor.Equals("S");
            }
        }

        public bool esUsuario
        {
            get
            {
                return (!Supervisor.Equals("S") && !Administrador.Equals("S"));
            }
        }

        public bool estaHabilitado
        {
            get
            {
                return (Estado.Equals("A"));
            }
        }

        public virtual Persona Persona { get; set; }
    }
}