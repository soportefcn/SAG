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
    public class ObjetivoCuentaController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /ObjetivoCuenta/

        public ViewResult Index(string q = "")
        {
            return View(db.ObjetivoCuenta.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /ObjetivoCuenta/Details/5

        public ViewResult Details(int id)
        {
            ObjetivoCuenta objetivocuenta = db.ObjetivoCuenta.Find(id);
            return View(objetivocuenta);
        }

        //
        // GET: /ObjetivoCuenta/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ObjetivoCuenta/Create

        [HttpPost]
        public ActionResult Create(ObjetivoCuenta objetivocuenta)
        {
            if (ModelState.IsValid)
            {
                db.ObjetivoCuenta.Add(objetivocuenta);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            return View(objetivocuenta);
        }
        
        //
        // GET: /ObjetivoCuenta/Edit/5
 
        public ActionResult Edit(int id)
        {
            ObjetivoCuenta objetivocuenta = db.ObjetivoCuenta.Find(id);
            return View(objetivocuenta);
        }

        //
        // POST: /ObjetivoCuenta/Edit/5

        [HttpPost]
        public ActionResult Edit(ObjetivoCuenta objetivocuenta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(objetivocuenta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(objetivocuenta);
        }

        //
        // GET: /ObjetivoCuenta/Delete/5
 /*
        public ActionResult Delete()
        {
            ObjetivoCuenta objetivocuenta = db.ObjetivoCuenta.Find(id);
            return View(objetivocuenta);
        }
*/
        //
        // POST: /ObjetivoCuenta/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ObjetivoCuenta objetivocuenta = db.ObjetivoCuenta.Find(id);
            db.ObjetivoCuenta.Remove(objetivocuenta);
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