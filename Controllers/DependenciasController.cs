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
    public class DependenciasController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Dependencias/

        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var dependencia = db.Dependencia.Where( d => d.ProyectoID == Proyecto.ID)  ;
            return View(dependencia.ToList());
        }

        //
        // GET: /Dependencias/Details/5

        public ViewResult Details(int id)
        {
            Dependencia dependencia = db.Dependencia.Find(id);
            return View(dependencia);
        }

        //
        // GET: /Dependencias/Create

        public ActionResult Create()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.ProyectoID = Proyecto.ID;
            ViewBag.NombreEstado = Proyecto.NombreLista;  
            return View();
        } 

        //
        // POST: /Dependencias/Create

        [HttpPost]
        public ActionResult Create(Dependencia dependencia)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Proyecto.ID == dependencia.ProyectoID)
            {
                dependencia.ProyectoID = Proyecto.ID;
                dependencia.Fecha = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Dependencia.Add(dependencia);
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + dependencia.Nombre;
                    return RedirectToAction("Create");
                }
            }
            else {
                var PrGrabar = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).ToList();
                foreach (var prIngr in PrGrabar)
                {
                    dependencia.ProyectoID = prIngr.ID;
                    dependencia.Fecha = DateTime.Now;
                    db.Dependencia.Add(dependencia);
                    db.SaveChanges(); 
                }
                TempData["Message"] = "Creado con exito " + dependencia.Nombre;
                return RedirectToAction("Create");
            }
            return View(dependencia);
        }
        
        //
        // GET: /Dependencias/Edit/5
 
        public ActionResult Edit(int id)
        {
            Dependencia dependencia = db.Dependencia.Find(id);
            return View(dependencia);
        }

        //
        // POST: /Dependencias/Edit/5

        [HttpPost]
        public ActionResult Edit(Dependencia dependencia)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            dependencia.ProyectoID = Proyecto.ID;
            dependencia.Fecha = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(dependencia).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + dependencia.Nombre;
                return RedirectToAction("Create");
            }

            return View(dependencia);
        }

        //
        // GET: /Dependencias/Delete/5
 
        public ActionResult Delete(int id)
        {
            Dependencia dependencia = db.Dependencia.Find(id);
            return View(dependencia);
        }

        //
        // POST: /Dependencias/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Dependencia dependencia = db.Dependencia.Find(id);
            db.Dependencia.Remove(dependencia);
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