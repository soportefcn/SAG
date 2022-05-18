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
    public class PaisController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Pais/

        public ViewResult Index(string q = "")
        {
            return View(db.Pais.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /Pais/Details/5

        public ViewResult Details(int id)
        {
            Pais pais = db.Pais.Find(id);
            return View(pais);
        }

        //
        // GET: /Pais/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Pais/Create

        [HttpPost]
        public ActionResult Create(Pais pais)
        {
            if (ModelState.IsValid)
            {
                db.Pais.Add(pais);
                db.SaveChanges();
                TempData["Message"] = "Grabado con exito " + pais.Nombre;
                return RedirectToAction("Create");  
            }

            return View(pais);
        }
        
        //
        // GET: /Pais/Edit/5
 
        public ActionResult Edit(int id)
        {
            Pais pais = db.Pais.Find(id);
            return View(pais);
        }

        //
        // POST: /Pais/Edit/5

        [HttpPost]
        public ActionResult Edit(Pais pais)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pais).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Grabado con exito " + pais.Nombre;
                return RedirectToAction("Create");
            }
            return View(pais);
        }

        //
        // GET: /Pais/Delete/5
        /*
        public ActionResult Delete()
        {
            Pais pais = db.Pais.Find(id);
            return View(pais);
        }
        */
        //
        // POST: /Pais/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Pais pais = db.Pais.Find(id);
            db.Pais.Remove(pais);
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