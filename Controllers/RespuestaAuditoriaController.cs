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
    public class RespuestaAuditoriaController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /RespuestaAuditoria/

        public ViewResult Index()
        {
            return View(db.RespuestaAuditoria.ToList());
        }

        //
        // GET: /RespuestaAuditoria/Details/5

        public ViewResult Details(int id)
        {
            RespuestaAuditoria respuestaauditoria = db.RespuestaAuditoria.Find(id);
            return View(respuestaauditoria);
        }

        //
        // GET: /RespuestaAuditoria/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /RespuestaAuditoria/Create

        [HttpPost]
        public ActionResult Create(RespuestaAuditoria respuestaauditoria)
        {
            if (ModelState.IsValid)
            {
                db.RespuestaAuditoria.Add(respuestaauditoria);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(respuestaauditoria);
        }
        
        //
        // GET: /RespuestaAuditoria/Edit/5
 
        public ActionResult Edit(int id)
        {
            RespuestaAuditoria respuestaauditoria = db.RespuestaAuditoria.Find(id);
            return View(respuestaauditoria);
        }

        //
        // POST: /RespuestaAuditoria/Edit/5

        [HttpPost]
        public ActionResult Edit(RespuestaAuditoria respuestaauditoria)
        {
            if (ModelState.IsValid)
            {
                db.Entry(respuestaauditoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(respuestaauditoria);
        }

        //
        // GET: /RespuestaAuditoria/Delete/5
 
        public ActionResult Delete(int id)
        {
            RespuestaAuditoria respuestaauditoria = db.RespuestaAuditoria.Find(id);
            return View(respuestaauditoria);
        }

        //
        // POST: /RespuestaAuditoria/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            RespuestaAuditoria respuestaauditoria = db.RespuestaAuditoria.Find(id);
            db.RespuestaAuditoria.Remove(respuestaauditoria);
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