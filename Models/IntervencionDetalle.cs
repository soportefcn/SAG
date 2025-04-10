using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class IntervencionDetalle
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string CodigoSename { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Nombre { get; set; }
        public int DiasAtencion { get; set; }
        public int DiasAusente { get; set; }
        public int NumInterOtro { get; set; }
        public int NumInter { get; set; }
        public int TotalIntervencionesAPagar { get; set; }
        public int TotalPagar { get; set; }
        public int TotalPagarNoBis { get; set; }
        public int TotalPagarBis { get; set; }

        public int Tipo { get; set; }
        public int Bis { get; set; }
        public int BisArch { get; set; }
        public int EstadoPago { get; set; }
        public double Uss { get; set; }
        public double UssQ { get; set; }
        public int ResumenID { get; set; }
        public string Discapacidad { get; set; }
        public string Art30 { get; set; }

        public DateTime FechaIngreso { get; set; }
        public virtual Proyecto Proyecto { get; set; }



    }
}