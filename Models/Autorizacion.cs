using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Autorizacion
    {
        public int ID { get; set; }
        public int? OriginalID { get; set; }
        public int? ModificadoID { get; set; }
        public int SolicitaID { get; set; }
        public int? AutorizaID { get; set; }
        public string Tipo { get; set; }
        public string Autorizado { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
    }
}