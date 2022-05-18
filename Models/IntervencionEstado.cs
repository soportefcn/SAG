using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG_5.Models
{
    public class IntervencionEstado
    {
        public int ID { get; set; }
        public int IntervencionResumenID { get; set; }
        public int IntervencionEstadoID { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Hora { get; set; }
        public DateTime FechaSistema { get; set; }
        public virtual IntervencionEstado Estado { get; set; }



    }
}