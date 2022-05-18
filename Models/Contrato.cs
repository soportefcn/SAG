using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Contrato
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        [Display(Name = "Servicio")]
        public int ServicioID { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        public string Comentario { get; set; }
        [Required]
        public decimal Monto { get; set; }
        [StringLength(1)]
        public string Activo { get; set; }

        [StringLength(5)]
        public string Moneda { get; set; }

        public virtual Servicio Servicio { get; set; }
        public virtual Proyecto Proyecto { get; set; }
    }
}