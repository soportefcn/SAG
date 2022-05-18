using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.Data.SqlClient;

namespace SAG2.Controllers
{ 
    public class EspecieController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Especie/

        public ViewResult Index()
        {
            return View(db.Especie.OrderBy(e => e.Nombre).ToList());
        }

        //
        // GET: /Especie/Details/5

        public ViewResult Details(int id)
        {
            Especie especie = db.Especie.Find(id);
            return View(especie);
        }

        //
        // GET: /Especie/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Especie/Create

        [HttpPost]
        public ActionResult Create(Especie especie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Especie.Add(especie);
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + especie.Nombre;
                    return RedirectToAction("Create");
                }
            }
            catch (SqlException)
            {
                ViewBag.Mensaje = "La especie ingresada ya existe.";
                return View(especie);
            }
            catch (Exception)
            {
                ViewBag.Mensaje = "Ocurrió un error al ingresar la especie.";
                return View(especie);
            }

            return View(especie);
        }
        
        //
        // GET: /Especie/Edit/5
 
        public ActionResult Edit(int id)
        {
            Especie especie = db.Especie.Find(id);
            return View(especie);
        }

        //
        // POST: /Especie/Edit/5

        [HttpPost]
        public ActionResult Edit(Especie especie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(especie).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + especie.Nombre;
                return RedirectToAction("Create");
            }
            return View(especie);
        }

        //
        // POST: /Especie/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Especie especie = db.Especie.Find(id);
            db.Especie.Remove(especie);
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