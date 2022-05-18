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
    public class ItemIntervencionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /ItemIntervencion/

        public ViewResult Index(string q = "")
        {
            return View(db.ItemIntervencion.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /ItemIntervencion/Details/5

        public ViewResult Details(int id)
        {
            ItemIntervencion itemintervencion = db.ItemIntervencion.Find(id);
            return View(itemintervencion);
        }

        //
        // GET: /ItemIntervencion/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ItemIntervencion/Create

        [HttpPost]
        public ActionResult Create(ItemIntervencion itemintervencion)
        {
            if (ModelState.IsValid)
            {
                db.ItemIntervencion.Add(itemintervencion);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            return View(itemintervencion);
        }
        
        //
        // GET: /ItemIntervencion/Edit/5
 
        public ActionResult Edit(int id)
        {
            ItemIntervencion itemintervencion = db.ItemIntervencion.Find(id);
            return View(itemintervencion);
        }

        //
        // POST: /ItemIntervencion/Edit/5

        [HttpPost]
        public ActionResult Edit(ItemIntervencion itemintervencion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemintervencion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(itemintervencion);
        }

        //
        // GET: /ItemIntervencion/Delete/5
 /*
        public ActionResult Delete()
        {
            ItemIntervencion itemintervencion = db.ItemIntervencion.Find(id);
            return View(itemintervencion);
        }
        */
        //
        // POST: /ItemIntervencion/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ItemIntervencion itemintervencion = db.ItemIntervencion.Find(id);
            db.ItemIntervencion.Remove(itemintervencion);
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