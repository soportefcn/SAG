using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Banco
    {
        public int ID {get; set;}
        public string Nombre { get; set; }

        // Listado de Cuentas Corrientes pertenecientes a un Banco
        public virtual List<CuentaCorriente> CuentasCorrientes { get; set; }

        public virtual string NombreLista
        {
            get { return ID.ToString() + " - " + Nombre; }
        }
    }
}