using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class Direccion
    {
        public int ID { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Depto { get; set; }
        public int ComunaID { get; set; }
        public int? Mostrar { get; set; }

        public virtual string DireccionLista
        {
            get 
            {
                if (Depto != "" && Depto != "0" && Depto != null)
                { 
                    return Calle + " " + Numero + " Depto. " + Depto;
                }
                return Calle + " " + Numero;
            }
        }

        public virtual Comuna Comuna { get; set; }
    }
}