using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Comun;
using SAG2.Models;
using SAG2.Classes;

namespace SAG2.Controllers
{ 
    public class CuentasController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private ControlLog logReg = new ControlLog();

        //
        // GET: /Cuentas/
        public ActionResult CuentasPresupuesto() {
            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).OrderBy(c => c.Orden);
            return View(cuenta.ToList()); 
        }
        public void HabilitarHijo(int idPadre)
        {
            var Hijo = db.Cuenta.Where(d => d.CuentaID == idPadre).ToList();
            foreach (Cuenta items in Hijo)
            {
                items.Presupuesto = 1;
                db.Entry(items).State = EntityState.Modified;
                db.SaveChanges();
                HabilitarHijo(items.ID);
            }

        } 
        public ActionResult Habilitar(int id)
        {
            Proyecto proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Cuenta Datos = db.Cuenta.Find(id);
                Datos.Presupuesto = 1;
                db.Entry(Datos).State = EntityState.Modified;
                db.SaveChanges();

                HabilitarHijo(id);

                int periodo = DateTime.Now.Year;
                int Mes = DateTime.Now.Month;
                int CLog = 0;
                string Descripcion = " Habilitar Cuenta de Propuesta Presupuesto" + Datos.NombreLista;
                CLog = logReg.RegistraControl("quitar Presupuesto", Descripcion, periodo, Mes, usuario.ID, proyecto.ID);

            }
            catch (Exception)
            {

            }
            return RedirectToAction("CuentasPresupuesto");
        }
        public void QuitarHijo(int  idPadre)
        {
            var Hijo = db.Cuenta.Where(d => d.CuentaID == idPadre).ToList();
            foreach (Cuenta items in Hijo ){
                items.Presupuesto = 0;
                db.Entry(items).State = EntityState.Modified;
                db.SaveChanges();
                QuitarHijo(items.ID);           
            }
  
        } 
        public ActionResult Quitar(int id)
        {
            Proyecto proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Usuario usuario = (Usuario)Session["Usuario"];
                Cuenta Datos = db.Cuenta.Find(id);
                Datos.Presupuesto = 0;
                db.Entry(Datos).State = EntityState.Modified;
                db.SaveChanges();
                
                QuitarHijo(id);

                int periodo = DateTime.Now.Year;
                int Mes = DateTime.Now.Month;
                int CLog = 0;
                string Descripcion = " Quitar Cuenta de Propuesta Presupuesto" + Datos.NombreLista;
                CLog = logReg.RegistraControl("quitar Presupuesto", Descripcion, periodo, Mes, usuario.ID, proyecto.ID);

            }
            catch (Exception)
            {

            }
            return RedirectToAction("CuentasPresupuesto");
        }
        public ViewResult Index(string q = "")
        {
            var cuenta = db.Cuenta.Include(c => c.Padre).Where(c => !c.Codigo.Equals("0")).Where(c => c.Nombre.Contains(q)).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        //
        // GET: /Cuentas/Details/5

        public ViewResult Details(int id)
        {
            Cuenta cuenta = db.Cuenta.Find(id);
            return View(cuenta);
        }

        //
        // GET: /Cuentas/Create

        public ActionResult Create()
        {
            ViewBag.CuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista");
            ViewBag.SenameUso = new SelectList(db.SenameUso, "SenameUsoID ", "Descripcion");
            return View();
        }

        public ActionResult Subir(int id)
        {
            Cuenta cuenta = db.Cuenta.Find(id);
            int orden = cuenta.Orden;
            int orden_anterior = orden - 1;

            try
            {
                Cuenta cuenta_anterior = db.Cuenta.Where(c => c.Orden == orden_anterior).Single();
                // Aumentamos en 1 el orden de la cuenta anterior (Bajamos)
                cuenta_anterior.Orden = cuenta_anterior.Orden + 1;
                db.Entry(cuenta_anterior).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                //return RedirectToAction("Index");
            }

            // Disminuimos en 1 el orden de la cuenta anterior (Subimos)
            if (orden_anterior != 0)
            {
                cuenta.Orden = cuenta.Orden - 1;
                db.Entry(cuenta).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Bajar(int id)
        {
            Cuenta cuenta = db.Cuenta.Find(id);
            int orden = cuenta.Orden;
            int orden_siguiente = orden + 1;

            try
            {
                Cuenta cuenta_siguiente = db.Cuenta.Where(c => c.Orden == orden_siguiente).Single();
                cuenta_siguiente.Orden = cuenta_siguiente.Orden - 1;
                db.Entry(cuenta_siguiente).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                //return RedirectToAction("Index");
            }

            cuenta.Orden = cuenta.Orden + 1;
            db.Entry(cuenta).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // POST: /Cuentas/Create

        [HttpPost]
        public ActionResult Create(Cuenta cuenta)
        {
            cuenta.Orden = 0;
            
            try
            {
                Cuenta padre = db.Cuenta.Find(cuenta.CuentaID);
                if (padre.ID == 1)
                {
                    cuenta.Tipo = "I";
                }
                else if (padre.ID == 6)
                {
                    cuenta.Tipo = "E";
                }
                else
                {
                    cuenta.Tipo = padre.Tipo;
                }
            }
            catch (Exception)
            { }

            if (ModelState.IsValid)
            {
                cuenta.Presupuesto = 1; 
                cuenta.Estado = 1;
                db.Cuenta.Add(cuenta);
                
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + cuenta.Descripcion;
                return RedirectToAction("Create");  
            }

            ViewBag.CuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista", cuenta.CuentaID);
            return View(cuenta);
        }
        
        //
        // GET: /Cuentas/Edit/5
 
        public ActionResult Edit(int id)
        {
            Cuenta cuenta = db.Cuenta.Find(id);
            ViewBag.CuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista", cuenta.CuentaID);
            ViewBag.SenameUso = new SelectList(db.SenameUso, "SenameUsoID ", "Descripcion");
            return View(cuenta);
        }

        //
        // POST: /Cuentas/Edit/5

        [HttpPost]
        public ActionResult Edit(Cuenta cuenta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuenta).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + cuenta.Descripcion;
                return RedirectToAction("Create");
            }
            ViewBag.CuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista", cuenta.CuentaID);
            return View(cuenta);
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Cuenta cuenta = db.Cuenta.Find(id);
            db.Cuenta.Remove(cuenta);
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