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
    public class AuditoriasDocumentosController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /AuditoriasDocumentos/

        public ViewResult Index()
        {
            return View(db.AuditoriasDocumento.ToList());
        }

        //
        // GET: /AuditoriasDocumentos/Details/5

        public ViewResult Details(int id)
        {
            AuditoriasDocumento auditoriasdocumento = db.AuditoriasDocumento.Find(id);
            return View(auditoriasdocumento);
        }

        //
        // GET: /AuditoriasDocumentos/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /AuditoriasDocumentos/Create

        [HttpPost]
        public ActionResult Create(AuditoriasDocumento auditoriasdocumento)
        {
            if (ModelState.IsValid)
            {
                db.AuditoriasDocumento.Add(auditoriasdocumento);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(auditoriasdocumento);
        }
        
        //
        // GET: /AuditoriasDocumentos/Edit/5
 
        public ActionResult Edit(int id)
        {
            AuditoriasDocumento auditoriasdocumento = db.AuditoriasDocumento.Find(id);
            return View(auditoriasdocumento);
        }

        //
        // POST: /AuditoriasDocumentos/Edit/5

        [HttpPost]
        public ActionResult Edit(AuditoriasDocumento auditoriasdocumento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auditoriasdocumento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auditoriasdocumento);
        }

        //
        // GET: /AuditoriasDocumentos/Delete/5
 
        public ActionResult Delete(int id)
        {
            AuditoriasDocumento auditoriasdocumento = db.AuditoriasDocumento.Find(id);
            return View(auditoriasdocumento);
        }

        //
        // POST: /AuditoriasDocumentos/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            AuditoriasDocumento auditoriasdocumento = db.AuditoriasDocumento.Find(id);
            db.AuditoriasDocumento.Remove(auditoriasdocumento);
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