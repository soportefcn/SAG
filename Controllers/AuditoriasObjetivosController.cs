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
    public class AuditoriasObjetivosController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /AuditoriasObjetivos/

        public ViewResult Index()
        {
            return View(db.AuditoriasObjetivo.ToList());
        }

        //
        // GET: /AuditoriasObjetivos/Details/5

        public ViewResult Details(int id)
        {
            AuditoriasObjetivo auditoriasobjetivo = db.AuditoriasObjetivo.Find(id);
            return View(auditoriasobjetivo);
        }

        //
        // GET: /AuditoriasObjetivos/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /AuditoriasObjetivos/Create

        [HttpPost]
        public ActionResult Create(AuditoriasObjetivo auditoriasobjetivo)
        {
            if (ModelState.IsValid)
            {
                db.AuditoriasObjetivo.Add(auditoriasobjetivo);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(auditoriasobjetivo);
        }
        
        //
        // GET: /AuditoriasObjetivos/Edit/5
 
        public ActionResult Edit(int id)
        {
            AuditoriasObjetivo auditoriasobjetivo = db.AuditoriasObjetivo.Find(id);
            return View(auditoriasobjetivo);
        }

        //
        // POST: /AuditoriasObjetivos/Edit/5

        [HttpPost]
        public ActionResult Edit(AuditoriasObjetivo auditoriasobjetivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auditoriasobjetivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auditoriasobjetivo);
        }

        //
        // GET: /AuditoriasObjetivos/Delete/5
 
        public ActionResult Delete(int id)
        {
            AuditoriasObjetivo auditoriasobjetivo = db.AuditoriasObjetivo.Find(id);
            return View(auditoriasobjetivo);
        }

        //
        // POST: /AuditoriasObjetivos/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            AuditoriasObjetivo auditoriasobjetivo = db.AuditoriasObjetivo.Find(id);
            db.AuditoriasObjetivo.Remove(auditoriasobjetivo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}