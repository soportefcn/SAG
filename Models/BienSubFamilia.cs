using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class BienSubFamilia
    {
        public int ID { get; set; }
        public int FamiliaID { get; set; }
        public string Nombre { get; set; }
    }

    public class BienSubFamiliaViewModel 
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string descFamilia { get; set; }
    }
}