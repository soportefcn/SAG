using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;

namespace SAG2.Controllers
{ 
    public class PreguntasFrecuentesController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();

        //
        // GET: /PreguntasFrecuentes/

        public ViewResult Index(string mensaje = "")
        {
            if (!mensaje.Equals(""))
            {
                ViewBag.Mensaje = utils.mensajeOK("Su pregunta fue enviada, ésta será revisada y respondida por los encargados de SAG2.");
            }

            return View(db.PreguntaFrecuente.OrderBy(p => p.Pregunta).ToList());
        }

        //
        // GET: /PreguntasFrecuentes/Details/5

        public ViewResult Details(int id)
        {
            PreguntaFrecuente preguntafrecuente = db.PreguntaFrecuente.Find(id);
            return View(preguntafrecuente);
        }

        //
        // GET: /PreguntasFrecuentes/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /PreguntasFrecuentes/Create

        [HttpPost]
        public ActionResult Create(PreguntaFrecuente preguntafrecuente)
        {
            Persona Persona = (Persona)Session["Persona"];

            preguntafrecuente.PreguntaFecha = DateTime.Now;
            preguntafrecuente.PersonaID = Persona.ID;
            preguntafrecuente.Activo = "N";

            if (ModelState.IsValid)
            {
                db.PreguntaFrecuente.Add(preguntafrecuente);
                db.SaveChanges();
                return RedirectToAction("Index", new { mensaje = "OK" });  
            }

            //return View(preguntafrecuente);
            return RedirectToAction("Index");
        }
        
        //
        // GET: /PreguntasFrecuentes/Edit/5
 
        public ActionResult Edit(int id)
        {
            PreguntaFrecuente preguntafrecuente = db.PreguntaFrecuente.Find(id);
            return View(preguntafrecuente);
        }

        //
        // POST: /PreguntasFrecuentes/Edit/5

        [HttpPost]
        public ActionResult Edit(PreguntaFrecuente preguntafrecuente)
        {
            preguntafrecuente.RespondeFecha = DateTime.Now;
            preguntafrecuente.Activo = "S";

            if (ModelState.IsValid)
            {
                db.Entry(preguntafrecuente).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(preguntafrecuente);
        }

        //
        // GET: /PreguntasFrecuentes/Delete/5
 
       
        //
        // POST: /PreguntasFrecuentes/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            PreguntaFrecuente preguntafrecuente = db.PreguntaFrecuente.Find(id);
            db.PreguntaFrecuente.Remove(preguntafrecuente);
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