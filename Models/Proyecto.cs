using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
  
    public class Proyecto
    {
        [ScaffoldColumn(false)]
        public int ID { get; set; }
        public int TipoProyectoID { get; set; }
        public int SistemaAsistencialID { get; set; }
        public int DireccionID { get; set; }
        public int ConvenioID { get; set; }
        /*public int CuentaContable { get; set; }*/
        public string Nombre { get; set; }
        public string CodCodeni { get; set; }
        public string CodSename { get; set; }
        public string Institucion { get; set; }
        public string Fono { get; set; }
        public string Fax { get; set; }
        public int? EdadMinima { get; set; }
        public int? EdadMaxima { get; set; }
        public long? ValorSubvencion { get; set; }
        public string Sexo { get; set; }
        public string Flexibilizacion { get; set; }
        public int? DiasAtencion { get; set; }
        public string Cerrado { get; set; }
        public string Eliminado { get; set; }
        public int? PeriodoInicio { get; set; }
        public int? MesInicio { get; set; }
        public int estado { get; set; }
		public int MI {get; set;}
        public int LF { get; set; }
        

        public virtual Direccion Direccion { get; set; }
        public virtual SistemaAsistencial SistemaAsistencial { get; set; }
        public virtual Convenio Convenio { get; set; }
        public virtual TipoProyecto TipoProyecto { get; set; }
        public virtual List<Movimiento> Movimientos { get; set; }
        public virtual List<CuentaCorriente> CuentasCorrientes { get; set; }
        public virtual List<FondoFijo> FondosFijos { get; set; }
        public virtual List<Rol> Roles { get; set; }
        public virtual List<BoletaHonorario> BoletasHonorarios { get; set; }
        public virtual List<Intervencion> Intervenciones { get; set; }
        public virtual List<Presupuesto> Presupuestos { get; set; }

        public virtual string NombreLista {
            get { return CodCodeni + " - " + Nombre; }
        }

        public virtual string NombreEstado
        {
            get 
            { 
                if (this.Cerrado != null)
                {
                    return CodCodeni + " - " + Nombre + " (CERRADO)";
                }
                else
                {
                    return CodCodeni + " - " + Nombre;
                }
            }
                
        }

        public virtual bool estaCerrado
        {
            get
            {
                if (Cerrado != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual bool estaEliminado
        {
            get
            {
                if (Eliminado != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    public class ProyectosExcluidos {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int UsuarioID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public DateTime Fecha { get; set; }
        public int Estado { get; set; }
    }
}