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
    public class ItemGastoController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /ItemGasto/

        public ViewResult Index(string q = "")
        {
            return View(db.ItemGasto.Where(a => a.Nombre.Contains(q)).OrderBy(a => a.Nombre).ToList());
        }

        //
        // GET: /ItemGasto/Details/5

        public ViewResult Details(int id)
        {
            ItemGasto itemgasto = db.ItemGasto.Find(id);
            return View(itemgasto);
        }

        //
        // GET: /ItemGasto/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ItemGasto/Create

        [HttpPost]
        public ActionResult Create(ItemGasto itemgasto)
        {
            if (ModelState.IsValid)
            {
                db.ItemGasto.Add(itemgasto);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            return View(itemgasto);
        }
        
        //
        // GET: /ItemGasto/Edit/5
 
        public ActionResult Edit(int id)
        {
            ItemGasto itemgasto = db.ItemGasto.Find(id);
            return View(itemgasto);
        }

        //
        // POST: /ItemGasto/Edit/5

        [HttpPost]
        public ActionResult Edit(ItemGasto itemgasto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemgasto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(itemgasto);
        }

        //
        // GET: /ItemGasto/Delete/5
 /*
        public ActionResult Delete()
        {
            ItemGasto itemgasto = db.ItemGasto.Find(id);
            return View(itemgasto);
        }
        */
        //
        // POST: /ItemGasto/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ItemGasto itemgasto = db.ItemGasto.Find(id);
            db.ItemGasto.Remove(itemgasto);
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