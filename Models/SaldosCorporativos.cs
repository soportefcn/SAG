using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class SaldosCorporativos
    {
        public int ID { get; set; }
	    public string RutEmpresa { get; set; }
	   public string  NombreEmpresa { get; set; }
	    public string Numerocuenta { get; set; }
	   public  string Moneda  { get; set; }
       public double SaldoContable { get; set; }
       public double Retencion { get; set; }
       public double Retencion1 { get; set; }
       public double Saldo_Contable { get; set; }
       public double Saldo_Disponible { get; set; }
      public int Periodo { get; set; }
      public int Mes { get; set; }
      public int UsuarioID { get; set; }
      public DateTime Fecha { get; set; }
    }
}