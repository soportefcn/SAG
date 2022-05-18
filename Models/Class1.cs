using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class DetalleReintegro
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int? MovimientoID { get; set; }
        public int? DocumentoID { get; set; }
        public int CuentaIDD { get; set; }
        public long? NDocumento { get; set; }
        public int Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Estado { get; set; }
        public int UsuarioID { get; set; }


        public virtual Documento Documento { get; set; }

    }
}