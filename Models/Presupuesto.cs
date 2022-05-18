using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Models
{
    public class Presupuesto
    {
        public int ID { get; set; }
        public int ProyectoID { get; set; }
        public int Periodo { get; set; }
        public int Mes { get; set; }
        public string Activo { get; set; }
        public string Confirmado { get; set; }
        public int SaldoInicial { get; set; }

        public virtual int Periodo_Inicio
        {
            get
            {
                return Periodo;
            }
        }

        public virtual int Mes_Inicio
        {
            get
            {
                return Mes;
            }
        }

        public virtual bool estaActivo
        {
            get
            {
                return (Activo != null);
            }
        }

        public virtual bool estaConfirmado
        {
            get
            {
                return (Confirmado != null);
            }
        }

        public virtual Proyecto Proyecto { get; set; } 
    }
}