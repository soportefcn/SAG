using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class AutorizacionMovimiento
    {        
        public int ID { get; set; }
        [Display(Name = "Proyecto")]
        public int ProyectoID { get; set; }
        public int? TipoComprobanteID { get; set; }
        [Display(Name = "Cuenta")]
        public int? CuentaID { get; set; }
        [Display(Name = "Cta. Cte.")]
        public int CuentaCorrienteID { get; set; }

        // Campos utilizados por reintegros
        //[Required(ErrorMessage = "Debe seleccionar un egreso.")]
        public int? DetalleEgresoID { get; set; }

        // Campos utilizados por egresos
        public int? PersonaID { get; set; }
        public int? ProveedorID { get; set; }
        public string Rut { get; set; }
        public string DV { get; set; }
        public string Beneficiario { get; set; }
        public string Nombre { get; set; }

        // Comienza en 1 cada año e ingresos, egresos y reintegros son independientes
        [Display(Name = "Nº Comprobante")]
        public int NumeroComprobante { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public DateTime? Fecha { get; set; }
        [Display(Name = "Nº Cheque")]
        //[Required(ErrorMessage = "Debe ingresar el número del cheque.")]
        public int? Cheque { get; set; }
        [Display(Name = "Fecha Cheque")]
        public DateTime? FechaCheque { get; set; }
        [Display(Name = "Monto $")]
        //[Required(ErrorMessage = "Debe ingresar el monto.")]
        public int Monto_Ingresos { get; set; }
        //[Required(ErrorMessage = "Debe ingresar el monto.")]
        [Display(Name = "Monto $")]
        public int Monto_Egresos { get; set; }
        public int Saldo { get; set; }
        public int Linea { get; set; }
        public string Conciliado { get; set; }//1
        public int Mes_Anterior { get; set; }
        public int Periodo_Anterior { get; set; }
        public string Cerrado { get; set; }
        public int Saldo_cartola { get; set; }
        public int Dep_No_Consignados { get; set; }
        public int Cheques_No_Cobrados { get; set; }
        public int Gastos_Bancarios { get; set; }
        //[Required(ErrorMessage="Debe ingresar la glosa.")]
        public string Descripcion { get; set; }
        public int Saldo_Libro { get; set; }
        public string Transtecnia { get; set; }
        public string Nulo { get; set; }
        public string Temporal { get; set; }
        public string Eliminado { get; set; }

        // Define si el movimiento contiene fondo fijo, usado para la impresion del comprobante.
        public string FondoFijo { get; set; }

        // Registro de usuario
        public int? UsuarioID { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int MovimientoID { get; set; }
    }
}