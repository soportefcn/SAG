using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.Entity;
namespace SAG2.Controllers
{ 
    public class BodegaCuentaController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();
        //
        // GET: /BodegaCuenta/

        public ViewResult Index()
        {
            var bodegacuenta = db.BodegaCuenta.Include(b => b.Cuenta);
            return View(bodegacuenta.ToList());
        }

        //
        // GET: /BodegaCuenta/Details/5

        public ViewResult Details(int id)
        {
            BodegaCuenta bodegacuenta = db.BodegaCuenta.Find(id);
            return View(bodegacuenta);
        }

        //
        // GET: /BodegaCuenta/Create

        public ActionResult Create()
        {
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "Nombre");
            return View();
        } 

        //
        // POST: /BodegaCuenta/Create

        [HttpPost]
        public ActionResult Create(BodegaCuenta bodegacuenta)
        {
            int cuentaID = Int32.Parse(Request.Form["CuentaID"].ToString());
            if (ModelState.IsValid)
            {
                bodegacuenta.CuentaID = cuentaID;
                db.BodegaCuenta.Add(bodegacuenta);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos), cuentaID);
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "Nombre", bodegacuenta.CuentaID);
            return View(bodegacuenta);
        }
        
        //
        // GET: /BodegaCuenta/Edit/5
 
        public ActionResult Edit(int id)
        {
            BodegaCuenta bodegacuenta = db.BodegaCuenta.Find(id);
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "Nombre", bodegacuenta.CuentaID);
            return View(bodegacuenta);
        }

        //
        // POST: /BodegaCuenta/Edit/5

        [HttpPost]
        public ActionResult Edit(BodegaCuenta bodegacuenta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bodegacuenta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "Nombre", bodegacuenta.CuentaID);
            return View(bodegacuenta);
        }

        //
        // GET: /BodegaCuenta/Delete/5
 
        public ActionResult Delete(int id)
        {
            BodegaCuenta bodegacuenta = db.BodegaCuenta.Find(id);
            return View(bodegacuenta);
        }

        //
        // POST: /BodegaCuenta/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            BodegaCuenta bodegacuenta = db.BodegaCuenta.Find(id);
            db.BodegaCuenta.Remove(bodegacuenta);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}