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
    public class ServiciosController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Servicios/

        public ViewResult Index()
        {
            return View(db.Servicio.ToList());
        }

        //
        // GET: /Servicios/Details/5

        public ViewResult Details(int id)
        {
            Servicio servicio = db.Servicio.Find(id);
            return View(servicio);
        }

        //
        // GET: /Servicios/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Servicios/Create

        [HttpPost]
        public ActionResult Create(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                db.Servicio.Add(servicio);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + servicio.Nombre;
                return RedirectToAction("Create");  
            }

            return View(servicio);
        }
        
        //
        // GET: /Servicios/Edit/5
 
        public ActionResult Edit(int id)
        {
            Servicio servicio = db.Servicio.Find(id);
            return View(servicio);
        }

        //
        // POST: /Servicios/Edit/5

        [HttpPost]
        public ActionResult Edit(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(servicio).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + servicio.Nombre;
                return RedirectToAction("create");
            }
            return View(servicio);
        }

        //
        // GET: /Servicios/Delete/5
 
  

        //
        // POST: /Servicios/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Servicio servicio = db.Servicio.Find(id);
            db.Servicio.Remove(servicio);
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