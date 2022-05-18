using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Tipo
    {
        [Key]
        public int ID { get; set; }
        public string nombre { get; set; }
    }
}
