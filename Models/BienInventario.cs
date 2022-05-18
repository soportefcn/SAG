using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class BienInventario
    {

        public int ID { get; set; }
        public int DependenciaID { get; set; }
        public int EspecieID { get; set; }
        public string NumeroInventario { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public int Procedencia { get; set; }
        public int Nce { get; set; }
        public int Periodo { get; set; }
        public string Factura { get; set; }
        public DateTime Fechacompra { get; set; }
        public int Estado { get; set; }
        public string Observacion { get; set; }
        public int ProyectoID { get; set; }

        public virtual Especie Especie { get; set; }
        public virtual Dependencia Dependencia { get; set; }
        public virtual string NombreCombo
        {
            get { return NumeroInventario + " / " + Descripcion; }
        }

    }
}
