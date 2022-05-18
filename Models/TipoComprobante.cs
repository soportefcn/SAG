using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class TipoComprobante
    {
        public int ID { get; set; }
        public string Nombre { get; set; }

        // Listado de movimientos relacionados con un tipo de comprobante
        public virtual List<Movimiento> Movimientos { get; set; }
    }
}