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
        public int Estado { get; set; }

    }
    public class movcuentatr
    {
        public string Tipo { get; set; }
        public string Region { get; set; }
        public string codCodeni { get; set; }
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
        public int Estado { get; set; }
    }
    public class filtroProyecto {
        public int ProyectoID { get; set; }
        public int TipoProgramaID { get; set; }
        public int regionID { get; set; }
    }
}