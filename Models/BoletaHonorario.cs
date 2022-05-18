using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class BoletaHonorario
    {
        public int ID { get; set; }
        [Display(Name = "Personal")]
        public int? PersonaID { get; set; }
        public int? ProveedorID { get; set; }
        public string Rut { get; set; }
        public string DV { get; set; }
        public string Beneficiario { get; set; }
        public int ProyectoID { get; set; }
        public int? EgresoID { get; set; }
        [Display(Name = "Nº Boleta")]
        public int NroBoleta { get; set; }
        public DateTime Fecha { get; set; }
        [Display(Name = "A Pagar $")]
        public int Neto { get; set; }
        [Display(Name = "Retención $")]
        public int Retencion { get; set; }
        [Display(Name = "Bruto $")]
        public int Bruto { get; set; }
        public string Concepto { get; set; }
        [ScaffoldColumn(false)]
        public int Periodo { get; set; }
        [ScaffoldColumn(false)]
        public int Mes { get; set; }
        [ScaffoldColumn(false)]
        public decimal Porcentaje { get; set; }
        [Display(Name = "Electrónica?")]
        public string Electronica { get; set; }
        [Display(Name = "Nula?")]
        public string Nula { get; set; }

        public virtual Persona Persona { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Movimiento Egreso { get; set; }
    }
}