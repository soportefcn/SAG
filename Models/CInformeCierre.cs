using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class CInformeCierre
    {
        public int ID { get; set; }
        public int CuentaID { get; set; }
        public int GinformeID { get; set; }
        public int CTotal { get; set; }
        public int valor { get; set; }
        public int tipoComprobante { get; set; }
        public virtual string Nombrecuenta
        {
            get
            {

                // int datoCuenta = 0;
                string dato = " ";

                try
                {
                    using (SAG2DB db = new SAG2DB())
                    {

                        string datoGrupo = db.GinformeCierre.Where(d => d.ID == this.GinformeID).FirstOrDefault().Descripcion;      
                        dato = db.Cuenta.Where(d => d.ID == this.CuentaID).FirstOrDefault().Nombre;
                        dato = datoGrupo + "/" + dato;

                        //dato = db.maestroMovimiento.Where(d => d.codigoB == codigoB && d.estadoE.Equals("V")).Sum(d => d.stock).Value;

                    }
                }
                catch (Exception)
                {
                    dato = " ";
                }

                return dato;

            }

        }
    }
}