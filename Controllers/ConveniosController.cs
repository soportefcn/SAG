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
    public class ConveniosController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Convenios/

        public ViewResult Index(string q = "")
        {
            return View(db.Convenio.Where(a => a.ResEx.Contains(q)).ToList());
        }

        //
        // GET: /Convenios/Details/5

        public ViewResult Details(int id)
        {
            Convenio convenio = db.Convenio.Find(id);
            return View(convenio);
        }

        //
        // GET: /Convenios/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Convenios/Create

        [HttpPost]
        public ActionResult Create(Convenio convenio)
        {
            if (ModelState.IsValid)
            {
                db.Convenio.Add(convenio);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            return View(convenio);
        }
        
        //
        // GET: /Convenios/Edit/5
 
        public ActionResult Edit(int id)
        {
            Convenio convenio = db.Convenio.Find(id);
            return View(convenio);
        }

        //
        // POST: /Convenios/Edit/5

        [HttpPost]
        public ActionResult Edit(Convenio convenio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(convenio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(convenio);
        }

        //
        // GET: /Convenios/Delete/5
 /*
        public ActionResult Delete()
        {
            Convenio convenio = db.Convenio.Find(id);
            return View(convenio);
        }
        */
        //
        // POST: /Convenios/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Convenio convenio = db.Convenio.Find(id);
            db.Convenio.Remove(convenio);
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