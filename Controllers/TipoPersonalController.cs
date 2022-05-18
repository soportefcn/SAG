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
    public class TipoPersonalController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /TipoPersonal/

        public ViewResult Index(string q = "")
        {
            return View(db.TipoPersonal.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /TipoPersonal/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /TipoPersonal/Create

        [HttpPost]
        public ActionResult Create(TipoPersonal tipopersonal)
        {
            if (ModelState.IsValid)
            {
                db.TipoPersonal.Add(tipopersonal);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + tipopersonal.Nombre;
                return RedirectToAction("Create");  
            }

            return View(tipopersonal);
        }
        
        //
        // GET: /TipoPersonal/Edit/5
 
        public ActionResult Edit(int id)
        {
            TipoPersonal tipopersonal = db.TipoPersonal.Find(id);
            return View(tipopersonal);
        }

        //
        // POST: /TipoPersonal/Edit/5

        [HttpPost]
        public ActionResult Edit(TipoPersonal tipopersonal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipopersonal).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + tipopersonal.Nombre;
                return RedirectToAction("Create");
            }
            return View(tipopersonal);
        }

        //
        // GET: /TipoPersonal/Delete/5
        /*
        public ActionResult Delete()
        {
            TipoPersonal tipopersonal = db.TipoPersonal.Find(id);
            return View(tipopersonal);
        }
        */
        //
        // POST: /TipoPersonal/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            TipoPersonal tipopersonal = db.TipoPersonal.Find(id);
            db.TipoPersonal.Remove(tipopersonal);
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