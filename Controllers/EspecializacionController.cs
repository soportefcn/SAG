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
    public class EspecializacionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Especializacion/

        public ViewResult Index(string q = "")
        {
            var especializacion = db.Especializacion.Include(e => e.Profesion).Where(a => a.Nombre.Contains(q));
            return View(especializacion.ToList());
        }

        //
        // GET: /Especializacion/Details/5

        public ViewResult Details(int id)
        {
            Especializacion especializacion = db.Especializacion.Find(id);
            return View(especializacion);
        }

        //
        // GET: /Especializacion/Create

        public ActionResult Create()
        {
            ViewBag.ProfesionID = new SelectList(db.Profesion, "ID", "Nombre");
            return View();
        } 

        //
        // POST: /Especializacion/Create

        [HttpPost]
        public ActionResult Create(Especializacion especializacion)
        {
            if (ModelState.IsValid)
            {
                db.Especializacion.Add(especializacion);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            ViewBag.ProfesionID = new SelectList(db.Profesion, "ID", "Nombre", especializacion.ProfesionID);
            return View(especializacion);
        }
        
        //
        // GET: /Especializacion/Edit/5
 
        public ActionResult Edit(int id)
        {
            Especializacion especializacion = db.Especializacion.Find(id);
            ViewBag.ProfesionID = new SelectList(db.Profesion, "ID", "Nombre", especializacion.ProfesionID);
            return View(especializacion);
        }

        //
        // POST: /Especializacion/Edit/5

        [HttpPost]
        public ActionResult Edit(Especializacion especializacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(especializacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            ViewBag.ProfesionID = new SelectList(db.Profesion, "ID", "Nombre", especializacion.ProfesionID);
            return View(especializacion);
        }

        //
        // GET: /Especializacion/Delete/5
        /*
        public ActionResult Delete()
        {
            Especializacion especializacion = db.Especializacion.Find(id);
            return View(especializacion);
        }
        */
        //
        // POST: /Especializacion/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Especializacion especializacion = db.Especializacion.Find(id);
            db.Especializacion.Remove(especializacion);
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