using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class IntervencionResumen
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int RegionID { get; set; }
        public int ComunaID { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public double Uss { get; set; }
        public double UssQ { get; set; }
        public int Valor { get; set; }
        public int Monto { get; set; }
        public int Plaza { get; set; }
        public int PlazaConvenio { get; set; }
        public int Tipo { get; set; }
        public int EstadoID { get; set; }
        public int NumDocumento { get; set; }
        public int CompIngreso { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Descripcion { get; set; }


        public virtual Proyecto Proyecto { get; set; }
        public virtual EstadoIntervencion Estado { get; set; }
     //   public List<IntervencionResumen> lista { get; set; }


    }
    public class IntervencionResumenLog
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int RegionID { get; set; }
        public int ComunaID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public double Uss { get; set; }
        public double UssQ { get; set; }
        public int Valor { get; set; }
        public int Monto { get; set; }
        public int Plaza { get; set; }
        public int PlazaConvenio { get; set; }
        public int Tipo { get; set; }
        public int EstadoID { get; set; }
        public int NumDocumento { get; set; }
        public int CompIngreso { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string Descripcion { get; set; }
        public int ControlID { get; set; }
    }
}