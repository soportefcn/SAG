using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Intervencion
    {
        public int ID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public int ProyectoID { get; set; }
        public int? Cobertura { get; set; }
        public int Atenciones { get; set; }

        public virtual float Porcentaje {
            get {
                return ((float.Parse(Atenciones.ToString()) / float.Parse(Cobertura.ToString())) * 100);
            }
        }

        public virtual int? Diferencia
        {
            get
            {
                return (Atenciones - Cobertura);
            }
        }

        public virtual Proyecto Establecimiento { get; set; }
    }
}