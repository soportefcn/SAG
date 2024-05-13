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

namespace SAG2.Controllers
{
    public class EstadoResultadoController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        //
        // GET: /EstadoResultado/ // Estandar
       
    // Programa
        public ActionResult ReporteProgramaEstandar()
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


            return View();

        }

        [HttpPost]
        public ActionResult ReporteProgramaEstandar(int Proyectos, int Mes, int? periodo)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            ViewBag.entrada = 2;
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");

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
            ViewBag.Periodo = periodo;
            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.ID == id && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.ID == id && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.ProyectoID == id).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
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
            ViewBag.SaldoInicial = SaldoInicial;
            return View();
        }

        public ActionResult ReporteProgramaEstandarExcel(int Proyectos, int Mes, int? periodo)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            ViewBag.entrada = 2;
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");

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
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.ID == id && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.ID == id && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.ProyectoID == id).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();
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
            ViewBag.SaldoInicial = SaldoInicial;
            return View();
        }

    // Linea 
        public ActionResult ReporteLineaEstandar()
        {
            int Mes = (int)Session["Mes"];
            int periodo = (int)Session["Periodo"];
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.LineaID = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Entrada = 1;
            return View();
        }

        [HttpPost]
        public ActionResult ReporteLineaEstandar(int LineaID, int Mes, int? periodo)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.LineaID = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla", LineaID);

            ViewBag.ID = LineaID;

            ViewBag.NombreLista = db.LineasAtencion.Find(LineaID).NombreLista;


            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == LineaID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == LineaID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();

            double SaldoInicial = 0;
            List<Saldo> ProgramSaldo = new List<Saldo>();
            try
            {
                 ProgramSaldo = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.Establecimiento.TipoProyecto.LineasAtencionID == LineaID).ToList();

                 foreach (Saldo PrSaldo in ProgramSaldo) {
                     SaldoInicial = PrSaldo.SaldoFinal + SaldoInicial;          
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
   
        public ActionResult ReporteLineaEstandarExcel(int LineaID, int Mes, int? periodo)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.LineaID = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla", LineaID);

            ViewBag.ID = LineaID;

            ViewBag.NombreLista = db.LineasAtencion.Find(LineaID).NombreLista;
            ViewBag.Sigla = db.LineasAtencion.Find(LineaID).Sigla;

            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == LineaID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyecto.LineasAtencionID == LineaID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.TipoProyecto.LineasAtencionID == LineaID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();

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

    // Region
        public ActionResult reporteRegionEstandar()
        {
            int Mes = (int)Session["Mes"];
            int periodo = (int)Session["Periodo"];
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.RegionID = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Entrada = 1;
            return View();
        }

        [HttpPost]
        public ActionResult reporteRegionEstandar(int RegionID, int Mes, int? periodo)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.RegionID = new SelectList(db.Region.ToList(), "ID", "Nombre",RegionID);

            ViewBag.ID = RegionID;

            ViewBag.NombreLista = db.Region.Find(RegionID).Nombre;


            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.Direccion.Comuna.RegionID == RegionID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.Direccion.Comuna.RegionID == RegionID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();

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

        public ActionResult reporteRegionEstandarExcel(int RegionID, int Mes, int? periodo)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.RegionID = new SelectList(db.Region.ToList(), "ID", "Nombre", RegionID);

            ViewBag.ID = RegionID;

            ViewBag.NombreLista = db.Region.Find(RegionID).Nombre;


            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.Direccion.Comuna.RegionID == RegionID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.Direccion.Comuna.RegionID == RegionID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.Direccion.Comuna.RegionID == RegionID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();

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
    
    // Tipo Programa
        public ActionResult reporteTipoEstandar()
        {
            int Mes = (int)Session["Mes"];
            int periodo = (int)Session["Periodo"];
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.TipoProyectoID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.Entrada = 1;
            return View();
        }

        [HttpPost]
        public ActionResult reporteTipoEstandar(int TipoProyectoID, int Mes, int? periodo)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.TipoProyectoID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla", TipoProyectoID);
            ViewBag.ID = TipoProyectoID;
            ViewBag.NombreLista = db.TipoProyecto.Find(TipoProyectoID).NombreLista;


            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == TipoProyectoID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyectoID == TipoProyectoID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();

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

        public ActionResult reporteTipoEstandarExcel(int TipoProyectoID, int Mes, int? periodo)
        {

            ViewBag.Entrada = 2;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.TipoProyectoID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla", TipoProyectoID);
            ViewBag.ID = TipoProyectoID;
            ViewBag.NombreLista = db.TipoProyecto.Find(TipoProyectoID).NombreLista;
            ViewBag.Sigla = db.TipoProyecto.Find(TipoProyectoID).Sigla;

            int PeriodoAnt = int.Parse(periodo.ToString()) - 1;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;
            // Movimientos 

            ViewBag.ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == TipoProyectoID && m.Periodo == periodo && m.Temporal == null && m.Nulo == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Proyecto.TipoProyectoID == TipoProyectoID && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.Nulo == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Proyecto.TipoProyectoID == TipoProyectoID).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.Nulo == null).OrderBy(m => m.CuentaIDD).ToList();

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
