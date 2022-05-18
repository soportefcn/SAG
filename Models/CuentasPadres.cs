using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class CuentasPadres
    {
        public int ID { get; set; }
        public int CuentaID { get; set; }
        public string NombreGrupo { get; set; }
        public int estado { get; set; }
    }
}