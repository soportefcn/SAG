using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Data;

namespace SAG2.Controllers
{
    public class RendicionController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();

        //
        // GET: /Rendicion/

        public ActionResult Index()
        {
            Proyecto proyecto = (Proyecto)Session["Proyecto"];
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];

            List<Periodo> periodos_anteriores = new List<Periodo>();
            try
            {
                periodos_anteriores = db.Periodo.Where(p => p.ProyectoID == proyecto.ID).OrderByDescending(p => p.ID).ToList();
            }
            catch (Exception)
            {
                
            }

            // Verificamos si existe DAP
            try
            {
                ViewBag.DAP = db.DetalleEgreso.Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Nulo == null && m.Temporal == null && m.Egreso.Eliminado == null && m.Cuenta.Codigo.StartsWith("10")).Sum(d => d.Monto);
            }
            catch (Exception)
            {
                ViewBag.DAP = 0;
            }

            ViewBag.periodos_anteriores = periodos_anteriores;

            try
            {
                ViewBag.nroComprobantes = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == proyecto.ID && m.Periodo == periodo && m.Mes == mes && m.Temporal == null && m.Nulo == null && m.Eliminado == null).Count();
            }
            catch (Exception)
            {
                ViewBag.nroComprobantes = 0;    
            }

            try
            {
                ViewBag.nroConciliados = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == proyecto.ID && m.Periodo == periodo && m.Mes == mes && m.Temporal == null && m.Nulo == null && m.Eliminado == null && m.Conciliado.Equals("S")).Count();
            }
            catch (Exception)
            {
                ViewBag.nroConciliados = 0;
            }

            return View();
        }

        public ActionResult ListadoRendiciones()
        {
            Proyecto proyecto = (Proyecto)Session["Proyecto"];
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];

            List<Periodo> periodos_anteriores = new List<Periodo>();
            try
            {
                periodos_anteriores = db.Periodo.Where(p => p.ProyectoID == proyecto.ID).OrderByDescending(p => p.ID).ToList();
            }
            catch (Exception)
            {

            }

            // Verificamos si existe DAP
            try
            {
                ViewBag.DAP = db.DetalleEgreso.Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Nulo == null && m.Temporal == null && m.Egreso.Eliminado == null && m.Cuenta.Codigo.StartsWith("10")).Sum(d => d.Monto);
            }
            catch (Exception)
            {
                ViewBag.DAP = 0;
            }

            ViewBag.periodos_anteriores = periodos_anteriores;

            try
            {
                ViewBag.nroComprobantes = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == proyecto.ID && m.Periodo == periodo && m.Mes == mes && m.Temporal == null && m.Nulo == null && m.Eliminado == null).Count();
            }
            catch (Exception)
            {
                ViewBag.nroComprobantes = 0;
            }

            try
            {
                ViewBag.nroConciliados = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == proyecto.ID && m.Periodo == periodo && m.Mes == mes && m.Temporal == null && m.Nulo == null && m.Eliminado == null && m.Conciliado.Equals("S")).Count();
            }
            catch (Exception)
            {
                ViewBag.nroConciliados = 0;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Guardar()
        {

            var Meses = new string[12]
            {
	            "Enero",
	            "Febrero",
	            "Marzo",
	            "Abril",
	            "Mayo",
	            "Junio",
	            "Julio",
	            "Agosto",
	            "Septiembre",
	            "Octubre",
	            "Noviembre",
	            "Diciembre"
	        };

            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente cuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            Persona persona = (Persona)Session["Persona"];
            int intervenciones = 0;
            int indemnizacion = 0;

            try
            {
                if (Request.Form["Intervenciones"] == null || Request.Form["Intervenciones"].ToString().Equals(""))
                {
                    throw new Exception("Debe ingresar el número de intervenciones correspondientes al período.");
                }

                intervenciones = Int32.Parse(Request.Form["Intervenciones"].ToString());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //@ViewBag.Mensaje = utils.mensajeError("Debe ingresar las intervenciones correspondientes al período.");
                //return View();
            }

            try
            {
                indemnizacion = Int32.Parse(Request.Form["Indemnizacion"].ToString());
            }
            catch (Exception)
            {
                indemnizacion = 0;
            }

            // Guardamos interveciones
            Intervencion intervencion = new Intervencion();
            intervencion.Mes = mes;
            intervencion.Periodo = periodo;
            intervencion.ProyectoID = proyecto.ID;
            intervencion.Cobertura = proyecto.Convenio.NroPlazas;
            intervencion.Atenciones = intervenciones;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Intervencion.Add(intervencion);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("ModelState no válido.");
                }
            }
            catch (Exception e)
            {
                //throw new Exception(e.StackTrace);
                @ViewBag.Mensaje = utils.mensajeError(utils.Mensaje(e));
                return View();
            }

            int prox_mes = mes;
            int prox_periodo = periodo;

            if (mes == 12)
            {
                prox_periodo++;
                prox_mes = 1;
            }
            else
            {
                prox_mes++;
            }

            try
            {
                // Actualizamos convenio para proximo mes
                //Response.Write("actualizando convenio/");
                //Response.Write("mes: " + mes + ", periodo:" + periodo + "/proxm:" + prox_mes+",prox_pe:" + prox_periodo);
                //Convenio Convenio = db.Convenio.Where(c => c.ProyectoID == proyecto.ID && c.Mes == mes && c.Periodo == periodo).Single();
                //if (Convenio.Periodo != prox_periodo && Convenio.Mes != prox_mes)
                //{
                    Proyecto Proyecto = db.Proyecto.Find(proyecto.ID);

                    //Convenio = null;
                    Convenio Convenio = new Convenio();

                    Convenio.ResEx = proyecto.Convenio.ResEx;
                    Convenio.NroPlazas = proyecto.Convenio.NroPlazas;
                    Convenio.Comentarios = proyecto.Convenio.Comentarios;
                    Convenio.ProyectoID = proyecto.ID;
                    Convenio.Periodo = prox_periodo;
                    Convenio.Mes = prox_mes;
                    Convenio.FechaInicio = proyecto.Convenio.FechaInicio;
                    Convenio.FechaTermino = proyecto.Convenio.FechaTermino;
                    db.Convenio.Add(Convenio);
                    db.SaveChanges();
                
                    Proyecto.Convenio = null;
                    Proyecto.ConvenioID = Convenio.ID;
                    db.Entry(Proyecto).State = EntityState.Modified;
                    db.SaveChanges();

                    Session.Remove("Proyecto");
                    Session.Add("Proyecto", Proyecto);

                    //Response.Write("convenio actualizado");
                //}
            }
            catch (Exception e)
            {
                //throw new Exception(e.StackTrace);
                @ViewBag.Mensaje = utils.mensajeError(utils.Mensaje(e));
                return View();
            }

            // Cerramos movimientos del período
            int suma_ingresos = 0;
            int suma_egresos = 0;

            try
            {
                //List<Movimiento> movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == proyecto.ID).ToList();
                
                //List<Movimiento> ingresos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == proyecto.ID).ToList();
                //List<Movimiento> egresos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == proyecto.ID).ToList();

                List<Movimiento> ingresos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.TipoComprobanteID != ctes.tipoEgreso).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).ToList();
                List<Movimiento> egresos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.Nulo == null && m.Temporal == null && m.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).ToList();

                suma_ingresos = ingresos.Sum(m => m.Monto_Ingresos);
                suma_egresos = egresos.Sum(m => m.Monto_Egresos);

                foreach(Movimiento movimiento in ingresos)
                {
                    movimiento.Cerrado = "S";
                    if (ModelState.IsValid)
                    {
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("ModelState no válido. (" + movimiento.ID + ")");
                    }
                }

                foreach (Movimiento movimiento in egresos)
                {
                    movimiento.Cerrado = "S";
                    if (ModelState.IsValid)
                    {
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("ModelState no válido. (" + movimiento.ID + ")");
                    }
                }
            }
            catch (Exception e)
            {
                @ViewBag.Mensaje = utils.mensajeError(utils.Mensaje(e));
                return View();
                //throw new Exception(e.StackTrace);
            }
       



            // Cerramos el periodo
            Periodo periodo_cierre = new Periodo();
            periodo_cierre.Mes = mes;
            periodo_cierre.Ano = periodo;
            periodo_cierre.ProyectoID = proyecto.ID;
            periodo_cierre.Indemnizacion = indemnizacion;
            periodo_cierre.Fecha = DateTime.Now;
            periodo_cierre.PersonaID = persona.ID;

            try
            {
                if (ModelState.IsValid)
                {
                    db.Periodo.Add(periodo_cierre);
                    db.SaveChanges();

                    if (mes == 12)
                    {
                        Session.Add("Mes", 1);
                        Session.Add("Periodo", periodo + 1);
                    }
                    else
                    {
                        Session.Add("Mes", mes + 1);
                        Session.Add("Periodo", periodo);
                    }

                    Session.Add("Fecha", Meses[(int)Session["Mes"]-1] + " " + Session["Periodo"].ToString());
                    //Session.Add("InformacionPie", proyecto.NombreLista + " (" + cuentaCorriente.NumeroLista + ") | " + persona.NombreCompleto + " | " + Session["Fecha"].ToString());

                    Usuario usuario = (Usuario)Session["Usuario"];
                    Persona Persona = (Persona)Session["Persona"];

                    if (usuario.esAdministrador)
                    {
                        Session.Add("InformacionPie", proyecto.NombreLista + " (" + cuentaCorriente.NumeroLista + ") | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString() + " | ProyectoID: " + proyecto.ID + " CtaCteID: " + cuentaCorriente.ID + " PersonaID: " + Persona.ID);
                    }
                    else
                    {
                        Session.Add("InformacionPie", proyecto.NombreLista + " (" + cuentaCorriente.NumeroLista + ") | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                //@ViewBag.Mensaje = utils.mensajeError(e.Message);
                //return View();
                throw new Exception(e.StackTrace);
            }

            try
            {
                Saldo saldoOriginal = db.Saldo.Where(m => m.CuentaCorrienteID == cuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single();
                //int saldoFinal = saldoOriginal.SaldoInicialCartola + suma_ingresos - suma_egresos - indemnizacion;
                int saldoFinal = saldoOriginal.SaldoInicialCartola + suma_ingresos - suma_egresos ;
                //saldoOriginal.SaldoFinalCartola = saldoFinal;
                saldoOriginal.SaldoFinal = saldoFinal;
                db.Entry(saldoOriginal).State = EntityState.Modified;
                db.SaveChanges();

                // ----

                Saldo saldo = new Saldo();
                saldo.CuentaCorrienteID = cuentaCorriente.ID;
                saldo.Mes = Int32.Parse(Session["Mes"].ToString());
                saldo.Periodo = Int32.Parse(Session["Periodo"].ToString());
                saldo.SaldoInicialCartola = saldoFinal;
                saldo.SaldoFinal = saldoFinal;
                //saldo.SaldoFinalCartola = saldoFinal;

                db.Saldo.Add(saldo);
                db.SaveChanges();

                // Actualizamos Saldos Bodega

                List<Bodega> BoProyecto = db.Bodega.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Saldo > 0).ToList();
                foreach (Bodega datbo in BoProyecto)
                {
                    Bodega nuevomes = new Bodega();
                    nuevomes.Mes = Int32.Parse(Session["Mes"].ToString());
                    nuevomes.Periodo = Int32.Parse(Session["Periodo"].ToString());
                    nuevomes.ProyectoID = proyecto.ID;
                    nuevomes.ArticuloID = datbo.ArticuloID;
                    nuevomes.SaldoInicial = datbo.Saldo;
                    nuevomes.Entrada = 0;
                    nuevomes.Salida = 0;
                    nuevomes.Saldo = (datbo.Entrada + datbo.SaldoInicial) - datbo.Salida ;
                    db.Bodega.Add(nuevomes);
                    db.SaveChanges();

                }
            }
            catch(Exception)
            {}

            @ViewBag.Mensaje = utils.mensajeOK("Rendición de cuentas generada con éxito.");
            return View();
            //return RedirectToAction("Index");
        }

        public ActionResult Codeni(int Periodo = 0, int Mes = 0)
        {
            if (Periodo == 0 || Mes == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }

            Proyecto proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente cuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;

            ViewBag.SaldoInicialCta = db.Saldo.Where(m => m.CuentaCorrienteID == cuentaCorriente.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes == Mes).Single().SaldoInicialCartola;
            ViewBag.SaldoFinalCta = db.Saldo.Where(m => m.CuentaCorrienteID == cuentaCorriente.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes == Mes).Single().SaldoFinal;

            ViewBag.CuentasIngresos = db.Cuenta.Where(c => c.Tipo.Equals("I")).Where(c => c.Codigo.Length <= 2).OrderBy(c => c.Orden).ToList();
            ViewBag.CuentasEgresos = db.Cuenta.Where(c => c.Tipo.Equals("E")).Where(c => c.Codigo.Length <= 2).OrderBy(c => c.Orden).ToList();

            try
            {
                ViewBag.plazas = db.Convenio.Where(c => c.ProyectoID == proyecto.ID && c.Mes == Mes && c.Periodo == Periodo).Single().NroPlazas.ToString();
            }
            catch (Exception)
            {
                ViewBag.plazas = "0";
            }


            try
            {
                ViewBag.Reintegros = db.Movimiento.Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.Periodo == Periodo).Where(m => m.Mes == Mes).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.CuentaID).ToList();
            }
            catch (Exception)
            {
                ViewBag.Reintegros = new List<Movimiento>();
            }
            
            try
            {
                ViewBag.Ingresos = db.Movimiento.Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.Periodo == Periodo).Where(m => m.Mes == Mes).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Nulo == null && m.CuentaID != 1 && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.CuentaID).ToList();
            }
            catch (Exception)
            {
                ViewBag.Ingresos = new List<Movimiento>();
            }
            
            try
            {
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.auto == 0).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes == Mes).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Nulo == null && m.Temporal == null && m.Egreso.Eliminado == null && ((m.CuentaID != 6 || m.CuentaID == null) || m.CuentaID == null)).OrderBy(m => m.CuentaID).ToList();
            } catch (Exception)
            {
                ViewBag.Egresos = new List<DetalleEgreso>();
            }

            try
            {
                ViewBag.indemnizacion = db.Periodo.Where(c => c.ProyectoID == proyecto.ID && c.Mes == Mes && c.Ano == Periodo).Single().Indemnizacion;
            }
            catch (Exception)
            {
                ViewBag.indemnizacion = 0;
            }

            // Periodo

            ViewBag.FechaRendicion = "0";

            try
            {
                Periodo PeriodoRendicion = db.Periodo.Where(p => p.Ano == Periodo && p.Mes == Mes && p.ProyectoID == proyecto.ID).Single();
                ViewBag.FechaRendicion = PeriodoRendicion.Fecha.Value.ToShortDateString() + " " + PeriodoRendicion.Fecha.Value.ToShortTimeString();
            }
            catch (Exception)
            {
                ViewBag.FechaRendicion = "0";
            }

            return View();
        }

        public ActionResult Sename()
        {
            return View();
        }

        public ActionResult MesAnterior()
        {
            Proyecto Proyecto = (Proyecto) Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente) Session["CuentaCorriente"];

            int saldoID = db.Saldo.Where(q => q.CuentaCorrienteID == CuentaCorriente.ID).Max(q => q.ID); // Borrar Max ID
            int intervencionID = db.Intervencion.Where(q => q.ProyectoID == Proyecto.ID).Max(q => q.ID);
            int periodoID = db.Periodo.Where(q => q.ProyectoID == Proyecto.ID).Max(q => q.ID);

            db.Database.ExecuteSqlCommand("DELETE FROM Saldo WHERE ID = " + saldoID);
            db.Database.ExecuteSqlCommand("DELETE FROM Intervenciones WHERE ID = " + intervencionID);
            db.Database.ExecuteSqlCommand("DELETE FROM Periodos WHERE ID = " + periodoID);

            return View();
        }
    }
}
