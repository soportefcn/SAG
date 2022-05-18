using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class movcuenta
    {
        public int ProyectoID { get; set; }
        public int TipoComprobanteID { get; set; }
        public int IDComprobante { get; set; }
        public int NumeroComprobante { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public DateTime? Fecha { get; set; }
        public int Monto { get; set; }
        public string CtaCte { get; set; }
        public string Glosa { get; set; }
        public int? Cheque { get; set; }
        public string NombreCuenta { get; set; }

    }
}