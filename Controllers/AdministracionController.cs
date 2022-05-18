using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.Data;
using SAG2.Classes;
using System.Data.Entity;

namespace SAG2.Controllers
{
    public class AdministracionController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();

        //
        // GET: /Administracion/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Correlativos(int state = 0)
        {
            ViewBag.Mensaje = string.Empty;

            if (state == 546)
            {
                ViewBag.Mensaje = utils.mensajeError("Existen contratos vigentes para este Proyecto, no es posible cerrarlo.");
            }
            else if (state == 324)
            {
                ViewBag.Mensaje = utils.mensajeError("Existen contratos vigentes para este Proyecto, no es posible eliminarlo.");
            }

           // ViewBag.Resolucion = db.Resolucion.Where(d => d.Estado == 1).ToList();

            List<Proyecto> proyecto = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            return View(proyecto.ToList());
        }

        public ActionResult Cierres(int periodo = 0, int proyectoID = 0)
        {
            ViewBag.Mensaje = string.Empty;

            if (periodo == 0)
            {
                ViewBag.periodo = DateTime.Now.Year;
            }
            else
            {
                ViewBag.periodo = periodo;
            }
            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", proyectoID);

            //ViewBag.Resolucion = db.Resolucion.Where(d => d.Estado == 1).ToList();
            List<Proyecto> proyecto = db.Proyecto.OrderBy(p => p.CodCodeni).ToList();
            List<Proyecto> proyecto2 = db.Proyecto.Where(p => p.ID == 0).OrderBy(p => p.CodCodeni).ToList();

            foreach (var item in proyecto)
            {
                if (item.estaCerrado == false && item.estaEliminado == false)
                {
                    proyecto2.Add(item);
                }
            }
            if (proyectoID != 0)
            {

                ViewBag.Seleccion = proyectoID;
                proyecto = db.Proyecto.Where(p => p.Eliminado == null && p.ID == proyectoID).OrderBy(p => p.CodCodeni).ToList();
                ViewBag.proyecto = proyecto.Find(p => p.ID == proyectoID).NombreLista;
                return View(proyecto.ToList());
            }

            return View(proyecto2.ToList());
        }
        public ActionResult CierresExcel(int periodo = 0, int proyectoID = 0)
        {
            ViewBag.Mensaje = string.Empty;
            ViewBag.proyecto = string.Empty;

            if (periodo == 0)
            {
                ViewBag.periodo = DateTime.Now.Year;
            }
            else
            {
                ViewBag.periodo = periodo;
            }
            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", proyectoID);

            //ViewBag.Resolucion = db.Resolucion.Where(d => d.Estado == 1).ToList();
            List<Proyecto> proyecto = db.Proyecto.OrderBy(p => p.CodCodeni).ToList();
            List<Proyecto> proyecto2 = db.Proyecto.Where(p => p.ID == 0).OrderBy(p => p.CodCodeni).ToList();

            foreach (var item in proyecto)
            {
                if (item.estaCerrado == false && item.estaEliminado == false)
                {
                    proyecto2.Add(item);
                }
            }
            if (proyectoID != 0)
            {

                ViewBag.Seleccion = proyectoID;
                proyecto = db.Proyecto.Where(p => p.Eliminado == null && p.ID == proyectoID).OrderBy(p => p.CodCodeni).ToList();
                ViewBag.proyecto = proyecto.Find(p => p.ID == proyectoID).NombreLista;
                return View(proyecto.ToList());
            }

            return View(proyecto2.ToList());
        }

        public ActionResult CierreLinea(int periodo = 0, int LineaAtencionID = 0)
        {
            ViewBag.Mensaje = string.Empty;

            if (periodo == 0)
            {
                ViewBag.periodo = DateTime.Now.Year;
                ViewBag.LineaAtencionID = 1;
                LineaAtencionID = 1;
            }
            else
            {
                ViewBag.periodo = periodo;
            }

            ViewBag.LineaAtencionID = new SelectList(db.LineasAtencion, "ID", "Nombre");
            //@ViewBag.Resolucion = db.Resolucion.Where(d => d.Estado == 1).ToList();
            List<Proyecto> proyecto = db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyecto.LineasAtencionID == LineaAtencionID).OrderBy(p => p.CodCodeni).ToList();
            return View(proyecto.ToList());
        }
        public ActionResult CerrarProyecto(int id)
        {
            if (db.Contrato.Where(c => c.ProyectoID == id).Where(c => c.Activo == "S").Count() > 0)
            {
                return RedirectToAction("Correlativos", new { state = 546 });
            }

            Proyecto proyecto = db.Proyecto.Find(id);
            proyecto.Cerrado = "S";
            db.Entry(proyecto).State = EntityState.Modified;
            db.SaveChanges();

            Persona Persona = (Persona)Session["Persona"];
            RegistroModificacionProyecto registro = new RegistroModificacionProyecto();
            registro.ProyectoID = id;
            registro.PersonaID = Persona.ID;
            registro.Descripcion = "CIERRE";
            db.RegistroModificacionProyecto.Add(registro);
            db.SaveChanges();
            utils.Log(1, "CIERRA PROYECTO / ProyectoID " + id + " PersonaID " + Persona.ID);
            return RedirectToAction("Correlativos");
        }

        public ActionResult EliminarProyecto(int id)
        {
            if (db.Contrato.Where(c => c.ProyectoID == id).Where(c => c.Activo == "S").Count() > 0)
            {
                return RedirectToAction("Correlativos", new { state = 324 });
            }

            Proyecto proyecto = db.Proyecto.Find(id);
            proyecto.Eliminado = "S";
            db.Entry(proyecto).State = EntityState.Modified;
            db.SaveChanges();

            Persona Persona = (Persona)Session["Persona"];
            RegistroModificacionProyecto registro = new RegistroModificacionProyecto();
            registro.ProyectoID = id;
            registro.PersonaID = Persona.ID;
            registro.Descripcion = "ELIMINA";
            db.RegistroModificacionProyecto.Add(registro);
            db.SaveChanges();

            utils.Log(1, "ELIMINA PROYECTO / ProyectoID " + id + " PersonaID " + Persona.ID);

            return RedirectToAction("Correlativos");
        }

        public ActionResult ReiniciarSaldo(int proyectoID, int? monto)
        {
            Proyecto Proyecto = db.Proyecto.Find(proyectoID);
            Persona Persona = (Persona)Session["Persona"];
            CuentaCorriente CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == proyectoID).Single();
            Saldo Saldo = new Saldo();

            try
            {
                Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == CuentaCorriente.ID).OrderByDescending(s => s.ID).Take(1).Single();
                if (monto != null)
                {
                    Saldo.SaldoInicialCartola = Int32.Parse(monto.ToString());
                }
                else
                {
                    Saldo.SaldoInicialCartola = 0;
                }

                Saldo.SaldoFinal = Saldo.SaldoInicialCartola;
                //Saldo.SaldoFinalCartola = Saldo.SaldoInicialCartola;
                db.Entry(Saldo).State = EntityState.Modified;
                db.SaveChanges();

                utils.Log(1, "REINICIO DE SALDO / ProyectoID " + proyectoID + " Monto " + monto + " PersonaID " + Persona.ID);
            }
            catch (Exception)
            { }

            return RedirectToAction("Correlativos");
        }



        public ActionResult ReiniciarProyecto(int proyectoID, int ingreso, int egreso, int reintegro, int deuda, int? monto)
        {
            Proyecto Proyecto = db.Proyecto.Find(proyectoID);
            CuentaCorriente CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == proyectoID).Single();
            Saldo Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == CuentaCorriente.ID).OrderByDescending(s => s.ID).Take(1).Single();
            int mes = Saldo.Mes;
            int periodo = Saldo.Periodo;


            db.Database.ExecuteSqlCommand("DELETE FROM Saldo WHERE CuentaCorrienteID = " + CuentaCorriente.ID);

            Saldo saldo = new Saldo();
            saldo.CuentaCorrienteID = CuentaCorriente.ID;
            saldo.Mes = mes;
            saldo.Periodo = periodo;
            if (monto != null)
            {
                saldo.SaldoInicialCartola = Int32.Parse(monto.ToString());
            }
            else
            {
                saldo.SaldoInicialCartola = 0;
            }
            saldo.SaldoFinal = saldo.SaldoInicialCartola;
            //saldo.SaldoFinalCartola = saldo.SaldoInicialCartola;

            db.Saldo.Add(saldo);
            db.SaveChanges();

            db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE ProyectoID = " + proyectoID);
            db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE ProyectoID = " + proyectoID);
            db.Database.ExecuteSqlCommand("UPDATE DeudaPendiente SET EgresoID = NULL WHERE ProyectoID = " + proyectoID);
            db.Database.ExecuteSqlCommand("UPDATE FondoFijoGrupo SET EgresoID = NULL, Activo = 'S' WHERE ProyectoID = " + proyectoID);
            db.Database.ExecuteSqlCommand("UPDATE FondoFijo SET EgresoID = NULL WHERE ProyectoID = " + proyectoID);
            db.Database.ExecuteSqlCommand("DELETE FROM DeudaPendiente WHERE ProyectoID = " + proyectoID);
            db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID IN (SELECT ID FROM Movimiento WHERE ProyectoID = " + proyectoID + ")");
            db.Database.ExecuteSqlCommand("DELETE FROM Movimiento WHERE ProyectoID = " + proyectoID);

            // Registramos Ingreso
            if (ingreso > 0)
            {
                try
                {
                    Movimiento Ingreso = new Movimiento();
                    Ingreso.NumeroComprobante = ingreso;
                    Ingreso.Monto_Ingresos = 0;
                    Ingreso.ProyectoID = Proyecto.ID;
                    Ingreso.CuentaCorrienteID = CuentaCorriente.ID;
                    Ingreso.Mes = mes;
                    Ingreso.Periodo = periodo;
                    Ingreso.PersonaID = null;
                    Ingreso.DetalleEgresoID = null;
                    Ingreso.ProveedorID = null;
                    Ingreso.TipoComprobanteID = ctes.tipoIngreso;
                    Ingreso.CuentaID = 1;
                    Ingreso.Saldo = 0;
                    Ingreso.Fecha = DateTime.Now;
                    Ingreso.FechaCheque = DateTime.Now;

                    db.Movimiento.Add(Ingreso);
                    db.SaveChanges();
                }
                catch (Exception)
                { }
            }

            // Registramos Egreso
            if (egreso > 0)
            {
                try
                {
                    Movimiento Egreso = new Movimiento();
                    Egreso.NumeroComprobante = egreso;
                    Egreso.Monto_Egresos = 0;
                    Egreso.ProyectoID = Proyecto.ID;
                    Egreso.CuentaCorrienteID = CuentaCorriente.ID;
                    Egreso.Mes = mes;
                    Egreso.Periodo = periodo;
                    Egreso.PersonaID = null;
                    Egreso.DetalleEgresoID = null;
                    Egreso.ProveedorID = null;
                    Egreso.TipoComprobanteID = ctes.tipoEgreso;
                    Egreso.CuentaID = 6;
                    Egreso.Saldo = 0;
                    Egreso.Fecha = DateTime.Now;
                    Egreso.FechaCheque = DateTime.Now;

                    db.Movimiento.Add(Egreso);
                    db.SaveChanges();
                }
                catch (Exception)
                { }
            }

            // Registramos Reintegro
            if (reintegro > 0)
            {
                try
                {
                    Movimiento Reintegro = new Movimiento();
                    Reintegro.NumeroComprobante = reintegro;
                    Reintegro.Monto_Egresos = 0;
                    Reintegro.ProyectoID = Proyecto.ID;
                    Reintegro.CuentaCorrienteID = CuentaCorriente.ID;
                    Reintegro.Mes = mes;
                    Reintegro.Periodo = periodo;
                    Reintegro.PersonaID = null;
                    Reintegro.DetalleEgresoID = null;
                    Reintegro.ProveedorID = null;
                    Reintegro.TipoComprobanteID = ctes.tipoReintegro;
                    Reintegro.CuentaID = 1;
                    Reintegro.Saldo = 0;
                    Reintegro.Fecha = DateTime.Now;
                    Reintegro.FechaCheque = DateTime.Now;

                    db.Movimiento.Add(Reintegro);
                    db.SaveChanges();
                }
                catch (Exception)
                { }
            }

            if (deuda > 0)
            {
                try
                {
                    DeudaPendiente deudapendiente = new DeudaPendiente();
                    deudapendiente.NumeroComprobante = deuda;
                    deudapendiente.ProyectoID = Proyecto.ID;
                    deudapendiente.CuentaID = 1;
                    deudapendiente.Mes = mes;
                    deudapendiente.Periodo = periodo;
                    deudapendiente.EgresoID = null;
                    deudapendiente.ProveedorID = null;
                    deudapendiente.Rut = null;
                    deudapendiente.DV = null;
                    deudapendiente.PersonaID = null;
                    deudapendiente.Monto = 0;
                    deudapendiente.Fecha = DateTime.Now;
                    deudapendiente.DocumentoID = 1;

                    db.DeudaPendiente.Add(deudapendiente);
                    db.SaveChanges();
                }
                catch (Exception)
                { }
            }

            Persona Persona = (Persona)Session["Persona"];
            RegistroModificacionProyecto registro = new RegistroModificacionProyecto();
            registro.ProyectoID = Proyecto.ID;
            registro.PersonaID = Persona.ID;
            registro.Descripcion = "REINICIO";
            db.RegistroModificacionProyecto.Add(registro);
            db.SaveChanges();
            utils.Log(1, "REINICIA PROYECTO / ProyectoID " + Proyecto.ID + " PersonaID " + Persona.ID);
            return RedirectToAction("Correlativos");
        }

        public ActionResult CambiarPeriodo(int? proyectoID, FormCollection form)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            if (Request.Form["CambiarPeriodo"] != null && !Request.Form["CambiarPeriodo"].ToString().Equals(""))
            {
                foreach (var key in form.AllKeys)
                {
                    if (key.Contains("Mes_") && key.Contains("_"))
                    {
                        string[] datos = key.Split('_');
                        if ("Mes".Equals(datos[0]) && datos.Count() == 2)
                        { 
                            // Limpiamos los datos
                            int movimientoID = Int32.Parse(datos[1]);
                            int mes = Int32.Parse(form[key].ToString());
                            int periodo = Int32.Parse(form["Periodo_" + movimientoID].ToString());

                            Movimiento mv = db.Movimiento.Find(movimientoID);
                            
                            // Si no hay cambios del periodo ni del mes
                            if (mes == mv.Mes && periodo == mv.Periodo)
                            {
                                continue;
                            }

                            // Obtenemos cuenta corriente del proyecto
                            CuentaCorriente cc = db.CuentaCorriente.Where(c => c.ProyectoID == mv.ProyectoID).Single();
                            int saldoInicial = 0;

                            try
                            {
                                saldoInicial = db.Saldo.Where(s => s.CuentaCorrienteID == cc.ID).Where(s => s.Periodo == periodo).Where(s => s.Mes == mes).Single().SaldoInicialCartola;
                            }
                            catch (Exception)
                            {
                                saldoInicial = 0;
                            }

                            // Si solo retrocedio el mes (mas comun)
                            if (mes < mv.Mes && periodo == mv.Periodo)
                            {
                                for (int i = mes; mes <= mv.Mes; i++)
                                {
                                    int ingresos = 0;
                                    int egresos = 0;

                                    try
                                    {
                                        ingresos = db.Movimiento.Where(a => a.ProyectoID == mv.ProyectoID && a.Periodo == periodo && a.Mes == i && ((a.TipoComprobanteID != 2 && a.CuentaID != 1)) && a.Eliminado == null && a.Nulo == null && a.Temporal == null).Sum(a => a.Monto_Ingresos);
                                    }
                                    catch (Exception)
                                    {
                                        ingresos = 0;
                                    }

                                    try
                                    {
                                        egresos = db.Movimiento.Where(a => a.ProyectoID == mv.ProyectoID && a.Periodo == periodo && a.Mes == i && (a.TipoComprobanteID == 2 && (a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Eliminado == null && a.Nulo == null && a.Temporal == null).Sum(a => a.Monto_Egresos);
                                    }
                                    catch (Exception)
                                    {
                                        egresos = 0;
                                    }

                                    int saldoFinal = saldoInicial + ingresos - egresos;

                                    Saldo Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == cc.ID && s.Periodo == periodo && s.Mes == i).Single();
                                    Saldo.SaldoFinal = saldoFinal;
                                    db.Entry(Saldo).State = EntityState.Modified;
                                    db.SaveChanges();
                                    
                                    saldoInicial = saldoFinal;
                                }

                                mv.Mes = mes;
                                db.Entry(mv).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            // Si solo retrocedio el periodo
                            if (mes == mv.Mes && periodo < mv.Periodo)
                            {
                                continue;
                            }

                            // Si retrocedio ambos
                            if (mes < mv.Mes && periodo < mv.Periodo)
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            if (proyectoID == null)
            {
                ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", Proyecto.ID);
                var movimiento = db.Movimiento.Where(a => a.ProyectoID == Proyecto.ID && a.Temporal == null && ((a.TipoComprobanteID != 2 && a.CuentaID != 1) || (a.TipoComprobanteID == 2 && (a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null)) && a.Eliminado == null).OrderBy(a => a.Proyecto.CodCodeni).ThenByDescending(a => a.Periodo).ThenByDescending(a => a.Mes).ThenByDescending(a => a.NumeroComprobante);
                return View(movimiento.ToList());
            }
            else
            {
                ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", proyectoID);
                var movimiento = db.Movimiento.Where(a => a.ProyectoID == proyectoID && a.Temporal == null && ((a.TipoComprobanteID != 2 && a.CuentaID != 1) || (a.TipoComprobanteID == 2 && (a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null)) && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.Mes).ThenByDescending(a => a.NumeroComprobante);
                return View(movimiento.ToList());
            }

            //return View(movimiento.ToList());
        }

        public ActionResult ConsolidadoCuentas()
        {
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaIngresos));
            ViewBag.Arbol += utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
            return View();
        }

        [HttpPost]
        public string GenerarConsolidado()
        {
            // var CuentaID trae las ID de las cuentas separadas por comma
            string [] cuentas = Request.Form["CuentaID"].ToString().Split(',');
            var movimientos = from m in db.Movimiento
                          where cuentas.Contains(m.CuentaID.ToString())
                          select m;

            string agrupar = Request.Form["Agrupar"].ToString();
            /*
            if (agrupar.Equals("Proyectos"))
            { 
                var proyectos = from m in movimientos
                                group m.Proyecto by m.ProyectoID into g
                                select new { ProyectoID = g.Key, Subvencion = g.Sum(g => g.mo) };
                    
            }
            */
            return Request.Form["CuentaID"];
        }

        
        public void AbrirMesAnterior(int proyectoID, int cuentaCorrienteID)
        {
            // Borrar intervencion
            Intervencion Intervencion = db.Intervencion.Where(i => i.ProyectoID == proyectoID).OrderBy(i => i.ID).Last();
            db.Entry(Intervencion).State = EntityState.Deleted;
            db.SaveChanges();

            // Borrar periodo
            Periodo Periodo = db.Periodo.Where(i => i.ProyectoID == proyectoID).OrderBy(i => i.ID).Last();
            db.Entry(Periodo).State = EntityState.Deleted;
            db.SaveChanges();

            // Borrar saldo
            Saldo Saldo = db.Saldo.Where(i => i.CuentaCorrienteID == cuentaCorrienteID).OrderBy(i => i.ID).Last();
            db.Entry(Saldo).State = EntityState.Deleted;
            db.SaveChanges();
        }

        public void RevisionProyectosCerrados()
        {
            // 5to dia habil del 2014
            int[] diasHabiles = { 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7 };
            int periodoActual = 0;
            int periodoAnterior = periodoActual - 1;
        }
    }
}
