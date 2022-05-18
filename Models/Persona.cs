using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Persona
    {
        [ScaffoldColumn(false)]
        public int ID {get; set;}
        public int DireccionID {get; set;}

        [StringLength(10)]
        public string Rut {get; set;}

        [StringLength(1)]
        public string DV {get; set;}

        [StringLength(100)]
        public string Nombres {get; set;}
        
        [StringLength(100)]
        public string ApellidoParterno {get; set;}
        
        [StringLength(100)]
        public string ApellidoMaterno {get; set;}
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? FechaIngresoSistema { get; set; }
        
        [StringLength(1)]
        public string Sexo { get; set; }
        
        [StringLength(1)]
        public string EstadoCivil { get; set; }

        [ScaffoldColumn(false)]
        public int? Mostrar { get; set; }

        // Datos de contacto
        public int? Celular { get; set; }
        public int? Fijo { get; set; }
        public string CorreoElectronico { get; set; }

        public int? TipoPersonalID { get; set; }
        public int? ProfesionID { get; set; }
        public int? EspecializacionID { get; set; }
        public int? CargoID { get; set; }
        public int? SueldoBase { get; set; }
        public int? BonoLocomocion { get; set; }
        public int? BonoColacion { get; set; }
        public int? BonoAsignacion { get; set; }
        public int? BonoReemplazo { get; set; }
        public int? Otros { get; set; }

        public virtual TipoPersonal TipoPersonal { get; set; }
        public virtual Profesion Profesion { get; set; }
        public virtual Especializacion Especializacion { get; set; }
        public virtual Direccion Direccion { get; set; }
        public virtual List<Usuario> Usuarios { get; set; }

        // Listado de Movimientos en la que pesona es beneficiaria
        public virtual List<Movimiento> Movimientos { get; set; }
        public virtual List<Rol> Roles { get; set; }
        public virtual List<BoletaHonorario> BoletasHonorarios { get; set; }

        public virtual string RutDV
        {
            get { return Rut.Trim() + "-" + DV.Trim(); }
        }

        public virtual string NombreCompleto
        {
            get {
                    if (ApellidoMaterno != null)
                    {
                        return Nombres.Trim() + " " + ApellidoParterno.Trim() + " " + ApellidoMaterno.Trim();
                    }
                    else 
                    {
                        return Nombres.Trim() + " " + ApellidoParterno.Trim();
                    }
                }
        }

        public virtual string NombreLista
        {
            get { return RutDV + " " + NombreCompleto; }
        }
    }
}