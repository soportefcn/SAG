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
    public class DocumentoController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Documento/

        public ViewResult Index(string q = "")
        {
            return View(db.Documento.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /Documento/Details/5

        public ViewResult Details(int id)
        {
            Documento documento = db.Documento.Find(id);
            return View(documento);
        }

        //
        // GET: /Documento/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Documento/Create

        [HttpPost]
        public ActionResult Create(Documento documento)
        {
            if (ModelState.IsValid)
            {
                db.Documento.Add(documento);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + documento.Nombre;
                return RedirectToAction("Create");  
            }

            return View(documento);
        }
        
        //
        // GET: /Documento/Edit/5
 
        public ActionResult Edit(int id)
        {
            Documento documento = db.Documento.Find(id);
            return View(documento);
        }

        //
        // POST: /Documento/Edit/5

        [HttpPost]
        public ActionResult Edit(Documento documento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(documento).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + documento.Nombre;
                return RedirectToAction("Create");
            }
            return View(documento);
        }

        //
        // GET: /Documento/Delete/5
 /*
        public ActionResult Delete()
        {
            Documento documento = db.Documento.Find(id);
            return View(documento);
        }
        */
        //
        // POST: /Documento/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Documento documento = db.Documento.Find(id);
            db.Documento.Remove(documento);
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