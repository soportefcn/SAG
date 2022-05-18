using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class ParametroInforme
    {
        public int ID { get; set; }
        public int CInformeID { get; set; }
        public int Tipo { get; set; }
        public int Parametro { get; set; }
        public int ProyectoID { get; set; }

        public virtual string Nombrecuenta
        {
            get
            {
                string dato = " ";
                try
                {
                    using (SAG2DB db = new SAG2DB())
                    {
                        var cinforme = db.CinformeCierre.Where(d => d.ID == this.CInformeID).FirstOrDefault();
                        string datoGrupo = db.GinformeCierre.Where(d => d.ID == cinforme.GinformeID).FirstOrDefault().Descripcion;
                        dato = db.Cuenta.Where(d => d.ID == cinforme.CuentaID).FirstOrDefault().Nombre;
                        dato = datoGrupo + "/" + dato;
                    }
                }
                catch (Exception)
                {
                    dato = " ";
                }
                return dato;
            }

        }
        public virtual string NombreProyecto
        {
            get
            {
                string dato = " ";
                try
                {
                    using (SAG2DB db = new SAG2DB())
                    {
                        dato = db.Proyecto.Where(d => d.ID == this.ProyectoID).FirstOrDefault().NombreLista;                          
                    }
                }
                catch (Exception)
                {
                    dato = " ";
                }
                return dato;
            }

        }
        /*
         	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CInformeID] [int] NULL,
	[Tipo] [int] NULL,
	[Parametro] [int] NULL,
	[ProyectoID] [int] NULL,
         * 
         * 
         */
    }
}