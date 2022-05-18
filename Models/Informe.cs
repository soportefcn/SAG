using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Informe
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public string nombreInforme { get; set; }
        public int tipoInformeID { get; set; }
        public byte estado { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha { get; set; }

        public virtual TipoInforme tipo { get; set; }
        //public virtual List<Comuna> Comunas { get; set; }
    }
}