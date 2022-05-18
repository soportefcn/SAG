using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAG2.Models
{
    public class RespuestaAuditoria
    {
        public int ID { get; set; }
       public int IndicadorCalidadID { get; set;}
      public DateTime FechaRespuestA {get; set; }
      public string Observacion { get; set; }
      public string AdjuntarInforme { get; set; }
      public DateTime FechaSistema {get; set;}
      public int UsuarioID { get; set; }
    }
}
