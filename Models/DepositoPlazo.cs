using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class DepositoPlazo
    {
        //[ScaffoldColumn(false)]
        public int ID {get; set;}
        //[ScaffoldColumn(false)]
        [Display(Name = "Proyecto")]
        public int ProyectoID {get; set;}
        //[ScaffoldColumn(false)]
        [Display(Name = "Cta. Cte.")]
        public int CuentaCorrienteID {get; set;}
        //[ScaffoldColumn(false)]
        public int Periodo { get; set; }
        //[ScaffoldColumn(false)]
        public int Mes { get; set; }
        [Display(Name = "Monto $")]
        public int Monto {get; set;}
        [Display(Name = "Interés %")]
        [DisplayFormat(DataFormatString = "{0:0,#}", ApplyFormatInEditMode = true)]
        public decimal? Interes {get; set;}
        [Display(Name = "Interés $")]
        [DisplayFormat(DataFormatString = "{0:0,#}", ApplyFormatInEditMode = true)]
        public decimal? Intereses { get; set; }
        [Display(Name = "A pagar $")]
        [DisplayFormat(DataFormatString = "{0:0,#}", ApplyFormatInEditMode = true)]
        public decimal? MontoFinal { get; set; }
        [StringLength(20)]
        [Display(Name = "Nº Depósito")]
        public string NumeroComprobante {get; set;}
        [Display(Name = "F. Depósito")]
        public DateTime FechaDeposito {get; set;}
        [Display(Name = "F. Vencimiento")]
        public DateTime FechaVencimiento {get; set;}
        [Display(Name = "Observaciones")]
        public string Comentario { get; set; }

        public virtual Proyecto Proyecto { get; set; }
        public virtual CuentaCorriente CuentaCorriente { get; set; }
    }
}