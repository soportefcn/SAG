using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace SAG2.Models
{
    public class Inventario
    {
        [Key]
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int TipoID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public string Numero { get; set; }
        public DateTime fechaGrabacion { get; set; }
        public int? MovimientoID { get; set; }

        public virtual Proyecto Proyecto { get; set; }
        public virtual Movimiento Movimiento { get; set; }
        public virtual List<InventarioBien> InventarioBien { get; set; }
        public virtual List<InventarioEspecie> InventarioEspecie { get; set; } 
        //public virtual Tipo Proyecto { get; set; }
        //public virtual DetalleEgreso Egreso { get; set; }
        //public virtual Especie Especie { get; set; }
        //public virtual InventarioAltas Alta { get; set; }
        //public virtual List<InventarioBajas> Bajas { get; set; }

       
    }
}