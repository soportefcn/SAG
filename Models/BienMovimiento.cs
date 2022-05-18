using System;

using System.Collections.Generic;

using System.Linq;

using System.Web;



namespace SAG2.Models
{

    public class BienMovimiento
    {

        public int ID { get; set; }

        public DateTime FechaMovimiento { get; set; }

        public int Cantidad { get; set; }

        public string NuevaUbicacion { get; set; }

        public string RutaArchivo { get; set; }

        public string Detalle { get; set; }

        public int AutorizacionAuditor { get; set; }

        public string ComentarioAuditor { get; set; }

        public int? AuditorID { get; set; }

        public int? UsuarioID { get; set; }

        public int BienID { get; set; }

        public int EstadoID { get; set; }

        public int? CondicionID { get; set; }

        public int? bienAnteriorID { get; set; }



        public virtual BienCondicion Condicion { get; set; }



        public virtual Usuario Auditor { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual BienModInventario Bien { get; set; }

        public virtual BienEstadoInventario Estado { get; set; }

    }



    /*

    public class BienMovimientoVM 

    {

        public int ID { get; set; }

        public string Estado { get; set; }

        public string Inventario { get; set; }

        public string Ubicacion { get; set; }

        public string Descripcion { get; set; }

        public string Proyecto { get; set; }

        public DateTime FechaSistema { get; set; }

        public string Usuario { get; set; }

        public List<BienMovimientoVM> lista { get; set; }

    }*/

}