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
    public class SistemaAsistencialController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /SistemaAsistencial/

        public ViewResult Index(string q = "")
        {
            var sistemaasistencial = db.SistemaAsistencial.Include(s => s.LineaAtencion).Where(a => a.Nombre.Contains(q));
            return View(sistemaasistencial.ToList());
        }

        //
        // GET: /SistemaAsistencial/Details/5

        public ViewResult Details(int id)
        {
            SistemaAsistencial sistemaasistencial = db.SistemaAsistencial.Find(id);
            return View(sistemaasistencial);
        }

        //
        // GET: /SistemaAsistencial/Create

        public ActionResult Create()
        {
            ViewBag.LineaAtencionID = new SelectList(db.LineasAtencion, "ID", "Nombre");
            return View();
        } 

        //
        // POST: /SistemaAsistencial/Create

        [HttpPost]
        public ActionResult Create(SistemaAsistencial sistemaasistencial)
        {
            if (ModelState.IsValid)
            {
                db.SistemaAsistencial.Add(sistemaasistencial);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            ViewBag.LineaAtencionID = new SelectList(db.LineasAtencion, "ID", "Nombre", sistemaasistencial.LineaAtencionID);
            return View(sistemaasistencial);
        }
        
        //
        // GET: /SistemaAsistencial/Edit/5
 
        public ActionResult Edit(int id)
        {
            SistemaAsistencial sistemaasistencial = db.SistemaAsistencial.Find(id);
            ViewBag.LineaAtencionID = new SelectList(db.LineasAtencion, "ID", "Nombre", sistemaasistencial.LineaAtencionID);
            return View(sistemaasistencial);
        }

        //
        // POST: /SistemaAsistencial/Edit/5

        [HttpPost]
        public ActionResult Edit(SistemaAsistencial sistemaasistencial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sistemaasistencial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            ViewBag.LineaAtencionID = new SelectList(db.LineasAtencion, "ID", "Nombre", sistemaasistencial.LineaAtencionID);
            return View(sistemaasistencial);
        }

        //
        // GET: /SistemaAsistencial/Delete/5
        /*
        public ActionResult Delete()
        {
            SistemaAsistencial sistemaasistencial = db.SistemaAsistencial.Find(id);
            return View(sistemaasistencial);
        }
        */
        //
        // POST: /SistemaAsistencial/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            SistemaAsistencial sistemaasistencial = db.SistemaAsistencial.Find(id);
            db.SistemaAsistencial.Remove(sistemaasistencial);
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