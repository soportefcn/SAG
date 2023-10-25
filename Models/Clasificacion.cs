using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace SAG2.Models
{
    // Maestros Cierre Inventario
    public class Grupo {
        [Key]
        public int ID { get; set; }
        public string Nombre { get; set; }    
    }

    public class Clasificacion
    {
        [Key]
        public int ID { get; set; }             
        public string Nombre { get; set; }
        public int Estado { get; set; }
        public int UsuarioId { get; set; }
        public DateTime Fecha { get; set;  }
        public int GrupoID { get; set; }

        public virtual Grupo grupo { get; set; }
    }

    public class ConceptosClasificacion
    {
        [Key]
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int ClasificacionID { get; set; }
        public int UsuarioID { get; set; }
        public DateTime Fecha { get; set; }
        public int Estado { get; set; }
        public int TipoID { get; set; }
        public virtual Clasificacion clasificacion { get; set; }

    }

    public class Etapa {
        [Key]
        public int ID { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioID { get; set; }       
        public int Estado { get; set; }
        public string Clase { get; set; }    
    }

    public class HallazgoCierre{
        [Key]
        public int ID { get; set; }
        public int ConceptosClasificacionID { get; set; }
        public int ProyectoID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public string Observaciones { get; set; }
        public int EtapaID { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioID { get; set; }       
        public int Estado { get; set; }
        public int Indice { get; set; }
        public DateTime FechaPeriodo { get; set; }

        public virtual ConceptosClasificacion conceptoCalsificacion { get; set; }
        public virtual Proyecto proyecto { get; set; }
        public virtual Etapa etapa { get; set; }
        public virtual Usuario usuario { get; set; }
    }
    public class HallazgoAvance {
        [Key]
        public int ID { get; set; }
        public int HallazgoCierreID { get; set; }
        public string Observaciones { get; set; }
        public int EtapaID { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioID { get; set; }

        public virtual HallazgoCierre hallazgoCierre { get; set; }
        public virtual Etapa etapa { get; set; }
        public virtual Usuario usuario { get; set; }
    
    }
    public class DatoHallazgo {
        [Key]
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }
    }
    public class DatoGrafico {
        public string Sigla { get; set; }
        public int Total { get; set; }         
    }
    public class GraficoPareto {
        public int ClasificacionID { get; set; }
        public string Clasificacion { get; set; }
        public int Frecuencia { get; set; }
        public double PorcentajeFrecuencia { get; set; }
        public int Acumulado { get; set; }
        public double PorcentajeAcumulado { get; set; }
    }
}
