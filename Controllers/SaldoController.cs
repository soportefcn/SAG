using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;

namespace SAG2.Controllers
{ 
    public class SaldoController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util util = new Util();
        private int TipoEgreso = 2;

        //
        // GET: /Saldo/

        public ViewResult Index(string q = "")
        {
            Proyecto Establecimiento = (Proyecto) Session["Proyecto"];
            List<Saldo> saldos = db.Saldo.Include(s => s.CuentaCorriente).Where(s => s.Mes == 1).Where(s => s.CuentaCorriente.ProyectoID == Establecimiento.ID).ToList();
            List<Saldo> saldos_tmp = new List<Saldo>();

            foreach (Saldo Saldo in saldos)
            {
                Saldo.SaldoFinal = saldoFinalPeriodo(Saldo);
                saldos_tmp.Add(Saldo);
            }

            return View(saldos_tmp);
        }

        //
        // GET: /Saldo/Details/5

        public ViewResult Details(int id)
        {
            Saldo saldo = db.Saldo.Find(id);
            return View(saldo);
        }

        //
        // GET: /Saldo/Create

        public ActionResult Create()
        {
            Proyecto Establecimiento = (Proyecto)Session["Proyecto"];
            ViewBag.CuentaCorrienteID = new SelectList(db.CuentaCorriente.Where(s => s.ProyectoID == Establecimiento.ID), "ID", "Numero");
            return View();
        } 

        //
        // POST: /Saldo/Create

        [HttpPost]
        public ActionResult Create(Saldo saldo)
        {
            // Se actualizan los saldos de acuerdo al periodo ingresado
            int saldoInicialSiguientePeriodo = 0;
            bool valido = true;
            Proyecto Establecimiento = (Proyecto)Session["Proyecto"];
            ViewBag.CuentaCorrienteID = new SelectList(db.CuentaCorriente.Where(c => c.ProyectoID == Establecimiento.ID), "ID", "Numero", saldo.CuentaCorrienteID);

            // Verificamos existencia del saldo
            try
            {
                Saldo verificaSaldo = db.Saldo.
                                            Where(s => s.Mes == 1).
                                            Where(s => s.Periodo == saldo.Periodo).
                                            Where(s => s.CuentaCorrienteID == saldo.CuentaCorrienteID).
                                            Single();
                verificaSaldo.SaldoInicialCartola = saldo.SaldoInicialCartola;
                return Edit(verificaSaldo);
            } 
            catch (Exception e) 
            {
                util.Log(2, "El saldo ingresado no existe se continua la ejecucion de forma normal. " + e.Message);
                if (ModelState.IsValid)
                {
                    try
                    {
                        saldo.Mes = 1;
                        saldo.SaldoFinal = actualizarMovimientos(saldo);
                        saldoInicialSiguientePeriodo = saldo.SaldoFinal;
                        db.Saldo.Add(saldo);
                        db.SaveChanges();
                    } 
                    catch (Exception ex)
                    {
                        util.Log(2, "Ha ocurrido un error al intentar actualizar el saldo. Mes 1 " + ex.Message);
                        util.erroresState(ModelState);
                        ViewBag.Mensaje = util.mensajeError("Ha ocurrido un error al intentar actualizar el saldo. " + e.InnerException.StackTrace);
                        return View(saldo);
                    }
                }
                else
                {
                    util.Log(2, "Model state no valido. ");
                    valido = false;
                }

                for (int i = 2; i < 13; i++)
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            Saldo saldoSiguiente = new Saldo();
                            saldoSiguiente.CuentaCorrienteID = saldo.CuentaCorrienteID;
                            saldoSiguiente.Periodo = saldo.Periodo;
                            saldoSiguiente.Mes = i;
                            saldoSiguiente.SaldoInicialCartola = saldoInicialSiguientePeriodo;
                            saldoSiguiente.SaldoFinal = actualizarMovimientos(saldoSiguiente);
                            saldoInicialSiguientePeriodo = saldoSiguiente.SaldoFinal;
                            db.Saldo.Add(saldoSiguiente);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            util.Log(2, "Ha ocurrido un error al intentar actualizar el saldo. Mes " + i + " " + ex.InnerException.StackTrace);
                            util.erroresState(ModelState);
                            ViewBag.Mensaje = util.mensajeError("Ha ocurrido un error al intentar actualizar el saldo. " + ex.Message);
                            return View(saldo);
                        }
                    }
                    else
                    {
                        valido = false;
                    }
                }

                if (valido)
                {
                    return RedirectToAction("Create");
                }

            }
            return View(saldo);
        }
        
        //
        // GET: /Saldo/Edit/5
 
        public ActionResult Edit(int id)
        {
            Saldo saldo = db.Saldo.Find(id);
            Proyecto Establecimiento = (Proyecto)Session["Proyecto"];
            ViewBag.CuentaCorrienteID = new SelectList(db.CuentaCorriente.Where(c => c.ProyectoID == Establecimiento.ID), "ID", "Numero", saldo.CuentaCorrienteID);
            return View(saldo);
        }

        //
        // POST: /Saldo/Edit/5

        [HttpPost]
        public ActionResult Edit(Saldo saldo)
        {
            // Se actualizan los saldos de acuerdo al periodo ingresado
            int saldoInicialSiguientePeriodo = 0;
            bool valido = true;
            int periodo = saldo.Periodo;
            int CuentaCorrienteID = saldo.CuentaCorrienteID;
            Proyecto Establecimiento = (Proyecto)Session["Proyecto"];
            ViewBag.CuentaCorrienteID = new SelectList(db.CuentaCorriente.Where(c => c.ProyectoID == Establecimiento.ID), "ID", "Numero", saldo.CuentaCorrienteID);

            if (ModelState.IsValid)
            {
                try
                {
                    saldo.Mes = 1;
                    saldo.SaldoFinal = actualizarMovimientos(saldo);
                    saldoInicialSiguientePeriodo = saldo.SaldoFinal;
                    db.Entry(saldo).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    util.Log(2, "Ha ocurrido un error al intentar actualizar el saldo. " + e.Message);
                    util.erroresState(ModelState);
                    ViewBag.Mensaje = util.mensajeError("Ha ocurrido un error al intentar actualizar el saldo. " + e.Message);
                    ViewBag.CuentaCorrienteID = new SelectList(db.CuentaCorriente, "ID", "Numero", saldo.CuentaCorrienteID);
                    return View(saldo);
                }
            }
            else
            {
                util.Log(2, "Model state no valido. ");
                valido = false;
            }

            for (int i = 2; i < 13; i++)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        Saldo saldoSiguiente = db.Saldo.Where(s => s.Periodo == periodo).Where(s => s.Mes == i).Where(s => s.CuentaCorrienteID == CuentaCorrienteID).Single();
                        saldoSiguiente.SaldoInicialCartola = saldoInicialSiguientePeriodo;
                        saldoSiguiente.SaldoFinal = actualizarMovimientos(saldoSiguiente);
                        saldoInicialSiguientePeriodo = saldoSiguiente.SaldoFinal;
                        db.Entry(saldoSiguiente).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        util.Log(2, "Ha ocurrido un error al intentar actualizar el saldo. " + i + ". " + e.Message);
                        util.erroresState(ModelState);
                        ViewBag.Mensaje = util.mensajeError("Ha ocurrido un error al intentar actualizar el saldo. " + i + ". " + e.Message);
                        ViewBag.CuentaCorrienteID = new SelectList(db.CuentaCorriente, "ID", "Numero", saldo.CuentaCorrienteID);
                        return View(saldo);
                    }
                }
                else
                {
                    valido = false;
                    util.Log(2, "Model state no valido. ");
                }
            }

            if (valido)
            {
                return RedirectToAction("Create");
            }

            return View(saldo);
        }

        //
        // GET: /Saldo/Delete/5
        /*
        public ActionResult Delete()
        {
            Saldo saldo = db.Saldo.Find(id);
            return View(saldo);
        }
        */
        //
        // POST: /Saldo/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Saldo saldo = db.Saldo.Find(id);
            db.Saldo.Remove(saldo);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private int actualizarMovimientos(Saldo saldo)
        {
            int SaldoInicialPeriodo = saldo.SaldoInicialCartola;
            // Obtener movimientos del periodo
            List<Movimiento> Movimientos = (from m in db.Movimiento
                                            where m.Periodo == saldo.Periodo && m.Mes == saldo.Mes && m.CuentaCorrienteID == saldo.CuentaCorrienteID
                                            orderby m.ID
                                            select m).ToList();

            foreach (Movimiento Movimiento in Movimientos)
            {
                if (Movimiento.TipoComprobanteID == TipoEgreso)
                {
                    // Egreso resta movimiento
                    Movimiento.Saldo = SaldoInicialPeriodo - Movimiento.Monto_Egresos;
                    SaldoInicialPeriodo -= Movimiento.Monto_Egresos;
                }
                else
                {
                    // Ingreso o reintegro suma movimiento
                    Movimiento.Saldo = SaldoInicialPeriodo + Movimiento.Monto_Ingresos;
                    SaldoInicialPeriodo += Movimiento.Monto_Ingresos;
                }

                db.Entry(Movimiento).State = EntityState.Modified;
                db.SaveChanges();
            }

            return SaldoInicialPeriodo;
        }

        public int saldoFinalPeriodo(Saldo saldo)
        {
            return db.Saldo.Where(s => s.Mes == 12).Where(s => s.Periodo == saldo.Periodo).Where(s => s.CuentaCorrienteID == saldo.CuentaCorrienteID).Select(s => s.SaldoFinal).Single();
        }
    }
}