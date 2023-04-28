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
    public class RolController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        //
        // GET: /Rol/

        public ViewResult Index(string q = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var rol = db.Rol.Include(r => r.Persona).Include(r => r.Proyecto).Include(r => r.TipoRol).Where(r => r.ProyectoID == Proyecto.ID).OrderBy(r => r.Persona.Nombres);
            return View(rol.ToList());
        }

        //
        // GET: /Rol/Details/5

        public ViewResult Details(int id)
        {
            Rol rol = db.Rol.Find(id);
            return View(rol);
        }

        //
        // GET: /Rol/Create

        public ActionResult Create()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.PersonaID = new SelectList(db.Rol.Where(r => r.ProyectoID == Proyecto.ID).Select(r => r.Persona).OrderBy(p => p.Nombres).Distinct().ToList(), "ID", "NombreLista");
            /*ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista");*/
            ViewBag.TipoRolID = new SelectList(db.TipoRol.OrderBy(r => r.Nombre).ToList() , "ID", "Nombre");
            return View();
        } 

        //
        // POST: /Rol/Create

        [HttpPost]
        public ActionResult Create(Rol rol)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            rol.ProyectoID = Proyecto.ID;
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Usuario usuario = (Usuario)Session["Usuario"];

            try
            {
                if (ModelState.IsValid)
                {
                    db.Rol.Add(rol);
                    db.SaveChanges();

                    InicioLog log = new InicioLog();
                    log.Tipo = "Responsabilidad Administrativa";
                    log.ProyectoId = Proyecto.ID;
                    log.Fecha = DateTime.Now;
                    log.Mes = mes;
                    log.Periodo = periodo;
                    log.RegistroID = rol.ID;
                    log.UsuarioID = usuario.ID;
                    log.Descripcion = "Rol :" + rol.TipoRol.Nombre + " " + rol.Persona.NombreCompleto  ;
                    db.InicioLog.Add(log);
                    db.SaveChanges();

                    TempData["Message"] = "Rol Asignado ";
                    return RedirectToAction("Create");
                }
            }
            catch (Exception)
            {
                ViewBag.Mensaje = utils.mensajeError("Un usuario solo puede tener un Rol por Proyecto.");
            }

            ViewBag.PersonaID = new SelectList(db.Rol.Where(r => r.ProyectoID == Proyecto.ID).Select(r => r.Persona).OrderBy(p => p.Nombres).Distinct().ToList(), "ID", "NombreLista", rol.PersonaID);
            /*ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista");*/
            ViewBag.TipoRolID = new SelectList(db.TipoRol.OrderBy(r => r.Nombre).ToList(), "ID", "Nombre");
            return View(rol);
        }
        
        //
        // GET: /Rol/Edit/5
 
        public ActionResult Edit(int id)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Rol rol = db.Rol.Find(id);
            ViewBag.PersonaID = new SelectList(db.Rol.Where(r => r.ProyectoID == Proyecto.ID).Select(r => r.Persona).OrderBy(p => p.Nombres).Distinct().ToList(), "ID", "NombreLista", rol.PersonaID);
            /*ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista");*/
            ViewBag.TipoRolID = new SelectList(db.TipoRol.OrderBy(r => r.Nombre).ToList(), "ID", "Nombre",rol.TipoRolID );
            return View(rol);
        }

        //
        // POST: /Rol/Edit/5

        [HttpPost]
        public ActionResult Edit(Rol rol)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            rol.ProyectoID = Proyecto.ID;
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Usuario usuario = (Usuario)Session["Usuario"];

            if (ModelState.IsValid)
            {
                db.Entry(rol).State = EntityState.Modified;
                db.SaveChanges();
                string TiporolNombre = db.TipoRol.Find(rol.TipoRolID).Nombre;
                string PersonaNombreCompleto =  db.Persona.Find(rol.PersonaID).NombreCompleto;
                InicioLog log = new InicioLog();
                log.Tipo = "Responsabilidad Administrativa";
                log.ProyectoId = Proyecto.ID;
                log.Fecha = DateTime.Now;
                log.Mes = mes;
                log.Periodo = periodo;
                log.RegistroID = rol.ID;
                log.UsuarioID = usuario.ID;
                log.Descripcion = "Asignado : " + TiporolNombre + " " + PersonaNombreCompleto;
                db.InicioLog.Add(log);
                db.SaveChanges();

                TempData["Message"] = "Rol Asignado ";
                return RedirectToAction("Create");
            }
            ViewBag.PersonaID = new SelectList(db.Rol.Where(r => r.ProyectoID == Proyecto.ID).Select(r => r.Persona).OrderBy(p => p.Nombres).Distinct().ToList(), "ID", "NombreLista", rol.PersonaID);
            /*ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista");*/
            ViewBag.TipoRolID = new SelectList(db.TipoRol.OrderBy(r => r.Nombre).ToList(), "ID", "Nombre");
            return View(rol);
        }

        //
        // GET: /Rol/Delete/5
        /*
        public ActionResult Delete()
        {
            Rol rol = db.Rol.Find(id);
            return View(rol);
        }
        */
        //
        // POST: /Rol/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Usuario usuario = (Usuario)Session["Usuario"];
            Rol rol = db.Rol.Find(id);

            InicioLog log = new InicioLog();
            log.Tipo = "Responsabilidad Administrativa";
            log.ProyectoId = rol.ProyectoID;
            log.Fecha = DateTime.Now;
            log.Mes = mes;
            log.Periodo = periodo;
            log.RegistroID = rol.ID;
            log.UsuarioID = usuario.ID;
            log.Descripcion = "Eliminado : " + rol.TipoRol.Nombre + " " + rol.Persona.NombreCompleto;
            db.InicioLog.Add(log);
            db.SaveChanges();

            db.Rol.Remove(rol);
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