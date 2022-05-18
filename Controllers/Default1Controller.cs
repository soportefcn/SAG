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
    public class Default1Controller : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Default1/

        public ViewResult Index()
        {
            return View(db.Objetivo.ToList());
        }

        //
        // GET: /Default1/Details/5

        public ViewResult Details(int id)
        {
            Objetivo objetivo = db.Objetivo.Find(id);
            return View(objetivo);
        }

        //
        // GET: /Default1/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Default1/Create

        [HttpPost]
        public ActionResult Create(Objetivo objetivo)
        {
            if (ModelState.IsValid)
            {
                db.Objetivo.Add(objetivo);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(objetivo);
        }
        
        //
        // GET: /Default1/Edit/5
 
        public ActionResult Edit(int id)
        {
            Objetivo objetivo = db.Objetivo.Find(id);
            return View(objetivo);
        }

        //
        // POST: /Default1/Edit/5

        [HttpPost]
        public ActionResult Edit(Objetivo objetivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(objetivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(objetivo);
        }

        //
        // GET: /Default1/Delete/5
 
        public ActionResult Delete(int id)
        {
            Objetivo objetivo = db.Objetivo.Find(id);
            return View(objetivo);
        }

        //
        // POST: /Default1/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Objetivo objetivo = db.Objetivo.Find(id);
            db.Objetivo.Remove(objetivo);
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