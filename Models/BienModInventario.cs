using System;

using System.Collections.Generic;

using System.Linq;

using System.Web;



namespace SAG2.Models
{

    public class BienModInventario
    {

        // [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int ID { get; set; }
        public DateTime Fecha { get; set; }
        public int FamiliaID { get; set; }
        public int SubFamiliaID { get; set; }
        public int ProcedenciaID { get; set; }
        public string DescripcionBien { get; set; }
        public int Cantidad { get; set; }
        public int Monto { get; set; }
        public string Ubicacion { get; set; }
        public int EstadoID { get; set; }
        public int ProyectoID { get; set; }
        public int UsuarioID { get; set; }
        public int? EgresoID { get; set; }
        public int? ReintegroID { get; set; }
        public int? MovimientoID { get; set; }
        public int? ProyectoAnteriorID { get; set; }
        public int CondicionID { get; set; }

   /*     public virtual BienFamilia Familia { get; set; }
        public virtual BienSubFamilia SubFamilia { get; set; }
        public virtual BienProcedencia Procedencia { get; set; }
        public virtual BienEstadoInventario Estado { get; set; }
        public virtual Proyecto Proyecto { get; set; }   
        public virtual Usuario Usuario { get; set; }
        public virtual BienCondicion Condicion { get; set; } */


        public virtual BienFamilia Familia { get; set; }
        public virtual BienSubFamilia SubFamilia { get; set; }
        public virtual BienProcedencia Procedencia { get; set; }
        public virtual BienEstadoInventario Estado { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public virtual Proyecto ProyectoAnterior { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual DetalleEgreso Egreso { get; set; }
        public virtual DetalleReintegro Reintegro { get; set; }
        public virtual BienCondicion Condicion { get; set; }
        public virtual Movimiento Movimiento { get; set; }
       
    }



    public class BienModInventarioVM
    {

        public int ID { get; set; }
        public int ProyectoAnteriorID { get; set; }
        public Proyecto ClaseProyecto { get; set; }
        public DateTime Fecha { get; set; }
        public string RutaArchivo { get; set; }
        public HttpPostedFileBase Archivo { get; set; }
        public string Detalle { get; set; }
        public string DescripcionBien { get; set; }
        public int Cantidad { get; set; }
        public int MontoInt { get; set; }
        public string Ubicacion { get; set; }
        public int FamiliaID { get; set; }
        public string Familia { get; set; }
        public int SubFamiliaID { get; set; }
        public string SubFamilia { get; set; }
        public int ProcedenciaID { get; set; }
        public string Procedencia { get; set; }
        public int EstadoID { get; set; }
        public string Estado { get; set; }
        public int ProyectoID { get; set; }
        public string Proyecto { get; set; }
        public string Usuario { get; set; }
        public int UsuarioID { get; set; }
        public int? EgresoID { get; set; }
        public int? ReintegroID { get; set; }
        public int CondicionID { get; set; }

        public string CondicionText { get; set; }
        public string Condicion { get; set; }

        public int? AuditorID { get; set; }

        public string ComentarioAuditor { get; set; }

        public int AutorizacionAuditor { get; set; }

        public string Ubicacion2 { get; set; }

        public string Monto { get; set; }

        public string NDocumento { get; set; }

        public int? MovimientoID { get; set; }

        public int MovimientoBienID { get; set; }



        public DateTime? Desde { get; set; }

        public DateTime? Hasta { get; set; }



        public virtual Usuario Auditor { get; set; }

        public virtual Movimiento Movimiento { get; set; }

        public virtual DetalleEgreso Egreso { get; set; }

        public virtual DetalleReintegro Reintegro { get; set; }

        public List<BienModInventarioVM> lista { get; set; }



    }

    public class BienModDonacion
    {
        public int ID { get; set; }

        public string RutaArchivo { get; set; }
        public HttpPostedFileBase Archivo { get; set; }
        public int FamiliaID { get; set; }
        public int UsuarioID { get; set; }
        public int CondicionID { get; set; }
        public int? EstadoID { get; set; }
        public int Cantidad { get; set; }
        public int MontoInt { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; }
        public string DescripcionBien { get; set; }
    
        public string Ubicacion { get; set; }


        public int SubFamiliaID { get; set; }
      


    }
}