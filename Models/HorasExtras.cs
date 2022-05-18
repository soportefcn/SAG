using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAG2.Models
{
    public class HorasExtras
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public int MontoHorasExtras { get; set; }
    }
}
