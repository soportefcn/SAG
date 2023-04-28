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
    public class ContratosController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Contratos/

        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var contrato = db.Contrato.Include(c => c.Servicio).Where(c => c.ProyectoID == Proyecto.ID);
            return View(contrato.ToList());
        }

        //
        // GET: /Contratos/Details/5

        public ViewResult Details(int id)
        {
            Contrato contrato = db.Contrato.Find(id);
            return View(contrato);
        }

        //
        // GET: /Contratos/Create

        public ActionResult Create()
        {
            ViewBag.ServicioID = new SelectList(db.Servicio, "ID", "Nombre");
            return View();
        } 

        //
        // POST: /Contratos/Create

        [HttpPost]
        public ActionResult Create(Contrato contrato)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Usuario Usuario = (Usuario)Session["Usuario"];

            contrato.Activo = "S";
            contrato.ProyectoID = Proyecto.ID;

            if (ModelState.IsValid)
            {
                db.Contrato.Add(contrato);
                db.SaveChanges();

                InicioLog log = new InicioLog();
                log.Tipo = "Contrato";
                log.ProyectoId = Proyecto.ID;
                log.Fecha = DateTime.Now;
                log.Mes = mes;
                log.Periodo = periodo;
                log.RegistroID = contrato.ID;
                log.UsuarioID = Usuario.ID;
                log.Descripcion = "Nuevo : " + contrato.Nombre;
                db.InicioLog.Add(log);
                db.SaveChanges();

                TempData["Message"] = "Creado con exito " + contrato.Nombre;
                return RedirectToAction("Create");  
            }

            ViewBag.ServicioID = new SelectList(db.Servicio, "ID", "Nombre", contrato.ServicioID);
            return View(contrato);
        }
        
        //
        // GET: /Contratos/Edit/5
 
        public ActionResult Edit(int id)
        {
            Contrato contrato = db.Contrato.Find(id);
            ViewBag.ServicioID = new SelectList(db.Servicio, "ID", "Nombre", contrato.ServicioID);
            return View(contrato);
        }

        //
        // POST: /Contratos/Edit/5

        [HttpPost]
        public ActionResult Edit(Contrato contrato)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Usuario Usuario = (Usuario)Session["Usuario"];

            if (ModelState.IsValid)
            {
                contrato.Activo = null;
                if (Request.Form["Estado"] != null && !Request.Form["Estado"].ToString().Equals(""))
                {
                    contrato.Activo = "S";
                }

                db.Entry(contrato).State = EntityState.Modified;
                db.SaveChanges();

                InicioLog log = new InicioLog();
                log.Tipo = "Contrato";
                log.ProyectoId = Proyecto.ID;
                log.Fecha = DateTime.Now;
                log.Mes = mes;
                log.Periodo = periodo;
                log.RegistroID = contrato.ID;
                log.UsuarioID = Usuario.ID;
                log.Descripcion = "Modificado : " + contrato.Nombre;
                db.InicioLog.Add(log);
                db.SaveChanges();

                TempData["Message"] = "Creado con exito " + contrato.Nombre;
                return RedirectToAction("Create");
            }
            ViewBag.ServicioID = new SelectList(db.Servicio, "ID", "Nombre", contrato.ServicioID);
            return View(contrato);
        }

        //
        // GET: /Contratos/Delete/5
        /*
        public ActionResult Delete(int id)
        {
            Contrato contrato = db.Contrato.Find(id);
            return View(contrato);
        }
        */
        //
        // POST: /Contratos/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Contrato contrato = db.Contrato.Find(id);
            db.Contrato.Remove(contrato);
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