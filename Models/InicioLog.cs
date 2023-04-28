using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class InicioLog
    {
        public int id { get; set; }
        public string Tipo { get; set; }
        public int ProyectoId { get; set; }
        public DateTime? Fecha { get; set; }
        public int Mes { get; set; }
        public int Periodo { get; set; }
        public int UsuarioID { get; set; }
        public string Descripcion { get; set; }
        public int RegistroID { get; set; }
    }
}