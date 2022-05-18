using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
	public class TipoInforme
	{
     //   [ScaffoldColumn(false)]
        public int ID { get; set; }
        public string Nombre { get; set; }
        public short Eliminado { get; set; }
    }
}