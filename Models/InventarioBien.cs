using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class InventarioBien
    {
        [Key]
        public int ID { get; set; }
       
        public int InventarioID { get; set; }
      

        public int BienInventarioID { get; set; }
        public int DependenciaID_Origen { get; set;  }
        public int DependenciaID_Destino { get; set; }
        
        public virtual Inventario Inventario { get; set; }
        public virtual BienInventario BienInventario { get; set; }
    }
}
