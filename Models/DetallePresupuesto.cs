using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class DetallePresupuesto
    {
        public int ID { get; set; }
        public int PresupuestoID { get; set; }
        public int CuentaID { get; set; }
        public int Monto { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }

        public virtual Presupuesto Presupuesto { get; set; }
        public virtual Cuenta Cuenta { get; set; }
    }
}