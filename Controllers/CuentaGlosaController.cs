using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using SAG2.Comun; 

namespace SAG2.Controllers
{ 
    public class CuentaGlosaController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        //
        // GET: /CuentaGlosa/

        public ViewResult Index()
        {
            return View(db.CuentaGlosa.ToList());
        }

        //
        // GET: /CuentaGlosa/Details/5

        public ViewResult Details(int id)
        {
            CuentaGlosa cuentaglosa = db.CuentaGlosa.Find(id);
            return View(cuentaglosa);
        }

        //
        // GET: /CuentaGlosa/Create

        public ActionResult Create()
        {
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "NombreLista");
            return View();
        } 

        //
        // POST: /CuentaGlosa/Create

        [HttpPost]
        public ActionResult Create(CuentaGlosa cuentaglosa)
        {
            if (ModelState.IsValid)
            {
                db.CuentaGlosa.Add(cuentaglosa);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(cuentaglosa);
        }
        
        //
        // GET: /CuentaGlosa/Edit/5
 
        public ActionResult Edit(int id)
        {
            CuentaGlosa cuentaglosa = db.CuentaGlosa.Find(id);
            return View(cuentaglosa);
        }

        //
        // POST: /CuentaGlosa/Edit/5

        [HttpPost]
        public ActionResult Edit(CuentaGlosa cuentaglosa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentaglosa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuentaglosa);
        }

        //
        // GET: /CuentaGlosa/Delete/5
 
        public ActionResult Delete(int id)
        {
            CuentaGlosa cuentaglosa = db.CuentaGlosa.Find(id);
            return View(cuentaglosa);
        }

        //
        // POST: /CuentaGlosa/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            CuentaGlosa cuentaglosa = db.CuentaGlosa.Find(id);
            db.CuentaGlosa.Remove(cuentaglosa);
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