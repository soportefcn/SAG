using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class ItemGasto
    {
        public int ID { get; set; }
        public string Nombre { get; set; }

        public virtual string NombreLista 
        {
            get { return ID.ToString() + " - " + Nombre; }
        }
    }
}