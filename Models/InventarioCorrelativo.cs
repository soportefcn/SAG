using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class InventarioCorrelativo
    {
        [Key]
        public int ID { get; set; }

        public int TipoDoc { get; set; }
        public int Periodo { get; set; }
        public int Valor { get; set; }
        public int ProyectoID { get; set; }
    }
}
