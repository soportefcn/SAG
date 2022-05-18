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
    public class BajasController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Bajas/

        public ViewResult Index()
        {
            var inventariobien = db.InventarioBien.Include(i => i.Inventario).Include(i => i.BienInventario);
            return View(inventariobien.ToList());
        }

        //
        // GET: /Bajas/Details/5

        public ViewResult Details(int id)
        {
            InventarioBien inventariobien = db.InventarioBien.Find(id);
            return View(inventariobien);
        }

        //
        // GET: /Bajas/Create

        public ActionResult Create()
        {
            ViewBag.InventarioID = new SelectList(db.Inventario, "ID", "Numero");
            ViewBag.BienInventarioID = new SelectList(db.BienInventario, "ID", "NumeroInventario");
            return View();
        } 

        //
        // POST: /Bajas/Create

        [HttpPost]
        public ActionResult Create(InventarioBien inventariobien)
        {
            if (ModelState.IsValid)
            {
                db.InventarioBien.Add(inventariobien);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.InventarioID = new SelectList(db.Inventario, "ID", "Numero", inventariobien.InventarioID);
            ViewBag.BienInventarioID = new SelectList(db.BienInventario, "ID", "NumeroInventario", inventariobien.BienInventarioID);
            return View(inventariobien);
        }
        
        //
        // GET: /Bajas/Edit/5
 
        public ActionResult Edit(int id)
        {
            InventarioBien inventariobien = db.InventarioBien.Find(id);
            ViewBag.InventarioID = new SelectList(db.Inventario, "ID", "Numero", inventariobien.InventarioID);
            ViewBag.BienInventarioID = new SelectList(db.BienInventario, "ID", "NumeroInventario", inventariobien.BienInventarioID);
            return View(inventariobien);
        }

        //
        // POST: /Bajas/Edit/5

        [HttpPost]
        public ActionResult Edit(InventarioBien inventariobien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventariobien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InventarioID = new SelectList(db.Inventario, "ID", "Numero", inventariobien.InventarioID);
            ViewBag.BienInventarioID = new SelectList(db.BienInventario, "ID", "NumeroInventario", inventariobien.BienInventarioID);
            return View(inventariobien);
        }

        //
        // GET: /Bajas/Delete/5
 
        public ActionResult Delete(int id)
        {
            InventarioBien inventariobien = db.InventarioBien.Find(id);
            return View(inventariobien);
        }

        //
        // POST: /Bajas/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            InventarioBien inventariobien = db.InventarioBien.Find(id);
            db.InventarioBien.Remove(inventariobien);
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