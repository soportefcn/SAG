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
    public class BienInventarioController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /BienInventario/

        public ViewResult Index()
        {
            return View(db.BienInventario.ToList());
        }

        //
        // GET: /BienInventario/Details/5

        public ViewResult Details(int id)
        {
            BienInventario bieninventario = db.BienInventario.Find(id);
            return View(bieninventario);
        }

        //
        // GET: /BienInventario/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /BienInventario/Create

        [HttpPost]
        public ActionResult Create(BienInventario bieninventario)
        {
            if (ModelState.IsValid)
            {
                db.BienInventario.Add(bieninventario);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(bieninventario);
        }
        
        //
        // GET: /BienInventario/Edit/5
 
        public ActionResult Edit(int id)
        {
            BienInventario bieninventario = db.BienInventario.Find(id);
            return View(bieninventario);
        }

        //
        // POST: /BienInventario/Edit/5

        [HttpPost]
        public ActionResult Edit(BienInventario bieninventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bieninventario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bieninventario);
        }

        //
        // GET: /BienInventario/Delete/5
 
        public ActionResult Delete(int id)
        {
            BienInventario bieninventario = db.BienInventario.Find(id);
            return View(bieninventario);
        }

        //
        // POST: /BienInventario/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            BienInventario bieninventario = db.BienInventario.Find(id);
            db.BienInventario.Remove(bieninventario);
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