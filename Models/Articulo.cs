using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Articulo
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
         [Required]
        [Display(Name = "Unidad de medida")]
        public int UnidadMedidaID { get; set; }
        [Required]
        [Display(Name = "Categoria")]
        public int CategoriaID { get; set; }
         [Required]
        [Display(Name = "Descripción")]
        public string Nombre { get; set; }

        public virtual string NombreLista {
            get {
                return this.Nombre.ToUpper() + " ("+this.UnidadMedida.Descripcion+")";
            }
        }

        public virtual string NombreCategoria
        {
            get
            {
                return this.Categoria.nombre + "/" +  this.Nombre.ToUpper() ;
            }
        }
        public virtual UnidadMedida UnidadMedida { get; set; }
        public virtual Categoria Categoria { get; set; }
        public virtual List<Bodega> Bodegas { get; set; }
    }

    public class Listado{
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }
}