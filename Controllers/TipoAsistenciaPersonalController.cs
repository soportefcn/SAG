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
    public class TipoAsistenciaPersonalController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /TipoAsistenciaPersonal/

        public ViewResult Index(string q = "")
        {
            return View(db.TipoAsistenciaPersonal.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /TipoAsistenciaPersonal/Details/5

        public ViewResult Details(int id)
        {
            TipoAsistenciaPersonal tipoasistenciapersonal = db.TipoAsistenciaPersonal.Find(id);
            return View(tipoasistenciapersonal);
        }

        //
        // GET: /TipoAsistenciaPersonal/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /TipoAsistenciaPersonal/Create

        [HttpPost]
        public ActionResult Create(TipoAsistenciaPersonal tipoasistenciapersonal)
        {
            if (ModelState.IsValid)
            {
                db.TipoAsistenciaPersonal.Add(tipoasistenciapersonal);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            return View(tipoasistenciapersonal);
        }
        
        //
        // GET: /TipoAsistenciaPersonal/Edit/5
 
        public ActionResult Edit(int id)
        {
            TipoAsistenciaPersonal tipoasistenciapersonal = db.TipoAsistenciaPersonal.Find(id);
            return View(tipoasistenciapersonal);
        }

        //
        // POST: /TipoAsistenciaPersonal/Edit/5

        [HttpPost]
        public ActionResult Edit(TipoAsistenciaPersonal tipoasistenciapersonal)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoasistenciapersonal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(tipoasistenciapersonal);
        }

        //
        // GET: /TipoAsistenciaPersonal/Delete/5
        /*
        public ActionResult Delete()
        {
            TipoAsistenciaPersonal tipoasistenciapersonal = db.TipoAsistenciaPersonal.Find(id);
            return View(tipoasistenciapersonal);
        }
        */
        //
        // POST: /TipoAsistenciaPersonal/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            TipoAsistenciaPersonal tipoasistenciapersonal = db.TipoAsistenciaPersonal.Find(id);
            db.TipoAsistenciaPersonal.Remove(tipoasistenciapersonal);
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