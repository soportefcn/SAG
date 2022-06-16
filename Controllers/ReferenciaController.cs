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
    public class ReferenciaController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Referencia/

        public ViewResult Index()
        {
            return View(db.Referencia.Where(d => d.ESTADO == 1).ToList());
        }

        //
        // GET: /Referencia/Details/5

        public ViewResult Details(int id)
        {
            Referencia referencia = db.Referencia.Find(id);
            return View(referencia);
        }

        //
        // GET: /Referencia/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Referencia/Create

        [HttpPost]
        public ActionResult Create(Referencia referencia)
        {
            referencia.ESTADO = 1; 
            if (ModelState.IsValid)
            {
                int contador = db.Referencia.Where(d => d.GRUPO == referencia.GRUPO).Count() + 1;
                referencia.COD_REFER = contador.ToString();
                if (referencia.PREDETERMINADO == 1)
                {
                    db.Database.ExecuteSqlCommand("UPDATE Referencia  SET PREDETERMINADO = 0 WHERE GRUPO = '" + referencia.GRUPO + "' ");
                }
                db.Referencia.Add(referencia);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(referencia);
        }
        
        //
        // GET: /Referencia/Edit/5
 
        public ActionResult Edit(int id)
        {
            ViewBag.grupo = "Tasa de Impuestos de Honorarios";
            Referencia referencia = db.Referencia.Find(id);
            return View(referencia);
        }

        //
        // POST: /Referencia/Edit/5

        [HttpPost]
        public ActionResult Edit(Referencia referencia)
        {
            if (ModelState.IsValid)
            {
                referencia.GRUPO = "PORCENTAJE"; 
                              
                referencia.ESTADO = 1;
                if (referencia.PREDETERMINADO == 1){
                    db.Database.ExecuteSqlCommand("UPDATE Referencia  SET PREDETERMINADO = 0 WHERE GRUPO = '" + referencia.GRUPO + "' ");
                }
                db.Entry(referencia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(referencia);
        }

        //
        // GET: /Referencia/Delete/5
 
        public ActionResult Delete(int id)
        {
           
            Referencia referencia = db.Referencia.Find(id);
            referencia.ESTADO = 0;
            db.Entry(referencia).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        //
        // POST: /Referencia/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Referencia referencia = db.Referencia.Find(id);
            db.Referencia.Remove(referencia);
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