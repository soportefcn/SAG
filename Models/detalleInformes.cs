using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class DetalleInformes
    {
        public int ID { get; set; }
        public int informeID { get; set; }
        public int cuentaID { get; set; }
        public string tipoCuenta { get; set; }
        public byte valor { get; set; }
        public DateTime fecha { get; set; }
        public byte estado { get; set; }
        //public int CuentaPadre_ID { get; set; }
        //public virtual Cuenta Cuenta { get; set; }
        //public virtual Cuenta CuentaPadre { get; set; }
    }
}