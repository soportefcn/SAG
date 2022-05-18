using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class DetalleEgreso
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int? MovimientoID { get; set; }
        public int? DeudaPendienteID { get; set; }
        public int? FondoFijoID { get; set; }
        public int? BoletaHonorarioID { get; set; }
        public int? DocumentoID { get; set; }
        public int CuentaID { get; set; }
        public string Glosa { get; set; }
        public int? NComprobanteDP { get; set; }
        public long? NDocumento { get; set; }
        public int Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Nulo { get; set; }
        public string Conciliado { get; set; }
        public string Temporal { get; set; }

        // Relaciones opcionales
        public virtual Movimiento Egreso { get; set; }
        public virtual DeudaPendiente DeudaPendiente { get; set; }
        public virtual FondoFijo FondoFijo { get; set; } 
        public virtual BoletaHonorario BoletaHonorario { get; set; }

        // Relaciones obligatorias
        public virtual Documento Documento { get; set; }
        public virtual Cuenta Cuenta { get; set; }
    }
}