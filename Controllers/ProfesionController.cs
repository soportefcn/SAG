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
    public class ProfesionController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();

        //
        // GET: /Profesion/

        public ViewResult Index(string q = "")
        {
            return View(db.Profesion.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /Profesion/Details/5

        public ViewResult Details(int id)
        {
            Profesion profesion = db.Profesion.Find(id);
            return View(profesion);
        }

        //
        // GET: /Profesion/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Profesion/Create

        [HttpPost]
        public ActionResult Create(Profesion profesion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    profesion.Nombre = utils.ToTitle(profesion.Nombre);
                    db.Profesion.Add(profesion);
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + profesion.Nombre;
                    return RedirectToAction("Create");
                }
            }
            catch (Exception)
            {
                @ViewBag.Mensaje = utils.mensajeError("La profesión ingresada ya existe.");
            }
            return View(profesion);
        }
        
        //
        // GET: /Profesion/Edit/5
 
        public ActionResult Edit(int id)
        {
            Profesion profesion = db.Profesion.Find(id);
            profesion.Nombre = utils.ToTitle(profesion.Nombre);
            return View(profesion);
        }

        //
        // POST: /Profesion/Edit/5

        [HttpPost]
        public ActionResult Edit(Profesion profesion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profesion).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + profesion.Nombre;
                return RedirectToAction("Create");
            }
            return View(profesion);
        }

        //
        // GET: /Profesion/Delete/5
        /*
        public ActionResult Delete()
        {
            Profesion profesion = db.Profesion.Find(id);
            return View(profesion);
        }
        */
        //
        // POST: /Profesion/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {      
            try
            {
                Profesion profesion = db.Profesion.Find(id);
                db.Profesion.Remove(profesion);
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