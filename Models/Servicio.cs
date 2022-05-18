using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Servicio
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        public virtual List<Contrato> Contratos { get; set; }
    }
}