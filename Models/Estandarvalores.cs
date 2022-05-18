using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Estandarvalores
    {
        public int ID { get; set; }
        public int EstandarID { get; set; }
        public int CuentaID {get; set;}
        public int predeterminado { get; set; }
        public int tipo { get; set; }

    }
}