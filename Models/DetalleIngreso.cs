using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class DetalleIngreso
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int? MovimientoID { get; set; }
        public string Cc { get; set; }
        public string Cta { get; set; }
        public string Reg { get; set; }
        public string Sename { get; set; }
        public string Servicio { get; set; }
        public string Subvencion { get; set; }
        public string Porcentaje { get; set; }
        public int Traspaso { get; set; }

        public virtual Movimiento Ingreso { get; set; }

    }

    public class DetalleIngresoIva {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int? MovimientoID { get; set; }
        public int CuentaID { get; set; }
        public int Iva { get; set; }
        public int ValorNeto { get; set; }
        public int ValorIva { get; set; }
        public int Total { get; set; }

        public virtual Movimiento Ingreso { get; set; }
        public virtual Cuenta Cuenta { get; set; }
    
    }
}