using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class ParametroUss
    {
        public int ID { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string Tipo { get; set; }
        public double uss { get; set; }
        public int? estado { get; set; }
        public DateTime FechaIngreso { get; set; }

    }
}