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
    public class UnidadesMedidasController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /UnidadesMedidas/

        public ViewResult Index()
        {
            return View(db.UnidadMedida.ToList());
        }

        //
        // GET: /UnidadesMedidas/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /UnidadesMedidas/Create

        [HttpPost]
        public ActionResult Create(UnidadMedida unidadmedida)
        {
            if (ModelState.IsValid)
            {
                db.UnidadMedida.Add(unidadmedida);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + unidadmedida.Descripcion;
                return RedirectToAction("Create");  
            }

            return View(unidadmedida);
        }
        
        //
        // GET: /UnidadesMedidas/Edit/5
 
        public ActionResult Edit(int id)
        {
            UnidadMedida unidadmedida = db.UnidadMedida.Find(id);
            return View(unidadmedida);
        }

        //
        // POST: /UnidadesMedidas/Edit/5

        [HttpPost]
        public ActionResult Edit(UnidadMedida unidadmedida)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unidadmedida).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + unidadmedida.Descripcion;
                return RedirectToAction("Create");
            }
            return View(unidadmedida);
        }

        //
        // GET: /UnidadesMedidas/Delete/5
 
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {    
        try
            {
            UnidadMedida unidadmedida = db.UnidadMedida.Find(id);
            db.UnidadMedida.Remove(unidadmedida);
            db.SaveChanges();
            return RedirectToAction("Create");
            }
        catch (Exception)
        {
            TempData["Message"] = "No se puede eliminar la unidad esta siendo utilizada";
            return RedirectToAction("Create");
        }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}