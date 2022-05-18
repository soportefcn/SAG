using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class UnidadMedida
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Display(Name = "Sigla")]
        public string Unidad { get; set; }

        public virtual List<Articulo> Articulos { get; set; }
    }
}