using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class CuentaGlosa
    {
        public int ID { get; set; }
        public int CuentaID { get; set; }
        public string  Glosa { get; set; }
        public string  Respaldo { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public int Estado { get; set; }

    }
}