using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Conciliacion
    {
        public int ID { get; set; }
	    public int ProyectoID { get; set; }
	    public int PersonaID { get; set; }
	    public int Periodo { get; set; }
	    public int Mes { get; set; }
	    public DateTime FechaConciliacion { get; set; }
	    public DateTime FechaCartola { get; set; }
	    public int SaldoCartola { get; set; }

        [Required(ErrorMessage = "Debe ingresar depositos.")]
	    public int Depositos { get; set; }
        public int Gastos { get; set; }

        public virtual Proyecto Proyecto { get; set; }
        public virtual Persona Persona { get; set; }
    }
}