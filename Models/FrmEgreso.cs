using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class FrmEgreso
    {
        public string Origen { get; set; }
        public string NComprobanteDP { get; set; }
        public string Documento { get; set; }
        public long?  NDocumento { get; set; }
        public int Monto { get; set; }
        public string FechaEmision { get; set; }
        public string FechaVencimiento { get; set; }
        public string NombreCuenta { get; set; }
        public string Glosa { get; set; }
        public int? DeudaPendienteID { get; set; }
        public int? FondoFijoID { get; set; }
        public int? BoletaHonorarioID { get; set; }
        public int? DetalleID { get; set; }
    }
}