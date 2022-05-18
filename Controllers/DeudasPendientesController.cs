using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace SAG2.Controllers
{ 
    public class DeudasPendientesController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();

        //
        // GET: /DeudasPendientes/

        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var deudapendientes = db.DeudaPendiente.Include(d => d.Persona).Include(d => d.Proveedor).Include(d => d.Documento).Include(d => d.Cuenta).Include(d => d.Proyecto).Where(m => m.ProyectoID == Proyecto.ID && m.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            return View(deudapendientes.ToList());
        }

        //
        // GET: /DeudasPendientes/Details/5

        public ViewResult Details(int id)
        {
            DeudaPendiente deudapendiente = db.DeudaPendiente.Find(id);
            return View(deudapendiente);
        }

        //
        // GET: /DeudasPendientes/Create

        public ActionResult Create()
        {
            int periodo = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            ViewBag.NroComprobante = "1";

            if (db.DeudaPendiente.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Periodo == periodo).Count() > 0)
            {
                ViewBag.NroComprobante = db.DeudaPendiente.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
            }

            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
            ViewBag.DocumentoID = new SelectList(db.Documento, "ID", "NombreLista");
            return View();
        } 

        //
        // POST: /DeudasPendientes/Create

        [HttpPost]
        public ActionResult Create(DeudaPendiente deudapendiente)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int periodo = (int)Session["Periodo"];
            deudapendiente.NumeroComprobante = 1;
            deudapendiente.ProyectoID = ((Proyecto)Session["Proyecto"]).ID;
            deudapendiente.CuentaID = Int32.Parse(Request.Form["CuentaID"].ToString());
            deudapendiente.Mes = (int)Session["Mes"];
            deudapendiente.Periodo = periodo;
            deudapendiente.EgresoID = null;

            if (Request.Form["tipoBeneficiario"].ToString().Equals("1"))
            {
                deudapendiente.ProveedorID = null;
                deudapendiente.Rut = null;
                deudapendiente.DV = null;
            }
            else if (Request.Form["tipoBeneficiario"].ToString().Equals("2")) 
            {
                deudapendiente.PersonaID = null;
                deudapendiente.Rut = null;
                deudapendiente.DV = null;
            }
            else if (Request.Form["tipoBeneficiario"].ToString().Equals("3"))
            {
                deudapendiente.ProveedorID = null;
                deudapendiente.PersonaID = null;
            }

            if (db.DeudaPendiente.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Periodo == periodo).Count() > 0)
            {
                deudapendiente.NumeroComprobante = db.DeudaPendiente.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    db.DeudaPendiente.Add(deudapendiente);
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }
            }
            catch (Exception e)
            {
                ViewBag.Mensaje = utils.mensajeError(utils.Mensaje(e));
            }

            ViewBag.NroComprobante = deudapendiente.NumeroComprobante;
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos), deudapendiente.CuentaID); 
            ViewBag.DocumentoID = new SelectList(db.Documento, "ID", "NombreLista", deudapendiente.DocumentoID);
            return View(deudapendiente);
        }
        
        //
        // GET: /DeudasPendientes/Edit/5
 
        public ActionResult Edit(int id)
        {
            DeudaPendiente deudapendiente = db.DeudaPendiente.Find(id);
            if (deudapendiente.PersonaID != null)
            {
                deudapendiente.ProveedorID = 0;
                deudapendiente.Rut = deudapendiente.Persona.Rut;
                deudapendiente.DV = deudapendiente.Persona.DV;
                deudapendiente.Beneficiario = deudapendiente.Persona.NombreCompleto; 
            }
            else if (deudapendiente.ProveedorID != null)
            {
                deudapendiente.PersonaID = 0;
                deudapendiente.Rut = deudapendiente.Proveedor.Rut;
                deudapendiente.DV = deudapendiente.Proveedor.DV;
                deudapendiente.Beneficiario = deudapendiente.Proveedor.Nombre;
            }
            else 
            {
                deudapendiente.PersonaID = 0;
                deudapendiente.ProveedorID = 0;
            }

            ViewBag.DocumentoID = new SelectList(db.Documento, "ID", "NombreLista", deudapendiente.DocumentoID);
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos), deudapendiente.CuentaID);
            return View(deudapendiente);
        }

        //
        // POST: /DeudasPendientes/Edit/5

        [HttpPost]
        public ActionResult Edit(DeudaPendiente deudapendiente)
        {
            if (Request.Form["tipoBeneficiario"].ToString().Equals("1"))
            {
                deudapendiente.ProveedorID = null;
                deudapendiente.Rut = null;
                deudapendiente.DV = null;
            }
            else if (Request.Form["tipoBeneficiario"].ToString().Equals("2"))
            {
                deudapendiente.PersonaID = null;
                deudapendiente.Rut = null;
                deudapendiente.DV = null;
            }
            else if (Request.Form["tipoBeneficiario"].ToString().Equals("3"))
            {
                deudapendiente.ProveedorID = null;
                deudapendiente.PersonaID = null;
            }

            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(deudapendiente).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }
            }
            catch (SqlException e)
            {
                ViewBag.Mensaje = utils.mensajeError(e.Message);
            }
            catch (Exception e)
            {
                ViewBag.Mensaje = utils.mensajeError(e.InnerException.Message);
            }
            
            if (deudapendiente.PersonaID != null)
            {
                deudapendiente.ProveedorID = 0;
                deudapendiente.Rut = deudapendiente.Persona.Rut;
                deudapendiente.DV = deudapendiente.Persona.DV;
                deudapendiente.Beneficiario = deudapendiente.Persona.NombreCompleto;
            }
            else if (deudapendiente.ProveedorID != null)
            {
                deudapendiente.PersonaID = 0;
                deudapendiente.Rut = deudapendiente.Proveedor.Rut;
                deudapendiente.DV = deudapendiente.Proveedor.DV;
                deudapendiente.Beneficiario = deudapendiente.Proveedor.Nombre;
            }
            else
            {
                deudapendiente.PersonaID = 0;
                deudapendiente.ProveedorID = 0;
            }

            ViewBag.DocumentoID = new SelectList(db.Documento, "ID", "NombreLista", deudapendiente.DocumentoID);
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos), deudapendiente.CuentaID);
            return View(deudapendiente);
        }

        public ActionResult ListadoEgreso(int personalID = 0, int proveedorID = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (personalID != 0)
            {
                return View(db.DeudaPendiente.Where(m => m.ProyectoID == Proyecto.ID).Where(d => d.PersonaID == personalID).Where(d => d.EgresoID == null).OrderByDescending(d => d.NumeroComprobante).ToList());
            }
            else if (proveedorID != 0)
            {
                return View(db.DeudaPendiente.Where(m => m.ProyectoID == Proyecto.ID).Where(d => d.ProveedorID == proveedorID).Where(d => d.EgresoID == null).OrderByDescending(d => d.NumeroComprobante).ToList());
            }
            else
            {
                return View(db.DeudaPendiente.Where(m => m.ProyectoID == Proyecto.ID).Where(d => d.ProveedorID == null).Where(d => d.PersonaID == null).Where(d => d.EgresoID == null).OrderByDescending(d => d.NumeroComprobante).ToList());
            }
        }

        //
        // GET: /DeudasPendientes/Delete/5
 
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            DeudaPendiente deudapendiente = db.DeudaPendiente.Find(id);
            db.DeudaPendiente.Remove(deudapendiente);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        public string Personal(string rut, string dv)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var rol = db.Rol.Include(r => r.Persona).Include(r => r.TipoRol).Where(r => r.ProyectoID == Proyecto.ID);
            var personal = (from r in rol
                            where r.Persona.Rut == rut && r.Persona.DV == dv
                            select new
                            {
                                Id = r.Persona.ID,
                                Nombre = r.Persona.Nombres,
                                Paterno = r.Persona.ApellidoParterno,
                                Materno = r.Persona.ApellidoMaterno
                            }).ToList();

            return new JavaScriptSerializer().Serialize(personal);
        }

        public string Proveedores(string rut, string dv)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var rol = db.RolProveedor.Include(r => r.Proveedor).Where(r => r.ProyectoID == Proyecto.ID);
            var proveedores = (from r in rol
                               where r.Proveedor.Rut == rut && r.Proveedor.DV == dv
                            select new
                            {
                                Id = r.Proveedor.ID,
                                Nombre = r.Proveedor.Nombre
                            }).ToList();

            return new JavaScriptSerializer().Serialize(proveedores);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}