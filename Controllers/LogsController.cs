using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.Data;

namespace SAG2.Controllers
{
    public class LogsController : Controller
    {
        private SAG2DB db = new SAG2DB();

        public ActionResult Index(int? proyectoID)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

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

        /*
        [HttpPost]
        public ActionResult Index(int? proyectoID)
        {
            if (proyectoID == null)
            {
                ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista");
                var movimiento = db.Movimiento.Where(a => a.Temporal == null && ((a.TipoComprobanteID != 2 && a.CuentaID != 1) || (a.TipoComprobanteID == 2 && (a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null)) && a.Eliminado == null).OrderBy(a => a.Proyecto.CodCodeni).ThenByDescending(a => a.Periodo).ThenByDescending(a => a.Mes).ThenByDescending(a => a.NumeroComprobante);
                return View(movimiento.ToList());
            }
            else
            {
                ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", proyectoID);
                var movimiento = db.Movimiento.Where(a => a.ProyectoID == proyectoID && a.Temporal == null && ((a.TipoComprobanteID != 2 && a.CuentaID != 1) || (a.TipoComprobanteID == 2 && (a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null)) && a.Eliminado == null).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.Mes).ThenByDescending(a => a.NumeroComprobante);
                return View(movimiento.ToList());
            }
        }
        */

        [HttpGet]
        public ActionResult Desconciliar(int ID)
        {
            int? proyectoID = null;

            try
            {
                db.Database.ExecuteSqlCommand("UPDATE DetalleEgreso SET Conciliado = NULL WHERE MovimientoID  = " + ID);
                db.Database.ExecuteSqlCommand("UPDATE Movimiento SET Conciliado = NULL WHERE ID = " + ID);
                Movimiento Movimiento = db.Movimiento.Find(ID);
                proyectoID = Movimiento.ProyectoID;
            }
            catch (Exception)
            { }

            return RedirectToAction("Index", "Logs", new { proyectoID = proyectoID });
        }

        [HttpGet]
        public ActionResult Conciliar(int detalleEgresoID = 0, int movimientoID = 0)
        {
            int proyectoID = 0;
            try
            {
                if (movimientoID > 0)
                {
                    // Conciliacion de Ingreso o Reintegro
                    Movimiento movimiento = db.Movimiento.Find(movimientoID);
                    proyectoID = movimiento.ProyectoID;
                    int periodo = movimiento.Periodo;
                    int mes = movimiento.Mes;
                    // Si el movimiento no ha sido conciliado, seguimos.
                    if (movimiento.Conciliado == null)
                    {
                        movimiento.Conciliado = "S";
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();

                        ConciliacionRegistro cr = new ConciliacionRegistro();
                        cr.MovimientoID = movimiento.ID;
                        cr.Periodo = periodo;
                        cr.Mes = mes;
                        db.ConciliacionRegistro.Add(cr);
                        db.SaveChanges();
                    }
                    else
                    {
                        movimiento.Conciliado = null;
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();

                        try
                        {
                            ConciliacionRegistro cr = db.ConciliacionRegistro.Where(c => c.MovimientoID == movimiento.ID).Single();
                            db.Entry(cr).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                        catch (Exception)
                        { }
                    }
                }

                if (detalleEgresoID > 0)
                {
                    // Conciliacion de Egreso
                    DetalleEgreso detalle = db.DetalleEgreso.Find(detalleEgresoID);
                    proyectoID = detalle.Egreso.ProyectoID;
                    int periodo = detalle.Egreso.Periodo;
                    int mes = detalle.Egreso.Mes;
                    // Si el movimiento no ha sido conciliado, seguimos.
                    if (detalle.Conciliado == null)
                    {
                        detalle.Conciliado = "S";
                        db.Entry(detalle).State = EntityState.Modified;
                        db.SaveChanges();

                        ConciliacionRegistro cr = new ConciliacionRegistro();
                        cr.DetalleID = detalle.ID;
                        cr.Periodo = periodo;
                        cr.Mes = mes;
                        db.ConciliacionRegistro.Add(cr);
                        db.SaveChanges();
                    }
                    else
                    {
                        detalle.Conciliado = null;
                        db.Entry(detalle).State = EntityState.Modified;
                        db.SaveChanges();

                        try
                        {
                            ConciliacionRegistro cr = db.ConciliacionRegistro.Where(c => c.DetalleID == detalle.ID).Single();
                            db.Entry(cr).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                        catch (Exception)
                        { }
                    }

                    int cantidad = db.DetalleEgreso.Where(d => d.MovimientoID == detalle.MovimientoID).Where(d => d.Conciliado == null).Where(m => m.Temporal == null).Count();

                    // Si se conciliaron todos los detalles, se concilia el Egreso
                    if (cantidad == 0)
                    {
                        Movimiento movimiento = db.Movimiento.Find(detalle.MovimientoID);
                        movimiento.Conciliado = "S";
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        Movimiento movimiento = db.Movimiento.Find(detalle.MovimientoID);
                        movimiento.Conciliado = null;
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            { }

            return RedirectToAction("Index", "Logs", new { proyectoID = proyectoID });
        }
    }
}