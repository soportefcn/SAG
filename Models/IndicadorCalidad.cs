using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAG2.Models
{
    public class IndicadorCalidad
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public int NumeroInforme { get; set; }
        public DateTime FechaInforme { get; set; }
        public int GastoObjetado { get; set;  }
         public int GastoRechazado { get; set;  }
          public int CantidadObservaciones { get; set;  }
         public string Observacion { get; set;  }
        public DateTime FechaCumplimiento { get; set; }
        public string AdjuntarInforme { get; set; }
        public int Tipo {get; set; }
        public DateTime FechaSistema { get; set; }
        public int UsuarioID {get; set; }

    }
}
