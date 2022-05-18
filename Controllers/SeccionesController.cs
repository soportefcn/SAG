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
    public class SeccionesController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        //
        // GET: /Secciones/

        public ViewResult Index(string q = "")
        {
            return View(db.Seccion.Where(a => a.Nombre.Contains(q)).OrderBy(s => s.Menu).OrderBy(s => s.Orden).ToList());
        }

        //
        // GET: /Secciones/Details/5

        public ViewResult Details(int id)
        {
            Seccion seccion = db.Seccion.Find(id);
            return View(seccion);
        }

        //
        // GET: /Secciones/Create

        public ActionResult Create()
        {
          //  ViewBag.PadreID = utils.selectSecciones(db.Seccion.Where(s => s.PadreID == null).Single());
            return View();
        } 

        //
        // POST: /Secciones/Create

        [HttpPost]
        public ActionResult Create(Seccion seccion)
        {
            if (ModelState.IsValid)
            {
                db.Seccion.Add(seccion);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }


            ViewBag.PadreID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista"); return View(seccion);
        }
        
        //
        // GET: /Secciones/Edit/5
 
        public ActionResult Edit(int id)
        {
            Seccion seccion = db.Seccion.Find(id);
            return View(seccion);
        }

        //
        // POST: /Secciones/Edit/5

        [HttpPost]
        public ActionResult Edit(Seccion seccion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(seccion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(seccion);
        }

        //
        // GET: /Secciones/Delete/5
 
        public ActionResult Delete(int id)
        {
            Seccion seccion = db.Seccion.Find(id);
            return View(seccion);
        }

        //
        // POST: /Secciones/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Seccion seccion = db.Seccion.Find(id);
            db.Seccion.Remove(seccion);
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