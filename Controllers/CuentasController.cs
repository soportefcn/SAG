using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;

namespace SAG2.Controllers
{ 
    public class CuentasController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Cuentas/

        public ViewResult Index(string q = "")
        {
            var cuenta = db.Cuenta.Include(c => c.Padre).Where(c => !c.Codigo.Equals("0")).Where(c => c.Nombre.Contains(q)).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        //
        // GET: /Cuentas/Details/5

        public ViewResult Details(int id)
        {
            Cuenta cuenta = db.Cuenta.Find(id);
            return View(cuenta);
        }

        //
        // GET: /Cuentas/Create

        public ActionResult Create()
        {
            ViewBag.CuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista");
            ViewBag.SenameUso = new SelectList(db.SenameUso, "SenameUsoID ", "Descripcion");
            return View();
        }

        public ActionResult Subir(int id)
        {
            Cuenta cuenta = db.Cuenta.Find(id);
            int orden = cuenta.Orden;
            int orden_anterior = orden - 1;

            try
            {
                Cuenta cuenta_anterior = db.Cuenta.Where(c => c.Orden == orden_anterior).Single();
                // Aumentamos en 1 el orden de la cuenta anterior (Bajamos)
                cuenta_anterior.Orden = cuenta_anterior.Orden + 1;
                db.Entry(cuenta_anterior).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                //return RedirectToAction("Index");
            }

            // Disminuimos en 1 el orden de la cuenta anterior (Subimos)
            if (orden_anterior != 0)
            {
                cuenta.Orden = cuenta.Orden - 1;
                db.Entry(cuenta).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Bajar(int id)
        {
            Cuenta cuenta = db.Cuenta.Find(id);
            int orden = cuenta.Orden;
            int orden_siguiente = orden + 1;

            try
            {
                Cuenta cuenta_siguiente = db.Cuenta.Where(c => c.Orden == orden_siguiente).Single();
                cuenta_siguiente.Orden = cuenta_siguiente.Orden - 1;
                db.Entry(cuenta_siguiente).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                //return RedirectToAction("Index");
            }

            cuenta.Orden = cuenta.Orden + 1;
            db.Entry(cuenta).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // POST: /Cuentas/Create

        [HttpPost]
        public ActionResult Create(Cuenta cuenta)
        {
            cuenta.Orden = 0;
            
            try
            {
                Cuenta padre = db.Cuenta.Find(cuenta.CuentaID);
                if (padre.ID == 1)
                {
                    cuenta.Tipo = "I";
                }
                else if (padre.ID == 6)
                {
                    cuenta.Tipo = "E";
                }
                else
                {
                    cuenta.Tipo = padre.Tipo;
                }
            }
            catch (Exception)
            { }

            if (ModelState.IsValid)
            {
                cuenta.Estado = 1;
                db.Cuenta.Add(cuenta);
                
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + cuenta.Descripcion;
                return RedirectToAction("Create");  
            }

            ViewBag.CuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista", cuenta.CuentaID);
            return View(cuenta);
        }
        
        //
        // GET: /Cuentas/Edit/5
 
        public ActionResult Edit(int id)
        {
            Cuenta cuenta = db.Cuenta.Find(id);
            ViewBag.CuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista", cuenta.CuentaID);
            ViewBag.SenameUso = new SelectList(db.SenameUso, "SenameUsoID ", "Descripcion");
            return View(cuenta);
        }

        //
        // POST: /Cuentas/Edit/5

        [HttpPost]
        public ActionResult Edit(Cuenta cuenta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuenta).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + cuenta.Descripcion;
                return RedirectToAction("Create");
            }
            ViewBag.CuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista", cuenta.CuentaID);
            return View(cuenta);
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Cuenta cuenta = db.Cuenta.Find(id);
            db.Cuenta.Remove(cuenta);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}