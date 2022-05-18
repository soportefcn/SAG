using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class CuentaCorriente
    {
        [ScaffoldColumn(false)]
        public int ID {get; set;}
        public int DireccionID {get; set;}
        
        public int BancoID {get; set;}
        public int ProyectoID { get; set; }
        public string Numero { get; set; }
        public string Sucursal {get; set;}
        public string Ejecutivo {get; set;}
        public string Fono {get; set;}
        public string Fax {get; set;}
        public string Movil {get; set;}
        public string CorreoElectronico { get; set; }
        public string PaginaWeb { get; set; }
        public string Comentarios {get; set;}
        public int? LineaCredito { get; set; }
        public int? Desactiva { get; set; }

        public virtual Proyecto Establecimiento { get; set; }
        public virtual Direccion Direccion { get; set; }
        public virtual Banco Banco { get; set; }
        public virtual List<Saldo> Saldos { get; set; }
        public virtual List<Movimiento> Movimientos { get; set; }

        public virtual string NumeroLista
        {
            get { return Numero.Trim(); }
        }
    }
}