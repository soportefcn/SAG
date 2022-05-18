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
    public class AuditoriasController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Auditorias/

        public ViewResult Index(string q = "")
        {
            var auditoria = db.Auditoria.Include(a => a.Rol);
            return View(auditoria.ToList());
        }

        //
        // GET: /Auditorias/Details/5

        public ViewResult Details(int id)
        {
            Auditoria auditoria = db.Auditoria.Find(id);
            return View(auditoria);
        }

        //
        // GET: /Auditorias/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Auditorias/Create

        [HttpPost]
        public ActionResult Create(Auditoria auditoria)
        {
            Rol Rol = (Rol)Session["Rol"];
            auditoria.RolID = Rol.ID;

            if (ModelState.IsValid)
            {
                db.Auditoria.Add(auditoria);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(auditoria);
        }

        //
        // GET: /Auditorias/Edit/5

        public ActionResult Edit(int id)
        {
            Auditoria auditoria = db.Auditoria.Find(id);
            return View(auditoria);
        }

        //
        // POST: /Auditorias/Edit/5

        [HttpPost]
        public ActionResult Edit(Auditoria auditoria)
        {
            Rol Rol = (Rol)Session["Rol"];
            auditoria.RolID = Rol.ID;

            if (ModelState.IsValid)
            {
                db.Entry(auditoria).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(auditoria);
        }


        //
        // POST: /Auditorias/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Auditoria auditoria = db.Auditoria.Find(id);
            db.Auditoria.Remove(auditoria);
            db.SaveChanges();
            return RedirectToAction("Create");
        }



        public ActionResult PlandeTrabajo()
        {
            Persona persona = (Persona)Session["Persona"];

            try
            {
                PlanTrabajoAuditoria PlanTrabajo = db.PlanTrabajoAuditoria.Where(p => p.PersonaID == persona.ID).Single();
                ViewBag.ListadoDocumentos = PlanTrabajo.Documentos.Split(',').ToList();
                ViewBag.Fecha = PlanTrabajo.Fecha.ToShortDateString();
            }
            catch (Exception)
            {
                PlanTrabajoAuditoria PlanTrabajo = new PlanTrabajoAuditoria();
                ViewBag.ListadoDocumentos = new List<string>(); ;
                ViewBag.Fecha = DateTime.Now.ToShortDateString();
            }

            ViewBag.Objetivos = db.AuditoriasObjetivo.OrderBy(a => a.Orden).ToList();
            ViewBag.Documentos = db.AuditoriasDocumento.OrderBy(a => a.Orden).ToList();
            ViewBag.Metodologia = db.AuditoriasMetodologia.OrderBy(a => a.Orden).ToList();
            ViewBag.Auditorias = db.ProgramaAnualAuditorias.Include(p => p.Proyecto).Include(p => p.Auditor).Where(p => p.PersonaID == persona.ID).OrderBy(p => p.FechaProgramada).ToList();
            ViewBag.Auditor = persona.NombreCompleto;

            return View();
        }

        [HttpPost]
        public ActionResult PlandeTrabajo(FormCollection Form)
        {
            string Fecha = Form["Fecha"].ToString();
            List<string> Documentos = new List<string>();
            Persona persona = (Persona)Session["Persona"];

            foreach (var key in Form.AllKeys)
            {
                if (!key.Equals("Documento"))
                    continue;

                string DocumentoID = Form[key].ToString();
                Documentos.Add(DocumentoID);
            }

            try
            {
                PlanTrabajoAuditoria PlanTrabajo = db.PlanTrabajoAuditoria.Where(p => p.PersonaID == persona.ID).Single();
                PlanTrabajo.Documentos = String.Join(",", Documentos);
                PlanTrabajo.PersonaID = persona.ID;
                PlanTrabajo.Fecha = DateTime.Parse(Fecha);
                db.Entry(PlanTrabajo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                PlanTrabajoAuditoria PlanTrabajo = new PlanTrabajoAuditoria();
                PlanTrabajo.Documentos = String.Join(",", Documentos);
                PlanTrabajo.PersonaID = persona.ID;
                PlanTrabajo.Fecha = DateTime.Parse(Fecha);
                db.PlanTrabajoAuditoria.Add(PlanTrabajo);
                db.SaveChanges();
            }

            foreach (var key in Form.AllKeys)
            {
                if (!key.Contains("Observaciones_"))
                    continue;

                string[] Informacion = key.Split('_');
                string Observaciones = Form[key].ToString();

                ProgramaAnualAuditorias programa = db.ProgramaAnualAuditorias.Find(Int32.Parse(Informacion[1].ToString()));
                programa.Observaciones = Observaciones;
                db.Entry(programa).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("PlandeTrabajo");
        }

        public ActionResult Informe()
        {
            Persona persona = (Persona)Session["Persona"];
            Proyecto proyecto = (Proyecto)Session["Proyecto"];

            ViewBag.Metodologia = db.AuditoriasMetodologia.OrderBy(a => a.Orden).ToList();
            ViewBag.Documentos = db.AuditoriasDocumento.OrderBy(a => a.Orden).ToList();
            ViewBag.Objetivos = db.AuditoriasObjetivo.OrderBy(a => a.Orden).ToList();

            InformeAuditoria InformeAuditoria = new InformeAuditoria();

            if (Request.QueryString["Programa"] != null)
            {
                //try
                //{
                int ProgramaID = 0;
                if (Request.QueryString["Programa"] != null)
                {
                    ProgramaID = Int32.Parse(Request.QueryString["Programa"].ToString());
                    InformeAuditoria = db.ProgramaAnualAuditorias.Include(i => i.Informe).Where(i => i.ID == ProgramaID).Select(i => i.Informe).Single();
                }
                else
                {
                    InformeAuditoria = db.InformeAuditoria.Where(i => i.ProyectoID == proyecto.ID).OrderByDescending(i => i.ID).First();
                }

                ViewBag.Auditorias = db.InformeAuditoria.Where(i => i.ProyectoID == proyecto.ID).ToList();
                /*}
                catch (Exception)
                {
                    InformeAuditoria.MesDesde = DateTime.Now.Month;
                    InformeAuditoria.MesHasta = DateTime.Now.Month;
                }*/

                ViewBag.ProgramaID = Request.QueryString["Programa"].ToString();
            }

            try
            {
                ViewBag.Auditor = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == InformeAuditoria.ProyectoID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto; ;
            }
            catch (Exception)
            {
                ViewBag.Auditor = "No definido";
            }
            
            ViewBag.NombreProyecto = InformeAuditoria.Proyecto.Nombre;
            ViewBag.CodigoProyecto = InformeAuditoria.Proyecto.CodCodeni;
            ViewBag.Cobertura = InformeAuditoria.Proyecto.Convenio.NroPlazas;

            try
            {
                ViewBag.Director = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == InformeAuditoria.ProyectoID).Where(r => r.TipoRolID == 1).Single().Persona.NombreCompleto;
            }
            catch (Exception)
            {
                ViewBag.Director = "No definido";
            }

            ViewBag.ProyectoID = InformeAuditoria.ProyectoID;

         
            try
            {
                ViewBag.TipoAuditoriaID = new SelectList(db.TipoAuditoria, "ID", "Nombre", InformeAuditoria.TipoAuditoriaID);
            }
            catch (Exception)
            {
                ViewBag.TipoAuditoriaID = new SelectList(db.TipoAuditoria, "ID", "Nombre");
            }

            return View(InformeAuditoria);
        }

        public ActionResult ImprimirInforme()
        {
            Persona persona = (Persona)Session["Persona"];
            Proyecto proyecto = (Proyecto)Session["Proyecto"];

            ViewBag.Metodologia = db.AuditoriasMetodologia.OrderBy(a => a.Orden).ToList();
            ViewBag.Documentos = db.AuditoriasDocumento.OrderBy(a => a.Orden).ToList();
            ViewBag.Objetivos = db.AuditoriasObjetivo.OrderBy(a => a.Orden).ToList();

            InformeAuditoria InformeAuditoria = new InformeAuditoria();

            if (Request.QueryString["Programa"] != null)
            {
                //try
                //{
                int ProgramaID = 0;
                if (Request.QueryString["Programa"] != null)
                {
                    ProgramaID = Int32.Parse(Request.QueryString["Programa"].ToString());
                    InformeAuditoria = db.ProgramaAnualAuditorias.Include(i => i.Informe).Where(i => i.ID == ProgramaID).Select(i => i.Informe).Single();
                }
                else
                {
                    InformeAuditoria = db.InformeAuditoria.Where(i => i.ProyectoID == proyecto.ID).OrderByDescending(i => i.ID).First();
                }

                ViewBag.Auditorias = db.InformeAuditoria.Where(i => i.ProyectoID == proyecto.ID).ToList();
                /*}
                catch (Exception)
                {
                    InformeAuditoria.MesDesde = DateTime.Now.Month;
                    InformeAuditoria.MesHasta = DateTime.Now.Month;
                }*/

                ViewBag.ProgramaID = Request.QueryString["Programa"].ToString();
            }

            try
            {
                ViewBag.Auditor = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == InformeAuditoria.ProyectoID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto; ;
            }
            catch (Exception)
            {
                ViewBag.Auditor = "No definido";
            }

            ViewBag.NombreProyecto = InformeAuditoria.Proyecto.Nombre;
            ViewBag.CodigoProyecto = InformeAuditoria.Proyecto.CodCodeni;
            ViewBag.Cobertura = InformeAuditoria.Proyecto.Convenio.NroPlazas;

            try
            {
                ViewBag.Director = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == InformeAuditoria.ProyectoID).Where(r => r.TipoRolID == 1).Single().Persona.NombreCompleto;
            }
            catch (Exception)
            {
                ViewBag.Director = "No definido";
            }

            ViewBag.ProyectoID = InformeAuditoria.ProyectoID;

        

            try
            {
                ViewBag.TipoAuditoriaID = new SelectList(db.TipoAuditoria, "ID", "Nombre", InformeAuditoria.TipoAuditoriaID);
            }
            catch (Exception)
            {
                ViewBag.TipoAuditoriaID = new SelectList(db.TipoAuditoria, "ID", "Nombre");
            }

            return View(InformeAuditoria);
        }

        [HttpPost]
        public ActionResult Informe(InformeAuditoria InformeAuditoria)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            int AuditoraID = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == Proyecto.ID).Where(r => r.TipoRolID == 4).Single().Persona.ID;
            InformeAuditoria.AuditoraID = AuditoraID;
          


            //Response.Write(documentos.Count().ToString());
            //Response.Write(InformeAuditoria.Documentos);
            //Response.End();
            if (InformeAuditoria.ID == 0)
            {
                db.InformeAuditoria.Add(InformeAuditoria);
            }
            else
            {
                db.Entry(InformeAuditoria).State = EntityState.Modified;
            }

            db.SaveChanges();

            // Actualizar Programa con ID del Informe de Auditoria
            int ProgramaID = Int32.Parse(Request.Form["ProgramaID"].ToString());
            ProgramaAnualAuditorias Programa = db.ProgramaAnualAuditorias.Find(ProgramaID);
            Programa.InformeAuditoriaID = InformeAuditoria.ID;

            if (Programa.FechaReal == null)
            {
                Programa.FechaReal = DateTime.Now;
            }

            db.Entry(Programa).State = EntityState.Modified;
            db.SaveChanges();

            Persona persona = (Persona)Session["Persona"];
            ViewBag.Metodologia = db.AuditoriasMetodologia.OrderBy(a => a.Orden).ToList();
            ViewBag.Documentos = db.AuditoriasDocumento.OrderBy(a => a.Orden).ToList();
            ViewBag.Objetivos = db.AuditoriasObjetivo.OrderBy(a => a.Orden).ToList();
            ViewBag.Auditor = persona.NombreCompleto;

            return RedirectToAction("Informe", new { Programa = ProgramaID });
        }

        public ActionResult ReporteIndividual(int Informe = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            InformeAuditoria InformeAuditoria = new InformeAuditoria();

            if (Informe == 0)
            {
                InformeAuditoria = db.InformeAuditoria.Where(i => i.ProyectoID == Proyecto.ID).OrderByDescending(i => i.ID).First();
            }
            else
            {
                InformeAuditoria = db.InformeAuditoria.Find(Informe);
            }

            ViewBag.Proyecto = InformeAuditoria.Proyecto.NombreLista;
            ViewBag.Fecha = InformeAuditoria.FechaAuditoria.ToShortDateString();

            try
            {
                ViewBag.Director = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == InformeAuditoria.ProyectoID).Where(r => r.TipoRolID == 1).Single().Persona.NombreCompleto;
            }
            catch (Exception)
            {
                ViewBag.Director = "No hay Director definido para el Proyecto.";
            }

            try
            {
                ViewBag.NombreAuditora = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == InformeAuditoria.ProyectoID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto;
            }
            catch (Exception)
            {
                ViewBag.NombreAuditora = "No hay Auditor definido para el Proyecto.";
            }

            // AREA PERSONAL
            int ok = 0;
            decimal total = 7;
            int na = 0;

            if (InformeAuditoria.VI_B_1_Convenio != null &&  InformeAuditoria.VI_B_1_Convenio.Equals("S")) ok++;
            if (InformeAuditoria.VI_B_2_Remuneraciones != null &&  InformeAuditoria.VI_B_2_Remuneraciones.Equals("S")) ok++;
            if (InformeAuditoria.VI_B_3_Electronica != null &&  InformeAuditoria.VI_B_3_Electronica.Equals("S")) ok++;
            if (InformeAuditoria.VI_B_4_InduccionDirector != null && InformeAuditoria.VI_B_4_InduccionDirector.Equals("S")) ok++;
            if (InformeAuditoria.VI_B_5_Induccion2do != null && InformeAuditoria.VI_B_5_Induccion2do.Equals("S")) ok++;
            if (InformeAuditoria.VI_B_6_InduccionSecretaria != null && InformeAuditoria.VI_B_6_InduccionSecretaria.Equals("S")) ok++;
            //if (InformeAuditoria.VI_B_7_Sueldo != null && InformeAuditoria.VI_B_7_Sueldo.Equals("S")) ok++;
            //if (InformeAuditoria.VI_B_8_Contrato != null && InformeAuditoria.VI_B_8_Contrato.Equals("S")) ok++;
            if (InformeAuditoria.VI_B_7_Sueldo != null &&  InformeAuditoria.VI_B_9_Bono.Equals("S")) ok++;
            //if (InformeAuditoria.VI_B_10_Descuentos != null && InformeAuditoria.VI_B_10_Descuentos.Equals("S")) ok++;

            if (InformeAuditoria.VI_B_1_Convenio != null && InformeAuditoria.VI_B_1_Convenio.Equals("A")) na++;
            if (InformeAuditoria.VI_B_2_Remuneraciones != null && InformeAuditoria.VI_B_2_Remuneraciones.Equals("A")) na++;
            if (InformeAuditoria.VI_B_3_Electronica != null && InformeAuditoria.VI_B_3_Electronica.Equals("A")) na++;
            if (InformeAuditoria.VI_B_4_InduccionDirector != null && InformeAuditoria.VI_B_4_InduccionDirector.Equals("A")) na++;
            if (InformeAuditoria.VI_B_5_Induccion2do != null && InformeAuditoria.VI_B_5_Induccion2do.Equals("A")) na++;
            if (InformeAuditoria.VI_B_6_InduccionSecretaria != null && InformeAuditoria.VI_B_6_InduccionSecretaria.Equals("A")) na++;
            //if (InformeAuditoria.VI_B_7_Sueldo != null && InformeAuditoria.VI_B_7_Sueldo.Equals("S")) ok++;
            //if (InformeAuditoria.VI_B_8_Contrato != null && InformeAuditoria.VI_B_8_Contrato.Equals("S")) ok++;
            if (InformeAuditoria.VI_B_9_Bono != null && InformeAuditoria.VI_B_9_Bono.Equals("S")) ok++;


            ViewBag.CumplimientoPersonal = (ok / (total - na))*100;

            // Contratos
            int okadmin = 0;
            decimal totaladmin = 0;
            ok = 0;
            total = 23;
            na = 0;

            if (InformeAuditoria.VI_C_1a1_Arriendo != null && InformeAuditoria.VI_C_1a1_Arriendo.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1a2_Copia != null && InformeAuditoria.VI_C_1a2_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1a3_Recibos != null && InformeAuditoria.VI_C_1a3_Recibos.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1a4_Termino != null && InformeAuditoria.VI_C_1a4_Termino.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1a5_Nombre != null && InformeAuditoria.VI_C_1a5_Nombre.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b1_Copia != null && InformeAuditoria.VI_C_1b1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b2_Escritura != null && InformeAuditoria.VI_C_1b2_Escritura.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1b3_Devolucion != null && InformeAuditoria.VI_C_1b3_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b4_Registro != null && InformeAuditoria.VI_C_1b4_Registro.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b5_Llamadas != null && InformeAuditoria.VI_C_1b5_Llamadas.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b6_Evidencia != null && InformeAuditoria.VI_C_1b6_Evidencia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c1_Copia != null && InformeAuditoria.VI_C_1c1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c2_Escritura != null && InformeAuditoria.VI_C_1c2_Escritura.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c3_Asignados != null && InformeAuditoria.VI_C_1c3_Asignados.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1c4_Devolucion != null && InformeAuditoria.VI_C_1c4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c5_Controles != null && InformeAuditoria.VI_C_1c5_Controles.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1d1_Copia != null && InformeAuditoria.VI_C_1d1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1d2_Escritura != null && InformeAuditoria.VI_C_1d2_Escritura.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1d3_Equipo != null && InformeAuditoria.VI_C_1d3_Equipo.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1d4_Devolucion != null && InformeAuditoria.VI_C_1d4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1e1_Copia != null && InformeAuditoria.VI_C_1e1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1e2_Escritura != null && InformeAuditoria.VI_C_1e2_Escritura.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1e3_Equipos != null && InformeAuditoria.VI_C_1e3_Equipos.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1e4_Devolucion != null && InformeAuditoria.VI_C_1e4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f1_Copia != null && InformeAuditoria.VI_C_1f1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f2_Certificados != null && InformeAuditoria.VI_C_1f2_Certificados.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f3_Bitacoras != null && InformeAuditoria.VI_C_1f3_Bitacoras.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f4_Factura != null && InformeAuditoria.VI_C_1f4_Factura.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1g1_Copia != null && InformeAuditoria.VI_C_1g1_Copia.Equals("S")) ok++;

            if (InformeAuditoria.VI_C_1a1_Arriendo != null &&  InformeAuditoria.VI_C_1a1_Arriendo.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1a2_Copia != null && InformeAuditoria.VI_C_1a2_Copia.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1a3_Recibos != null && InformeAuditoria.VI_C_1a3_Recibos.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1a4_Termino != null && InformeAuditoria.VI_C_1a4_Termino.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1a5_Nombre != null && InformeAuditoria.VI_C_1a5_Nombre.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1b1_Copia != null && InformeAuditoria.VI_C_1b1_Copia.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1b2_Escritura != null && InformeAuditoria.VI_C_1b2_Escritura.Equals("A")) na++;
            //if (InformeAuditoria.VI_C_1b3_Devolucion != null && InformeAuditoria.VI_C_1b3_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b4_Registro != null && InformeAuditoria.VI_C_1b4_Registro.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1b5_Llamadas != null && InformeAuditoria.VI_C_1b5_Llamadas.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1b6_Evidencia != null && InformeAuditoria.VI_C_1b6_Evidencia.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1c1_Copia != null && InformeAuditoria.VI_C_1c1_Copia.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1c2_Escritura != null && InformeAuditoria.VI_C_1c2_Escritura.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1c3_Asignados != null && InformeAuditoria.VI_C_1c3_Asignados.Equals("A")) na++;
            //if (InformeAuditoria.VI_C_1c4_Devolucion != null && InformeAuditoria.VI_C_1c4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c5_Controles != null && InformeAuditoria.VI_C_1c5_Controles.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1d1_Copia != null && InformeAuditoria.VI_C_1d1_Copia.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1d2_Escritura != null && InformeAuditoria.VI_C_1d2_Escritura.Equals("A")) na++;
            //if (InformeAuditoria.VI_C_1d3_Equipo != null && InformeAuditoria.VI_C_1d3_Equipo.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1d4_Devolucion != null && InformeAuditoria.VI_C_1d4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1e1_Copia != null && InformeAuditoria.VI_C_1e1_Copia.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1e2_Escritura != null && InformeAuditoria.VI_C_1e2_Escritura.Equals("A")) na++;
            //if (InformeAuditoria.VI_C_1e3_Equipos != null && InformeAuditoria.VI_C_1e3_Equipos.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1e4_Devolucion != null && InformeAuditoria.VI_C_1e4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f1_Copia != null && InformeAuditoria.VI_C_1f1_Copia.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1f2_Certificados != null && InformeAuditoria.VI_C_1f2_Certificados.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1f3_Bitacoras != null && InformeAuditoria.VI_C_1f3_Bitacoras.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1f4_Factura != null && InformeAuditoria.VI_C_1f4_Factura.Equals("A")) na++;
            if (InformeAuditoria.VI_C_1g1_Copia != null && InformeAuditoria.VI_C_1g1_Copia.Equals("A")) na++;
  
            totaladmin = total;
            okadmin = ok;
            ViewBag.CumplimientoContrato = (ok / (total - na)) * 100;

            // Inventario
            ok = 0;
            total = 16;
            na = 0;
            if (InformeAuditoria.VI_C_2a1_General != null && InformeAuditoria.VI_C_2a1_General.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2a2_Firmado != null && InformeAuditoria.VI_C_2a2_Firmado.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2a3_Copia != null && InformeAuditoria.VI_C_2a3_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2a4_Distribucion != null && InformeAuditoria.VI_C_2a4_Distribucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2a5_Activo != null && InformeAuditoria.VI_C_2a5_Activo.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2a6_Robos != null && InformeAuditoria.VI_C_2a6_Robos.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2a7_Informado != null && InformeAuditoria.VI_C_2a7_Informado.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2b1_Hoja != null && InformeAuditoria.VI_C_2b1_Hoja.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2b2_Copia != null && InformeAuditoria.VI_C_2b2_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2b3_Actualizadas != null && InformeAuditoria.VI_C_2b3_Actualizadas.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2c1_Pendientes != null && InformeAuditoria.VI_C_2c1_Pendientes.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2c2_NoIncorporadas != null && InformeAuditoria.VI_C_2c2_NoIncorporadas.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2d1_Pendientes != null && InformeAuditoria.VI_C_2d1_Pendientes.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2d2_NoIncorporadas != null && InformeAuditoria.VI_C_2d2_NoIncorporadas.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2e1_Pendientes != null && InformeAuditoria.VI_C_2e1_Pendientes.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_2e2_NoIncorporadas != null && InformeAuditoria.VI_C_2e2_NoIncorporadas.Equals("S")) ok++;

            if (InformeAuditoria.VI_C_2a1_General != null && InformeAuditoria.VI_C_2a1_General.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2a2_Firmado != null && InformeAuditoria.VI_C_2a2_Firmado.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2a3_Copia != null && InformeAuditoria.VI_C_2a3_Copia.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2a4_Distribucion != null && InformeAuditoria.VI_C_2a4_Distribucion.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2a5_Activo != null && InformeAuditoria.VI_C_2a5_Activo.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2a6_Robos != null && InformeAuditoria.VI_C_2a6_Robos.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2a7_Informado != null && InformeAuditoria.VI_C_2a7_Informado.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2b1_Hoja != null && InformeAuditoria.VI_C_2b1_Hoja.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2b2_Copia != null && InformeAuditoria.VI_C_2b2_Copia.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2b3_Actualizadas != null && InformeAuditoria.VI_C_2b3_Actualizadas.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2c1_Pendientes != null && InformeAuditoria.VI_C_2c1_Pendientes.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2c2_NoIncorporadas != null && InformeAuditoria.VI_C_2c2_NoIncorporadas.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2d1_Pendientes != null && InformeAuditoria.VI_C_2d1_Pendientes.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2d2_NoIncorporadas != null && InformeAuditoria.VI_C_2d2_NoIncorporadas.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2e1_Pendientes != null && InformeAuditoria.VI_C_2e1_Pendientes.Equals("A")) ok++;
            if (InformeAuditoria.VI_C_2e2_NoIncorporadas != null && InformeAuditoria.VI_C_2e2_NoIncorporadas.Equals("A")) ok++;

            ViewBag.CumplimientoInventario = (ok / (total - na)) * 100;
            totaladmin = totaladmin + total;
            okadmin = okadmin + ok;

            // Control presupuestario
            ok = 0;
            total = 3;

            if (InformeAuditoria.VI_C_3a_Control != null && InformeAuditoria.VI_C_3a_Control.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_3b_Existe != null && InformeAuditoria.VI_C_3b_Existe.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_3c_Director != null && InformeAuditoria.VI_C_3c_Director.Equals("S")) ok++;

            ViewBag.CumplimientoPresupuesto = (ok / total) * 100;
            totaladmin = totaladmin + total;
            okadmin = okadmin + ok;

            // Control de Bodega
            if (InformeAuditoria.VI_C_5_Utiliza != null && InformeAuditoria.VI_C_5_Utiliza.Equals("S"))
            {
                ok = 0;
                total = 5;

                if (InformeAuditoria.VI_C_5a_Compras != null && InformeAuditoria.VI_C_5a_Compras.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_5b_Autorizado != null && InformeAuditoria.VI_C_5b_Autorizado.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_5c_Ordenes != null && InformeAuditoria.VI_C_5c_Ordenes.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_5d_Actualizados != null && InformeAuditoria.VI_C_5d_Actualizados.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_5e_Saldos != null && InformeAuditoria.VI_C_5e_Saldos.Equals("S")) ok++;

                ViewBag.CumplimientoBodega = (ok / total) * 100;
                totaladmin = totaladmin + total;
                okadmin = okadmin + ok;
            }

            ViewBag.CumplimientoAdministrativa = 100 * (okadmin) / (totaladmin);

            // AREA FINANCIEROS
            int ok_financieros = 0;
            decimal total_financieros = 0;

            //7. Provisión
            ok = 0;
            total = 1;
            if (InformeAuditoria.VI_7_a_Provisiona != null && InformeAuditoria.VI_7_a_Provisiona.Equals("S")) ok++;
            ViewBag.CumplimientoProvision = 100 * (ok) / (total);
            ok_financieros = ok_financieros + ok;
            total_financieros = total_financieros + total;

            //6. Retención 
            ok = 0;
            total = 8;
            na = 0;
            if (InformeAuditoria.VI_6_a_Secretaria != null && InformeAuditoria.VI_6_a_Secretaria.Equals("S")) ok++;
            if (InformeAuditoria.VI_6_b_Impuesto != null && InformeAuditoria.VI_6_b_Impuesto.Equals("S")) ok++;
            if (InformeAuditoria.VI_6_c_Declaraciones != null && InformeAuditoria.VI_6_c_Declaraciones.Equals("S")) ok++;
            if (InformeAuditoria.VI_6_d_Plazo != null && InformeAuditoria.VI_6_d_Plazo.Equals("S")) ok++;
            if (InformeAuditoria.VI_6_e_Formulario29 != null && InformeAuditoria.VI_6_e_Formulario29.Equals("S")) ok++;
            if (InformeAuditoria.VI_6_f_Autocuidado != null && InformeAuditoria.VI_6_f_Autocuidado.Equals("S")) ok++;
            if (InformeAuditoria.VI_6_g_Contrato != null && InformeAuditoria.VI_6_g_Contrato.Equals("S")) ok++;
            //if (InformeAuditoria.VI_6_h_Inconsistencias != null && InformeAuditoria.VI_6_h_Inconsistencias.Equals("S")) ok++;
            if (InformeAuditoria.VI_6_y_Rectificadas != null && InformeAuditoria.VI_6_y_Rectificadas.Equals("S")) ok++;

            if (InformeAuditoria.VI_6_a_Secretaria != null && InformeAuditoria.VI_6_a_Secretaria.Equals("A")) na++;
            if (InformeAuditoria.VI_6_b_Impuesto != null && InformeAuditoria.VI_6_b_Impuesto.Equals("A")) na++;
            if (InformeAuditoria.VI_6_c_Declaraciones != null && InformeAuditoria.VI_6_c_Declaraciones.Equals("A")) na++;
            if (InformeAuditoria.VI_6_d_Plazo != null && InformeAuditoria.VI_6_d_Plazo.Equals("A")) na++;
            if (InformeAuditoria.VI_6_e_Formulario29 != null && InformeAuditoria.VI_6_e_Formulario29.Equals("A")) na++;
            if (InformeAuditoria.VI_6_f_Autocuidado != null && InformeAuditoria.VI_6_f_Autocuidado.Equals("A")) na++;
            if (InformeAuditoria.VI_6_g_Contrato != null && InformeAuditoria.VI_6_g_Contrato.Equals("A")) na++;
            //if (InformeAuditoria.VI_6_h_Inconsistencias != null && InformeAuditoria.VI_6_h_Inconsistencias.Equals("S")) ok++;
            if (InformeAuditoria.VI_6_y_Rectificadas != null && InformeAuditoria.VI_6_y_Rectificadas.Equals("A")) na++;
            
            ViewBag.CumplimientoRetencion = 100 * (ok) / (total - na);
            ok_financieros = ok_financieros + ok;
            total_financieros = total_financieros + total;

            //5. Rendiciones 
            ok = 0;
            total = 2;
            if (InformeAuditoria.VI_5_a_Direcciones != null && InformeAuditoria.VI_5_a_Direcciones.Equals("S")) ok++;
            if (InformeAuditoria.VI_5_b_Firma != null && InformeAuditoria.VI_5_b_Firma.Equals("S")) ok++;
            ViewBag.CumplimientoRendiciones = 100 * (ok) / (total);
            ok_financieros = ok_financieros + ok;
            total_financieros = +total;

            //4. Programa 
            ok = 0;
            total = 3;
            if (InformeAuditoria.VI_4_a_Programas != null && InformeAuditoria.VI_4_a_Programas.Equals("S")) ok++;
            if (InformeAuditoria.VI_4_b_Elaborados != null && InformeAuditoria.VI_4_b_Elaborados.Equals("S")) ok++;
            if (InformeAuditoria.VI_4_c_Firmas != null && InformeAuditoria.VI_4_c_Firmas.Equals("S")) ok++;
            ViewBag.CumplimientoPrograma = 100 * (ok) / (total);
            ok_financieros = ok_financieros + ok;
            total_financieros = total_financieros + total;

            //3. Fondo 
            ok = 0;
            total = 13;
            na = 0;

             if (InformeAuditoria.VI_3_a_Monto != null && InformeAuditoria.VI_3_a_Monto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_b_Arqueo != null && InformeAuditoria.VI_3_b_Arqueo.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_c_Firma != null && InformeAuditoria.VI_3_c_Firma.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_d_Efecto != null && InformeAuditoria.VI_3_d_Efecto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_e_Reposicion != null && InformeAuditoria.VI_3_e_Reposicion.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_f_Proyecto != null && InformeAuditoria.VI_3_f_Proyecto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_g_Reembolsos != null && InformeAuditoria.VI_3_g_Reembolsos.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_h_Boletas != null && InformeAuditoria.VI_3_h_Boletas.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_i_Superiores != null && InformeAuditoria.VI_3_i_Superiores.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_j_Combustibles != null && InformeAuditoria.VI_3_j_Combustibles.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_k_Planillas != null && InformeAuditoria.VI_3_k_Planillas.Equals("S")) ok++;
                //if (InformeAuditoria.VI_3_l_Actividades != null && InformeAuditoria.VI_3_l_Actividades.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_m_Movilizacion != null && InformeAuditoria.VI_3_m_Movilizacion.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_n_Interurbana != null && InformeAuditoria.VI_3_n_Interurbana.Equals("S")) ok++;

                if (InformeAuditoria.VI_3_a_Monto != null && InformeAuditoria.VI_3_a_Monto.Equals("A")) na++;
                if (InformeAuditoria.VI_3_b_Arqueo != null && InformeAuditoria.VI_3_b_Arqueo.Equals("A")) na++;
                if (InformeAuditoria.VI_3_c_Firma != null && InformeAuditoria.VI_3_c_Firma.Equals("A")) na++;
                if (InformeAuditoria.VI_3_d_Efecto != null && InformeAuditoria.VI_3_d_Efecto.Equals("A")) na++;
                if (InformeAuditoria.VI_3_e_Reposicion != null && InformeAuditoria.VI_3_e_Reposicion.Equals("A")) na++;
                if (InformeAuditoria.VI_3_f_Proyecto != null && InformeAuditoria.VI_3_f_Proyecto.Equals("A")) na++;
                if (InformeAuditoria.VI_3_g_Reembolsos != null && InformeAuditoria.VI_3_g_Reembolsos.Equals("A")) na++;
                if (InformeAuditoria.VI_3_h_Boletas != null && InformeAuditoria.VI_3_h_Boletas.Equals("A")) na++;
                if (InformeAuditoria.VI_3_i_Superiores != null && InformeAuditoria.VI_3_i_Superiores.Equals("A")) na++;
                if (InformeAuditoria.VI_3_j_Combustibles != null && InformeAuditoria.VI_3_j_Combustibles.Equals("A")) na++;
                if (InformeAuditoria.VI_3_k_Planillas != null && InformeAuditoria.VI_3_k_Planillas.Equals("A")) na++;
                //if (InformeAuditoria.VI_3_l_Actividades != null && InformeAuditoria.VI_3_l_Actividades.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_m_Movilizacion != null && InformeAuditoria.VI_3_m_Movilizacion.Equals("A")) na++;
                if (InformeAuditoria.VI_3_n_Interurbana != null && InformeAuditoria.VI_3_n_Interurbana.Equals("A")) na++;
                ViewBag.CumplimientoFondo = 100 * (ok) / (total - na);

            ok_financieros = ok_financieros + ok;
            total_financieros = total_financieros + total;

            //2. Comprobantes 
            ok = 0;
            total = 7;
            na = 0;
            if (InformeAuditoria.VI_2_a_Coincide != null && InformeAuditoria.VI_2_a_Coincide.Equals("S")) ok++;
            if (InformeAuditoria.VI_2_b_Glosa != null && InformeAuditoria.VI_2_b_Glosa.Equals("S")) ok++;
            if (InformeAuditoria.VI_2_c_Firma != null && InformeAuditoria.VI_2_c_Firma.Equals("S")) ok++;
            if (InformeAuditoria.VI_2_d_Egresos != null && InformeAuditoria.VI_2_d_Egresos.Equals("S")) ok++;
            if (InformeAuditoria.VI_2_e_EgresosNoConforme != null && InformeAuditoria.VI_2_e_EgresosNoConforme.Equals("S")) ok++;
            if (InformeAuditoria.VI_2_f_SinDoc != null && InformeAuditoria.VI_2_f_SinDoc.Equals("S")) ok++;
            if (InformeAuditoria.VI_2_g_Envia != null && InformeAuditoria.VI_2_g_Envia.Equals("S")) ok++;

            if (InformeAuditoria.VI_2_a_Coincide != null && InformeAuditoria.VI_2_a_Coincide.Equals("A")) na++;
            if (InformeAuditoria.VI_2_b_Glosa != null && InformeAuditoria.VI_2_b_Glosa.Equals("A")) na++;
            if (InformeAuditoria.VI_2_c_Firma != null && InformeAuditoria.VI_2_c_Firma.Equals("A")) na++;
            if (InformeAuditoria.VI_2_d_Egresos != null && InformeAuditoria.VI_2_d_Egresos.Equals("A")) na++;
            if (InformeAuditoria.VI_2_e_EgresosNoConforme != null && InformeAuditoria.VI_2_e_EgresosNoConforme.Equals("A")) na++;
            if (InformeAuditoria.VI_2_f_SinDoc != null && InformeAuditoria.VI_2_f_SinDoc.Equals("A")) na++;
            if (InformeAuditoria.VI_2_g_Envia != null && InformeAuditoria.VI_2_g_Envia.Equals("A")) na++;
            ViewBag.CumplimientoComprobantes = 100 * (ok) / (total - na);

            ok_financieros = ok_financieros + ok;
            total_financieros = total_financieros + total;

            //1. Libro
            ok = 0;
            total = 11;
            na = 0;
            if (InformeAuditoria.VI_1_b_Emitido != null && InformeAuditoria.VI_1_b_Emitido.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_b_Archivo != null && InformeAuditoria.VI_1_b_Archivo.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_b_Firmado != null && InformeAuditoria.VI_1_b_Firmado.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_b_Correlativo != null && InformeAuditoria.VI_1_b_Correlativo.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_b_Cheques != null && InformeAuditoria.VI_1_b_Cheques.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_c_Cartolas != null && InformeAuditoria.VI_1_c_Cartolas.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_c_EnviaCartolas != null && InformeAuditoria.VI_1_c_EnviaCartolas.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_d_Suma != null && InformeAuditoria.VI_1_d_Suma.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_d_Archivadas != null && InformeAuditoria.VI_1_d_Archivadas.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_d_Cheques != null && InformeAuditoria.VI_1_d_Cheques.Equals("S")) ok++;
            if (InformeAuditoria.VI_1_d_Firma != null && InformeAuditoria.VI_1_d_Firma.Equals("S")) ok++;

            if (InformeAuditoria.VI_1_b_Emitido != null && InformeAuditoria.VI_1_b_Emitido.Equals("A")) na++;
            if (InformeAuditoria.VI_1_b_Archivo != null && InformeAuditoria.VI_1_b_Archivo.Equals("A")) na++;
            if (InformeAuditoria.VI_1_b_Firmado != null && InformeAuditoria.VI_1_b_Firmado.Equals("A")) na++;
            if (InformeAuditoria.VI_1_b_Correlativo != null && InformeAuditoria.VI_1_b_Correlativo.Equals("A")) na++;
            if (InformeAuditoria.VI_1_b_Cheques != null && InformeAuditoria.VI_1_b_Cheques.Equals("A")) na++;
            if (InformeAuditoria.VI_1_c_Cartolas != null && InformeAuditoria.VI_1_c_Cartolas.Equals("A")) na++;
            if (InformeAuditoria.VI_1_c_EnviaCartolas != null && InformeAuditoria.VI_1_c_EnviaCartolas.Equals("A")) na++;
            if (InformeAuditoria.VI_1_d_Suma != null && InformeAuditoria.VI_1_d_Suma.Equals("A")) na++;
            if (InformeAuditoria.VI_1_d_Archivadas != null && InformeAuditoria.VI_1_d_Archivadas.Equals("A")) na++;
            if (InformeAuditoria.VI_1_d_Cheques != null && InformeAuditoria.VI_1_d_Cheques.Equals("A")) na++;
            if (InformeAuditoria.VI_1_d_Firma != null && InformeAuditoria.VI_1_d_Firma.Equals("A")) na++;

            ViewBag.CumplimientoLibro = 100 * (ok) / (total - na);

            ok_financieros = ok_financieros + ok;
            total_financieros = total_financieros + total;

            ViewBag.CumplimientoFinanciero = 100 * (ok_financieros) / (total_financieros);

            return View(InformeAuditoria);
        }


        public ActionResult ReporteGrupal(int DesdeMes = 0, int DesdePeriodo = 0, int HastaMes = 0, int HastaPeriodo = 0, int tipoProyectoID = 0)
        {
            // Se filtra de acuerdo al tipo de proyecto
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            if (DesdeMes < 1 || DesdePeriodo < 1 || HastaMes < 1 || HastaPeriodo < 1)
            {
                DesdeMes = DateTime.Now.Month;
                DesdePeriodo = DateTime.Now.Year - 1;
                HastaMes = DateTime.Now.Month;
                HastaPeriodo = DateTime.Now.Year;
            }

            ViewBag.DesdeMes = DesdeMes;
            ViewBag.DesdePeriodo = DesdePeriodo;
            ViewBag.HastaMes = HastaMes;
            ViewBag.HastaPeriodo = HastaPeriodo;

            if (tipoProyectoID == 0)
            {
                tipoProyectoID = db.TipoProyecto.OrderBy(i => i.Nombre).Take(1).Single().ID;
            }

            ViewBag.TipoProyectoID = new SelectList(db.TipoProyecto.OrderBy(i => i.Nombre), "ID", "NombreListaCompleto", tipoProyectoID);

            try
            {
                ViewBag.NombreAuditora = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == Proyecto.ID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto;
            }
            catch (Exception)
            {
                ViewBag.NombreAuditora = "No hay Auditor definido para el Proyecto.";
            }

            int AuditoraID = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == Proyecto.ID).Where(r => r.TipoRolID == 4).Single().Persona.ID;
            List<InformeAuditoria> Informes = db.InformeAuditoria
                                            .Where(i => i.AuditoraID == AuditoraID
                                                && i.Proyecto.TipoProyectoID == tipoProyectoID
                                                && (i.PeriodoDesde > DesdePeriodo || (i.MesDesde >= DesdeMes && i.PeriodoDesde == DesdePeriodo))
                                                && (i.PeriodoHasta < HastaPeriodo || (i.MesHasta <= HastaMes && i.PeriodoHasta == HastaPeriodo))
                                                && (i.PeriodoDesde >= i.PeriodoHasta || (i.PeriodoDesde == i.PeriodoHasta && i.MesDesde <= i.MesHasta))
                                                )
                                            .ToList();

            decimal totalInformes = Informes.Count;
            ViewBag.TotalInformes = totalInformes;

            if (totalInformes < 1)
            {
                ViewBag.SinInfomes = 1;
                return View();
            }

            int obs_sename_superadas = 0;
            int obs_sename_pendientes = 0;
            int obs_codeni_superadas = 0;
            int obs_codeni_pendientes = 0;

            List<ItemInforme> personal = new List<ItemInforme>();

            // Administrativos
            List<ItemInforme> contratos = new List<ItemInforme>();
            List<ItemInforme> inventarios = new List<ItemInforme>();
            List<ItemInforme> presupuesto = new List<ItemInforme>();
            List<ItemInforme> bodega = new List<ItemInforme>();

            // Financieros
            List<ItemInforme> libro = new List<ItemInforme>();
            List<ItemInforme> comprobantes = new List<ItemInforme>();
            List<ItemInforme> fondo = new List<ItemInforme>();
            List<ItemInforme> programa = new List<ItemInforme>();
            List<ItemInforme> rendicion = new List<ItemInforme>();
            List<ItemInforme> retencion = new List<ItemInforme>();
            List<ItemInforme> provision = new List<ItemInforme>();

            foreach (var InformeAuditoria in Informes)
            {
                // AREA PERSONAL
                decimal total = 10;
                int ok = 0;
                if (InformeAuditoria.VI_B_1_Convenio != null && InformeAuditoria.VI_B_1_Convenio.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_2_Remuneraciones != null && InformeAuditoria.VI_B_2_Remuneraciones.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_3_Electronica != null && InformeAuditoria.VI_B_3_Electronica.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_4_InduccionDirector != null && InformeAuditoria.VI_B_4_InduccionDirector.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_5_Induccion2do != null && InformeAuditoria.VI_B_5_Induccion2do.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_6_InduccionSecretaria != null && InformeAuditoria.VI_B_6_InduccionSecretaria.Equals("S")) ok++;
                //if (InformeAuditoria.VI_B_7_Sueldo != null && InformeAuditoria.VI_B_7_Sueldo.Equals("S")) ok++;
                //if (InformeAuditoria.VI_B_8_Contrato != null && InformeAuditoria.VI_B_8_Contrato.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_9_Bono != null && InformeAuditoria.VI_B_9_Bono.Equals("S")) ok++;
                //if (InformeAuditoria.VI_B_10_Descuentos != null && InformeAuditoria.VI_B_10_Descuentos.Equals("S")) ok++;

                try
                {
                    ItemInforme item = personal.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = personal.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    personal.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    personal.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    personal.Add(item);
                }

                // Contratos
                int okadmin = 0;
                decimal totaladmin = 0;
                ok = 0;
                total = 29;

                if (InformeAuditoria.VI_C_1a1_Arriendo != null && InformeAuditoria.VI_C_1a1_Arriendo.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1a2_Copia != null && InformeAuditoria.VI_C_1a2_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1a3_Recibos != null && InformeAuditoria.VI_C_1a3_Recibos.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1a4_Termino != null && InformeAuditoria.VI_C_1a4_Termino.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1a5_Nombre != null && InformeAuditoria.VI_C_1a5_Nombre.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1b1_Copia != null && InformeAuditoria.VI_C_1b1_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1b2_Escritura != null && InformeAuditoria.VI_C_1b2_Escritura.Equals("S")) ok++;
                //if (InformeAuditoria.VI_C_1b3_Devolucion != null && InformeAuditoria.VI_C_1b3_Devolucion.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1b4_Registro != null && InformeAuditoria.VI_C_1b4_Registro.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1b5_Llamadas != null && InformeAuditoria.VI_C_1b5_Llamadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1b6_Evidencia != null && InformeAuditoria.VI_C_1b6_Evidencia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1c1_Copia != null && InformeAuditoria.VI_C_1c1_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1c2_Escritura != null && InformeAuditoria.VI_C_1c2_Escritura.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1c3_Asignados != null && InformeAuditoria.VI_C_1c3_Asignados.Equals("S")) ok++;
                //if (InformeAuditoria.VI_C_1c4_Devolucion != null && InformeAuditoria.VI_C_1c4_Devolucion.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1c5_Controles != null && InformeAuditoria.VI_C_1c5_Controles.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1d1_Copia != null && InformeAuditoria.VI_C_1d1_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1d2_Escritura != null && InformeAuditoria.VI_C_1d2_Escritura.Equals("S")) ok++;
                //if (InformeAuditoria.VI_C_1d3_Equipo != null && InformeAuditoria.VI_C_1d3_Equipo.Equals("S")) ok++;
                //if (InformeAuditoria.VI_C_1d4_Devolucion != null && InformeAuditoria.VI_C_1d4_Devolucion.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1e1_Copia != null && InformeAuditoria.VI_C_1e1_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1e2_Escritura != null && InformeAuditoria.VI_C_1e2_Escritura.Equals("S")) ok++;
                //if (InformeAuditoria.VI_C_1e3_Equipos != null && InformeAuditoria.VI_C_1e3_Equipos.Equals("S")) ok++;
                //if (InformeAuditoria.VI_C_1e4_Devolucion != null && InformeAuditoria.VI_C_1e4_Devolucion.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1f1_Copia != null && InformeAuditoria.VI_C_1f1_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1f2_Certificados != null && InformeAuditoria.VI_C_1f2_Certificados.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1f3_Bitacoras != null && InformeAuditoria.VI_C_1f3_Bitacoras.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1f4_Factura != null && InformeAuditoria.VI_C_1f4_Factura.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_1g1_Copia != null && InformeAuditoria.VI_C_1g1_Copia.Equals("S")) ok++;
            
                totaladmin = total;
                okadmin = ok;
                ViewBag.CumplimientoContrato = (ok / total) * 100;
                try
                {
                    ItemInforme item = contratos.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = contratos.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    contratos.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    contratos.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    contratos.Add(item);
                }
                // Inventario
                ok = 0;
                total = 16;

                if (InformeAuditoria.VI_C_2a1_General != null && InformeAuditoria.VI_C_2a1_General.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a2_Firmado != null && InformeAuditoria.VI_C_2a2_Firmado.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a3_Copia != null && InformeAuditoria.VI_C_2a3_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a4_Distribucion != null && InformeAuditoria.VI_C_2a4_Distribucion.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a5_Activo != null && InformeAuditoria.VI_C_2a5_Activo.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a6_Robos != null && InformeAuditoria.VI_C_2a6_Robos.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a7_Informado != null && InformeAuditoria.VI_C_2a7_Informado.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2b1_Hoja != null && InformeAuditoria.VI_C_2b1_Hoja.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2b2_Copia != null && InformeAuditoria.VI_C_2b2_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2b3_Actualizadas != null && InformeAuditoria.VI_C_2b3_Actualizadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2c1_Pendientes != null && InformeAuditoria.VI_C_2c1_Pendientes.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2c2_NoIncorporadas != null && InformeAuditoria.VI_C_2c2_NoIncorporadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2d1_Pendientes != null && InformeAuditoria.VI_C_2d1_Pendientes.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2d2_NoIncorporadas != null && InformeAuditoria.VI_C_2d2_NoIncorporadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2e1_Pendientes != null && InformeAuditoria.VI_C_2e1_Pendientes.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2e2_NoIncorporadas != null && InformeAuditoria.VI_C_2e2_NoIncorporadas.Equals("S")) ok++;

                ViewBag.CumplimientoInventario = (ok / total) * 100;
                totaladmin = totaladmin + total;
                okadmin = okadmin + ok;
                try
                {
                    ItemInforme item = inventarios.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = inventarios.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    inventarios.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    inventarios.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    inventarios.Add(item);
                }
                // Control presupuestario
                ok = 0;
                total = 3;

                if (InformeAuditoria.VI_C_3a_Control != null && InformeAuditoria.VI_C_3a_Control.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_3b_Existe != null && InformeAuditoria.VI_C_3b_Existe.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_3c_Director != null && InformeAuditoria.VI_C_3c_Director.Equals("S")) ok++;

                ViewBag.CumplimientoPresupuesto = (ok / total) * 100;
                totaladmin = totaladmin + total;
                okadmin = okadmin + ok;
                try
                {
                    ItemInforme item = presupuesto.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = presupuesto.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    presupuesto.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    presupuesto.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    presupuesto.Add(item);
                }
                // Control de Bodega
                if (InformeAuditoria.VI_C_5_Utiliza != null && InformeAuditoria.VI_C_5_Utiliza.Equals("S"))
                {
                    ok = 0;
                    total = 5;

                    if (InformeAuditoria.VI_C_5a_Compras != null && InformeAuditoria.VI_C_5a_Compras.Equals("S")) ok++;
                    if (InformeAuditoria.VI_C_5b_Autorizado != null && InformeAuditoria.VI_C_5b_Autorizado.Equals("S")) ok++;
                    if (InformeAuditoria.VI_C_5c_Ordenes != null && InformeAuditoria.VI_C_5c_Ordenes.Equals("S")) ok++;
                    if (InformeAuditoria.VI_C_5d_Actualizados != null && InformeAuditoria.VI_C_5d_Actualizados.Equals("S")) ok++;
                    if (InformeAuditoria.VI_C_5e_Saldos != null && InformeAuditoria.VI_C_5e_Saldos.Equals("S")) ok++;

                    ViewBag.CumplimientoBodega = (ok / total) * 100;
                    totaladmin = totaladmin + total;
                    okadmin = okadmin + ok;
                }
                try
                {
                    ItemInforme item = bodega.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = bodega.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    bodega.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    bodega.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    bodega.Add(item);
                }
                ViewBag.CumplimientoAdministrativa = 100 * (okadmin) / (totaladmin);

                // AREA FINANCIEROS
                int ok_financieros = 0;
                decimal total_financieros = 0;

                //7. Provisión
                ok = 0;
                total = 1;
                if (InformeAuditoria.VI_7_a_Provisiona != null && InformeAuditoria.VI_7_a_Provisiona.Equals("S")) ok++;
                ViewBag.CumplimientoProvision = 100 * (ok) / (total);
                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;

                try
                {
                    ItemInforme item = provision.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = provision.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    provision.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    provision.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    provision.Add(item);
                }

                //6. Retención 
                ok = 0;
                total = 9;
                if (InformeAuditoria.VI_6_a_Secretaria != null && InformeAuditoria.VI_6_a_Secretaria.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_b_Impuesto != null && InformeAuditoria.VI_6_b_Impuesto.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_c_Declaraciones != null && InformeAuditoria.VI_6_c_Declaraciones.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_d_Plazo != null && InformeAuditoria.VI_6_d_Plazo.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_e_Formulario29 != null && InformeAuditoria.VI_6_e_Formulario29.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_f_Autocuidado != null && InformeAuditoria.VI_6_f_Autocuidado.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_g_Contrato != null && InformeAuditoria.VI_6_g_Contrato.Equals("S")) ok++;
                //if (InformeAuditoria.VI_6_h_Inconsistencias != null && InformeAuditoria.VI_6_h_Inconsistencias.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_y_Rectificadas != null && InformeAuditoria.VI_6_y_Rectificadas.Equals("S")) ok++;
                ViewBag.CumplimientoRetencion = 100 * (ok) / (total);
                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;
                try
                {
                    ItemInforme item = retencion.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = retencion.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    retencion.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    retencion.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    retencion.Add(item);
                }
                //5. Rendiciones 
                ok = 0;
                total = 2;
                if (InformeAuditoria.VI_5_a_Direcciones != null && InformeAuditoria.VI_5_a_Direcciones.Equals("S")) ok++;
                if (InformeAuditoria.VI_5_b_Firma != null && InformeAuditoria.VI_5_b_Firma.Equals("S")) ok++;
                ViewBag.CumplimientoRendiciones = 100 * (ok) / (total);
                ok_financieros = ok_financieros + ok;
                total_financieros = +total;
                try
                {
                    ItemInforme item = rendicion.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = rendicion.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    rendicion.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    rendicion.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    rendicion.Add(item);
                }
                //4. Programa 
                ok = 0;
                total = 3;
                if (InformeAuditoria.VI_4_a_Programas != null && InformeAuditoria.VI_4_a_Programas.Equals("S")) ok++;
                if (InformeAuditoria.VI_4_b_Elaborados != null && InformeAuditoria.VI_4_b_Elaborados.Equals("S")) ok++;
                if (InformeAuditoria.VI_4_c_Firmas != null && InformeAuditoria.VI_4_c_Firmas.Equals("S")) ok++;
                ViewBag.CumplimientoPrograma = 100 * (ok) / (total);
                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;
                try
                {
                    ItemInforme item = programa.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = programa.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    programa.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    programa.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    programa.Add(item);
                }
                //3. Fondo 
                ok = 0;
                total = 14;
                if (InformeAuditoria.VI_3_a_Monto != null && InformeAuditoria.VI_3_a_Monto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_b_Arqueo != null && InformeAuditoria.VI_3_b_Arqueo.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_c_Firma != null && InformeAuditoria.VI_3_c_Firma.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_d_Efecto != null && InformeAuditoria.VI_3_d_Efecto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_e_Reposicion != null && InformeAuditoria.VI_3_e_Reposicion.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_f_Proyecto != null && InformeAuditoria.VI_3_f_Proyecto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_g_Reembolsos != null && InformeAuditoria.VI_3_g_Reembolsos.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_h_Boletas != null && InformeAuditoria.VI_3_h_Boletas.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_i_Superiores != null && InformeAuditoria.VI_3_i_Superiores.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_j_Combustibles != null && InformeAuditoria.VI_3_j_Combustibles.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_k_Planillas != null && InformeAuditoria.VI_3_k_Planillas.Equals("S")) ok++;
                //if (InformeAuditoria.VI_3_l_Actividades != null && InformeAuditoria.VI_3_l_Actividades.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_m_Movilizacion != null && InformeAuditoria.VI_3_m_Movilizacion.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_n_Interurbana != null && InformeAuditoria.VI_3_n_Interurbana.Equals("S")) ok++;
                ViewBag.CumplimientoFondo = 100 * (ok) / (total);

                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;
                try
                {
                    ItemInforme item = fondo.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = fondo.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    fondo.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    fondo.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    fondo.Add(item);
                }
                //2. Comprobantes 
                ok = 0;
                total = 7;
                if (InformeAuditoria.VI_2_a_Coincide != null && InformeAuditoria.VI_2_a_Coincide.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_b_Glosa != null && InformeAuditoria.VI_2_b_Glosa.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_c_Firma != null && InformeAuditoria.VI_2_c_Firma.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_d_Egresos != null && InformeAuditoria.VI_2_d_Egresos.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_e_EgresosNoConforme != null && InformeAuditoria.VI_2_e_EgresosNoConforme.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_f_SinDoc != null && InformeAuditoria.VI_2_f_SinDoc.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_g_Envia != null && InformeAuditoria.VI_2_g_Envia.Equals("S")) ok++;
                ViewBag.CumplimientoComprobantes = 100 * (ok) / (total);

                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;
                try
                {
                    ItemInforme item = comprobantes.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = comprobantes.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    comprobantes.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    comprobantes.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    comprobantes.Add(item);
                }
                //1. Libro
                ok = 0;
                total = 11;
                if (InformeAuditoria.VI_1_b_Emitido != null && InformeAuditoria.VI_1_b_Emitido.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_b_Archivo != null && InformeAuditoria.VI_1_b_Archivo.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_b_Firmado != null && InformeAuditoria.VI_1_b_Firmado.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_b_Correlativo != null && InformeAuditoria.VI_1_b_Correlativo.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_b_Cheques != null && InformeAuditoria.VI_1_b_Cheques.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_c_Cartolas != null && InformeAuditoria.VI_1_c_Cartolas.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_c_EnviaCartolas != null && InformeAuditoria.VI_1_c_EnviaCartolas.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_d_Suma != null && InformeAuditoria.VI_1_d_Suma.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_d_Archivadas != null && InformeAuditoria.VI_1_d_Archivadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_d_Cheques != null && InformeAuditoria.VI_1_d_Cheques.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_d_Firma != null && InformeAuditoria.VI_1_d_Firma.Equals("S")) ok++;

                ViewBag.CumplimientoLibro = 100 * (ok) / (total);

                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;

                ViewBag.CumplimientoFinanciero = 100 * (ok_financieros) / (total_financieros);
                try
                {
                    ItemInforme item = libro.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = libro.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    libro.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    libro.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    libro.Add(item);
                }

                // Observaciones superadas SENAME
                if (InformeAuditoria.V_US_ObsSuperadas != null && InformeAuditoria.V_US_ObsSuperadas.Equals("S"))
                {
                    obs_sename_superadas++;
                }

                if (InformeAuditoria.V_US_ObsPendientes != null && InformeAuditoria.V_US_ObsPendientes.Equals("S"))
                {
                    obs_sename_pendientes++;
                }

                if (InformeAuditoria.V_UC_ObsSuperadas != null && InformeAuditoria.V_UC_ObsSuperadas.Equals("S"))
                {
                    obs_codeni_superadas++;
                }

                if (InformeAuditoria.V_UC_ObsPendientes != null && InformeAuditoria.V_UC_ObsPendientes.Equals("S"))
                {
                    obs_codeni_pendientes++;
                }
            }

            ViewBag.Personal = personal.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Contratos = contratos.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Inventarios = inventarios.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Presupuesto = presupuesto.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Bodega = bodega.OrderByDescending(i => i.porcentaje).ToList();

            ViewBag.Libro = libro.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Comprobantes = comprobantes.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Fondo = fondo.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Programa = programa.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Rendicion = rendicion.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Retencion = retencion.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Provision = provision.OrderByDescending(i => i.porcentaje).ToList();

            ViewBag.SenameSuperadas = 100 * obs_sename_superadas / totalInformes;
            ViewBag.SenamePendientes = 100 * obs_sename_pendientes / totalInformes;
            ViewBag.CodeniSuperadas = 100 * obs_codeni_superadas / totalInformes;
            ViewBag.CodeniPendientes = 100 * obs_codeni_pendientes / totalInformes;

            return View();
        }

        public ActionResult ReporteGeneral(int DesdeMes = 0, int DesdePeriodo = 0, int HastaMes = 0, int HastaPeriodo = 0)
        {
            // Se filtra de acuerdo al tipo de proyecto
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            if (DesdeMes < 1 || DesdePeriodo < 1 || HastaMes < 1 || HastaPeriodo < 1)
            {
                DesdeMes = DateTime.Now.Month;
                DesdePeriodo = DateTime.Now.Year - 1;
                HastaMes = DateTime.Now.Month;
                HastaPeriodo = DateTime.Now.Year;
            }

            ViewBag.DesdeMes = DesdeMes;
            ViewBag.DesdePeriodo = DesdePeriodo;
            ViewBag.HastaMes = HastaMes;
            ViewBag.HastaPeriodo = HastaPeriodo;

            int AuditoraID = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == Proyecto.ID).Where(r => r.TipoRolID == 4).Single().Persona.ID;
            List<InformeAuditoria> Informes = db.InformeAuditoria
                                            .Where(i =>  (i.PeriodoDesde > DesdePeriodo) || (i.MesDesde >= DesdeMes && i.PeriodoDesde == DesdePeriodo) 
                                                && (i.PeriodoHasta < HastaPeriodo ||  i.MesHasta <= HastaMes && i.PeriodoHasta == HastaPeriodo )
                                                && (i.PeriodoDesde >= i.PeriodoHasta || (i.PeriodoDesde == i.PeriodoHasta && i.MesDesde <= i.MesHasta)
                                                ))
                                            .ToList();

            decimal totalInformes = Informes.Count;
            ViewBag.TotalInformes = totalInformes;
            
            if (totalInformes < 1)
            {
                ViewBag.SinInfomes = 1;
                return View();
            }

            int obs_sename_superadas = 0;
            int obs_sename_pendientes = 0;
            int obs_codeni_superadas = 0;
            int obs_codeni_pendientes = 0;

            List<ItemInforme> personal = new List<ItemInforme>();
            
            // Administrativos
            List<ItemInforme> contratos = new List<ItemInforme>();
            List<ItemInforme> inventarios = new List<ItemInforme>();
            List<ItemInforme> presupuesto = new List<ItemInforme>();
            List<ItemInforme> bodega = new List<ItemInforme>();

            // Financieros
            List<ItemInforme> libro = new List<ItemInforme>();
            List<ItemInforme> comprobantes = new List<ItemInforme>();
            List<ItemInforme> fondo = new List<ItemInforme>();
            List<ItemInforme> programa = new List<ItemInforme>();
            List<ItemInforme> rendicion = new List<ItemInforme>();
            List<ItemInforme> retencion = new List<ItemInforme>();
            List<ItemInforme> provision = new List<ItemInforme>();

            foreach (var InformeAuditoria in Informes)
            {
                // AREA PERSONAL
                decimal total = 10;
                int ok = 0;
                if (InformeAuditoria.VI_B_1_Convenio != null && InformeAuditoria.VI_B_1_Convenio.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_2_Remuneraciones != null && InformeAuditoria.VI_B_2_Remuneraciones.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_3_Electronica != null && InformeAuditoria.VI_B_3_Electronica.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_4_InduccionDirector != null && InformeAuditoria.VI_B_4_InduccionDirector.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_5_Induccion2do != null && InformeAuditoria.VI_B_5_Induccion2do.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_6_InduccionSecretaria != null && InformeAuditoria.VI_B_6_InduccionSecretaria.Equals("S")) ok++;
                //if (InformeAuditoria.VI_B_7_Sueldo != null && InformeAuditoria.VI_B_7_Sueldo.Equals("S")) ok++;
                //if (InformeAuditoria.VI_B_8_Contrato != null && InformeAuditoria.VI_B_8_Contrato.Equals("S")) ok++;
                if (InformeAuditoria.VI_B_9_Bono != null && InformeAuditoria.VI_B_9_Bono.Equals("S")) ok++;
                //if (InformeAuditoria.VI_B_10_Descuentos != null && InformeAuditoria.VI_B_10_Descuentos.Equals("S")) ok++;

                try
                {
                    ItemInforme item = personal.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = personal.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    personal.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    personal.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    personal.Add(item);
                }

                // Contratos
                int okadmin = 0;
                decimal totaladmin = 0;
                ok = 0;
                total = 29;

                if (InformeAuditoria.VI_C_1a1_Arriendo != null && InformeAuditoria.VI_C_1a1_Arriendo.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1a2_Copia != null && InformeAuditoria.VI_C_1a2_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1a3_Recibos != null && InformeAuditoria.VI_C_1a3_Recibos.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1a4_Termino != null && InformeAuditoria.VI_C_1a4_Termino.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1a5_Nombre != null && InformeAuditoria.VI_C_1a5_Nombre.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b1_Copia != null && InformeAuditoria.VI_C_1b1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b2_Escritura != null && InformeAuditoria.VI_C_1b2_Escritura.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1b3_Devolucion != null && InformeAuditoria.VI_C_1b3_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b4_Registro != null && InformeAuditoria.VI_C_1b4_Registro.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b5_Llamadas != null && InformeAuditoria.VI_C_1b5_Llamadas.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1b6_Evidencia != null && InformeAuditoria.VI_C_1b6_Evidencia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c1_Copia != null && InformeAuditoria.VI_C_1c1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c2_Escritura != null && InformeAuditoria.VI_C_1c2_Escritura.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c3_Asignados != null && InformeAuditoria.VI_C_1c3_Asignados.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1c4_Devolucion != null && InformeAuditoria.VI_C_1c4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1c5_Controles != null && InformeAuditoria.VI_C_1c5_Controles.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1d1_Copia != null && InformeAuditoria.VI_C_1d1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1d2_Escritura != null && InformeAuditoria.VI_C_1d2_Escritura.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1d3_Equipo != null && InformeAuditoria.VI_C_1d3_Equipo.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1d4_Devolucion != null && InformeAuditoria.VI_C_1d4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1e1_Copia != null && InformeAuditoria.VI_C_1e1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1e2_Escritura != null && InformeAuditoria.VI_C_1e2_Escritura.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1e3_Equipos != null && InformeAuditoria.VI_C_1e3_Equipos.Equals("S")) ok++;
            //if (InformeAuditoria.VI_C_1e4_Devolucion != null && InformeAuditoria.VI_C_1e4_Devolucion.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f1_Copia != null && InformeAuditoria.VI_C_1f1_Copia.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f2_Certificados != null && InformeAuditoria.VI_C_1f2_Certificados.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f3_Bitacoras != null && InformeAuditoria.VI_C_1f3_Bitacoras.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1f4_Factura != null && InformeAuditoria.VI_C_1f4_Factura.Equals("S")) ok++;
            if (InformeAuditoria.VI_C_1g1_Copia != null && InformeAuditoria.VI_C_1g1_Copia.Equals("S")) ok++;
            
                totaladmin = total;
                okadmin = ok;
                ViewBag.CumplimientoContrato = (ok / total) * 100;
                try
                {
                    ItemInforme item = contratos.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = contratos.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    contratos.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    contratos.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    contratos.Add(item);
                }
                // Inventario
                ok = 0;
                total = 16;

                if (InformeAuditoria.VI_C_2a1_General != null && InformeAuditoria.VI_C_2a1_General.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a2_Firmado != null && InformeAuditoria.VI_C_2a2_Firmado.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a3_Copia != null && InformeAuditoria.VI_C_2a3_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a4_Distribucion != null && InformeAuditoria.VI_C_2a4_Distribucion.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a5_Activo != null && InformeAuditoria.VI_C_2a5_Activo.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a6_Robos != null && InformeAuditoria.VI_C_2a6_Robos.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2a7_Informado != null && InformeAuditoria.VI_C_2a7_Informado.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2b1_Hoja != null && InformeAuditoria.VI_C_2b1_Hoja.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2b2_Copia != null && InformeAuditoria.VI_C_2b2_Copia.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2b3_Actualizadas != null && InformeAuditoria.VI_C_2b3_Actualizadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2c1_Pendientes != null && InformeAuditoria.VI_C_2c1_Pendientes.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2c2_NoIncorporadas != null && InformeAuditoria.VI_C_2c2_NoIncorporadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2d1_Pendientes != null && InformeAuditoria.VI_C_2d1_Pendientes.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2d2_NoIncorporadas != null && InformeAuditoria.VI_C_2d2_NoIncorporadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2e1_Pendientes != null && InformeAuditoria.VI_C_2e1_Pendientes.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_2e2_NoIncorporadas != null && InformeAuditoria.VI_C_2e2_NoIncorporadas.Equals("S")) ok++;

                ViewBag.CumplimientoInventario = (ok / total) * 100;
                totaladmin = totaladmin + total;
                okadmin = okadmin + ok;
                try
                {
                    ItemInforme item = inventarios.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = inventarios.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    inventarios.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    inventarios.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    inventarios.Add(item);
                }
                // Control presupuestario
                ok = 0;
                total = 3;

                if (InformeAuditoria.VI_C_3a_Control != null && InformeAuditoria.VI_C_3a_Control.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_3b_Existe != null && InformeAuditoria.VI_C_3b_Existe.Equals("S")) ok++;
                if (InformeAuditoria.VI_C_3c_Director != null && InformeAuditoria.VI_C_3c_Director.Equals("S")) ok++;

                ViewBag.CumplimientoPresupuesto = (ok / total) * 100;
                totaladmin = totaladmin + total;
                okadmin = okadmin + ok;
                try
                {
                    ItemInforme item = presupuesto.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = presupuesto.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    presupuesto.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    presupuesto.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    presupuesto.Add(item);
                }
                // Control de Bodega
                if (InformeAuditoria.VI_C_5_Utiliza != null && InformeAuditoria.VI_C_5_Utiliza.Equals("S"))
                {
                    ok = 0;
                    total = 5;

                    if (InformeAuditoria.VI_C_5a_Compras != null && InformeAuditoria.VI_C_5a_Compras.Equals("S")) ok++;
                    if (InformeAuditoria.VI_C_5b_Autorizado != null && InformeAuditoria.VI_C_5b_Autorizado.Equals("S")) ok++;
                    if (InformeAuditoria.VI_C_5c_Ordenes != null && InformeAuditoria.VI_C_5c_Ordenes.Equals("S")) ok++;
                    if (InformeAuditoria.VI_C_5d_Actualizados != null && InformeAuditoria.VI_C_5d_Actualizados.Equals("S")) ok++;
                    if (InformeAuditoria.VI_C_5e_Saldos != null && InformeAuditoria.VI_C_5e_Saldos.Equals("S")) ok++;

                    ViewBag.CumplimientoBodega = (ok / total) * 100;
                    totaladmin = totaladmin + total;
                    okadmin = okadmin + ok;
                }
                try
                {
                    ItemInforme item = bodega.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = bodega.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    bodega.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    bodega.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    bodega.Add(item);
                }
                ViewBag.CumplimientoAdministrativa = 100 * (okadmin) / (totaladmin);

                // AREA FINANCIEROS
                int ok_financieros = 0;
                decimal total_financieros = 0;

                //7. Provisión
                ok = 0;
                total = 1;
                if (InformeAuditoria.VI_7_a_Provisiona != null && InformeAuditoria.VI_7_a_Provisiona.Equals("S")) ok++;
                ViewBag.CumplimientoProvision = 100 * (ok) / (total);
                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;

                try
                {
                    ItemInforme item = provision.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = provision.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    provision.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    provision.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    provision.Add(item);
                }

                //6. Retención 
                ok = 0;
                total = 9;
                if (InformeAuditoria.VI_6_a_Secretaria != null && InformeAuditoria.VI_6_a_Secretaria.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_b_Impuesto != null && InformeAuditoria.VI_6_b_Impuesto.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_c_Declaraciones != null && InformeAuditoria.VI_6_c_Declaraciones.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_d_Plazo != null && InformeAuditoria.VI_6_d_Plazo.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_e_Formulario29 != null && InformeAuditoria.VI_6_e_Formulario29.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_f_Autocuidado != null && InformeAuditoria.VI_6_f_Autocuidado.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_g_Contrato != null && InformeAuditoria.VI_6_g_Contrato.Equals("S")) ok++;
                //if (InformeAuditoria.VI_6_h_Inconsistencias != null && InformeAuditoria.VI_6_h_Inconsistencias.Equals("S")) ok++;
                if (InformeAuditoria.VI_6_y_Rectificadas != null && InformeAuditoria.VI_6_y_Rectificadas.Equals("S")) ok++;
                ViewBag.CumplimientoRetencion = 100 * (ok) / (total);
                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;
                try
                {
                    ItemInforme item = retencion.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = retencion.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    retencion.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    retencion.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    retencion.Add(item);
                }
                //5. Rendiciones 
                ok = 0;
                total = 2;
                if (InformeAuditoria.VI_5_a_Direcciones != null && InformeAuditoria.VI_5_a_Direcciones.Equals("S")) ok++;
                if (InformeAuditoria.VI_5_b_Firma != null && InformeAuditoria.VI_5_b_Firma.Equals("S")) ok++;
                ViewBag.CumplimientoRendiciones = 100 * (ok) / (total);
                ok_financieros = ok_financieros + ok;
                total_financieros = +total;
                try
                {
                    ItemInforme item = rendicion.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = rendicion.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    rendicion.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    rendicion.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    rendicion.Add(item);
                }
                //4. Programa 
                ok = 0;
                total = 3;
                if (InformeAuditoria.VI_4_a_Programas != null && InformeAuditoria.VI_4_a_Programas.Equals("S")) ok++;
                if (InformeAuditoria.VI_4_b_Elaborados != null && InformeAuditoria.VI_4_b_Elaborados.Equals("S")) ok++;
                if (InformeAuditoria.VI_4_c_Firmas != null && InformeAuditoria.VI_4_c_Firmas.Equals("S")) ok++;
                ViewBag.CumplimientoPrograma = 100 * (ok) / (total);
                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;
                try
                {
                    ItemInforme item = programa.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = programa.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    programa.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    programa.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    programa.Add(item);
                }
                //3. Fondo 
                ok = 0;
                total = 14;
                if (InformeAuditoria.VI_3_a_Monto != null && InformeAuditoria.VI_3_a_Monto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_b_Arqueo != null && InformeAuditoria.VI_3_b_Arqueo.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_c_Firma != null && InformeAuditoria.VI_3_c_Firma.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_d_Efecto != null && InformeAuditoria.VI_3_d_Efecto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_e_Reposicion != null && InformeAuditoria.VI_3_e_Reposicion.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_f_Proyecto != null && InformeAuditoria.VI_3_f_Proyecto.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_g_Reembolsos != null && InformeAuditoria.VI_3_g_Reembolsos.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_h_Boletas != null && InformeAuditoria.VI_3_h_Boletas.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_i_Superiores != null && InformeAuditoria.VI_3_i_Superiores.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_j_Combustibles != null && InformeAuditoria.VI_3_j_Combustibles.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_k_Planillas != null && InformeAuditoria.VI_3_k_Planillas.Equals("S")) ok++;
                //if (InformeAuditoria.VI_3_l_Actividades != null && InformeAuditoria.VI_3_l_Actividades.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_m_Movilizacion != null && InformeAuditoria.VI_3_m_Movilizacion.Equals("S")) ok++;
                if (InformeAuditoria.VI_3_n_Interurbana != null && InformeAuditoria.VI_3_n_Interurbana.Equals("S")) ok++;
                ViewBag.CumplimientoFondo = 100 * (ok) / (total);

                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;
                try
                {
                    ItemInforme item = fondo.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = fondo.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    fondo.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    fondo.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    fondo.Add(item);
                }
                //2. Comprobantes 
                ok = 0;
                total = 7;
                if (InformeAuditoria.VI_2_a_Coincide != null && InformeAuditoria.VI_2_a_Coincide.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_b_Glosa != null && InformeAuditoria.VI_2_b_Glosa.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_c_Firma != null && InformeAuditoria.VI_2_c_Firma.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_d_Egresos != null && InformeAuditoria.VI_2_d_Egresos.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_e_EgresosNoConforme != null && InformeAuditoria.VI_2_e_EgresosNoConforme.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_f_SinDoc != null && InformeAuditoria.VI_2_f_SinDoc.Equals("S")) ok++;
                if (InformeAuditoria.VI_2_g_Envia != null && InformeAuditoria.VI_2_g_Envia.Equals("S")) ok++;
                ViewBag.CumplimientoComprobantes = 100 * (ok) / (total);

                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;
                try
                {
                    ItemInforme item = comprobantes.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = comprobantes.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    comprobantes.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    comprobantes.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    comprobantes.Add(item);
                }
                //1. Libro
                ok = 0;
                total = 11;
                if (InformeAuditoria.VI_1_b_Emitido != null && InformeAuditoria.VI_1_b_Emitido.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_b_Archivo != null && InformeAuditoria.VI_1_b_Archivo.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_b_Firmado != null && InformeAuditoria.VI_1_b_Firmado.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_b_Correlativo != null && InformeAuditoria.VI_1_b_Correlativo.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_b_Cheques != null && InformeAuditoria.VI_1_b_Cheques.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_c_Cartolas != null && InformeAuditoria.VI_1_c_Cartolas.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_c_EnviaCartolas != null && InformeAuditoria.VI_1_c_EnviaCartolas.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_d_Suma != null && InformeAuditoria.VI_1_d_Suma.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_d_Archivadas != null && InformeAuditoria.VI_1_d_Archivadas.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_d_Cheques != null && InformeAuditoria.VI_1_d_Cheques.Equals("S")) ok++;
                if (InformeAuditoria.VI_1_d_Firma != null && InformeAuditoria.VI_1_d_Firma.Equals("S")) ok++;

                ViewBag.CumplimientoLibro = 100 * (ok) / (total);

                ok_financieros = ok_financieros + ok;
                total_financieros = total_financieros + total;

                ViewBag.CumplimientoFinanciero = 100 * (ok_financieros) / (total_financieros);
                try
                {
                    ItemInforme item = libro.Where(i => i.proyectoID == InformeAuditoria.ProyectoID).Single();
                    int index = libro.FindIndex(i => i.proyectoID == InformeAuditoria.ProyectoID);
                    libro.RemoveAt(index);
                    item.total += total;
                    item.ok += ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    libro.Add(item);
                }
                catch (Exception)
                {
                    ItemInforme item = new ItemInforme();
                    item.proyectoID = InformeAuditoria.ProyectoID;
                    item.proyectoNombre = InformeAuditoria.Proyecto.NombreLista;
                    item.total = total;
                    item.ok = ok;
                    item.porcentaje = 100 * item.ok / item.total;
                    libro.Add(item);
                }

                // Observaciones superadas SENAME
                if (InformeAuditoria.V_US_ObsSuperadas != null && InformeAuditoria.V_US_ObsSuperadas.Equals("S"))
                {
                    obs_sename_superadas++;
                }

                if (InformeAuditoria.V_US_ObsPendientes != null && InformeAuditoria.V_US_ObsPendientes.Equals("S"))
                {
                    obs_sename_pendientes++;
                }

                if (InformeAuditoria.V_UC_ObsSuperadas != null && InformeAuditoria.V_UC_ObsSuperadas.Equals("S"))
                {
                    obs_codeni_superadas++;
                }

                if ( InformeAuditoria.V_UC_ObsPendientes != null && InformeAuditoria.V_UC_ObsPendientes.Equals("S"))
                {
                    obs_codeni_pendientes++;
                }
            }

            ViewBag.Personal = personal.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Contratos = contratos.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Inventarios = inventarios.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Presupuesto = presupuesto.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Bodega = bodega.OrderByDescending(i => i.porcentaje).ToList();

            ViewBag.Libro = libro.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Comprobantes = comprobantes.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Fondo = fondo.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Programa = programa.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Rendicion = rendicion.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Retencion = retencion.OrderByDescending(i => i.porcentaje).ToList();
            ViewBag.Provision = provision.OrderByDescending(i => i.porcentaje).ToList();

            ViewBag.SenameSuperadas = 100 * obs_sename_superadas / totalInformes;
            ViewBag.SenamePendientes = 100 * obs_sename_pendientes / totalInformes;
            ViewBag.CodeniSuperadas = 100 * obs_codeni_superadas / totalInformes;
            ViewBag.CodeniPendientes = 100 * obs_codeni_pendientes / totalInformes;

            return View();
        }


        // Total Egresos Periodo SAG
        public void TotalEgresos(int informeID, int mesDesde, int anoDesde, int mesHasta, int MesDesde)
        { 
            
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}