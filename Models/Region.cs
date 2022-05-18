using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Region
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int PaisID { get; set; }
        public string Nombre { get; set; }

        public virtual Pais Pais { get; set;}
        public virtual List<Comuna> Comunas { get; set; }
    }
}