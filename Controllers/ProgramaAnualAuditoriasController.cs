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
    public class ProgramaAnualAuditoriasController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /ProgramaAnualAuditorias/

        public ViewResult Index()
        {
            Persona persona = (Persona)Session["Persona"];
            var programaanualauditorias = db.ProgramaAnualAuditorias.Include(p => p.Proyecto).Include(p => p.Auditor).Where(p => p.PersonaID == persona.ID).OrderByDescending(i => i.FechaProgramada);
            ViewBag.Auditor = persona.NombreCompleto;
            return View(programaanualauditorias.ToList());
        }
        public ViewResult ListadoAuditoria()
        {
            int filtro = int.Parse(Session["Filtro"].ToString());  
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            
            if (filtro == 1)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).OrderBy(p => p.CodCodeni).ToList();
            }
            else
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            }
            ViewBag.PersonaID = db.Persona.ToList();   //new SelectList(db.Rol.Where(r => r.ProyectoID == Proyecto.ID).Select(r => r.Persona).OrderBy(p => p.Nombres).Distinct().ToList(), "ID", "NombreCompleto");

            var programaanualauditorias = db.ProgramaAnualAuditorias.Include(p => p.Proyecto).Include(p => p.Auditor).OrderByDescending(i => i.FechaProgramada);
            //ViewBag.Auditor = persona.NombreCompleto;
            return View(programaanualauditorias.ToList());
        }

        //Post
        [HttpPost]
        public ViewResult ListadoAuditoria(ProgramaAnualAuditorias frmprogramaanualauditorias)
        {
            int filtro = int.Parse(Session["Filtro"].ToString()); 
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
        
            if (filtro == 1)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).OrderBy(p => p.CodCodeni).ToList();
            }
            else
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            }
            ViewBag.PersonaID = db.Persona.ToList();   //new SelectList(db.Rol.Where(r => r.ProyectoID == Proyecto.ID).Select(r => r.Persona).OrderBy(p => p.Nombres).Distinct().ToList(), "ID", "NombreCompleto");
            
            int Perid = frmprogramaanualauditorias.PersonaID;
            int ProyID = frmprogramaanualauditorias.ProyectoID;
            var programaanualauditorias = db.ProgramaAnualAuditorias.Include(p => p.Proyecto).Include(p => p.Auditor).OrderByDescending(i => i.FechaProgramada); ;
            if (Perid != 0 && ProyID != 0)
            {
                programaanualauditorias = db.ProgramaAnualAuditorias.Include(p => p.Proyecto).Include(p => p.Auditor).Where(p => p.PersonaID == Perid).Where(p => p.ProyectoID == ProyID).OrderByDescending(i => i.FechaProgramada); 
            }
            else{
                if (Perid != 0 && ProyID == 0) {
                    programaanualauditorias = db.ProgramaAnualAuditorias.Include(p => p.Proyecto).Include(p => p.Auditor).Where(p => p.PersonaID == Perid).OrderByDescending(i => i.FechaProgramada); 
                }
                if (Perid == 0 && ProyID != 0)
                {
                    programaanualauditorias = db.ProgramaAnualAuditorias.Include(p => p.Proyecto).Include(p => p.Auditor).Where(p => p.ProyectoID == ProyID).OrderByDescending(i => i.FechaProgramada); 
                }
            }

           
            //ViewBag.Auditor = persona.NombreCompleto;
            return View(programaanualauditorias.ToList());
        }

        //
        // GET: /ProgramaAnualAuditorias/Details/5

        public ViewResult Details(int id)
        {
            ProgramaAnualAuditorias programaanualauditorias = db.ProgramaAnualAuditorias.Find(id);
            return View(programaanualauditorias);
        }

        //
        // GET: /ProgramaAnualAuditorias/Create

        public ActionResult Create()
        {
            Persona Persona = (Persona)Session["Persona"];
            List<Proyecto> proyecto = new List<Proyecto>();
            //proyecto = db.Rol.Where(r => r.PersonaID == persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).Where(r => r.Cerrado == null).Distinct().ToList();
            Usuario usuario = (Usuario)Session["Usuario"];

            if (usuario.esAdministrador)
            {
                proyecto = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            }
            else if (usuario.esSupervisor)
            {
                proyecto = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }
            else
            {
                proyecto = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }

            ViewBag.ProyectoID = new SelectList(proyecto, "ID", "NombreLista");
            return View();
        } 

        //
        // POST: /ProgramaAnualAuditorias/Create

        [HttpPost]
        public ActionResult Create(ProgramaAnualAuditorias programaanualauditorias)
        {
            Persona persona = (Persona)Session["Persona"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            programaanualauditorias.PersonaID = persona.ID;
            int numero = 1;

            InformeAuditoria InformeAuditoria = new InformeAuditoria();
            InformeAuditoria.AuditoraID = persona.ID;
            InformeAuditoria.ProyectoID = programaanualauditorias.ProyectoID;// Proyecto.ID;
            InformeAuditoria.FechaAuditoria = programaanualauditorias.FechaProgramada;
            InformeAuditoria.FechaInforme = programaanualauditorias.FechaProgramada;
            InformeAuditoria.MesDesde = programaanualauditorias.FechaProgramada.Month;
            InformeAuditoria.MesHasta = programaanualauditorias.FechaProgramada.Month;
            InformeAuditoria.PeriodoDesde = programaanualauditorias.FechaProgramada.Year;
            InformeAuditoria.PeriodoHasta = programaanualauditorias.FechaProgramada.Year;
        
            InformeAuditoria.TipoAuditoriaID = 1;

            db.InformeAuditoria.Add(InformeAuditoria);
            db.SaveChanges();

        //    programaanualauditorias.InformeAuditoriaID = null;
            programaanualauditorias.InformeAuditoriaID = InformeAuditoria.ID;

            if (db.ProgramaAnualAuditorias.Where(m => m.ProyectoID == programaanualauditorias.ProyectoID).Count() > 0)
            {
                numero = db.ProgramaAnualAuditorias.Where(m => m.ProyectoID == programaanualauditorias.ProyectoID).Max(a => a.Numero) + 1;
            }

            programaanualauditorias.Numero = numero;
            //programaanualauditorias.InformeAuditoriaID = null;

            if (ModelState.IsValid)
            {
                db.ProgramaAnualAuditorias.Add(programaanualauditorias);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            

            List<Proyecto> proyecto = new List<Proyecto>();
            proyecto = db.Rol.Where(r => r.PersonaID == persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).Where(r => r.Cerrado == null).Distinct().ToList();
            ViewBag.ProyectoID = new SelectList(proyecto, "ID", "NombreLista", programaanualauditorias.ProyectoID);
            return View(programaanualauditorias);
        }
        
        //
        // GET: /ProgramaAnualAuditorias/Edit/5
 
        public ActionResult Edit(int id)
        {
            ProgramaAnualAuditorias programaanualauditorias = db.ProgramaAnualAuditorias.Find(id);
            Persona persona = (Persona)Session["Persona"];
            List<Proyecto> proyecto = new List<Proyecto>();
            proyecto = db.Rol.Where(r => r.PersonaID == persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).Where(r => r.Cerrado == null).Distinct().ToList();
            ViewBag.ProyectoID = new SelectList(proyecto, "ID", "NombreLista", programaanualauditorias.ProyectoID);
            return View(programaanualauditorias);
        }

        //
        // POST: /ProgramaAnualAuditorias/Edit/5

        [HttpPost]
        public ActionResult Edit(ProgramaAnualAuditorias programaanualauditorias)
        {
            Persona persona = (Persona)Session["Persona"];
            programaanualauditorias.PersonaID = persona.ID;

            if (ModelState.IsValid)
            {
                db.Entry(programaanualauditorias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<Proyecto> proyecto = new List<Proyecto>();
            proyecto = db.Rol.Where(r => r.PersonaID == persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).Where(r => r.Cerrado == null).Distinct().ToList();
            ViewBag.ProyectoID = new SelectList(proyecto, "ID", "NombreLista", programaanualauditorias.ProyectoID);
            return View(programaanualauditorias);
        }

        //
        // GET: /ProgramaAnualAuditorias/Delete/5
 
        public ActionResult Delete(int id)
        {
            ProgramaAnualAuditorias programaanualauditorias = db.ProgramaAnualAuditorias.Find(id);
            return View(programaanualauditorias);
        }

        //
        // POST: /ProgramaAnualAuditorias/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ProgramaAnualAuditorias programaanualauditorias = db.ProgramaAnualAuditorias.Find(id);
            db.ProgramaAnualAuditorias.Remove(programaanualauditorias);
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