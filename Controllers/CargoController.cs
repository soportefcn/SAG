using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Globalization;

namespace SAG2.Controllers
{ 
    public class CargoController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();

        //
        // GET: /Cargo/

        public ViewResult Index(string q = "")
        {
            return View(db.Cargo.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /Cargo/Details/5

        public ViewResult Details(int id)
        {
            Cargo cargo = db.Cargo.Find(id);
            return View(cargo);
        }

        //
        // GET: /Cargo/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Cargo/Create

        [HttpPost]
        public ActionResult Create(Cargo cargo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cargo.Nombre = utils.ToTitle(cargo.Nombre);
                    db.Cargo.Add(cargo);
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + cargo.Nombre;
                    return RedirectToAction("Create");
                }
            }
            catch (Exception)
            {
                @ViewBag.Mensaje = utils.mensajeError("El cargo ingresado ya existe.");
            }
            return View(cargo);
        }
        
        //
        // GET: /Cargo/Edit/5
 
        public ActionResult Edit(int id)
        {
            Cargo cargo = db.Cargo.Find(id);
            cargo.Nombre = utils.ToTitle(cargo.Nombre);
            return View(cargo);
        }

        //
        // POST: /Cargo/Edit/5

        [HttpPost]
        public ActionResult Edit(Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cargo).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + cargo.Nombre;
                return RedirectToAction("Create");
            }
            return View(cargo);
        }

        //
        // GET: /Cargo/Delete/5
        /*
        public ActionResult Delete()
        {
            Cargo cargo = db.Cargo.Find(id);
            return View(cargo);
        }
        */
        //
        // POST: /Cargo/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Cargo cargo = db.Cargo.Find(id);
                db.Cargo.Remove(cargo);
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            catch (Exception)
            { }

            return RedirectToAction("Edit", new { @id = id });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}