using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Saldo
    {
        public int ID { get; set; }
        public int CuentaCorrienteID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public int SaldoInicialCartola { get; set; }
        public int SaldoFinal { get; set; }
        //public int? SaldoFinalCartola { get; set; }

        public virtual CuentaCorriente CuentaCorriente { get; set; }
    }
}