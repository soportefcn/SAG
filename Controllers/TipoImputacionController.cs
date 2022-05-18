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
    public class TipoImputacionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /TipoImputacion/

        public ViewResult Index(string q = "")
        {
            return View(db.TipoImputacion.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /TipoImputacion/Details/5

        public ViewResult Details(int id)
        {
            TipoImputacion tipoimputacion = db.TipoImputacion.Find(id);
            return View(tipoimputacion);
        }

        //
        // GET: /TipoImputacion/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /TipoImputacion/Create

        [HttpPost]
        public ActionResult Create(TipoImputacion tipoimputacion)
        {
            if (ModelState.IsValid)
            {
                db.TipoImputacion.Add(tipoimputacion);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + tipoimputacion.Nombre;
                return RedirectToAction("Create");  
            }

            return View(tipoimputacion);
        }
        
        //
        // GET: /TipoImputacion/Edit/5
 
        public ActionResult Edit(int id)
        {
            TipoImputacion tipoimputacion = db.TipoImputacion.Find(id);
            return View(tipoimputacion);
        }

        //
        // POST: /TipoImputacion/Edit/5

        [HttpPost]
        public ActionResult Edit(TipoImputacion tipoimputacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoimputacion).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + tipoimputacion.Nombre;
                return RedirectToAction("Create");
            }
            return View(tipoimputacion);
        }

        //
        // GET: /TipoImputacion/Delete/5
        /*
        public ActionResult Delete()
        {
            TipoImputacion tipoimputacion = db.TipoImputacion.Find(id);
            return View(tipoimputacion);
        }
        */
        //
        // POST: /TipoImputacion/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            TipoImputacion tipoimputacion = db.TipoImputacion.Find(id);
            db.TipoImputacion.Remove(tipoimputacion);
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