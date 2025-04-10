using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Presupuesto
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public string Activo { get; set; }
        public string Confirmado { get; set; }
        public int SaldoInicial { get; set; }

        public virtual int Periodo_Inicio
        {
            get
            {
                return Periodo;
            }
        }

        public virtual int Mes_Inicio
        {
            get
            {
                return Mes;
            }
        }

        public virtual bool estaActivo
        {
            get
            {
                return (Activo != null);
            }
        }

        public virtual bool estaConfirmado
        {
            get
            {
                return (Confirmado != null);
            }
        }

        public virtual Proyecto Proyecto { get; set; } 
    }
    public class InformeCuenta
    {
        public int ID { get; set; }
        public int LineaID { get; set; }
        public string LineaSigla { get; set; }
        public int ProgramaID { get; set; }
        public string Sigla { get; set; }
        public string Region { get; set; }
        public string CodCodeni { get; set; }
        public string Programa { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public int CuentaID { get; set; }
        public string Codigo { get; set; }
        public string Cuenta { get; set; }
        public string Tipo { get; set; }
        public int UsuarioID { get; set; }
        public DateTime fechaACT { get; set; }
        public decimal Ingreso { get; set; }
        public decimal Egreso { get; set; }
        public decimal Reintegros { get; set; }
        public decimal valor_GastosReintegros { get; set; }
        public decimal valor_reintegrosgastos { get; set; }
        public decimal Total { get; set; }
    }
    public class InformeCuentaPto
    {
        public int ID { get; set; }
        public int LineaID { get; set; }
        public string LineaSigla { get; set; }
        public int ProgramaID { get; set; }
        public string TipoProyecto { get; set; }
        public string Region { get; set; }
        public string CodCodeni { get; set; }
        public string Programa { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public int CuentaID { get; set; }
        public string Codigo { get; set; }
        public string Cuenta { get; set; }
        public string Tipo { get; set; }
        public int UsuarioID { get; set; }
        public DateTime fechaACT { get; set; }
        public decimal Ingreso { get; set; }
        public decimal Egreso { get; set; }
        public decimal Reintegros { get; set; }
        public decimal valor_GastosReintegros { get; set; }
        public decimal valor_reintegrosgastos { get; set; }
        public decimal Real { get; set; }
        public decimal Ppto { get; set; }
    }

    public class InformeCuentaDetalle
    {
        public int ID { get; set; }
        public int LineaID { get; set; }
        public string LineaSigla { get; set; }
        public int ProgramaID { get; set; }
        public string TipoPrograma { get; set; }
        public string Region { get; set; }
        public string CC { get; set; }
        public string CodigoExterno { get; set; }
        public string Programa { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public int NumeroComprobante { get; set; }
        public DateTime FechaDoc { get; set; }
        public int CuentaID { get; set; }
        public string Padre { get; set; }
        public string Codigo { get; set; }
        public string Cuenta { get; set; }
        public string TipoCuenta { get; set; }
        public string TipoBeneficiario { get; set; }
        public string RutTB { get; set; }
        public string NombreTB { get; set; }
        public string Medio { get; set; }
        public int MedioNum { get; set; }
        public decimal Valor { get; set; }
        public int UsuarioID { get; set; }
        public DateTime FechaSis { get; set; }
        public string TipoDoc { get; set; }
        public string Glosa { get; set; }
      
    }



}