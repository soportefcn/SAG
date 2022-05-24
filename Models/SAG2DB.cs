using System.Data.Entity;

namespace SAG2.Models
{
    public class SAG2DB : DbContext
    {
        public DbSet<Banco> Banco { get; set; }
        public DbSet<CuentaCorriente> CuentaCorriente { get; set; }
        public DbSet<Direccion> Direccion { get; set; }
        public DbSet<Proyecto> Proyecto { get; set; }
        public DbSet<Movimiento> Movimiento { get; set; }
        public DbSet<Persona> Persona { get; set; }
        public DbSet<SistemaAsistencial> SistemaAsistencial { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }

        // Agregados el 24 de Enero de 2012
        public DbSet<FondoFijo> FondoFijo { get; set; }
        public DbSet<FondoFijoGrupo> FondoFijoGrupo { get; set; }
        public DbSet<ItemGasto> ItemGasto { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

        // 25 de Enero 2012
        public DbSet<DepositoPlazo> DepositoPlazo { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Comuna> Comuna { get; set; }
        public DbSet<Pais> Pais { get; set; }
        public DbSet<LineasAtencion> LineasAtencion { get; set; }
        public DbSet<Documento> Documento { get; set; }

        // 12 Feb 2012
        public DbSet<Saldo> Saldo { get; set; }
        public DbSet<Rol> Rol { get; set; }

        public DbSet<ObjetivoCuenta> ObjetivoCuenta { get; set; }
        public DbSet<ItemIntervencion> ItemIntervencion { get; set; }
        public DbSet<Profesion> Profesion { get; set; }
        public DbSet<Especializacion> Especializacion { get; set; }
        public DbSet<Cargo> Cargo { get; set; }
        public DbSet<Servicio> Servicio { get; set; }
        public DbSet<Contrato> Contrato { get; set; }

        // Tipos
        public DbSet<TipoAsistenciaPersonal> TipoAsistenciaPersonal { get; set; }
        public DbSet<TipoBajaInventario> TipoBajaInventario { get; set; }
        public DbSet<TipoComprobante> TipoComprobante { get; set; }
        public DbSet<TipoImputacion> TipoImputacion { get; set; }
        public DbSet<TipoOrigenAdquisicion> TipoOrigenAdquisicion { get; set; }
        public DbSet<TipoPersonal> TipoPersonal { get; set; }
        public DbSet<TipoProyecto> TipoProyecto { get; set; }
        public DbSet<TipoRol> TipoRol { get; set; }
        public DbSet<Periodo> Periodo { get; set; }
        public DbSet<Convenio> Convenio { get; set; }
        public DbSet<Auditoria> Auditoria { get; set; }
        public DbSet<Supervision> Supervision { get; set; }
        public DbSet<Permiso> Permiso { get; set; }
        public DbSet<Seccion> Seccion { get; set; }
        public DbSet<Cuenta> Cuenta { get; set; }
        public DbSet<BoletaHonorario> BoletaHonorario { get; set; }
        public DbSet<DeudaPendiente> DeudaPendiente { get; set; }
        public DbSet<DetalleEgreso> DetalleEgreso { get; set; }
        public DbSet<Intervencion> Intervencion { get; set; }

        // Mantenedores > Inventarios
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<UnidadMedida> UnidadMedida { get; set; }
        public DbSet<Bodega> Bodega { get; set; }
        public DbSet<MovimientosBodega> MovimientoBodega { get; set; }
        public DbSet<RolProveedor> RolProveedor { get; set; }
        public DbSet<BodegaCuenta> BodegaCuenta { get; set; }
        public DbSet<BodegaCuentaCategoria> BodegaCuentaCategoria { get; set; }


        public DbSet<Presupuesto> Presupuesto { get; set; }
        public DbSet<DetallePresupuesto> DetallePresupuesto { get; set; }

        // Conciliacion
        public DbSet<Conciliacion> Conciliacion { get; set; }
        public DbSet<ConciliacionRegistro> ConciliacionRegistro { get; set; }


        // Autorizaciones
        public DbSet<OpcionesSupervision> OpcionesSupervision { get; set; }
        public DbSet<Autorizacion> Autorizacion { get; set; }


        // Auditorias
        public DbSet<ProgramaAnualAuditorias> ProgramaAnualAuditorias { get; set; }
        public DbSet<AuditoriasDocumento> AuditoriasDocumento { get; set; }
        public DbSet<AuditoriasMetodologia> AuditoriasMetodologia { get; set; }
        public DbSet<AuditoriasObjetivo> AuditoriasObjetivo { get; set; }
        public DbSet<PlanTrabajoAuditoria> PlanTrabajoAuditoria { get; set; }
        public DbSet<InformeAuditoria> InformeAuditoria { get; set; }
        public DbSet<TipoAuditoria> TipoAuditoria { get; set; }

        // Indice Gestion
        public DbSet<IndicadoresGestion> IndicadoresGestion { get; set; }

        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Especie> Especie { get; set; }
        public DbSet<Familia> Familia { get; set; }
        public DbSet<Dependencia> Dependencia { get; set; }


        public DbSet<RegistroModificacionProyecto> RegistroModificacionProyecto { get; set; }
        public DbSet<AutorizacionMovimiento> AutorizacionMovimiento { get; set; }

        // Preguntas Frecuestes
        public DbSet<PreguntaFrecuente> PreguntaFrecuente { get; set; }
        // Sename
        public DbSet<Objetivo> Objetivo { get; set; }
        public DbSet<SenameUso> SenameUso { get; set; }
        // inventario
        public DbSet<InventarioEspecie> InventarioEspecie { get; set; }
        public DbSet<InventarioBien> InventarioBien { get; set; }
        public DbSet<BienInventario> BienInventario { get; set; }
        public DbSet<Tipo> Tipo { get; set; }

        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<EspecieCuenta> EspecieCuenta { get; set; }

        public DbSet<InventarioCorrelativo> InventarioCorrelativo { get; set; }
        // Indicador Cuenta
        public DbSet<IndicadoresCuenta> IndicadoresCuenta { get; set; }

        public DbSet<Variable> Variable { get; set; }
        public DbSet<IndicadorCalidad> IndicadorCalidad { get; set; }
        public DbSet<RespuestaAuditoria> RespuestaAuditoria { get; set; }
        public DbSet<HorasExtras> HorasExtras { get; set; }
        public DbSet<DetalleReintegro> DetalleReintegro { get; set; }
        public DbSet<DetalleIngreso> DetalleIngreso { get; set; }

        //Agregado desde julio 2017 - nuevo modulo de inventario
        public DbSet<BienSubFamilia> BienSubFamilia { get; set; }
        public DbSet<BienFamilia> BienFamilia { get; set; }
        public DbSet<BienEstadoInventario> BienEstadoInventario { get; set; }
        public DbSet<BienModInventario> BienModInventario { get; set; }
        public DbSet<BienMovimiento> BienMovimiento { get; set; }
        public DbSet<BienCondicion> BienCondicion { get; set; }
        public DbSet<BienProcedencia> BienProcedencia { get; set; }
        // agregado por Resoluciones
        public DbSet<Resolucion> Resolucion { get; set; }
        public DbSet<Estandarvalores> Estandarvalores { get; set; }
        //Reportes

        public DbSet<TipoInforme> TipoInforme { get; set; }
        public DbSet<Informe> Informe { get; set; }
        //Intervenciones
        public DbSet<EstadoIntervencion> EstadoIntervencion { get; set; }
        public DbSet<ParametroUss> ParametroUss { get; set; }
        public DbSet<IntervencionDetalle> IntervencionDetalle { get; set; }
        public DbSet<IntervencionResumen> IntervencionResumen { get; set; }
        public DbSet<Programa> Programa { get; set; }
        public DbSet<ProgramaQ> ProgramaQ { get; set; }
        public DbSet<PorcentajeZona> PorcentajeZona { get; set; }
        public DbSet<cuentaGrupo> cuentaGrupo { get; set; }
        public DbSet<SaldosCorporativos> SaldosCorporativos { get; set; }
        public DbSet<GinformeCierre> GinformeCierre { get; set; }
        public DbSet<CInformeCierre> CinformeCierre { get; set; }
        public DbSet<ParametroInforme> ParametroInforme { get; set; }

        public DbSet<Referencia> Referencia { get; set; }
        public DbSet<ControlFlujo> ControlFlujo { get; set; }
        public DbSet<PeriodoLog> PeriodoLog { get; set; }
        public DbSet<IntervencionLog> IntervencionLog { get; set; } 
        public DbSet <IntervencionResumenLog> IntervencionResumenLog { get; set; }
        public DbSet<TipoSename > TipoSename { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {          
            modelBuilder.Entity<Estandarvalores>().ToTable("Estandarvalores"); 
            modelBuilder.Entity<InventarioCorrelativo>().ToTable("InventarioCorrelativo");
            modelBuilder.Entity<EspecieCuenta>().ToTable("EspecieCuenta");
            modelBuilder.Entity<BodegaCuentaCategoria>().ToTable("BodegaCuentaCategoria");
            modelBuilder.Entity<BodegaCuenta>().ToTable("BodegaCuenta");
            modelBuilder.Entity<Categoria>().ToTable("Categoria");
            modelBuilder.Entity<Tipo>().ToTable("Tipo");
            modelBuilder.Entity<InventarioBien>().ToTable("InventarioBien");
            modelBuilder.Entity<BienInventario>().ToTable("BienInventario");
            modelBuilder.Entity<InventarioEspecie>().ToTable("InventarioEspecie");
            modelBuilder.Entity<SenameUso>().ToTable("SenameUso");
            modelBuilder.Entity<Objetivo>().ToTable("Objetivo");
            modelBuilder.Entity<Banco>().ToTable("Banco");
            modelBuilder.Entity<CuentaCorriente>().ToTable("CuentaCorriente");
            modelBuilder.Entity<Direccion>().ToTable("Direccion");
            modelBuilder.Entity<Proyecto>().ToTable("Proyectos");
            modelBuilder.Entity<Movimiento>().ToTable("Movimiento");
            modelBuilder.Entity<Persona>().ToTable("Persona");
            modelBuilder.Entity<SistemaAsistencial>().ToTable("SistemaAsistencial");
            modelBuilder.Entity<Proveedor>().ToTable("Proveedor");

            // Agregados el 24 de Enero de 2012
            modelBuilder.Entity<ItemGasto>().ToTable("ItemGasto");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");

            // 25 de Enero de 2012
            modelBuilder.Entity<DepositoPlazo>().ToTable("DepositoPlazo");
            modelBuilder.Entity<Region>().ToTable("Regiones");
            modelBuilder.Entity<Pais>().ToTable("Paises");
            modelBuilder.Entity<Comuna>().ToTable("Comunas");
            modelBuilder.Entity<LineasAtencion>().ToTable("LineasAtencion");
            modelBuilder.Entity<Documento>().ToTable("Documento");

            // 12 Feb 2012
            modelBuilder.Entity<Saldo>().ToTable("Saldo");
            modelBuilder.Entity<Rol>().ToTable("Rol");

            modelBuilder.Entity<ObjetivoCuenta>().ToTable("ObjetivoCuenta");
            modelBuilder.Entity<ItemIntervencion>().ToTable("ItemIntervencion");
            modelBuilder.Entity<Profesion>().ToTable("Profesion");
            modelBuilder.Entity<Especializacion>().ToTable("Especializacion");
            modelBuilder.Entity<Cargo>().ToTable("Cargo");

            
            // Tipos
            modelBuilder.Entity<TipoAsistenciaPersonal>().ToTable("TipoAsistenciaPersonal");
            modelBuilder.Entity<TipoBajaInventario>().ToTable("TipoBajaInventario");
            modelBuilder.Entity<TipoComprobante>().ToTable("TipoComprobante");
            modelBuilder.Entity<TipoImputacion>().ToTable("TipoImputacion");
            modelBuilder.Entity<TipoOrigenAdquisicion>().ToTable("TipoOrigenAdquisicion");
            modelBuilder.Entity<TipoPersonal>().ToTable("TipoPersonal");
            modelBuilder.Entity<TipoRol>().ToTable("TipoRol");
            modelBuilder.Entity<TipoProyecto>().ToTable("TipoProyecto");
            modelBuilder.Entity<Convenio>().ToTable("Convenios");
            modelBuilder.Entity<Periodo>().ToTable("Periodos");

            modelBuilder.Entity<Supervision>().ToTable("Supervisiones");
            modelBuilder.Entity<Auditoria>().ToTable("Auditorias");
            modelBuilder.Entity<Seccion>().ToTable("Secciones");
            modelBuilder.Entity<Permiso>().ToTable("Permisos");

            modelBuilder.Entity<Servicio>().ToTable("Servicios");
            modelBuilder.Entity<Contrato>().ToTable("Contratos");
            modelBuilder.Entity<Cuenta>().ToTable("Cuentas");
            modelBuilder.Entity<DeudaPendiente>().ToTable("DeudaPendiente");
            modelBuilder.Entity<FondoFijo>().ToTable("FondoFijo");
            modelBuilder.Entity<FondoFijoGrupo>().ToTable("FondoFijoGrupo");
            modelBuilder.Entity<BoletaHonorario>().ToTable("BoletaHonorario");
            modelBuilder.Entity<DetalleEgreso>().ToTable("DetalleEgreso");
            modelBuilder.Entity<Intervencion>().ToTable("Intervenciones");

            // Mantenedores > Inventarios
            modelBuilder.Entity<Articulo>().ToTable("Articulos");
            modelBuilder.Entity<UnidadMedida>().ToTable("UnidadesMedida");
            modelBuilder.Entity<Bodega>().ToTable("Bodega");
            modelBuilder.Entity<MovimientosBodega>().ToTable("BodegaMovimientos");

            modelBuilder.Entity<Presupuesto>().ToTable("Presupuesto");
            modelBuilder.Entity<DetallePresupuesto>().ToTable("DetallePresupuesto");
            modelBuilder.Entity<RolProveedor>().ToTable("RolProveedor");

            modelBuilder.Entity<Conciliacion>().ToTable("Conciliacion");
            modelBuilder.Entity<ConciliacionRegistro>().ToTable("ConciliacionRegistro");
            modelBuilder.Entity<OpcionesSupervision>().ToTable("OpcionesSupervision");
            modelBuilder.Entity<Autorizacion>().ToTable("Autorizaciones");


            // Auditorias
            modelBuilder.Entity<ProgramaAnualAuditorias>().ToTable("ProgramaAnualAuditorias");
            modelBuilder.Entity<AuditoriasDocumento>().ToTable("AuditoriasDocumento");
            modelBuilder.Entity<AuditoriasMetodologia>().ToTable("AuditoriasMetodologia");
            modelBuilder.Entity<AuditoriasObjetivo>().ToTable("AuditoriasObjetivo");
            modelBuilder.Entity<PlanTrabajoAuditoria>().ToTable("PlanTrabajoAuditoria");

            modelBuilder.Entity<IndicadoresGestion>().ToTable("IndicadoresGestion");
            modelBuilder.Entity<InformeAuditoria>().ToTable("InformeAuditoria");
            modelBuilder.Entity<TipoAuditoria>().ToTable("TipoAuditoria");

            modelBuilder.Entity<Inventario>().ToTable("Inventario");
            modelBuilder.Entity<Familia>().ToTable("Familia");
            modelBuilder.Entity<Especie>().ToTable("Especie");
            modelBuilder.Entity<Dependencia>().ToTable("Dependencia");


            modelBuilder.Entity<RegistroModificacionProyecto>().ToTable("RegistroModificacionProyecto");
            //Reportes

            modelBuilder.Entity<TipoInforme>().ToTable("TipoInforme");
            modelBuilder.Entity<Informe>().ToTable("Informe");

            modelBuilder.Entity<PreguntaFrecuente>().ToTable("PreguntasFrecuentes");
            modelBuilder.Entity<AutorizacionMovimiento>().ToTable("AutorizacionMovimiento");
            modelBuilder.Entity<IndicadoresCuenta>().ToTable("IndicadoresCuenta");
            modelBuilder.Entity<Variable>().ToTable("Variable");
            modelBuilder.Entity<IndicadorCalidad>().ToTable("IndicadorCalidad");
            modelBuilder.Entity<RespuestaAuditoria>().ToTable("RespuestaAuditoria");
            modelBuilder.Entity<HorasExtras>().ToTable("HorasExtras");
            modelBuilder.Entity<DetalleReintegro>().ToTable("DetalleReintegro");
            modelBuilder.Entity<DetalleIngreso>().ToTable("DetalleIngreso");

            //Agregado desde julio 2017 - nuevo modulo de inventario
            modelBuilder.Entity<BienSubFamilia>().ToTable("BienSubFamilia");
            modelBuilder.Entity<BienFamilia>().ToTable("BienFamilia");
            modelBuilder.Entity<BienEstadoInventario>().ToTable("BienEstadoInventario");
            modelBuilder.Entity<BienModInventario>().ToTable("BienModInventario");
            modelBuilder.Entity<BienMovimiento>().ToTable("BienMovimientoInventario");
            modelBuilder.Entity<BienCondicion>().ToTable("BienCondicion");
            modelBuilder.Entity<BienProcedencia>().ToTable("BienProcedencia");
            modelBuilder.Entity<Resolucion>().ToTable("Resolucion");
            //Agregado Intervencion
            modelBuilder.Entity<ProgramaQ>().ToTable("programa_q");
            modelBuilder.Entity<EstadoIntervencion>().ToTable("estado_intervencion");
            modelBuilder.Entity<ParametroUss>().ToTable("parametro_uss");
            modelBuilder.Entity<IntervencionDetalle>().ToTable("intervencion_detalle");
            modelBuilder.Entity<IntervencionResumen>().ToTable("intervencion_resumen");
            modelBuilder.Entity<Programa>().ToTable("programa");
            modelBuilder.Entity<PorcentajeZona>().ToTable("PorcentajeZona");
            modelBuilder.Entity<cuentaGrupo>().ToTable("cuentaGrupo");
            modelBuilder.Entity<SaldosCorporativos>().ToTable("SaldosCorporativos");
            modelBuilder.Entity<GinformeCierre>().ToTable("GinformeCierre");
            modelBuilder.Entity<CInformeCierre>().ToTable("CinformeCierre");
            modelBuilder.Entity<ParametroInforme>().ToTable("ParametroInforme");
            modelBuilder.Entity<Referencia>().ToTable("Referencia");
            modelBuilder.Entity<ControlFlujo>().ToTable("Control");
            modelBuilder.Entity<IntervencionLog>().ToTable("IntervencionesLog");
            modelBuilder.Entity<PeriodoLog>().ToTable("PeriodoLog");
            modelBuilder.Entity<IntervencionResumenLog>().ToTable("intervencionresumenLog");
            modelBuilder.Entity<TipoSename>().ToTable("TipoSename");   
        }
        public System.Data.Entity.DbSet<SAG2.Models.DetalleInformes> DetalleInformes { get; set; }
        public System.Data.Entity.DbSet<SAG2.Models.CuentasPadres> CuentasPadres { get; set; }
    }
}