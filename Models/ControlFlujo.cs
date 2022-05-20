using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class ControlFlujo
    {
        public int ID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public string Proceso { get; set; }
        public int PeriodoSistema { get; set; }
        public int MesSistema { get; set; }
        public int ProyectoID { get; set; }
        public string Descripcion { get; set; }

    }
}