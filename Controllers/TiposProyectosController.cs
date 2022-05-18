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
    public class TiposProyectosController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /TiposProyectos/

        public ViewResult Index(string q = "")
        {
            var tipoproyecto = db.TipoProyecto.Include(t => t.LineaAtencion).Where(a => a.Nombre.Contains(q));
            return View(tipoproyecto.ToList());
        }

        //
        // GET: /TiposProyectos/Details/5

        public ViewResult Details(int id)
        {
            TipoProyecto tipoproyecto = db.TipoProyecto.Find(id);
            return View(tipoproyecto);
        }

        //
        // GET: /TiposProyectos/Create

        public ActionResult Create()
        {
            ViewBag.LineasAtencionID = new SelectList(db.LineasAtencion, "ID", "NombreLista");
            return View();
        } 

        //
        // POST: /TiposProyectos/Create

        [HttpPost]
        public ActionResult Create(TipoProyecto tipoproyecto)
        {
            if (ModelState.IsValid)
            {
                string nombre = Request.Form["nombre"].ToString();
                if (nombre.Length >= 15)
                {
                    db.TipoProyecto.Add(tipoproyecto);
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + tipoproyecto.Nombre;
                }
                else
                {
                    TempData["Message"] = "El nombre como minimo debe tener 15 caracteres de largo";
                }

                return RedirectToAction("Create");  
            }

            ViewBag.LineasAtencionID = new SelectList(db.LineasAtencion, "ID", "NombreLista", tipoproyecto.LineasAtencionID);
            return View(tipoproyecto);
        }
        
        //
        // GET: /TiposProyectos/Edit/5
 
        public ActionResult Edit(int id)
        {
            TipoProyecto tipoproyecto = db.TipoProyecto.Find(id);
            ViewBag.LineasAtencionID = new SelectList(db.LineasAtencion, "ID", "NombreLista", tipoproyecto.LineasAtencionID);
            return View(tipoproyecto);
        }

        //
        // POST: /TiposProyectos/Edit/5

        [HttpPost]
        public ActionResult Edit(TipoProyecto tipoproyecto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoproyecto).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + tipoproyecto.Nombre;
                return RedirectToAction("Create");
            }
            ViewBag.LineasAtencionID = new SelectList(db.LineasAtencion, "ID", "NombreLista", tipoproyecto.LineasAtencionID);
            return View(tipoproyecto);
        }

        //
        // GET: /TiposProyectos/Delete/5
        /*
        public ActionResult Delete()
        {
            TipoProyecto tipoproyecto = db.TipoProyecto.Find(id);
            return View(tipoproyecto);
        }
        */
        //
        // POST: /TiposProyectos/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            TipoProyecto tipoproyecto = db.TipoProyecto.Find(id);
            db.TipoProyecto.Remove(tipoproyecto);
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