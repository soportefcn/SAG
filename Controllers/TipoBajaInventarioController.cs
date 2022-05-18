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
    public class TipoBajaInventarioController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /TipoBajaInventario/

        public ViewResult Index(string q = "")
        {
            return View(db.TipoBajaInventario.Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /TipoBajaInventario/Details/5

        public ViewResult Details(int id)
        {
            TipoBajaInventario tipobajainventario = db.TipoBajaInventario.Find(id);
            return View(tipobajainventario);
        }

        //
        // GET: /TipoBajaInventario/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /TipoBajaInventario/Create

        [HttpPost]
        public ActionResult Create(TipoBajaInventario tipobajainventario)
        {
            if (ModelState.IsValid)
            {
                db.TipoBajaInventario.Add(tipobajainventario);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            return View(tipobajainventario);
        }
        
        //
        // GET: /TipoBajaInventario/Edit/5
 
        public ActionResult Edit(int id)
        {
            TipoBajaInventario tipobajainventario = db.TipoBajaInventario.Find(id);
            return View(tipobajainventario);
        }

        //
        // POST: /TipoBajaInventario/Edit/5

        [HttpPost]
        public ActionResult Edit(TipoBajaInventario tipobajainventario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipobajainventario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(tipobajainventario);
        }

        //
        // GET: /TipoBajaInventario/Delete/5
        /*
        public ActionResult Delete()
        {
            TipoBajaInventario tipobajainventario = db.TipoBajaInventario.Find(id);
            return View(tipobajainventario);
        }
        */
        //
        // POST: /TipoBajaInventario/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            TipoBajaInventario tipobajainventario = db.TipoBajaInventario.Find(id);
            db.TipoBajaInventario.Remove(tipobajainventario);
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