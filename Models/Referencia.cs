using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Referencia
    {
        public int ID { get; set; }
        public string GRUPO { get; set; }
        public string COD_REFER { get; set; }
        public decimal VALOR { get; set; }
        public int PERIODO { get; set; }
        public int PREDETERMINADO { get; set; }
        public int  ESTADO  { get; set; }
        // cambiar cod_refer por int
    }
}