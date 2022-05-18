using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Comuna
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int RegionID { get; set; }
        public string Nombre { get; set; }

        public virtual List<Direccion> Direcciones { get; set; }
        public virtual Region Region { get; set; }
    }
}