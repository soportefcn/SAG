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
    public class LineasAtencionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /LineasAtencion/

        public ViewResult Index(string q = "")
        {
            return View(db.LineasAtencion.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /LineasAtencion/Details/5

        public ViewResult Details(int id)
        {
            LineasAtencion lineasatencion = db.LineasAtencion.Find(id);
            return View(lineasatencion);
        }

        //
        // GET: /LineasAtencion/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /LineasAtencion/Create

        [HttpPost]
        public ActionResult Create(LineasAtencion lineasatencion)
        {
            if (ModelState.IsValid)
            {
                db.LineasAtencion.Add(lineasatencion);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + lineasatencion.Nombre;
                return RedirectToAction("Create");  
            }

            return View(lineasatencion);
        }
        
        //
        // GET: /LineasAtencion/Edit/5
 
        public ActionResult Edit(int id)
        {
            LineasAtencion lineasatencion = db.LineasAtencion.Find(id);
            return View(lineasatencion);
        }

        //
        // POST: /LineasAtencion/Edit/5

        [HttpPost]
        public ActionResult Edit(LineasAtencion lineasatencion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lineasatencion).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + lineasatencion.Nombre;
                return RedirectToAction("Create");
            }
            return View(lineasatencion);
        }

        //
        // GET: /LineasAtencion/Delete/5
        /*
        public ActionResult Delete()
        {
            LineasAtencion lineasatencion = db.LineasAtencion.Find(id);
            return View(lineasatencion);
        }
        */
        //
        // POST: /LineasAtencion/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            LineasAtencion lineasatencion = db.LineasAtencion.Find(id);
            db.LineasAtencion.Remove(lineasatencion);
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