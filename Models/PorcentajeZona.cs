using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class PorcentajeZona
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public int Valor { get; set; }
        public int? estado { get; set; }
        public int ComunaID { get; set; }
    }
}