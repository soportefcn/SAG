using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAG2.Classes
{
    public class Constantes
    {
        public Constantes()
        { }

        public int raizCuentaIngresos 
        {
            get
            { 
                return Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["ID_CUENTA_INGRESO"]);    
            }
        }

        public int raizCuentaEgresos 
        {
            get
            { 
                return Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["ID_CUENTA_EGRESO"]);    
            }
        }


        public int tipoIngreso 
        {
            get
            { 
                return 1;    
            }
        }

        public int tipoEgreso
        {
            get
            {
                return 2;
            }
        }

        public int tipoReintegro
        {
            get
            {
                return 3;
            }
        }

        public int montoFondoFijo
        { 
            get
            {
                return 100000;
            }
        }

        public double porcentajeFondoFijo
        {
            get
            {
                return .75;
            }
        }
    }
}