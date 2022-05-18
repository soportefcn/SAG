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
    public class RegionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Region/

        public ViewResult Index(string q = "")
        {
            var region = db.Region.Include(r => r.Pais).Where(a => a.Nombre.Contains(q));
            return View(region.ToList());
        }

        //
        // GET: /Region/Details/5

        public ViewResult Details(int id)
        {
            Region region = db.Region.Find(id);
            return View(region);
        }

        //
        // GET: /Region/Create

        public ActionResult Create()
        {
            ViewBag.PaisID = new SelectList(db.Pais, "ID", "Nombre");
            return View();
        } 

        //
        // POST: /Region/Create

        [HttpPost]
        public ActionResult Create(Region region)
        {
            if (ModelState.IsValid)
            {
                db.Region.Add(region);
                db.SaveChanges();
                TempData["Message"] = "Grabado con exito " + region.Nombre ;
                return RedirectToAction("Create");  
            }

            ViewBag.PaisID = new SelectList(db.Pais, "ID", "Nombre", region.PaisID);
            return View(region);
        }
        
        //
        // GET: /Region/Edit/5
 
        public ActionResult Edit(int id)
        {
            Region region = db.Region.Find(id);
            ViewBag.PaisID = new SelectList(db.Pais, "ID", "Nombre", region.PaisID);
            return View(region);
        }

        //
        // POST: /Region/Edit/5

        [HttpPost]
        public ActionResult Edit(Region region)
        {
            if (ModelState.IsValid)
            {
                db.Entry(region).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Grabado con exito " + region.Nombre;
                return RedirectToAction("Create");
            }
            ViewBag.PaisID = new SelectList(db.Pais, "ID", "Nombre", region.PaisID);
            return View(region);
        }

        //
        // GET: /Region/Delete/5
        /*
        public ActionResult Delete()
        {
            Region region = db.Region.Find(id);
            return View(region);
        }
        */
        //
        // POST: /Region/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Region region = db.Region.Find(id);
            db.Region.Remove(region);
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