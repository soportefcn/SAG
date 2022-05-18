using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Web.Script.Serialization;

namespace SAG2.Controllers
{
    public class InformesController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Constantes ctes = new Constantes();
        private Util utils = new Util();
        //
        // GET: /Informes/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegistroSemanalPago()
        {
            DateTime inicio;
            DateTime fin;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            ViewBag.CodSename = Proyecto.CodSename;   
            if (Request.QueryString["i"] != null && Request.QueryString["f"] != null && Request.QueryString["s"] != null)
            {
                @ViewBag.Fecha = Request.QueryString["s"].ToString();

                inicio = DateTime.Parse(Request.QueryString["i"].ToString());
                fin = DateTime.Parse(Request.QueryString["f"].ToString()).AddDays(1);
            }
            else
            {
                DateTime Fecha = new DateTime();
                try
                {
                    Fecha = new DateTime((int)Session["Periodo"], (int)Session["Mes"], DateTime.Now.Day);
                }
                catch (Exception)
                {
                    try
                    {
                        Fecha = new DateTime((int)Session["Periodo"], (int)Session["Mes"], 30);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            Fecha = new DateTime((int)Session["Periodo"], (int)Session["Mes"], 29);
                        }
                        catch (Exception)
                        {
                            Fecha = new DateTime((int)Session["Periodo"], (int)Session["Mes"], 28);
                        }
                    }
                }
                @ViewBag.Fecha = Fecha.ToShortDateString();
                // Si semana empieza el domingo, agregar 2
                int add = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["addDays"]);
                int rm = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["rmDays"]);
                DateTime to = DateTime.Now.AddDays(add); // stores today date
                int x = (int)to.DayOfWeek; // gets the day of this week
                inicio = to.AddDays(-x - rm); // get the start date of this week
                fin = to.AddDays(6 - x - rm); // get the end date of this week
            }

            List<Movimiento> Ingresos = db.Movimiento.Where(m=> m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID != ctes.tipoEgreso).Where(m => m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).ToList();
            List<Movimiento> Egresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();
            List<Movimiento> Egresost = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Temporal == null && m.Eliminado == null && m.Nulo == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();
            @ViewBag.Ingresos_Reintegros = Ingresos;
            @ViewBag.Egresos = Egresos;
            ViewBag.total_ingresos = Ingresos.Sum(i => i.Monto_Ingresos);
            ViewBag.total_egresos = Egresost.Sum(e => e.Monto_Egresos);
            ViewBag.Monto_Ingresos = 0;
            ViewBag.Monto_Egresos = 0;
            ViewBag.SaldoFinal = 0;

            try
            {
                DateTime inicioTmp = new DateTime(inicio.Year, inicio.Month, 1);

                //DateTime inicioTmp = DateTime.Parse("01-" + inicio.Month + "-" + inicio.Year);
                //inicio = inicioTmp;
                //int ingresos = db.Movimiento.Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos);
                //int egresos = db.Movimiento.Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).Sum(m => m.Monto_Egresos);
                //Movimiento mv = db.Movimiento.Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null).Where(a => a.Temporal == null).OrderBy(m => m.ID).Take(1).Single();

                List<Movimiento> Ingresos_t = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicioTmp).Where(m => m.Fecha < inicio).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID != ctes.tipoEgreso).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).ToList();
                List<Movimiento> Egresos_t = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicioTmp).Where(m => m.Fecha < inicio).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();


                ViewBag.Monto_Ingresos = Ingresos_t.Sum(i => i.Monto_Ingresos); ;
                ViewBag.Monto_Egresos = Egresos_t.Sum(e => e.Monto_Egresos);
                //ViewBag.SaldoFinal = ingresos - egresos;//mv.Saldo;
            }
            catch
            { }

            ViewBag.SaldoPeriodo = 0;

            // Obtenemos saldo hasta la semana seleccionada
            int periodo = inicio.Year;
            int mes = inicio.Month;
            int saldoInicial = 0;

            try
            {
                saldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID && m.Periodo == periodo & m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            { 
                // Error, no ha saldo para el periodo
                try
                {
                    Saldo Saldo = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Single();
                    if (periodo == Saldo.Periodo && mes > Saldo.Mes)
                    {
                        saldoInicial = Saldo.SaldoInicialCartola;
                    }
                }
                catch (Exception)
                { }
            }

            try
            {

                int ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha < inicio && m.Periodo == periodo && m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos);
                int egresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha < inicio && m.Periodo == periodo && m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).Sum(m => m.Monto_Egresos);
                ViewBag.SaldoPeriodo = ingresos - egresos + db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                //Response.Write(e.Message + " "+e.StackTrace);
            }

            ViewBag.SaldoInicial = saldoInicial;
            return View();
        }

        public ActionResult RegistroPagoFechas()
        {
            DateTime inicio;
            DateTime fin;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            int numberOfDays = DateTime.DaysInMonth(año, mes);
            if (Request.QueryString["i"] != null && Request.QueryString["f"] != null && Request.QueryString["s"] != null)
            {
                @ViewBag.Fecha = Request.QueryString["s"].ToString();
                inicio = DateTime.Parse(Request.QueryString["i"].ToString());
                fin = DateTime.Parse(Request.QueryString["f"].ToString()).AddDays(1);
            }
            else
            {

                inicio = new DateTime(año, mes, 1);
                fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = inicio.ToShortDateString();
                ViewBag.Hasta = fin.ToShortDateString();


            }

            List<Movimiento> Ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID != ctes.tipoEgreso).Where(m => m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).ToList();
            List<Movimiento> Egresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();
            List<Movimiento> Egresost = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Temporal == null && m.Eliminado == null && m.Nulo == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();
            @ViewBag.Ingresos_Reintegros = Ingresos;
            @ViewBag.Egresos = Egresos;
            ViewBag.total_ingresos = Ingresos.Sum(i => i.Monto_Ingresos);
            ViewBag.total_egresos = Egresost.Sum(e => e.Monto_Egresos);
            ViewBag.Monto_Ingresos = 0;
            ViewBag.Monto_Egresos = 0;
            ViewBag.SaldoFinal = 0;

            try
            {
                DateTime inicioTmp = new DateTime(inicio.Year, inicio.Month, 1);

                //DateTime inicioTmp = DateTime.Parse("01-" + inicio.Month + "-" + inicio.Year);
                //inicio = inicioTmp;
                //int ingresos = db.Movimiento.Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos);
                //int egresos = db.Movimiento.Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).Sum(m => m.Monto_Egresos);
                //Movimiento mv = db.Movimiento.Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null).Where(a => a.Temporal == null).OrderBy(m => m.ID).Take(1).Single();

                List<Movimiento> Ingresos_t = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicioTmp).Where(m => m.Fecha < inicio).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID != ctes.tipoEgreso).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).ToList();
                List<Movimiento> Egresos_t = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicioTmp).Where(m => m.Fecha < inicio).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();


                ViewBag.Monto_Ingresos = Ingresos_t.Sum(i => i.Monto_Ingresos); ;
                ViewBag.Monto_Egresos = Egresos_t.Sum(e => e.Monto_Egresos);
                //ViewBag.SaldoFinal = ingresos - egresos;//mv.Saldo;
            }
            catch
            { }

            ViewBag.SaldoPeriodo = 0;

            // Obtenemos saldo hasta la semana seleccionada
            int periodo = inicio.Year;
            mes = inicio.Month;
            int saldoInicial = 0;

            try
            {
                saldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID && m.Periodo == periodo & m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                // Error, no ha saldo para el periodo
                try
                {
                    Saldo Saldo = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Single();
                    if (periodo == Saldo.Periodo && mes > Saldo.Mes)
                    {
                        saldoInicial = Saldo.SaldoInicialCartola;
                    }
                }
                catch (Exception)
                { }
            }

            try
            {

                int ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha < inicio && m.Periodo == periodo && m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos);
                int egresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha < inicio && m.Periodo == periodo && m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).Sum(m => m.Monto_Egresos);
                ViewBag.SaldoPeriodo = ingresos - egresos + db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                //Response.Write(e.Message + " "+e.StackTrace);
            }

            ViewBag.SaldoInicial = saldoInicial;
            return View();
        }

        [HttpPost]
        public ActionResult RegistroPagoFechas(string Desde = "", string Hasta = "")
        {
            DateTime inicio;
            DateTime fin;
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            int numberOfDays = DateTime.DaysInMonth(año, mes);
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            // int mes;

            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                inicio = DateTime.Parse(Desde);
                fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
            }
            else
            {
                inicio = new DateTime(año, mes, 1);
                fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = inicio.ToShortDateString();
                ViewBag.Hasta = fin.ToShortDateString();
            }

            List<Movimiento> Ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID != ctes.tipoEgreso).Where(m => m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).ToList();
            List<Movimiento> Egresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();
            List<Movimiento> Egresost = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicio).Where(m => m.Fecha <= fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Temporal == null && m.Eliminado == null && m.Nulo == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();
            @ViewBag.Ingresos_Reintegros = Ingresos;
            @ViewBag.Egresos = Egresos;
            ViewBag.total_ingresos = Ingresos.Sum(i => i.Monto_Ingresos);
            ViewBag.total_egresos = Egresost.Sum(e => e.Monto_Egresos);
            ViewBag.Monto_Ingresos = 0;
            ViewBag.Monto_Egresos = 0;
            ViewBag.SaldoFinal = 0;

            try
            {
                DateTime inicioTmp = new DateTime(inicio.Year, inicio.Month, 1);



                List<Movimiento> Ingresos_t = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicioTmp).Where(m => m.Fecha < inicio).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID != ctes.tipoEgreso).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).ToList();
                List<Movimiento> Egresos_t = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= inicioTmp).Where(m => m.Fecha < inicio).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();


                ViewBag.Monto_Ingresos = Ingresos_t.Sum(i => i.Monto_Ingresos); ;
                ViewBag.Monto_Egresos = Egresos_t.Sum(e => e.Monto_Egresos);

            }
            catch
            { }

            ViewBag.SaldoPeriodo = 0;


            int periodo = inicio.Year;
            mes = inicio.Month;
            int saldoInicial = 0;

            try
            {
                saldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID && m.Periodo == periodo & m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                // Error, no ha saldo para el periodo
                try
                {
                    Saldo Saldo = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Single();
                    if (periodo == Saldo.Periodo && mes > Saldo.Mes)
                    {
                        saldoInicial = Saldo.SaldoInicialCartola;
                    }
                }
                catch (Exception)
                { }
            }

            try
            {

                int ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha < inicio && m.Periodo == periodo && m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos);
                int egresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha < inicio && m.Periodo == periodo && m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).Sum(m => m.Monto_Egresos);
                ViewBag.SaldoPeriodo = ingresos - egresos + db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                //Response.Write(e.Message + " "+e.StackTrace);
            }

            ViewBag.SaldoInicial = saldoInicial;
            return View();
        }

        [HttpGet]
        public ActionResult Ingresos(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            if (Periodo != 0 && Mes != 0)
            {
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();

                ViewBag.Rendicion = "Rendicion";
                var ingresos = db.Movimiento.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(ingresos.ToList());
            }
            else
            {
                int mes = (int)Session["Mes"];
                int año = (int)Session["Periodo"];
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(ingresos.ToList());
            }
        }

        [HttpPost]
        public ActionResult Ingresos(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(ingresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var ingresos = db.Movimiento.Where(m => m.Mes >= Mes).Where(m => m.Periodo <= Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(ingresos.ToList());
            }
        }

        public ActionResult Reintegros(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            if (Periodo != 0 && Mes != 0)
            {
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                ViewBag.Rendicion = "Rendicion";
                var ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);;
                return View(ingresos.ToList());
            }
            else
            {
                int mes = (int)Session["Mes"];
                int año = (int)Session["Periodo"];
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(ingresos.ToList());
            }
        }

        [HttpPost]
        public ActionResult Reintegros(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(ingresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var ingresos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(ingresos.ToList());
            }
        }

        public ActionResult Cuentas()
        {
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            int numberOfDays = DateTime.DaysInMonth(año, mes);
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, numberOfDays);
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var pr_id = Proyecto.ID;
            ViewBag.Entrada = 0;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));


            return View();

        }
        [HttpPost]
        public ActionResult Cuentas(FormCollection form)
        {
            ViewBag.Entrada = 1;
            List<movcuenta> mcuenta = new List<movcuenta>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int pr_id = Proyecto.ID;
            DateTime Inicio = DateTime.Parse(form["desde"].ToString());
            DateTime Fin = DateTime.Parse(form["hasta"].ToString());
            int CuentaID = Int32.Parse(form["CuentaID"].ToString());
            ViewBag.Cuenta = CuentaID;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
            var Ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);

            var Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            //List<Movimiento> Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // List<Movimiento> Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            var ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.Reintegro.Fecha >= Inicio).Where(m => m.Reintegro.Fecha <= Fin).Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(m => m.CuentaIDD == CuentaID).OrderBy(m => m.CuentaIDD).ToList();
            var Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.CuentaID == CuentaID).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // movcuenta mcuenta = new movcuenta();
            // int valor_GastosReintegros = reintegrosGastos.Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(d => d.CuentaIDD.Equals(cuenta1.ID)).Where(d => d.Reintegro.Mes == mes_inicio).Where(d => d.Reintegro.Periodo == periodo_actual).Sum(d => d.Monto);


            foreach (var Ingreso in Ingresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoIngreso;
                cuenta.IDComprobante = Ingreso.ID;
                cuenta.NumeroComprobante = Ingreso.NumeroComprobante;

                cuenta.Periodo = Ingreso.Periodo;
                cuenta.Mes = Ingreso.Mes;
                cuenta.Monto = Ingreso.Monto_Ingresos;
                cuenta.ProyectoID = Ingreso.ProyectoID;
                cuenta.Fecha = Ingreso.Fecha;
                cuenta.Cheque = Ingreso.Cheque;
                cuenta.CtaCte = Ingreso.CuentaCorriente.Numero;
                cuenta.Glosa = Ingreso.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;

                mcuenta.Add(cuenta);
            }

            foreach (var Egreso in Egresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoEgreso;
                cuenta.IDComprobante = Egreso.Egreso.ID;
                cuenta.NumeroComprobante = Egreso.Egreso.NumeroComprobante;
                cuenta.Periodo = Egreso.Egreso.Periodo;
                cuenta.Mes = Egreso.Egreso.Mes;
                cuenta.Monto = Egreso.Monto;
                cuenta.ProyectoID = Egreso.Egreso.ProyectoID;
                cuenta.Fecha = Egreso.Egreso.Fecha;
                cuenta.Cheque = Egreso.Egreso.Cheque;
                cuenta.Glosa = Egreso.Glosa;
                cuenta.CtaCte = Egreso.Egreso.CuentaCorriente.Numero;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var Reintegro in Reintegros)
            {
                int valor_gasto = 0;
                try
                {
                    valor_gasto = db.DetalleReintegro.Where(d => d.Reintegro.CuentaID == 60 && d.MovimientoID == Reintegro.ID).Sum(d => d.Monto);
                }
                catch
                {
                    valor_gasto = 0;
                }

                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoReintegro;
                cuenta.IDComprobante = Reintegro.ID;
                cuenta.NumeroComprobante = Reintegro.NumeroComprobante;
                cuenta.Periodo = Reintegro.Periodo;
                cuenta.Mes = Reintegro.Mes;
                cuenta.Monto = Reintegro.Monto_Ingresos + valor_gasto;
                cuenta.ProyectoID = Reintegro.ProyectoID;
                cuenta.Fecha = Reintegro.Fecha;
                cuenta.Cheque = Reintegro.Cheque;
                cuenta.CtaCte = Reintegro.CuentaCorriente.Numero;
                cuenta.Glosa = Reintegro.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var ReintegrosGasto in ReintegrosGastos)
            {
                if (ReintegrosGasto.Monto > 0)
                {
                    movcuenta cuenta = new movcuenta();
                    cuenta.TipoComprobanteID = 4;
                    cuenta.IDComprobante = ReintegrosGasto.Reintegro.ID;
                    cuenta.NumeroComprobante = ReintegrosGasto.Reintegro.NumeroComprobante;

                    cuenta.Periodo = ReintegrosGasto.Reintegro.Periodo;
                    cuenta.Mes = ReintegrosGasto.Reintegro.Mes;
                    cuenta.Monto = ReintegrosGasto.Monto;
                    cuenta.ProyectoID = ReintegrosGasto.Reintegro.ProyectoID;
                    cuenta.Fecha = ReintegrosGasto.Reintegro.Fecha;
                    cuenta.Cheque = ReintegrosGasto.Reintegro.Cheque;
                    cuenta.CtaCte = ReintegrosGasto.Reintegro.CuentaCorriente.Numero;
                    cuenta.Glosa = ReintegrosGasto.Reintegro.Descripcion;
                    cuenta.NombreCuenta = nombre_cuenta;
                    mcuenta.Add(cuenta);
                }
            }

            return View(mcuenta.OrderByDescending(a => a.Fecha).ToList());
        }

        public ActionResult CuentasExcel(DateTime Hasta, DateTime Desde, int Cuenta )
        {
            ViewBag.Entrada = 1;
            List<movcuenta> mcuenta = new List<movcuenta>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int pr_id = Proyecto.ID;
            DateTime Inicio = Desde;
            DateTime Fin =Hasta;
            int CuentaID = Cuenta;
            ViewBag.Cuenta = CuentaID;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
            var Ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);

            var Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            //List<Movimiento> Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // List<Movimiento> Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            var ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.Reintegro.Fecha >= Inicio).Where(m => m.Reintegro.Fecha <= Fin).Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(m => m.CuentaIDD == CuentaID).OrderBy(m => m.CuentaIDD).ToList();
            var Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.CuentaID == CuentaID).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // movcuenta mcuenta = new movcuenta();
            // int valor_GastosReintegros = reintegrosGastos.Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(d => d.CuentaIDD.Equals(cuenta1.ID)).Where(d => d.Reintegro.Mes == mes_inicio).Where(d => d.Reintegro.Periodo == periodo_actual).Sum(d => d.Monto);


            foreach (var Ingreso in Ingresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoIngreso;
                cuenta.IDComprobante = Ingreso.ID;
                cuenta.NumeroComprobante = Ingreso.NumeroComprobante;

                cuenta.Periodo = Ingreso.Periodo;
                cuenta.Mes = Ingreso.Mes;
                cuenta.Monto = Ingreso.Monto_Ingresos;
                cuenta.ProyectoID = Ingreso.ProyectoID;
                cuenta.Fecha = Ingreso.Fecha;
                cuenta.Cheque = Ingreso.Cheque;
                cuenta.CtaCte = Ingreso.CuentaCorriente.Numero;
                cuenta.Glosa = Ingreso.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;

                mcuenta.Add(cuenta);
            }

            foreach (var Egreso in Egresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoEgreso;
                cuenta.IDComprobante = Egreso.Egreso.ID;
                cuenta.NumeroComprobante = Egreso.Egreso.NumeroComprobante;
                cuenta.Periodo = Egreso.Egreso.Periodo;
                cuenta.Mes = Egreso.Egreso.Mes;
                cuenta.Monto = Egreso.Monto;
                cuenta.ProyectoID = Egreso.Egreso.ProyectoID;
                cuenta.Fecha = Egreso.Egreso.Fecha;
                cuenta.Cheque = Egreso.Egreso.Cheque;
                cuenta.Glosa = Egreso.Glosa;
                cuenta.CtaCte = Egreso.Egreso.CuentaCorriente.Numero;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var Reintegro in Reintegros)
            {
                int valor_gasto = 0;
                try
                {
                    valor_gasto = db.DetalleReintegro.Where(d => d.Reintegro.CuentaID == 60 && d.MovimientoID == Reintegro.ID).Sum(d => d.Monto);
                }
                catch
                {
                    valor_gasto = 0;
                }

                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoReintegro;
                cuenta.IDComprobante = Reintegro.ID;
                cuenta.NumeroComprobante = Reintegro.NumeroComprobante;
                cuenta.Periodo = Reintegro.Periodo;
                cuenta.Mes = Reintegro.Mes;
                cuenta.Monto = Reintegro.Monto_Ingresos + valor_gasto;
                cuenta.ProyectoID = Reintegro.ProyectoID;
                cuenta.Fecha = Reintegro.Fecha;
                cuenta.Cheque = Reintegro.Cheque;
                cuenta.CtaCte = Reintegro.CuentaCorriente.Numero;
                cuenta.Glosa = Reintegro.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var ReintegrosGasto in ReintegrosGastos)
            {
                if (ReintegrosGasto.Monto > 0)
                {
                    movcuenta cuenta = new movcuenta();
                    cuenta.TipoComprobanteID = 4;
                    cuenta.IDComprobante = ReintegrosGasto.Reintegro.ID;
                    cuenta.NumeroComprobante = ReintegrosGasto.Reintegro.NumeroComprobante;

                    cuenta.Periodo = ReintegrosGasto.Reintegro.Periodo;
                    cuenta.Mes = ReintegrosGasto.Reintegro.Mes;
                    cuenta.Monto = ReintegrosGasto.Monto;
                    cuenta.ProyectoID = ReintegrosGasto.Reintegro.ProyectoID;
                    cuenta.Fecha = ReintegrosGasto.Reintegro.Fecha;
                    cuenta.Cheque = ReintegrosGasto.Reintegro.Cheque;
                    cuenta.CtaCte = ReintegrosGasto.Reintegro.CuentaCorriente.Numero;
                    cuenta.Glosa = ReintegrosGasto.Reintegro.Descripcion;
                    cuenta.NombreCuenta = nombre_cuenta;
                    mcuenta.Add(cuenta);
                }
            }

            return View(mcuenta.OrderByDescending(a => a.Fecha).ToList());
        }

        public ActionResult CuentasGeneral()
        {
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            int numberOfDays = DateTime.DaysInMonth(año, mes);
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, numberOfDays);
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var pr_id = Proyecto.ID;
            ViewBag.Entrada = 0;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));


            return View();

        }
        [HttpPost]
        public ActionResult CuentasGeneral(FormCollection form)
        {
            ViewBag.Entrada = 1;
            List<movcuenta> mcuenta = new List<movcuenta>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int pr_id = Proyecto.ID;
            DateTime Inicio = DateTime.Parse(form["desde"].ToString());
            DateTime Fin = DateTime.Parse(form["hasta"].ToString());
            int CuentaID = Int32.Parse(form["CuentaID"].ToString());
            ViewBag.Cuenta = CuentaID;
            //string excel = (form["Excel"].ToString());
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
            var Ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);

            var Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            //List<Movimiento> Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // List<Movimiento> Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            var ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Fecha >= Inicio).Where(m => m.Reintegro.Fecha <= Fin).Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(m => m.CuentaIDD == CuentaID).OrderBy(m => m.CuentaIDD).ToList();
            var Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.CuentaID == CuentaID).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // movcuenta mcuenta = new movcuenta();
            // int valor_GastosReintegros = reintegrosGastos.Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(d => d.CuentaIDD.Equals(cuenta1.ID)).Where(d => d.Reintegro.Mes == mes_inicio).Where(d => d.Reintegro.Periodo == periodo_actual).Sum(d => d.Monto);


            foreach (var Ingreso in Ingresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoIngreso;
                cuenta.IDComprobante = Ingreso.ID;
                cuenta.NumeroComprobante = Ingreso.NumeroComprobante;

                cuenta.Periodo = Ingreso.Periodo;
                cuenta.Mes = Ingreso.Mes;
                cuenta.Monto = Ingreso.Monto_Ingresos;
                cuenta.ProyectoID = Ingreso.ProyectoID;
                cuenta.Fecha = Ingreso.Fecha;
                cuenta.Cheque = Ingreso.Cheque;
                cuenta.CtaCte = Ingreso.CuentaCorriente.Numero;
                cuenta.Glosa = Ingreso.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;

                mcuenta.Add(cuenta);
            }

            foreach (var Egreso in Egresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoEgreso;
                cuenta.IDComprobante = Egreso.Egreso.ID;
                cuenta.NumeroComprobante = Egreso.Egreso.NumeroComprobante;
                cuenta.Periodo = Egreso.Egreso.Periodo;
                cuenta.Mes = Egreso.Egreso.Mes;
                cuenta.Monto = Egreso.Monto;
                cuenta.ProyectoID = Egreso.Egreso.ProyectoID;
                cuenta.Fecha = Egreso.Egreso.Fecha;
                cuenta.Cheque = Egreso.Egreso.Cheque;
                cuenta.Glosa = Egreso.Glosa;
                cuenta.CtaCte = Egreso.Egreso.CuentaCorriente.Numero;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var Reintegro in Reintegros)
            {
                int valor_gasto = 0;
                try
                {
                    valor_gasto = db.DetalleReintegro.Where(d => d.Reintegro.CuentaID == 60 && d.MovimientoID == Reintegro.ID).Sum(d => d.Monto);
                }
                catch
                {
                    valor_gasto = 0;
                }

                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoReintegro;
                cuenta.IDComprobante = Reintegro.ID;
                cuenta.NumeroComprobante = Reintegro.NumeroComprobante;
                cuenta.Periodo = Reintegro.Periodo;
                cuenta.Mes = Reintegro.Mes;
                cuenta.Monto = Reintegro.Monto_Ingresos + valor_gasto;
                cuenta.ProyectoID = Reintegro.ProyectoID;
                cuenta.Fecha = Reintegro.Fecha;
                cuenta.Cheque = Reintegro.Cheque;
                cuenta.CtaCte = Reintegro.CuentaCorriente.Numero;
                cuenta.Glosa = Reintegro.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var ReintegrosGasto in ReintegrosGastos)
            {
                if (ReintegrosGasto.Monto > 0)
                {
                    movcuenta cuenta = new movcuenta();
                    cuenta.TipoComprobanteID = 4;
                    cuenta.IDComprobante = ReintegrosGasto.Reintegro.ID;
                    cuenta.NumeroComprobante = ReintegrosGasto.Reintegro.NumeroComprobante;

                    cuenta.Periodo = ReintegrosGasto.Reintegro.Periodo;
                    cuenta.Mes = ReintegrosGasto.Reintegro.Mes;
                    cuenta.Monto = ReintegrosGasto.Monto;
                    cuenta.ProyectoID = ReintegrosGasto.Reintegro.ProyectoID;
                    cuenta.Fecha = ReintegrosGasto.Reintegro.Fecha;
                    cuenta.Cheque = ReintegrosGasto.Reintegro.Cheque;
                    cuenta.CtaCte = ReintegrosGasto.Reintegro.CuentaCorriente.Numero;
                    cuenta.Glosa = ReintegrosGasto.Reintegro.Descripcion;
                    cuenta.NombreCuenta = nombre_cuenta;
                    mcuenta.Add(cuenta);
                }
            }

            //if (excel == "Excel")
            //{
            //    return View("CuentasGeneralExcel", mcuenta.OrderByDescending(a => a.Fecha).ToList());
            //}
            //else
            //{
                return View(mcuenta.OrderByDescending(a => a.Fecha).ToList());
            //}
        }

        public ActionResult CuentasGeneralExcel(DateTime Hasta, DateTime Desde, int Cuenta)
        {
            ViewBag.Entrada = 1;
            List<movcuenta> mcuenta = new List<movcuenta>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int pr_id = Proyecto.ID;
            DateTime Inicio = Desde;
            DateTime Fin = Hasta;
            int CuentaID = Cuenta;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
            var Ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);

            var Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            //List<Movimiento> Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // List<Movimiento> Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            var ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Fecha >= Inicio).Where(m => m.Reintegro.Fecha <= Fin).Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(m => m.CuentaIDD == CuentaID).OrderBy(m => m.CuentaIDD).ToList();
            var Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.CuentaID == CuentaID).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // movcuenta mcuenta = new movcuenta();
            // int valor_GastosReintegros = reintegrosGastos.Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(d => d.CuentaIDD.Equals(cuenta1.ID)).Where(d => d.Reintegro.Mes == mes_inicio).Where(d => d.Reintegro.Periodo == periodo_actual).Sum(d => d.Monto);


            foreach (var Ingreso in Ingresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoIngreso;
                cuenta.IDComprobante = Ingreso.ID;
                cuenta.NumeroComprobante = Ingreso.NumeroComprobante;

                cuenta.Periodo = Ingreso.Periodo;
                cuenta.Mes = Ingreso.Mes;
                cuenta.Monto = Ingreso.Monto_Ingresos;
                cuenta.ProyectoID = Ingreso.ProyectoID;
                cuenta.Fecha = Ingreso.Fecha;
                cuenta.Cheque = Ingreso.Cheque;
                cuenta.CtaCte = Ingreso.CuentaCorriente.Numero;
                cuenta.Glosa = Ingreso.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;

                mcuenta.Add(cuenta);
            }

            foreach (var Egreso in Egresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoEgreso;
                cuenta.IDComprobante = Egreso.Egreso.ID;
                cuenta.NumeroComprobante = Egreso.Egreso.NumeroComprobante;
                cuenta.Periodo = Egreso.Egreso.Periodo;
                cuenta.Mes = Egreso.Egreso.Mes;
                cuenta.Monto = Egreso.Monto;
                cuenta.ProyectoID = Egreso.Egreso.ProyectoID;
                cuenta.Fecha = Egreso.Egreso.Fecha;
                cuenta.Cheque = Egreso.Egreso.Cheque;
                cuenta.Glosa = Egreso.Glosa;
                cuenta.CtaCte = Egreso.Egreso.CuentaCorriente.Numero;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var Reintegro in Reintegros)
            {
                int valor_gasto = 0;
                try
                {
                    valor_gasto = db.DetalleReintegro.Where(d => d.Reintegro.CuentaID == 60 && d.MovimientoID == Reintegro.ID).Sum(d => d.Monto);
                }
                catch
                {
                    valor_gasto = 0;
                }

                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoReintegro;
                cuenta.IDComprobante = Reintegro.ID;
                cuenta.NumeroComprobante = Reintegro.NumeroComprobante;
                cuenta.Periodo = Reintegro.Periodo;
                cuenta.Mes = Reintegro.Mes;
                cuenta.Monto = Reintegro.Monto_Ingresos + valor_gasto;
                cuenta.ProyectoID = Reintegro.ProyectoID;
                cuenta.Fecha = Reintegro.Fecha;
                cuenta.Cheque = Reintegro.Cheque;
                cuenta.CtaCte = Reintegro.CuentaCorriente.Numero;
                cuenta.Glosa = Reintegro.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var ReintegrosGasto in ReintegrosGastos)
            {
                if (ReintegrosGasto.Monto > 0)
                {
                    movcuenta cuenta = new movcuenta();
                    cuenta.TipoComprobanteID = 4;
                    cuenta.IDComprobante = ReintegrosGasto.Reintegro.ID;
                    cuenta.NumeroComprobante = ReintegrosGasto.Reintegro.NumeroComprobante;

                    cuenta.Periodo = ReintegrosGasto.Reintegro.Periodo;
                    cuenta.Mes = ReintegrosGasto.Reintegro.Mes;
                    cuenta.Monto = ReintegrosGasto.Monto;
                    cuenta.ProyectoID = ReintegrosGasto.Reintegro.ProyectoID;
                    cuenta.Fecha = ReintegrosGasto.Reintegro.Fecha;
                    cuenta.Cheque = ReintegrosGasto.Reintegro.Cheque;
                    cuenta.CtaCte = ReintegrosGasto.Reintegro.CuentaCorriente.Numero;
                    cuenta.Glosa = ReintegrosGasto.Reintegro.Descripcion;
                    cuenta.NombreCuenta = nombre_cuenta;
                    mcuenta.Add(cuenta);
                }
            }

            return View(mcuenta.OrderByDescending(a => a.Fecha).ToList());
            
        }

        public ActionResult CuentasRegion()
        {
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            int numberOfDays = DateTime.DaysInMonth(año, mes);
            ViewBag.regiones = db.Region.ToList();
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, numberOfDays);
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var pr_id = Proyecto.ID;
            ViewBag.Entrada = 0;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));


            return View();

        }
        [HttpPost]
        public ActionResult CuentasRegion(FormCollection form)
        {
            ViewBag.Entrada = 1;
            List<movcuenta> mcuenta = new List<movcuenta>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int pr_id = Proyecto.ID;
            DateTime Inicio = DateTime.Parse(form["desde"].ToString());
            DateTime Fin = DateTime.Parse(form["hasta"].ToString());
            int CuentaID = Int32.Parse(form["CuentaID"].ToString());
            int region = Int32.Parse(form["Region"].ToString());
            ViewBag.Cuenta = CuentaID;
            ViewBag.Region = region;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.regiones = db.Region.ToList();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
            var Ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(T => T.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);

            var Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(T => T.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            //List<Movimiento> Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // List<Movimiento> Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            var ReintegrosGastos = db.DetalleReintegro.Where(T => T.Reintegro.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Reintegro.Fecha >= Inicio).Where(m => m.Reintegro.Fecha <= Fin).Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(m => m.CuentaIDD == CuentaID).OrderBy(m => m.CuentaIDD).ToList();
            var Egresos = db.DetalleEgreso.Where(T => T.Egreso.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.CuentaID == CuentaID).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // movcuenta mcuenta = new movcuenta();
            // int valor_GastosReintegros = reintegrosGastos.Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(d => d.CuentaIDD.Equals(cuenta1.ID)).Where(d => d.Reintegro.Mes == mes_inicio).Where(d => d.Reintegro.Periodo == periodo_actual).Sum(d => d.Monto);


            foreach (var Ingreso in Ingresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoIngreso;
                cuenta.IDComprobante = Ingreso.ID;
                cuenta.NumeroComprobante = Ingreso.NumeroComprobante;

                cuenta.Periodo = Ingreso.Periodo;
                cuenta.Mes = Ingreso.Mes;
                cuenta.Monto = Ingreso.Monto_Ingresos;
                cuenta.ProyectoID = Ingreso.ProyectoID;
                cuenta.Fecha = Ingreso.Fecha;
                cuenta.Cheque = Ingreso.Cheque;
                cuenta.CtaCte = Ingreso.CuentaCorriente.Numero;
                cuenta.Glosa = Ingreso.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;

                mcuenta.Add(cuenta);
            }

            foreach (var Egreso in Egresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoEgreso;
                cuenta.IDComprobante = Egreso.Egreso.ID;
                cuenta.NumeroComprobante = Egreso.Egreso.NumeroComprobante;
                cuenta.Periodo = Egreso.Egreso.Periodo;
                cuenta.Mes = Egreso.Egreso.Mes;
                cuenta.Monto = Egreso.Monto;
                cuenta.ProyectoID = Egreso.Egreso.ProyectoID;
                cuenta.Fecha = Egreso.Egreso.Fecha;
                cuenta.Cheque = Egreso.Egreso.Cheque;
                cuenta.Glosa = Egreso.Glosa;
                cuenta.CtaCte = Egreso.Egreso.CuentaCorriente.Numero;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var Reintegro in Reintegros)
            {
                int valor_gasto = 0;
                try
                {
                    valor_gasto = db.DetalleReintegro.Where(d => d.Reintegro.CuentaID == 60 && d.MovimientoID == Reintegro.ID).Sum(d => d.Monto);
                }
                catch
                {
                    valor_gasto = 0;
                }

                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoReintegro;
                cuenta.IDComprobante = Reintegro.ID;
                cuenta.NumeroComprobante = Reintegro.NumeroComprobante;
                cuenta.Periodo = Reintegro.Periodo;
                cuenta.Mes = Reintegro.Mes;
                cuenta.Monto = Reintegro.Monto_Ingresos + valor_gasto;
                cuenta.ProyectoID = Reintegro.ProyectoID;
                cuenta.Fecha = Reintegro.Fecha;
                cuenta.Cheque = Reintegro.Cheque;
                cuenta.CtaCte = Reintegro.CuentaCorriente.Numero;
                cuenta.Glosa = Reintegro.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var ReintegrosGasto in ReintegrosGastos)
            {
                if (ReintegrosGasto.Monto > 0)
                {
                    movcuenta cuenta = new movcuenta();
                    cuenta.TipoComprobanteID = 4;
                    cuenta.IDComprobante = ReintegrosGasto.Reintegro.ID;
                    cuenta.NumeroComprobante = ReintegrosGasto.Reintegro.NumeroComprobante;

                    cuenta.Periodo = ReintegrosGasto.Reintegro.Periodo;
                    cuenta.Mes = ReintegrosGasto.Reintegro.Mes;
                    cuenta.Monto = ReintegrosGasto.Monto;
                    cuenta.ProyectoID = ReintegrosGasto.Reintegro.ProyectoID;
                    cuenta.Fecha = ReintegrosGasto.Reintegro.Fecha;
                    cuenta.Cheque = ReintegrosGasto.Reintegro.Cheque;
                    cuenta.CtaCte = ReintegrosGasto.Reintegro.CuentaCorriente.Numero;
                    cuenta.Glosa = ReintegrosGasto.Reintegro.Descripcion;
                    cuenta.NombreCuenta = nombre_cuenta;
                    mcuenta.Add(cuenta);
                }
            }

            return View(mcuenta.OrderByDescending(a => a.Fecha).ToList());
        }

        
        public ActionResult CuentasRegionExcel(DateTime Hasta, DateTime Desde, int Cuenta, int Region)
        {
            ViewBag.Entrada = 1;
            List<movcuenta> mcuenta = new List<movcuenta>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int pr_id = Proyecto.ID;
            DateTime Inicio = Desde;
            DateTime Fin = Hasta;
            int CuentaID = Cuenta;
            int region = Region;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.regiones = db.Region.ToList();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
            var Ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(T => T.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);

            var Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(T => T.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            //List<Movimiento> Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // List<Movimiento> Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            var ReintegrosGastos = db.DetalleReintegro.Where(T => T.Reintegro.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Reintegro.Fecha >= Inicio).Where(m => m.Reintegro.Fecha <= Fin).Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(m => m.CuentaIDD == CuentaID).OrderBy(m => m.CuentaIDD).ToList();
            var Egresos = db.DetalleEgreso.Where(T => T.Egreso.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.CuentaID == CuentaID).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // movcuenta mcuenta = new movcuenta();
            // int valor_GastosReintegros = reintegrosGastos.Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(d => d.CuentaIDD.Equals(cuenta1.ID)).Where(d => d.Reintegro.Mes == mes_inicio).Where(d => d.Reintegro.Periodo == periodo_actual).Sum(d => d.Monto);


            foreach (var Ingreso in Ingresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoIngreso;
                cuenta.IDComprobante = Ingreso.ID;
                cuenta.NumeroComprobante = Ingreso.NumeroComprobante;

                cuenta.Periodo = Ingreso.Periodo;
                cuenta.Mes = Ingreso.Mes;
                cuenta.Monto = Ingreso.Monto_Ingresos;
                cuenta.ProyectoID = Ingreso.ProyectoID;
                cuenta.Fecha = Ingreso.Fecha;
                cuenta.Cheque = Ingreso.Cheque;
                cuenta.CtaCte = Ingreso.CuentaCorriente.Numero;
                cuenta.Glosa = Ingreso.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;

                mcuenta.Add(cuenta);
            }

            foreach (var Egreso in Egresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoEgreso;
                cuenta.IDComprobante = Egreso.Egreso.ID;
                cuenta.NumeroComprobante = Egreso.Egreso.NumeroComprobante;
                cuenta.Periodo = Egreso.Egreso.Periodo;
                cuenta.Mes = Egreso.Egreso.Mes;
                cuenta.Monto = Egreso.Monto;
                cuenta.ProyectoID = Egreso.Egreso.ProyectoID;
                cuenta.Fecha = Egreso.Egreso.Fecha;
                cuenta.Cheque = Egreso.Egreso.Cheque;
                cuenta.Glosa = Egreso.Glosa;
                cuenta.CtaCte = Egreso.Egreso.CuentaCorriente.Numero;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var Reintegro in Reintegros)
            {
                int valor_gasto = 0;
                try
                {
                    valor_gasto = db.DetalleReintegro.Where(d => d.Reintegro.CuentaID == 60 && d.MovimientoID == Reintegro.ID).Sum(d => d.Monto);
                }
                catch
                {
                    valor_gasto = 0;
                }

                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoReintegro;
                cuenta.IDComprobante = Reintegro.ID;
                cuenta.NumeroComprobante = Reintegro.NumeroComprobante;
                cuenta.Periodo = Reintegro.Periodo;
                cuenta.Mes = Reintegro.Mes;
                cuenta.Monto = Reintegro.Monto_Ingresos + valor_gasto;
                cuenta.ProyectoID = Reintegro.ProyectoID;
                cuenta.Fecha = Reintegro.Fecha;
                cuenta.Cheque = Reintegro.Cheque;
                cuenta.CtaCte = Reintegro.CuentaCorriente.Numero;
                cuenta.Glosa = Reintegro.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var ReintegrosGasto in ReintegrosGastos)
            {
                if (ReintegrosGasto.Monto > 0)
                {
                    movcuenta cuenta = new movcuenta();
                    cuenta.TipoComprobanteID = 4;
                    cuenta.IDComprobante = ReintegrosGasto.Reintegro.ID;
                    cuenta.NumeroComprobante = ReintegrosGasto.Reintegro.NumeroComprobante;

                    cuenta.Periodo = ReintegrosGasto.Reintegro.Periodo;
                    cuenta.Mes = ReintegrosGasto.Reintegro.Mes;
                    cuenta.Monto = ReintegrosGasto.Monto;
                    cuenta.ProyectoID = ReintegrosGasto.Reintegro.ProyectoID;
                    cuenta.Fecha = ReintegrosGasto.Reintegro.Fecha;
                    cuenta.Cheque = ReintegrosGasto.Reintegro.Cheque;
                    cuenta.CtaCte = ReintegrosGasto.Reintegro.CuentaCorriente.Numero;
                    cuenta.Glosa = ReintegrosGasto.Reintegro.Descripcion;
                    cuenta.NombreCuenta = nombre_cuenta;
                    mcuenta.Add(cuenta);
                }
            }

            return View(mcuenta.OrderByDescending(a => a.Fecha).ToList());
        }

        public ActionResult CuentasTP()
        {
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            int numberOfDays = DateTime.DaysInMonth(año, mes);
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, numberOfDays);
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var pr_id = Proyecto.ID;
            ViewBag.Entrada = 0;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));


            return View();

        }
        [HttpPost]
        public ActionResult CuentasTP(FormCollection form)
        {
            ViewBag.Entrada = 1;
            List<movcuenta> mcuenta = new List<movcuenta>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int pr_id = Proyecto.ID;
            DateTime Inicio = DateTime.Parse(form["desde"].ToString());
            DateTime Fin = DateTime.Parse(form["hasta"].ToString());
            int CuentaID = Int32.Parse(form["CuentaID"].ToString());

            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Cuenta = CuentaID;
            int TPID = Int32.Parse(form["TProyecto"].ToString());
            ViewBag.TP = TPID;
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
            var Ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(T=> T.Proyecto.TipoProyectoID == TPID).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);

            var Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(T => T.Proyecto.TipoProyectoID == TPID).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            //List<Movimiento> Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // List<Movimiento> Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            var ReintegrosGastos = db.DetalleReintegro.Where(T => T.Reintegro.Proyecto.TipoProyectoID == TPID).Where(m => m.Reintegro.Fecha >= Inicio).Where(m => m.Reintegro.Fecha <= Fin).Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(m => m.CuentaIDD == CuentaID).OrderBy(m => m.CuentaIDD).ToList();
            var Egresos = db.DetalleEgreso.Where(T => T.Egreso.Proyecto.TipoProyectoID == TPID).Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.CuentaID == CuentaID).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // movcuenta mcuenta = new movcuenta();
            // int valor_GastosReintegros = reintegrosGastos.Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(d => d.CuentaIDD.Equals(cuenta1.ID)).Where(d => d.Reintegro.Mes == mes_inicio).Where(d => d.Reintegro.Periodo == periodo_actual).Sum(d => d.Monto);


            foreach (var Ingreso in Ingresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoIngreso;
                cuenta.IDComprobante = Ingreso.ID;
                cuenta.NumeroComprobante = Ingreso.NumeroComprobante;

                cuenta.Periodo = Ingreso.Periodo;
                cuenta.Mes = Ingreso.Mes;
                cuenta.Monto = Ingreso.Monto_Ingresos;
                cuenta.ProyectoID = Ingreso.ProyectoID;
                cuenta.Fecha = Ingreso.Fecha;
                cuenta.Cheque = Ingreso.Cheque;
                cuenta.CtaCte = Ingreso.CuentaCorriente.Numero;
                cuenta.Glosa = Ingreso.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;

                mcuenta.Add(cuenta);
            }

            foreach (var Egreso in Egresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoEgreso;
                cuenta.IDComprobante = Egreso.Egreso.ID;
                cuenta.NumeroComprobante = Egreso.Egreso.NumeroComprobante;
                cuenta.Periodo = Egreso.Egreso.Periodo;
                cuenta.Mes = Egreso.Egreso.Mes;
                cuenta.Monto = Egreso.Monto;
                cuenta.ProyectoID = Egreso.Egreso.ProyectoID;
                cuenta.Fecha = Egreso.Egreso.Fecha;
                cuenta.Cheque = Egreso.Egreso.Cheque;
                cuenta.Glosa = Egreso.Glosa;
                cuenta.CtaCte = Egreso.Egreso.CuentaCorriente.Numero;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var Reintegro in Reintegros)
            {
                int valor_gasto = 0;
                try
                {
                    valor_gasto = db.DetalleReintegro.Where(d => d.Reintegro.CuentaID == 60 && d.MovimientoID == Reintegro.ID).Sum(d => d.Monto);
                }
                catch
                {
                    valor_gasto = 0;
                }

                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoReintegro;
                cuenta.IDComprobante = Reintegro.ID;
                cuenta.NumeroComprobante = Reintegro.NumeroComprobante;
                cuenta.Periodo = Reintegro.Periodo;
                cuenta.Mes = Reintegro.Mes;
                cuenta.Monto = Reintegro.Monto_Ingresos + valor_gasto;
                cuenta.ProyectoID = Reintegro.ProyectoID;
                cuenta.Fecha = Reintegro.Fecha;
                cuenta.Cheque = Reintegro.Cheque;
                cuenta.CtaCte = Reintegro.CuentaCorriente.Numero;
                cuenta.Glosa = Reintegro.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var ReintegrosGasto in ReintegrosGastos)
            {
                if (ReintegrosGasto.Monto > 0)
                {
                    movcuenta cuenta = new movcuenta();
                    cuenta.TipoComprobanteID = 4;
                    cuenta.IDComprobante = ReintegrosGasto.Reintegro.ID;
                    cuenta.NumeroComprobante = ReintegrosGasto.Reintegro.NumeroComprobante;

                    cuenta.Periodo = ReintegrosGasto.Reintegro.Periodo;
                    cuenta.Mes = ReintegrosGasto.Reintegro.Mes;
                    cuenta.Monto = ReintegrosGasto.Monto;
                    cuenta.ProyectoID = ReintegrosGasto.Reintegro.ProyectoID;
                    cuenta.Fecha = ReintegrosGasto.Reintegro.Fecha;
                    cuenta.Cheque = ReintegrosGasto.Reintegro.Cheque;
                    cuenta.CtaCte = ReintegrosGasto.Reintegro.CuentaCorriente.Numero;
                    cuenta.Glosa = ReintegrosGasto.Reintegro.Descripcion;
                    cuenta.NombreCuenta = nombre_cuenta;
                    mcuenta.Add(cuenta);
                }
            }

            return View(mcuenta.OrderByDescending(a => a.Fecha).ToList());
        }

        public ActionResult CuentasTPExcel(DateTime Hasta, DateTime Desde, int Cuenta, int TP)
        {
            ViewBag.Entrada = 1;
            List<movcuenta> mcuenta = new List<movcuenta>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int pr_id = Proyecto.ID;
            DateTime Inicio = Desde;
            DateTime Fin = Hasta;
            int CuentaID = Cuenta;
            int TPID = TP;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
            var Ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(T => T.Proyecto.TipoProyectoID == TPID).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);

            var Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(T => T.Proyecto.TipoProyectoID == TPID).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID == CuentaID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            //List<Movimiento> Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // List<Movimiento> Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            var ReintegrosGastos = db.DetalleReintegro.Where(T => T.Reintegro.Proyecto.TipoProyectoID == TPID).Where(m => m.Reintegro.Fecha >= Inicio).Where(m => m.Reintegro.Fecha <= Fin).Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(m => m.CuentaIDD == CuentaID).OrderBy(m => m.CuentaIDD).ToList();
            var Egresos = db.DetalleEgreso.Where(T => T.Egreso.Proyecto.TipoProyectoID == TPID).Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.CuentaID == CuentaID).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            // movcuenta mcuenta = new movcuenta();
            // int valor_GastosReintegros = reintegrosGastos.Where(d => d.Reintegro.Cuenta.Codigo.Equals("7.1.9")).Where(d => d.CuentaIDD.Equals(cuenta1.ID)).Where(d => d.Reintegro.Mes == mes_inicio).Where(d => d.Reintegro.Periodo == periodo_actual).Sum(d => d.Monto);


            foreach (var Ingreso in Ingresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoIngreso;
                cuenta.IDComprobante = Ingreso.ID;
                cuenta.NumeroComprobante = Ingreso.NumeroComprobante;

                cuenta.Periodo = Ingreso.Periodo;
                cuenta.Mes = Ingreso.Mes;
                cuenta.Monto = Ingreso.Monto_Ingresos;
                cuenta.ProyectoID = Ingreso.ProyectoID;
                cuenta.Fecha = Ingreso.Fecha;
                cuenta.Cheque = Ingreso.Cheque;
                cuenta.CtaCte = Ingreso.CuentaCorriente.Numero;
                cuenta.Glosa = Ingreso.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;

                mcuenta.Add(cuenta);
            }

            foreach (var Egreso in Egresos)
            {
                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoEgreso;
                cuenta.IDComprobante = Egreso.Egreso.ID;
                cuenta.NumeroComprobante = Egreso.Egreso.NumeroComprobante;
                cuenta.Periodo = Egreso.Egreso.Periodo;
                cuenta.Mes = Egreso.Egreso.Mes;
                cuenta.Monto = Egreso.Monto;
                cuenta.ProyectoID = Egreso.Egreso.ProyectoID;
                cuenta.Fecha = Egreso.Egreso.Fecha;
                cuenta.Cheque = Egreso.Egreso.Cheque;
                cuenta.Glosa = Egreso.Glosa;
                cuenta.CtaCte = Egreso.Egreso.CuentaCorriente.Numero;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var Reintegro in Reintegros)
            {
                int valor_gasto = 0;
                try
                {
                    valor_gasto = db.DetalleReintegro.Where(d => d.Reintegro.CuentaID == 60 && d.MovimientoID == Reintegro.ID).Sum(d => d.Monto);
                }
                catch
                {
                    valor_gasto = 0;
                }

                movcuenta cuenta = new movcuenta();
                cuenta.TipoComprobanteID = ctes.tipoReintegro;
                cuenta.IDComprobante = Reintegro.ID;
                cuenta.NumeroComprobante = Reintegro.NumeroComprobante;
                cuenta.Periodo = Reintegro.Periodo;
                cuenta.Mes = Reintegro.Mes;
                cuenta.Monto = Reintegro.Monto_Ingresos + valor_gasto;
                cuenta.ProyectoID = Reintegro.ProyectoID;
                cuenta.Fecha = Reintegro.Fecha;
                cuenta.Cheque = Reintegro.Cheque;
                cuenta.CtaCte = Reintegro.CuentaCorriente.Numero;
                cuenta.Glosa = Reintegro.Descripcion;
                cuenta.NombreCuenta = nombre_cuenta;
                mcuenta.Add(cuenta);
            }

            foreach (var ReintegrosGasto in ReintegrosGastos)
            {
                if (ReintegrosGasto.Monto > 0)
                {
                    movcuenta cuenta = new movcuenta();
                    cuenta.TipoComprobanteID = 4;
                    cuenta.IDComprobante = ReintegrosGasto.Reintegro.ID;
                    cuenta.NumeroComprobante = ReintegrosGasto.Reintegro.NumeroComprobante;

                    cuenta.Periodo = ReintegrosGasto.Reintegro.Periodo;
                    cuenta.Mes = ReintegrosGasto.Reintegro.Mes;
                    cuenta.Monto = ReintegrosGasto.Monto;
                    cuenta.ProyectoID = ReintegrosGasto.Reintegro.ProyectoID;
                    cuenta.Fecha = ReintegrosGasto.Reintegro.Fecha;
                    cuenta.Cheque = ReintegrosGasto.Reintegro.Cheque;
                    cuenta.CtaCte = ReintegrosGasto.Reintegro.CuentaCorriente.Numero;
                    cuenta.Glosa = ReintegrosGasto.Reintegro.Descripcion;
                    cuenta.NombreCuenta = nombre_cuenta;
                    mcuenta.Add(cuenta);
                }
            }

            return View(mcuenta.OrderByDescending(a => a.Fecha).ToList());
        }


        /*Revisa como se producen*/
        public ActionResult Egresos(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            if (Periodo != 0 && Mes != 0)
            {
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                ViewBag.Rendicion = "Rendicion";
                //var movimientos = db.Movimiento.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int mes = (int)Session["Mes"];
                int año = (int)Session["Periodo"];
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                //var movimientos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        [HttpPost]
        public ActionResult Egresos(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        
        /*Revisa como se producen*/
        public ActionResult Fondosrendir(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Periodo != 0 && Mes != 0)
            {
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                ViewBag.Rendicion = "Rendicion";
                ViewBag.CodSename = Proyecto.CodSename;  
                //var movimientos = db.Movimiento.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int mes = (int)Session["Mes"];
                int año = (int)Session["Periodo"];
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                //var movimientos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        [HttpPost]
        public ActionResult Fondosrendir(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                ViewBag.CodSename = Proyecto.CodSename;  
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        public ActionResult FondosrendirCuenta(int Periodo = 0, int Mes = 0)
        {
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));

            int numberOfDays = DateTime.DaysInMonth(año, mes);
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, 1);
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            //if (cuenta == 0 || cuenta == null)
            //{
            //    cuenta = 0;
            //}
            //int CuentaID = cuenta;
            if (Periodo != 0 && Mes != 0)
            {
            
                ViewBag.Rendicion = "Rendicion";
                
                
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                //var movimientos = db.Movimiento.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                //var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(a => a.Temporal == null && a.Eliminado == null && ( a.CuentaID == cuenta)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View();
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                
            
                //ViewBag.Cuenta = cuenta;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                //var movimientos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                //var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(a => a.Temporal == null && a.Eliminado == null && ( a.CuentaID == cuenta)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View();
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        [HttpPost]
        public ActionResult FondosrendirCuenta(string Desde = "", string Hasta = "" )
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            List<movcuenta> mcuenta = new List<movcuenta>();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                
                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin)/*.Where(m => m.TipoComprobanteID == 2)*/.Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        
        public ActionResult FondosRendirCuentaExcel(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            List<movcuenta> mcuenta = new List<movcuenta>();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;

                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin)/*.Where(m => m.TipoComprobanteID == 2)*/.Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        public ActionResult FondosrendirTP(int Periodo = 0, int Mes = 0)
        {
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));

            int numberOfDays = DateTime.DaysInMonth(año, mes);
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, 1);
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //if (cuenta == 0 || cuenta == null)
            //{
            //    cuenta = 0;
            //}
            //int CuentaID = cuenta;
            if (Periodo != 0 && Mes != 0)
            {

                ViewBag.Rendicion = "Rendicion";


                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                //var movimientos = db.Movimiento.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                //var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(a => a.Temporal == null && a.Eliminado == null && ( a.CuentaID == cuenta)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View();
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {


                //ViewBag.Cuenta = cuenta;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                //var movimientos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                //var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(a => a.Temporal == null && a.Eliminado == null && ( a.CuentaID == cuenta)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View();
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        [HttpPost]
        public ActionResult FondosrendirTP(FormCollection form, string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            List<movcuenta> mcuenta = new List<movcuenta>();
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                int TPID = Int32.Parse(form["TProyecto"].ToString());
                ViewBag.TP = TPID;

                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(p => p.Proyecto.TipoProyectoID == TPID).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                int TPID = Int32.Parse(form["TProyecto"].ToString());
                ViewBag.TP = TPID;
                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(p=> p.Proyecto.TipoProyectoID == TPID)/*.Where(m => m.TipoComprobanteID == 2)*/.Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }


        public ActionResult FondosRendirTPExcel(int TPID, string Desde = "", string Hasta = "" )
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            List<movcuenta> mcuenta = new List<movcuenta>();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;

                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(p => p.Proyecto.TipoProyectoID == TPID).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(p => p.Proyecto.TipoProyectoID == TPID)/*.Where(m => m.TipoComprobanteID == 2)*/.Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        public ActionResult FondosrendirRegion(int Periodo = 0, int Mes = 0)
        {
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));

            int numberOfDays = DateTime.DaysInMonth(año, mes);
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, 1);
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.regiones = db.Region.ToList();
            //if (cuenta == 0 || cuenta == null)
            //{
            //    cuenta = 0;
            //}
            //int CuentaID = cuenta;
            if (Periodo != 0 && Mes != 0)
            {

                ViewBag.Rendicion = "Rendicion";


                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                //var movimientos = db.Movimiento.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                //var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(a => a.Temporal == null && a.Eliminado == null && ( a.CuentaID == cuenta)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View();
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {


                //ViewBag.Cuenta = cuenta;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                //var movimientos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                //var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(a => a.Temporal == null && a.Eliminado == null && ( a.CuentaID == cuenta)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View();
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        [HttpPost]
        public ActionResult FondosrendirRegion(FormCollection form, string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            List<movcuenta> mcuenta = new List<movcuenta>();
            ViewBag.regiones = db.Region.ToList();
            int region = Int32.Parse(form["Region"].ToString());
            ViewBag.Region = region;
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                ViewBag.Region = region;

                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(p => p.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(p => p.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }


        public ActionResult FondosRendirRegionExcel(int region, string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            List<movcuenta> mcuenta = new List<movcuenta>();
            ViewBag.regiones = db.Region.ToList();
            ViewBag.Region = region;
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));

            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;

                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(p => p.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.Fecha <= Fin).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                //int CuentaID = Int32.Parse(form["CuentaID"].ToString());
                //ViewBag.Cuenta = CuentaID;
                //ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
                //string nombre_cuenta = db.Cuenta.Where(m => m.ID == CuentaID).First().NombreLista;
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(p => p.Proyecto.Direccion.Comuna.RegionID == region).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }

        /*Revisa como se producen*/
        public ActionResult FondosrendirA(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Persona Persona = (Persona)Session["Persona"];

            Usuario usuario = (Usuario)Session["Usuario"];

            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            }
            else if (usuario.esSupervisor)
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }
            else
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }
            if (Periodo != 0 && Mes != 0)
            {
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.ProSel = Proyecto.ID;
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                ViewBag.Rendicion = "Rendicion";
                //var movimientos = db.Movimiento.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int mes = (int)Session["Mes"];
                int año = (int)Session["Periodo"];
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                ViewBag.ProSel = Proyecto.ID;
                //var movimientos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null).OrderBy(m => m.Fecha);
                var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }
    
        public ActionResult Aportes(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Periodo != 0 && Mes != 0)
            {
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();

                ViewBag.Rendicion = "Aportes Comparacion";
                var ingresos = db.DetalleIngreso.Where(m => m.Ingreso.Mes == Mes).Where(m => m.Ingreso.Periodo == Periodo).Where(m => m.Ingreso.ProyectoID == Proyecto.ID).Where(m => m.Ingreso.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.Ingreso.CuentaID == 10).Where(a => a.Ingreso.Temporal == null && a.Ingreso.Eliminado == null && a.Ingreso.CuentaID != 1).OrderByDescending(a => a.Ingreso.Periodo).ThenBy(a => a.Ingreso.NumeroComprobante);
                return View(ingresos.ToList());
            }
            else
            {
                int mes = (int)Session["Mes"];
                int año = (int)Session["Periodo"];
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var ingresos = db.DetalleIngreso.Where(m => m.Ingreso.Fecha >= Inicio).Where(m => m.Ingreso.Fecha <= Fin).Where(m => m.Ingreso.ProyectoID == Proyecto.ID).Where(m => m.Ingreso.CuentaID == 10).Where(m => m.Ingreso.TipoComprobanteID == ctes.tipoIngreso && m.Ingreso.CuentaID != 1 && m.Ingreso.Eliminado == null && m.Ingreso.Temporal == null).OrderByDescending(a => a.Ingreso.Periodo).ThenBy(a => a.Ingreso.NumeroComprobante);
                return View(ingresos.ToList());
            }


        }

        [HttpPost]
        public ActionResult Aportes(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var ingresos = db.DetalleIngreso.Where(m => m.Ingreso.Fecha >= Inicio).Where(m => m.Ingreso.Fecha <= Fin).Where(m => m.Ingreso.ProyectoID == Proyecto.ID).Where(m => m.Ingreso.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.Ingreso.CuentaID == 10).Where(a => a.Ingreso.Temporal == null && a.Ingreso.Eliminado == null && a.Ingreso.CuentaID != 1).OrderByDescending(a => a.Ingreso.Periodo).ThenBy(a => a.Ingreso.NumeroComprobante);
                return View(ingresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var ingresos = db.DetalleIngreso.Where(m => m.Ingreso.Mes >= Mes).Where(m => m.Ingreso.Periodo <= Periodo).Where(m => m.Ingreso.ProyectoID == Proyecto.ID).Where(m => m.Ingreso.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.Ingreso.CuentaID == 10).Where(a => a.Ingreso.Temporal == null && a.Ingreso.Eliminado == null && a.Ingreso.CuentaID != 1).OrderByDescending(a => a.Ingreso.Periodo).ThenBy(a => a.Ingreso.NumeroComprobante);
                return View(ingresos.ToList());
            }
        }

        public ActionResult ExcelAportes(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var ingresos = db.DetalleIngreso.Where(m => m.Ingreso.Fecha >= Inicio).Where(m => m.Ingreso.Fecha <= Fin).Where(m => m.Ingreso.ProyectoID == Proyecto.ID).Where(m => m.Ingreso.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.Ingreso.CuentaID == 10).Where(a => a.Ingreso.Temporal == null && a.Ingreso.Eliminado == null && a.Ingreso.CuentaID != 1).OrderByDescending(a => a.Ingreso.Periodo).ThenBy(a => a.Ingreso.NumeroComprobante);
                return View(ingresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var ingresos = db.DetalleIngreso.Where(m => m.Ingreso.Mes >= Mes).Where(m => m.Ingreso.Periodo <= Periodo).Where(m => m.Ingreso.ProyectoID == Proyecto.ID).Where(m => m.Ingreso.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.Ingreso.CuentaID == 10).Where(a => a.Ingreso.Temporal == null && a.Ingreso.Eliminado == null && a.Ingreso.CuentaID != 1).OrderByDescending(a => a.Ingreso.Periodo).ThenBy(a => a.Ingreso.NumeroComprobante);
                return View(ingresos.ToList());
            }
        }
        [HttpPost]
        public ActionResult FondosrendirA(string Desde = "", string Hasta = "", int ProyID = 0)
        {
            //Proyecto Proyecto = db.Proyecto.Where(p => p.ID == ProyID);
            Persona Persona = (Persona)Session["Persona"];

            Usuario usuario = (Usuario)Session["Usuario"];
            ViewBag.ProSel = ProyID;
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            }
            else if (usuario.esSupervisor)
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }
            else
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var movimientos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == ProyID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                int mes = Mes;
                int año = Periodo;
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var movimientos = db.Movimiento.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == ProyID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
                return View(movimientos.ToList());
                //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Mes >= Mes).Where(m => m.Egreso.Periodo <= Periodo).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                //return View(egresos.ToList());
            }
        }
        public ActionResult DeudasPendientes(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Clasificacion = "Todos";
            if (Periodo != 0 && Mes != 0)
            {
                ViewBag.Rendicion = "Rendicion";
                var dp = db.DeudaPendiente.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID && m.CuentaID != 1).OrderBy(m => m.NumeroComprobante);
                return View(dp.ToList());
            }
            else
            {
                int mes = (int)Session["Mes"];
                int año = (int)Session["Periodo"];
                int numberOfDays = DateTime.DaysInMonth(año, mes);
                DateTime Inicio = new DateTime(año, mes, 1);
                DateTime Fin = new DateTime(año, mes, numberOfDays);
                ViewBag.Desde = Inicio.ToShortDateString();
                ViewBag.Hasta = Fin.ToShortDateString();
                var dp = db.DeudaPendiente.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID && m.CuentaID != 1).OrderBy(m => m.ID);
                return View(dp.ToList());
            }
        }

        [HttpPost]
        public ActionResult DeudasPendientes(string Desde = "", string Hasta = "", string Clasificacion = "Todos")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Clasificacion = Clasificacion;
            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                if (Clasificacion.Equals("Cancelados"))
                {
                    var dp = db.DeudaPendiente.Where(m => m.EgresoID != null).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID && m.CuentaID != 1).OrderBy(m => m.ID);
                    return View(dp.ToList());
                }
                else if (Clasificacion.Equals("NoCancelados"))
                {
                    var dp = db.DeudaPendiente.Where(m => m.EgresoID == null).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID && m.CuentaID != 1).OrderBy(m => m.ID);
                    return View(dp.ToList());
                }
                else
                {
                    var dp = db.DeudaPendiente.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                    return View(dp.ToList());
                }
            }
            else
            {
                int Periodo = (int)Session["Periodo"];
                int Mes = (int)Session["Mes"];
                if (Clasificacion.Equals("Cancelados"))
                {
                    var dp = db.DeudaPendiente.Where(m => m.EgresoID != null).Where(m => m.Mes >= Mes).Where(m => m.Periodo <= Periodo).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                    return View(dp.ToList());
                }
                else if (Clasificacion.Equals("NoCancelados"))
                {
                    var dp = db.DeudaPendiente.Where(m => m.EgresoID == null).Where(m => m.Mes >= Mes).Where(m => m.Periodo <= Periodo).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                    return View(dp.ToList());
                }
                else
                {
                    var dp = db.DeudaPendiente.Where(m => m.Mes >= Mes).Where(m => m.Periodo <= Periodo).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
                    return View(dp.ToList());
                }
            }
        }
        // Honorarios filtro
        // Rescatar Proyectos por tipo
        public string ProyectoTipo(int id)
        {
            var Proyectos = (from c in db.Proyecto  
                           where (c.TipoProyectoID  == id) && (c.Eliminado == null)
                           orderby c.CodCodeni , c.Nombre  
                           select new
                           {
                               Value = c.ID,
                               Text = (c.CodCodeni + " - " + c.Nombre) 
                           }).ToList();

            return new JavaScriptSerializer().Serialize(Proyectos);
        }
        public ActionResult HonorariosFiltros(int Periodo = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Periodo != 0 )
            {
                ViewBag.Rendicion = "Rendicion";
            }
            else
            {
                Periodo = (int)Session["Periodo"];
               
            }
            ViewBag.tipoBeneficiario = 0;
            ViewBag.Periodo = Periodo;
            ViewBag.Proyecto = Proyecto.ID;
            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyectoID == Proyecto.TipoProyectoID).OrderBy(p => p.CodCodeni), "ID", "NombreLista", Proyecto.ID);
            ViewBag.TipoProgramaID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla", Proyecto.TipoProyectoID   );  
            ViewBag.PersonaID = new SelectList(db.Persona.Where(d => d.TipoPersonalID  == 2).ToList()  , "ID", "NombreLista");
            ViewBag.Nboleta = "";
            ViewBag.rut = "";
            ViewBag.dv = "";
            ViewBag.nombre = "";
            var bh = db.BoletaHonorario.Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
            return View(bh.ToList());
        }
        [HttpPost]
        public ActionResult HonorariosFiltros(FormCollection datos)
        {
            int nboleta = 0;
            int nPersona = 0;
            int nProveedor = 0;
            int ntipoBeneficiario = 0;
            int ProyectoID = Int32.Parse(datos["ProyectoID"].ToString());
            int TipoProyectoID = Int32.Parse(datos["TipoProgramaID"].ToString());
            string nRut = "";
            string ndv = "";
            string nbeneficiario = "";
            try
            {

            }
            catch (Exception) { 
            
            
            }

            // Por Boleta
            try
            {
                nboleta = int.Parse(datos["Nboleta"].ToString());
                ViewBag.Nboleta = nboleta.ToString() ;
            }
            catch (Exception)
            {
                nboleta = 0;
            }
            // Por Personal
            try
            {
                nRut = datos["Rut"].ToString();
                ndv = datos["DV"].ToString();
                nbeneficiario = datos["Beneficiario"].ToString();
                nPersona = int.Parse(datos["PersonaID"].ToString());
            }
            catch (Exception)
            {
                nPersona = 0;
            }
            // Por Proveedor
            try {
                nRut = datos["Rut"].ToString();
                ndv = datos["DV"].ToString();
                nbeneficiario = datos["Beneficiario"].ToString();
                nProveedor = int.Parse(datos["ProveedorID"].ToString());
            }
            catch (Exception) {
                nProveedor = 0;
            }
            // Por Otros
            try
            {
                nRut = datos["Rut"].ToString();
                ndv = datos["DV"].ToString();
                nbeneficiario = datos["Beneficiario"].ToString();
                ntipoBeneficiario = int.Parse(datos["tipoBeneficiario"].ToString());
            }
            catch (Exception)
            {
                nRut = "";
            }
            int Periodo = (int)Session["Periodo"];
            ViewBag.Proyecto = ProyectoID;
            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyectoID == TipoProyectoID).OrderBy(p => p.CodCodeni), "ID", "NombreLista", ProyectoID);
            ViewBag.TipoProgramaID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla", TipoProyectoID);
            ViewBag.PersonaID = new SelectList(db.Persona.Where(d => d.TipoPersonalID == 2).ToList(), "ID", "NombreLista");
            ViewBag.rut = nRut;
            ViewBag.dv = ndv;
            ViewBag.nombre = nbeneficiario;
            // Busqueda filtros



            var bh = db.BoletaHonorario.Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == ProyectoID).OrderBy(m => m.ID); 
            if (nboleta != 0)
            {
                bh = db.BoletaHonorario.Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == ProyectoID && m.NroBoleta == nboleta).OrderBy(m => m.ID);
            }
            if (nPersona != 0)
            {
                bh = db.BoletaHonorario.Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == ProyectoID && m.PersonaID == nPersona).OrderBy(m => m.ID);
            }
            if (nProveedor != 0)
            {
                bh = db.BoletaHonorario.Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == ProyectoID && m.ProveedorID == nProveedor).OrderBy(m => m.ID);
            }
            if (ntipoBeneficiario == 3) {
                bh = db.BoletaHonorario.Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == ProyectoID && m.Rut == nRut).OrderBy(m => m.ID);
            }

            ViewBag.tipoBeneficiario = ntipoBeneficiario;

            return View(bh.ToList());
        }
        
        public ActionResult Honorarios(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Periodo != 0 && Mes != 0)
            {
                ViewBag.Rendicion = "Rendicion";
            }
            else
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }

            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;
            var bh = db.BoletaHonorario.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
            return View(bh.ToList());
        }

        [HttpPost]
        public ActionResult Honorarios(int Periodo = 0, int Mes = 0, int flag = 1)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Periodo == 0 && Mes == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }

            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;
            var bh = db.BoletaHonorario.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
            return View(bh.ToList());
        }

        public ActionResult HonorariosBuscar(string busqueda = "", int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int aux = 0;
            var bh = db.BoletaHonorario.Where(m => m.Mes == 0).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID).ToList();
            ViewBag.Busqueda = busqueda;
            ViewBag.mesInicio = mesInicio;
            ViewBag.anioInicio = anioInicio;
            ViewBag.mesHasta = mesHasta;
            ViewBag.anioHasta = aniohasta;
              bh = db.BoletaHonorario.Where(m => m.ProyectoID == Proyecto.ID).Where(n => (n.Egreso.NumeroComprobante.ToString().Contains(busqueda)) || (n.NroBoleta.ToString().Contains(busqueda))||(n.Persona.Nombres.ToString().Contains(busqueda))|| (n.Proveedor.Nombre.ToString().Contains(busqueda))|| (n.Beneficiario.ToString().Contains(busqueda)) || (n.Rut.ToString().Contains(busqueda))|| (n.Proveedor.Rut.ToString().Contains(busqueda))|| (n.Persona.Rut.ToString().Contains(busqueda))).OrderByDescending(a => a.Periodo).ToList();
           
            if (anioInicio != aniohasta && aniohasta != 0 && anioInicio != 0)
            {
                while (anioInicio < aniohasta)
                {
                    if (aux == 0)
                    {
                        foreach (var item in db.BoletaHonorario.Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            bh.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in db.BoletaHonorario.Where(d => (d.Mes >= 1 && d.Mes <= 12) && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            bh.Add(item);
                        }
                    }
                    aux++;
                    anioInicio++;
                }
                foreach (var item in db.BoletaHonorario.Where(d => (d.Mes >= 1 && d.Mes <= mesHasta) && d.Periodo == aniohasta && d.ProyectoID == Proyecto.ID).ToList())
                {
                    bh.Add(item);
                }

                return View(bh.ToList());
            }

            return View(bh.ToList());
        }

        public ActionResult HonorariosAdmin(string busqueda = "", int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int aux = 0;
            var bh = db.BoletaHonorario.Where(m => m.Mes == 0).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID).ToList();
            ViewBag.Busqueda = busqueda;
            ViewBag.mesInicio = mesInicio;
            ViewBag.anioInicio = anioInicio;
            ViewBag.mesHasta = mesHasta;
            ViewBag.anioHasta = aniohasta;
            bh = db.BoletaHonorario.Where(m => m.ProyectoID == Proyecto.ID).Where(n => (n.Egreso.NumeroComprobante.ToString().Contains(busqueda)) || (n.NroBoleta.ToString().Contains(busqueda)) || (n.Persona.Nombres.ToString().Contains(busqueda)) || (n.Proveedor.Nombre.ToString().Contains(busqueda)) || (n.Beneficiario.ToString().Contains(busqueda)) || (n.Rut.ToString().Contains(busqueda)) || (n.Proveedor.Rut.ToString().Contains(busqueda)) || (n.Persona.Rut.ToString().Contains(busqueda))).OrderByDescending(a => a.Periodo).ToList();

            if (anioInicio != aniohasta && aniohasta != 0 && anioInicio != 0)
            {
                while (anioInicio < aniohasta)
                {
                    if (aux == 0)
                    {
                        foreach (var item in db.BoletaHonorario.Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            bh.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in db.BoletaHonorario.Where(d => (d.Mes >= 1 && d.Mes <= 12) && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            bh.Add(item);
                        }
                    }
                    aux++;
                    anioInicio++;
                }
                foreach (var item in db.BoletaHonorario.Where(d => (d.Mes >= 1 && d.Mes <= mesHasta) && d.Periodo == aniohasta && d.ProyectoID == Proyecto.ID).ToList())
                {
                    bh.Add(item);
                }

                return View(bh.ToList());
            }

            return View(bh.ToList());
        }

        public ActionResult FondoFijo(int Periodo = 0, int Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Periodo != 0 && Mes != 0)
            {
                ViewBag.Rendicion = "Rendicion";
            }
            else
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }

            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;
            var ff = db.FondoFijo.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
            return View(ff.ToList());
        }

        [HttpPost]
        public ActionResult FondoFijo(int Periodo = 0, int Mes = 0, int flag = 1)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Periodo == 0 && Mes == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }

            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;
            var ff = db.FondoFijo.Where(m => m.Mes == Mes).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
            return View(ff.ToList());
        }

        public ActionResult ExcelIngresos(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            @ViewBag.Proyecto = Proyecto.NombreLista;
            @ViewBag.Desde = Desde;
            @ViewBag.Hasta = Hasta;
            @ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;

            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var ingresos = db.Movimiento.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderBy(m => m.NumeroComprobante);
                return View(ingresos.ToList());
            }

            return null;
        }

        public ActionResult ExcelReintegros(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            @ViewBag.Proyecto = Proyecto.NombreLista;
            @ViewBag.Desde = Desde;
            @ViewBag.Hasta = Hasta;
            @ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;

            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var reintegros = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderBy(m => m.NumeroComprobante);
                return View(reintegros.ToList());
            }

            return null;
        }

        public ActionResult ExcelEgresos(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            DateTime Inicio = DateTime.Parse(Desde);
            DateTime Fin = DateTime.Parse(Hasta);
            @ViewBag.Proyecto = Proyecto.NombreLista;
            ViewBag.Desde = Desde;
            ViewBag.Hasta = Hasta;
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            return View(movimientos.ToList());
            //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
            //return View(egresos.ToList());
        }
        
        public ActionResult Excelfondorendir(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            DateTime Inicio = DateTime.Parse(Desde);
            DateTime Fin = DateTime.Parse(Hasta);
            @ViewBag.Proyecto = Proyecto.NombreLista;
            ViewBag.Desde = Desde;
            ViewBag.Hasta = Hasta;
            var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == 2).Where(a => a.Temporal == null && a.Eliminado == null && (a.CuentaID != 6 || a.CuentaID == null)).OrderByDescending(a => a.Periodo).ThenBy(a => a.NumeroComprobante);
            return View(movimientos.ToList());
            //var egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= Inicio).Where(m => m.Egreso.Fecha <= Fin).Where(m => m.Egreso.ProyectoID == Proyecto.ID).OrderBy(m => m.ID);
            //return View(egresos.ToList());
        }


        public ActionResult ExcelFondoFijo(int Mes = 0, int Periodo = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            @ViewBag.Proyecto = Proyecto.NombreLista;
            @ViewBag.Mes = Mes;
            @ViewBag.Periodo = Periodo;
           
            if (Mes > 0 && Periodo > 0)
            {
                var fondofijo = db.FondoFijo.Where(f => f.ProyectoID == Proyecto.ID).Where(f => f.Mes == Mes).Where(f => f.Periodo == Periodo).OrderBy(f => f.Cuenta.Orden);
                return View(fondofijo.ToList());
            }

            return null;
        }

        public ActionResult ExcelHonorarios(int Mes = 0, int Periodo = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            @ViewBag.Proyecto = Proyecto.NombreLista;
            @ViewBag.CodSename = Proyecto.CodSename;  
            @ViewBag.Mes = Mes;
            @ViewBag.Periodo = Periodo;

            if (Mes > 0 && Periodo > 0)
            {
                var boletahonorario = db.BoletaHonorario.Where(f => f.ProyectoID == Proyecto.ID).Where(b => b.Mes == Mes).Where(b => b.Periodo == Periodo);
                return View(boletahonorario.ToList());
            }

            return null;
        }
        public ActionResult ExcelHonorariosBuscar(string busqueda = "", int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int aux = 0;
            var bh = db.BoletaHonorario.Where(m => m.Mes == 0).OrderBy(m => m.ID).ToList();
            ViewBag.Busqueda = busqueda;
            bh = db.BoletaHonorario.Where(p => p.ProyectoID == Proyecto.ID).Where(n => (n.Egreso.NumeroComprobante.ToString().Contains(busqueda)) || (n.NroBoleta.ToString().Contains(busqueda)) || (n.Persona.Nombres.ToString().Contains(busqueda)) || (n.Proveedor.Nombre.ToString().Contains(busqueda)) || (n.Beneficiario.ToString().Contains(busqueda)) || (n.Rut.ToString().Contains(busqueda)) || (n.Proveedor.Rut.ToString().Contains(busqueda)) || (n.Persona.Rut.ToString().Contains(busqueda))).OrderByDescending(a => a.Periodo).ToList();

            if (anioInicio != aniohasta && aniohasta != 0 && anioInicio != 0)
            {
                while (anioInicio < aniohasta)
                {
                    if (aux == 0)
                    {
                        foreach (var item in db.BoletaHonorario.Where(p => p.ProyectoID == Proyecto.ID).Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio).ToList())
                        {
                            bh.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in db.BoletaHonorario.Where(p => p.ProyectoID == Proyecto.ID).Where(d => (d.Mes >= 1 && d.Mes <= 12) && d.Periodo == anioInicio ).ToList())
                        {
                            bh.Add(item);
                        }
                    }
                    aux++;
                    anioInicio++;
                }
                foreach (var item in db.BoletaHonorario.Where(p => p.ProyectoID == Proyecto.ID).Where(d => (d.Mes >= 1 && d.Mes <= mesHasta) && d.Periodo == aniohasta).ToList())
                {
                    bh.Add(item);
                }

                return View(bh.ToList());
            }

            return View(bh.ToList());
        }
        public ActionResult ExcelHonorariosAdmin(string busqueda = "", int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int aux = 0;
            var bh = db.BoletaHonorario.Where(m => m.Mes == 0).OrderBy(m => m.ID).ToList();
            ViewBag.Busqueda = busqueda;
            bh = db.BoletaHonorario.Where(n => (n.Egreso.NumeroComprobante.ToString().Contains(busqueda)) || (n.NroBoleta.ToString().Contains(busqueda)) || (n.Persona.Nombres.ToString().Contains(busqueda)) || (n.Proveedor.Nombre.ToString().Contains(busqueda)) || (n.Beneficiario.ToString().Contains(busqueda)) || (n.Rut.ToString().Contains(busqueda)) || (n.Proveedor.Rut.ToString().Contains(busqueda)) || (n.Persona.Rut.ToString().Contains(busqueda))).OrderByDescending(a => a.Periodo).ToList();

            if (anioInicio != aniohasta && aniohasta != 0 && anioInicio != 0)
            {
                while (anioInicio < aniohasta)
                {
                    if (aux == 0)
                    {
                        foreach (var item in db.BoletaHonorario.Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio).ToList())
                        {
                            bh.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in db.BoletaHonorario.Where(d => (d.Mes >= 1 && d.Mes <= 12) && d.Periodo == anioInicio).ToList())
                        {
                            bh.Add(item);
                        }
                    }
                    aux++;
                    anioInicio++;
                }
                foreach (var item in db.BoletaHonorario.Where(d => (d.Mes >= 1 && d.Mes <= mesHasta) && d.Periodo == aniohasta).ToList())
                {
                    bh.Add(item);
                }

                return View(bh.ToList());
            }

            return View(bh.ToList());
        }

        public ActionResult ExcelDeudasPendientes(string Desde = "", string Hasta = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            @ViewBag.Proyecto = Proyecto.NombreLista;
            @ViewBag.Desde = Desde;
            @ViewBag.Hasta = Hasta;

            if (!Desde.Equals("") && !Hasta.Equals(""))
            {
                DateTime Inicio = DateTime.Parse(Desde);
                DateTime Fin = DateTime.Parse(Hasta);
                ViewBag.Desde = Desde;
                ViewBag.Hasta = Hasta;
                var deudapendientes = db.DeudaPendiente.Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin).Where(m => m.ProyectoID == Proyecto.ID && m.CuentaID != 1).OrderBy(d => d.Cuenta.Orden);
                return View(deudapendientes.ToList());
            }

            return null;
        }
    }
}