using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class ProgramaQ
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public string Codigo { get; set; }
        public string CodigoSename { get; set; }
        public double Valor { get; set; }
        public DateTime FechaIngreso { get; set; }
        public int? Estado { get; set; }

        public virtual Proyecto Proyecto { get; set; }


    }
}