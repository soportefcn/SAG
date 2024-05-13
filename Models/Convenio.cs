using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Convenio
    {
        public int ID { get; set; }
        public int? ProyectoID { get; set; }
        public string ResEx { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public int NroPlazas { get; set; }
        public string Comentarios { get; set; }
        public int? Periodo { get; set; }
        public int? Mes { get; set; }
        public int Tintervencion { get; set; }
    }
    public class ConvenioDescarga
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public string NombreArchivo { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public int Estado { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
    public class FormConvenio
    {
        public int ID { get; set; }
        public string ResEx { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public int NroPlazas { get; set; }
        public string Comentarios { get; set; }

    }
}