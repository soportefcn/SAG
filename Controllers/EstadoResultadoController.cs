using SAG2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAG2.Classes;
using System.Data.SqlClient;
using System.Configuration;

namespace SAG2.Controllers
{
    public class EstadoResultadoController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        //
        // GET: /EstadoResultado/ // Estandar
       
    // Programa
        public ActionResult ReporteProgramaEstandar(string TipoReporte)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            int Mes = (int)Session["Mes"];
            int periodo = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int Proyectos = Proyecto.ID;
            ViewBag.entrada = 1;
            if (filtro == 1)
            {
                ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null), "ID", "NombreLista", Proyectos);
            }
            else
            {
                ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado == null), "ID", "NombreLista", Proyectos);
            }
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            ViewBag.TipoReporte = TipoReporte;
            return View();
        }

        [HttpPost]
        public ActionResult ReporteProgramaEstandar(int Proyectos, int Mes, int? periodo, string TipoReporte)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            int id = Proyectos;
            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;

            List<DetallePresupuesto> dp = new List<DetallePresupuesto>();
            ViewBag.entrada = 2;
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.TipoReporte = TipoReporte;
            if (filtro == 1)
            {
                ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null), "ID", "NombreLista", Proyectos);
            }
            else
            {
                ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado == null), "ID", "NombreLista", Proyectos);
            }
            ViewBag.ID = Proyectos;
          
            ViewBag.NombreLista = db.Proyecto.Find(id).NombreLista;
            ViewBag.Periodo = periodo;
           
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            ViewBag.ConIva = db.Proyecto.Find(Proyectos).MI;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.ID == id && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.ID == id && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.ProyectoID == id).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.IngresosIva = db.DetalleIngresoIva.Where(m => m.Ingreso.ProyectoID == id).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).ToList(); 
            
            int SaldoInicial = 0;
            try
            {
                var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
                SaldoInicial = presupuesto.SaldoInicial;
                ViewBag.DetallePresupuesto = db.DetallePresupuesto.Where(d => d.PresupuestoID == presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();
            }
            catch (Exception)
            {
                try
                {
                    SaldoInicial = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.ProyectoID == Proyectos).FirstOrDefault().SaldoFinal;

                }
                catch (Exception)
                {
                    SaldoInicial = 0;
                }

            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            ViewBag.Cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden).ToList();
            ViewBag.SaldoInicial = SaldoInicial;
            ViewBag.Detalle = dp;
            return View();
        }

        public ActionResult ReporteProgramaEstandarExcel(int Proyectos, int Mes, int? periodo, string TipoReporte)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            int strTipoReporte = int.Parse(TipoReporte);
            ViewBag.entrada = 2;
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.TipoReporte = TipoReporte;
            string SiglaRep = "";
            switch (strTipoReporte)
            {
                case 1:
                    SiglaRep = "RptNRealPR";
                    break;
                case 2:
                    SiglaRep = "RptSRealPR";
                    break;
                case 3:
                    SiglaRep = "RptERealPR";
                     break;
            }
            ViewBag.SiglaRep = SiglaRep;
             
            if (filtro == 1)
            {
                ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null), "ID", "NombreLista", Proyectos);
            }
            else
            {
                ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado == null), "ID", "NombreLista", Proyectos);
            }
            ViewBag.ID = Proyectos;
            int id = Proyectos;
            ViewBag.NombreLista = db.Proyecto.Find(id).NombreLista;
            ViewBag.CC = db.Proyecto.Find(id).CodCodeni;
            ViewBag.Periodo = periodo;
            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            ViewBag.cuentasT = db.Cuenta.ToList();
            ViewBag.ConIva = db.Proyecto.Find(Proyectos).MI;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.ID == id && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.ID == id && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.ProyectoID == id).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.IngresosIva = db.DetalleIngresoIva.Where(m => m.Ingreso.ProyectoID == id).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).ToList(); 

            int SaldoInicial = 0;
            try
            {
                var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
                SaldoInicial = presupuesto.SaldoInicial;
            }
            catch (Exception)
            {
                try
                {
                    SaldoInicial = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.ProyectoID == Proyectos).FirstOrDefault().SaldoFinal;

                }
                catch (Exception)
                {
                    SaldoInicial = 0;
                }

            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            ViewBag.Cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden).ToList();
            ViewBag.SaldoInicial = SaldoInicial;
            return View();
        }

        // Region
        public ActionResult reporteRegionEstandar(string TipoReporte)
        {
            int Mes = (int)Session["Mes"];
            int periodo = (int)Session["Periodo"];
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.RegionID = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Entrada = 1;
            ViewBag.TipoReporte = TipoReporte;
            return View();
        }

        [HttpPost]
        public ActionResult reporteRegionEstandar(int RegionID, int Mes, int? periodo, string TipoReporte)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.RegionID = new SelectList(db.Region.ToList(), "ID", "Nombre", RegionID);
            ViewBag.TipoReporte = TipoReporte;
            ViewBag.ID = RegionID;

            ViewBag.NombreLista = db.Region.Find(RegionID).Nombre;


            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden).ToList();
            ViewBag.Mes = Mes;
            int ConIva = 0;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.Direccion.Comuna.RegionID == RegionID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.Direccion.Comuna.RegionID == RegionID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.IngresosIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).ToList();
            int IngIva = 0;
            int EgIva = 0;
            try
            {
                 IngIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).Count();
            }
            catch(Exception) {       
                IngIva = 0;     
            }
            try
            {
                EgIva = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.Direccion.Comuna.RegionID == RegionID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null && e.Cuenta.Codigo.Contains("12.1")).Count();
            }
            catch (Exception) {
                EgIva = 0;
            } 

            if (EgIva != 0) {
                ConIva = 1; 
            }
            if (IngIva != 0) {
                ConIva = 1; 
            }
            ViewBag.ConIva = ConIva;  
            int SaldoInicial = 0;
            try
            {
                SaldoInicial = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.Establecimiento.Direccion.Comuna.RegionID == RegionID).Sum(d => d.SaldoFinal);
            }
            catch (Exception)
            {
                SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            ViewBag.SaldoInicial = SaldoInicial;

            return View();
        }

        public ActionResult reporteRegionEstandarExcel(int RegionID, int Mes, int? periodo, string TipoReporte)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.RegionID = new SelectList(db.Region.ToList(), "ID", "Nombre", RegionID);
            ViewBag.ID = RegionID;
            ViewBag.TipoReporte = TipoReporte;
            ViewBag.NombreLista = db.Region.Find(RegionID).Nombre;
            int strTipoReporte = int.Parse(TipoReporte);
            string SiglaRep = "";
            switch (strTipoReporte)
            {
                case 1:
                    SiglaRep = "RptNRealRG";
                    break;
                case 2:
                    SiglaRep = "RptSRealRG";
                    break;
                case 3:
                    SiglaRep = "RptERealRG";
                    break;
            }
            ViewBag.SiglaRep = SiglaRep;
            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 
            
            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.Direccion.Comuna.RegionID == RegionID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.Direccion.Comuna.RegionID == RegionID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.IngresosIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).ToList();
            int IngIva = 0;
            int EgIva = 0;
            int ConIva = 0;
            try
            {
                IngIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).Count();
            }
            catch (Exception)
            {
                IngIva = 0;
            }
            try
            {
                EgIva = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.Direccion.Comuna.RegionID == RegionID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null && e.Cuenta.Codigo.Contains("12.1")).Count();
            }
            catch (Exception)
            {
                EgIva = 0;
            }

            if (EgIva != 0)
            {
                ConIva = 1;
            }
            if (IngIva != 0)
            {
                ConIva = 1;
            }
            ViewBag.ConIva = ConIva;  
            
            
            int SaldoInicial = 0;
            try
            {
                SaldoInicial = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.Establecimiento.Direccion.Comuna.RegionID == RegionID).Sum(d => d.SaldoFinal);
            }
            catch (Exception)
            {
                SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            ViewBag.SaldoInicial = SaldoInicial;

            return View();
        }

    // Linea 
        public ActionResult ReporteLineaEstandar(string TipoReporte)
        {
            int Mes = (int)Session["Mes"];
            int periodo = (int)Session["Periodo"];
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.LineaID = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Entrada = 1;
            ViewBag.TipoReporte = TipoReporte;
            return View();
        }

        [HttpPost]
        public ActionResult ReporteLineaEstandar(int LineaID, int Mes, int? periodo, string TipoReporte)
        {

            ViewBag.Entrada = 2;
            ViewBag.TipoReporte = TipoReporte;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.LineaID = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla", LineaID);

            ViewBag.ID = LineaID;

            ViewBag.NombreLista = db.LineasAtencion.Find(LineaID).NombreLista;


            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden).ToList();
            ViewBag.Mes = Mes;
            
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == LineaID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null && m.Proyecto.LF == 0).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null && e.Egreso.Proyecto.LF == 0 ).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo && m.Proyecto.LF == 0).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null && m.Reintegro.Proyecto.LF == 0).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.IngresosIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null && m.Ingreso.Proyecto.LF == 0 ).ToList();
            int ConIva = 0; 
            int IngIva = 0;
            int EgIva = 0;
            try
            {
                IngIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID && m.Ingreso.Proyecto.LF == 0).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).Count();
            }
            catch (Exception)
            {
                IngIva = 0;
            }
            try
            {
                EgIva = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null && e.Cuenta.Codigo.Contains("12.1") && e.Egreso.Proyecto.LF == 0).Count();
            }
            catch (Exception)
            {
                EgIva = 0;
            }

            if (EgIva != 0)
            {
                ConIva = 1;
            }
            if (IngIva != 0)
            {
                ConIva = 1;
            }
            ViewBag.ConIva = ConIva;

            // PeriodoAnt
            double SaldoInicial = 0;
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);

          
            try
            {
                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SAG2DB"].ConnectionString);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "execute SaldoLinea " + PeriodoAnt.ToString() + ",  12, " + LineaID.ToString();
                cmd.Connection.Open();
                cmd.ExecuteNonQuery(); sda.Fill(ds);
                cmd.Connection.Close();
                foreach (DataRow spf in ds.Tables[0].Rows)
                {
                    SaldoInicial = long.Parse(spf[1].ToString());
      
                }
            
            }
            catch (Exception)
            {
                SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            ViewBag.SaldoInicial = SaldoInicial;

            return View();
        }

       public ActionResult ReporteLineaEstandarExcel(int LineaID, int Mes, int? periodo, string TipoReporte)
        {
            ViewBag.TipoReporte = TipoReporte;
            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.LineaID = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla", LineaID);
            int strTipo = int.Parse(TipoReporte);
            string SiglaRep = "";
            switch (strTipo)
            {
                case 1:
                    SiglaRep = "RptNRealLA";
                    break;
                case 2:
                    SiglaRep = "RptSRealLA";
                    break;
                case 3:
                    SiglaRep = "RptERealLA";
                    break;
            }
            ViewBag.Siglastr = SiglaRep;
            ViewBag.ID = LineaID;

            ViewBag.NombreLista = db.LineasAtencion.Find(LineaID).NombreLista;
            ViewBag.Sigla = db.LineasAtencion.Find(LineaID).Sigla;

            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == LineaID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.IngresosIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).ToList();
            int ConIva = 0;
            int IngIva = 0;
            int EgIva = 0;
            try
            {
                IngIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).Count();
            }
            catch (Exception)
            {
                IngIva = 0;
            }
            try
            {
                EgIva = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null && e.Cuenta.Codigo.Contains("12.1")).Count();
            }
            catch (Exception)
            {
                EgIva = 0;
            }

            if (EgIva != 0)
            {
                ConIva = 1;
            }
            if (IngIva != 0)
            {
                ConIva = 1;
            }
            ViewBag.ConIva = ConIva; 
 
            int SaldoInicial = 0;
            try
            {
                SaldoInicial = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.Establecimiento.TipoProyecto.LineasAtencionID == LineaID).Sum(d => d.SaldoFinal);
            }
            catch (Exception)
            {
                SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            ViewBag.SaldoInicial = SaldoInicial;

            return View();
        }

 
    // Tipo Programa
        public ActionResult reporteTipoEstandar(string TipoReporte)
        {
            int Mes = (int)Session["Mes"];
            int periodo = (int)Session["Periodo"];
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.TipoProyectoID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.Entrada = 1;
            ViewBag.TipoReporte = TipoReporte;
            return View();
        }

        [HttpPost]
        public ActionResult reporteTipoEstandar(int TipoProyectoID, int Mes, int? periodo, string TipoReporte)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.TipoProyectoID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla", TipoProyectoID);
            ViewBag.ID = TipoProyectoID;
            ViewBag.NombreLista = db.TipoProyecto.Find(TipoProyectoID).NombreLista;
            ViewBag.TipoReporte = TipoReporte;

            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden).ToList();
            ViewBag.Mes = Mes;
            int ConIva = 0;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == TipoProyectoID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyectoID == TipoProyectoID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.IngresosIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).ToList();
            int IngIva = 0;
            int EgIva = 0;
            try
            {
                IngIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).Count();
            }
            catch (Exception)
            {
                IngIva = 0;
            }
            try
            {
                EgIva = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyectoID == TipoProyectoID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null && e.Cuenta.Codigo.Contains("12.1")).Count();
            }
            catch (Exception)
            {
                EgIva = 0;
            }

            if (EgIva != 0)
            {
                ConIva = 1;
            }
            if (IngIva != 0)
            {
                ConIva = 1;
            }
            ViewBag.ConIva = ConIva; 
            int SaldoInicial = 0;
            try
            {
                SaldoInicial = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.Establecimiento.TipoProyectoID == TipoProyectoID).Sum(d => d.SaldoFinal);
            }
            catch (Exception)
            {
                SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {  1, 2, 3, 4, 5, 6 };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            ViewBag.SaldoInicial = SaldoInicial;

            return View();
        }

        public ActionResult reporteTipoEstandarExcel(int TipoProyectoID, int Mes, int? periodo, string TipoReporte)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.TipoProyectoID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla", TipoProyectoID);
            ViewBag.ID = TipoProyectoID;
            ViewBag.NombreLista = db.TipoProyecto.Find(TipoProyectoID).NombreLista;
            ViewBag.Sigla = db.TipoProyecto.Find(TipoProyectoID).Sigla;
            ViewBag.TipoReporte = TipoReporte;
            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden).ToList();

            ViewBag.Mes = Mes;
            int strTipoReporte = int.Parse(TipoReporte);
            string SiglaRep = "";
            switch (strTipoReporte)
            {
                case 1:
                    SiglaRep = "RptNRealTP";
                    break;
                case 2:
                    SiglaRep = "RptSRealTP";
                    break;
                case 3:
                    SiglaRep = "RptERealTP";
                    break;
            }
            ViewBag.SiglaRep = SiglaRep;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == TipoProyectoID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyectoID == TipoProyectoID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.IngresosIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).ToList();
            int ConIva = 0;
            int IngIva = 0;
            int EgIva = 0;
            try
            {
                IngIva = db.DetalleIngresoIva.Where(m => m.Ingreso.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.Ingreso.Periodo == periodo && m.Ingreso.Nulo == null && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).Count();
            }
            catch (Exception)
            {
                IngIva = 0;
            }
            try
            {
                EgIva = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyectoID == TipoProyectoID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null && e.Cuenta.Codigo.Contains("12.1")).Count();
            }
            catch (Exception)
            {
                EgIva = 0;
            }

            if (EgIva != 0)
            {
                ConIva = 1;
            }
            if (IngIva != 0)
            {
                ConIva = 1;
            }
            ViewBag.ConIva = ConIva; 
            int SaldoInicial = 0;
            try
            {
                SaldoInicial = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.Establecimiento.TipoProyectoID == TipoProyectoID).Sum(d => d.SaldoFinal);
            }
            catch (Exception)
            {
                SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] { 1, 2, 3, 4, 5, 6 };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            ViewBag.SaldoInicial = SaldoInicial;

            return View();
        }
       
    
    
    }
}
