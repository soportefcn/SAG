using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class OpcionesSupervision
    {
        public int ID { get; set; }
        public string Descripcion { get; set; }
        public string Opcion { get; set; }

        public virtual bool SI 
        {
            get 
            {
                if (Opcion != null && Opcion.Equals("S"))
                    return true;
                return false;
            }    
        }
    }
}