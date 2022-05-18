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
    public class SupervisionesController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Supervisiones/

        public ViewResult Index(string q = "")
        {
            var supervision = db.Supervision.Include(s => s.Rol);
            return View(supervision.ToList());
        }

        //
        // GET: /Supervisiones/Details/5

        public ViewResult Details(int id)
        {
            Supervision supervision = db.Supervision.Find(id);
            return View(supervision);
        }

        //
        // GET: /Supervisiones/Create

        public ActionResult Create()
        {
            ViewBag.RolID = new SelectList(db.Rol, "ID", "Comentarios");
            return View();
        } 

        //
        // POST: /Supervisiones/Create

        [HttpPost]
        public ActionResult Create(Supervision supervision)
        {
            Rol Rol = (Rol)Session["Rol"];
            supervision.RolID = Rol.ID;

            if (ModelState.IsValid)
            {
                db.Supervision.Add(supervision);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.RolID = new SelectList(db.Rol, "ID", "Comentarios", supervision.RolID);
            return View(supervision);
        }
        
        //
        // GET: /Supervisiones/Edit/5
 
        public ActionResult Edit(int id)
        {
            Supervision supervision = db.Supervision.Find(id);
            ViewBag.RolID = new SelectList(db.Rol, "ID", "Comentarios", supervision.RolID);
            return View(supervision);
        }

        //
        // POST: /Supervisiones/Edit/5

        [HttpPost]
        public ActionResult Edit(Supervision supervision)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supervision).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RolID = new SelectList(db.Rol, "ID", "Comentarios", supervision.RolID);
            return View(supervision);
        }

        

        //
        // POST: /Supervisiones/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Supervision supervision = db.Supervision.Find(id);
            db.Supervision.Remove(supervision);
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