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
    public class TipoOrigenAdquisicionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /TipoOrigenAdquisicion/

        public ViewResult Index(string q = "")
        {
            return View(db.TipoOrigenAdquisicion.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /TipoOrigenAdquisicion/Details/5

        public ViewResult Details(int id)
        {
            TipoOrigenAdquisicion tipoorigenadquisicion = db.TipoOrigenAdquisicion.Find(id);
            return View(tipoorigenadquisicion);
        }

        //
        // GET: /TipoOrigenAdquisicion/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /TipoOrigenAdquisicion/Create

        [HttpPost]
        public ActionResult Create(TipoOrigenAdquisicion tipoorigenadquisicion)
        {
            if (ModelState.IsValid)
            {
                db.TipoOrigenAdquisicion.Add(tipoorigenadquisicion);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + tipoorigenadquisicion.Nombre;
                return RedirectToAction("Create");  
            }

            return View(tipoorigenadquisicion);
        }
        
        //
        // GET: /TipoOrigenAdquisicion/Edit/5
 
        public ActionResult Edit(int id)
        {
            TipoOrigenAdquisicion tipoorigenadquisicion = db.TipoOrigenAdquisicion.Find(id);
            return View(tipoorigenadquisicion);
        }

        //
        // POST: /TipoOrigenAdquisicion/Edit/5

        [HttpPost]
        public ActionResult Edit(TipoOrigenAdquisicion tipoorigenadquisicion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoorigenadquisicion).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + tipoorigenadquisicion.Nombre;
                return RedirectToAction("Create");
            }
            return View(tipoorigenadquisicion);
        }

        //
        // GET: /TipoOrigenAdquisicion/Delete/5
        /*
        public ActionResult Delete()
        {
            TipoOrigenAdquisicion tipoorigenadquisicion = db.TipoOrigenAdquisicion.Find(id);
            return View(tipoorigenadquisicion);
        }
        */
        //
        // POST: /TipoOrigenAdquisicion/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            TipoOrigenAdquisicion tipoorigenadquisicion = db.TipoOrigenAdquisicion.Find(id);
            db.TipoOrigenAdquisicion.Remove(tipoorigenadquisicion);
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