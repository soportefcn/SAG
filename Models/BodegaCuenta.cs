using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace SAG2.Models
{
    public class BodegaCuenta
    {
        public int ID { get; set; }
        public int CuentaID { get; set; }
        public int Tipo { get; set; }
        
        public virtual Cuenta Cuenta { get; set; }
    }
}
