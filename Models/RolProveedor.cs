using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class RolProveedor
    {
        public int ID { get; set; }
        public int ProveedorID { get; set; }
        public int ProyectoID { get; set; }

        public virtual Proveedor Proveedor { get; set; }
        public virtual Proyecto Proyecto { get; set; }
    }
}