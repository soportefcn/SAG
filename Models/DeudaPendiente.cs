using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class DeudaPendiente
    {
        public int ID { get; set; }
        public int? PersonaID { get; set; }
        public int? ProveedorID { get; set; }

        [Display(Name = "Documento")]
        public int DocumentoID { get; set; }
        [Display(Name="Cuenta")]
        public int CuentaID { get; set; }
        [Display(Name = "Proyecto")]
        public int ProyectoID { get; set; }
        [Display(Name = "Nº Comprobante")]
        public int NumeroComprobante { get; set; }
        [Display(Name = "Emisión")]
        public DateTime FechaEmision { get; set; }
        [Display(Name = "Vencimiento")]
        public DateTime FechaVencimiento { get; set; }
        [Display(Name = "Nº Documento")]
        public int NumeroDocumento { get; set; }
        public DateTime Fecha { get; set; }
        [Display(Name = "Monto $")]
        public int Monto { get; set; }
        public string Glosa { get; set; }
        public int? EgresoID { get; set; }
        public int Mes { get; set; }
        public int Periodo { get; set; }

        public string Rut { get; set; }
        public string DV { get; set; }
        public string Beneficiario { get; set; }

        public virtual Persona Persona { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual Documento Documento { get; set; }
        public virtual Cuenta Cuenta { get; set; }
        public virtual Proyecto Proyecto { get; set; } 
    }
}