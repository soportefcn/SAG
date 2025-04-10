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
    public class TipoRolController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /TipoRol/

        public ViewResult Index()
        {
            return View(db.TipoRol.ToList());
        }

        //
        // GET: /TipoRol/Details/5

        public ViewResult Details(int id)
        {
            TipoRol tiporol = db.TipoRol.Find(id);
            return View(tiporol);
        }

        //
        // GET: /TipoRol/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /TipoRol/Create

        [HttpPost]
        public ActionResult Create(TipoRol tiporol)
        {
            if (ModelState.IsValid)
            {
                db.TipoRol.Add(tiporol);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + tiporol.Nombre;
                return RedirectToAction("Edit", new { @id = tiporol.ID}); 
            }

            return View(tiporol);
        }
        
        //
        // GET: /TipoRol/Edit/5
 
        public ActionResult Edit(int id)
        {
            TipoRol tiporol = db.TipoRol.Find(id);
            return View(tiporol);
        }

        //
        // POST: /TipoRol/Edit/5

        [HttpPost]
        public ActionResult Edit(TipoRol tiporol)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tiporol).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Modificado con exito " + tiporol.Nombre;
                return View(tiporol);
            }
            return View(tiporol);
        }

        //
        // GET: /TipoRol/Delete/5
 
  

        //
        // POST: /TipoRol/Delete/5

       [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {

            List<Rol> Rev = new List<Rol>();
            Rev = db.Rol.Where(d => d.TipoRolID == id).ToList();
            if (Rev.Count() == 0)
            {                
                TipoRol tiporol = db.TipoRol.Find(id);
                string nombre = tiporol.Nombre;
                db.TipoRol.Remove(tiporol);
                db.SaveChanges();
                TempData["Message"] = "Eliminado con exito " + nombre;
                return RedirectToAction("Create");
            }
            else {
                TipoRol tiporol = db.TipoRol.Find(id);
                TempData["Message"] = "No se puede eliminar esta asociado el Rol " + tiporol.Nombre ;
                return RedirectToAction("Edit", new { @id = tiporol.ID }); 
            }
           
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}