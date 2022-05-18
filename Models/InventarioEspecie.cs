using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace SAG2.Models
{
    public class InventarioEspecie
    {
        [Key]
        public int ID { get; set; }
        public int EspecieID { get; set; }
        public int InventarioID { get; set; }
        public int Cantidad { get; set; }
        public int expandir { get; set; }
        public int ProcedID { get; set; }
        public int Valor { get; set; }
        public string Observacion { get; set; }
        public int DetalleEgresoID { get; set; }
        
        public virtual Inventario Inventario { get; set; }
        public virtual Especie Especie { get; set; }
        public virtual DetalleEgreso DetalleEgreso { get; set; }

    }
}
