using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.Web.Script.Serialization;

namespace SAG2.Controllers
{ 
    public class ComunaController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Comuna/

        public ViewResult Index(string q = "")
        {
            var comuna = db.Comuna.Include(c => c.Region).Where(a => a.Nombre.Contains(q));
            return View(comuna.ToList());
        }

        //
        // GET: /Comuna/Details/5

        public ViewResult Details(int id)
        {
            Comuna comuna = db.Comuna.Find(id);
            return View(comuna);
        }

        //
        // GET: /Comuna/Create

        public ActionResult Create()
        {
            ViewBag.RegionID = new SelectList(db.Region, "ID", "Nombre");
            return View();
        } 

        //
        // POST: /Comuna/Create

        [HttpPost]
        public ActionResult Create(Comuna comuna)
        {
            if (ModelState.IsValid)
            {
                db.Comuna.Add(comuna);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " +  comuna.Nombre;
                return RedirectToAction("Create");  
            }

            ViewBag.RegionID = new SelectList(db.Region, "ID", "Nombre", comuna.RegionID);
            return View(comuna);
        }
        
        //
        // GET: /Comuna/Edit/5
 
        public ActionResult Edit(int id)
        {
            Comuna comuna = db.Comuna.Find(id);
            ViewBag.RegionID = new SelectList(db.Region, "ID", "Nombre", comuna.RegionID);
            return View(comuna);
        }

        //
        // POST: /Comuna/Edit/5

        [HttpPost]
        public ActionResult Edit(Comuna comuna)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comuna).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + comuna.Nombre;
                return RedirectToAction("Create");
            }
            ViewBag.RegionID = new SelectList(db.Region, "ID", "Nombre", comuna.RegionID);
            return View(comuna);
        }

        //
        // GET: /Comuna/Delete/5
        /*
        public ActionResult Delete()
        {
            Comuna comuna = db.Comuna.Find(id);
            return View(comuna);
        }
        */
        //
        // POST: /Comuna/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Comuna comuna = db.Comuna.Find(id);
            db.Comuna.Remove(comuna);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        public string Comunas(int id)
        {
            var Comunas = (from c in db.Comuna
                                     where c.RegionID == id
                                     orderby c.Nombre
                                     select new
                                     {
                                         Value = c.ID,
                                         Text = c.Nombre
                                     }).ToList();

            return new JavaScriptSerializer().Serialize(Comunas);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}