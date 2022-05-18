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
    public class AuditoriasMetodologiasController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /AuditoriasMetodologias/

        public ViewResult Index()
        {
            return View(db.AuditoriasMetodologia.ToList());
        }

        //
        // GET: /AuditoriasMetodologias/Details/5

        public ViewResult Details(int id)
        {
            AuditoriasMetodologia auditoriasmetodologia = db.AuditoriasMetodologia.Find(id);
            return View(auditoriasmetodologia);
        }

        //
        // GET: /AuditoriasMetodologias/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /AuditoriasMetodologias/Create

        [HttpPost]
        public ActionResult Create(AuditoriasMetodologia auditoriasmetodologia)
        {
            if (ModelState.IsValid)
            {
                db.AuditoriasMetodologia.Add(auditoriasmetodologia);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(auditoriasmetodologia);
        }
        
        //
        // GET: /AuditoriasMetodologias/Edit/5
 
        public ActionResult Edit(int id)
        {
            AuditoriasMetodologia auditoriasmetodologia = db.AuditoriasMetodologia.Find(id);
            return View(auditoriasmetodologia);
        }

        //
        // POST: /AuditoriasMetodologias/Edit/5

        [HttpPost]
        public ActionResult Edit(AuditoriasMetodologia auditoriasmetodologia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auditoriasmetodologia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auditoriasmetodologia);
        }

        //
        // GET: /AuditoriasMetodologias/Delete/5
 
        public ActionResult Delete(int id)
        {
            AuditoriasMetodologia auditoriasmetodologia = db.AuditoriasMetodologia.Find(id);
            return View(auditoriasmetodologia);
        }

        //
        // POST: /AuditoriasMetodologias/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            AuditoriasMetodologia auditoriasmetodologia = db.AuditoriasMetodologia.Find(id);
            db.AuditoriasMetodologia.Remove(auditoriasmetodologia);
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