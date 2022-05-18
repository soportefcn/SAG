using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class IndicadoresCuenta
    {
        public int ID { get; set; }
        public int  FACTOR { get; set; }
        public string VARIABLE { get; set; }
        public string INDICADOR { get; set; }


    }
}

