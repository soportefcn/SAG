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
    public class ResolucionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Resolucion/

        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            
            return View(db.Resolucion.Where(d => d.ProyectoID == Proyecto.ID).ToList());
        }

        //
        // GET: /Resolucion/Details/5

        public ViewResult Details(int id)
        {
            Resolucion resolucion = db.Resolucion.Find(id);
            return View(resolucion);
        }

        //
        // GET: /Resolucion/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Resolucion/Create

        [HttpPost]
        public ActionResult Create(Resolucion resolucion)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (ModelState.IsValid)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];

                db.Database.ExecuteSqlCommand("UPDATE Resolucion SET estado = 0 WHERE ProyectoID = " + Proyecto.ID);

                if (!Usuario.esAdministrador)
                {
                    resolucion.FechaProrroga = resolucion.FechaTermino.AddMonths(6);
                }
                resolucion.ProyectoID = Proyecto.ID;
                resolucion.Estado = 1;
       
                db.Resolucion.Add(resolucion);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + resolucion.ResEx ;
                return RedirectToAction("Create");  
            }

            return View(resolucion);
        }
        
        //
        // GET: /Resolucion/Edit/5
 
        public ActionResult Edit(int id)
        {
            Resolucion resolucion = db.Resolucion.Find(id);
            return View(resolucion);
        }

        //
        // POST: /Resolucion/Edit/5

        [HttpPost]
        public ActionResult Edit(Resolucion resolucion)
        {
            if (ModelState.IsValid)
            {
                var a = resolucion.Estado;
                var d = resolucion.ID;
                resolucion.FechaProrroga = resolucion.FechaTermino.AddMonths(6);
                db.Entry(resolucion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return RedirectToAction("Create");
        }

        //
        // GET: /Resolucion/Delete/5
 
        public ActionResult Delete(int id)
        {
            Resolucion resolucion = db.Resolucion.Find(id);
            return View(resolucion);
        }

        //
        // POST: /Resolucion/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Resolucion resolucion = db.Resolucion.Find(id);
            if (resolucion.Estado != 1)
            {
                db.Resolucion.Remove(resolucion);
                db.SaveChanges();
            }
            TempData["Message"] = "Modificado con Exito " ;
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}