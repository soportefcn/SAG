using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Resolucion
    {
        public int ID { get; set; }
        public int? ProyectoID { get; set; }
        public string tipo { get; set; }
        public string ResEx { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public string Comentarios { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public int? Estado { get; set; }


        public virtual Proyecto Proyecto { get; set; }
    }

    public class ResolucionDescarga
    {
        public int ID { get; set; }
        public int ResolucionID { get; set; }
        public string NombreArchivo { get; set; }

        public virtual Resolucion Resolucion { get; set; }

    }
    public class ResolucionInforme
    {
        public int ID { get; set; }
        public int? ProyectoID { get; set; }
        public string tipo { get; set; }
        public string ResEx { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public string Comentarios { get; set; }

    }
}