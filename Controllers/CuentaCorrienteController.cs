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
    public class CuentaCorrienteController : Controller
    {
        private Util utils = new Util();
        private SAG2DB db = new SAG2DB();

        //
        // GET: /CuentaCorriente/

        public ViewResult Index(string q = "")
        {
            Proyecto Proyecto = (Proyecto) Session["Proyecto"];
            return View(db.CuentaCorriente.Include(c => c.Establecimiento).Include(c => c.Direccion).Include(c => c.Banco).Where(a => a.ProyectoID == Proyecto.ID).Where(a => a.Numero.Contains(q)).ToList());
        }

        //
        // GET: /CuentaCorriente/Details/5

        public ViewResult Details(int id)
        {
            CuentaCorriente cuentacorriente = db.CuentaCorriente.Find(id);
            return View(cuentacorriente);
        }

        //
        // GET: /CuentaCorriente/Create

        public ActionResult Create(int BancoID = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.EstablecimientoID = new SelectList(db.Proyecto, "ID", "NombreLista", Proyecto.ID);
            ViewBag.BancoID = new SelectList(db.Banco, "ID", "Nombre", BancoID);
            ViewBag.RegionID = new SelectList(db.Region.OrderBy(a => a.Nombre), "ID", "Nombre");

            try
            {
                int cuentaCorrienteID = db.CuentaCorriente.Include(c => c.Establecimiento).Include(c => c.Direccion).Include(c => c.Banco).Where(a => a.ProyectoID == Proyecto.ID).Single().ID;
                return RedirectToAction("Edit", new { id = cuentaCorrienteID });
            }
            catch (Exception)
            {}

            return View();
        } 

        //
        // POST: /CuentaCorriente/Create

        [HttpPost]
        public ActionResult Create(CuentaCorriente cuentacorriente)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                    Direccion direccion = cuentacorriente.Direccion;
                    direccion.Mostrar = 1;
                    direccion.ComunaID = Int32.Parse(Request.Form["ComunaID"].ToString());
                    db.Direccion.Add(direccion);
                    db.SaveChanges();
                    cuentacorriente.ProyectoID = Proyecto.ID;
                    db.CuentaCorriente.Add(cuentacorriente);
                    db.SaveChanges();
                    return Create();
                } 
                catch (Exception e)
                {
                    ViewBag.Mensaje = utils.mensajeError("Ya existe una cuenta corriente asignada a este Proyecto.");
                    utils.Log(2, e.InnerException.Message);
                }
            }

            ViewBag.EstablecimientoID = new SelectList(db.Proyecto, "ID", "Nombre", cuentacorriente.ProyectoID);
            ViewBag.DireccionID = new SelectList(db.Direccion, "ID", "Calle", cuentacorriente.DireccionID);
            ViewBag.RegionID = new SelectList(db.Region, "ID", "Nombre", cuentacorriente.Direccion.Comuna.RegionID);
            return View(cuentacorriente);
        }
        
        //
        // GET: /CuentaCorriente/Edit/5
 
        public ActionResult Edit(int id)
        {
            CuentaCorriente cuentacorriente = db.CuentaCorriente.Find(id);
            ViewBag.EstablecimientoID = new SelectList(db.Proyecto, "ID", "Nombre", cuentacorriente.ProyectoID);
            ViewBag.BancoID = new SelectList(db.Banco, "ID", "Nombre", cuentacorriente.BancoID);
            //ViewBag.ComunaID = new SelectList(db.Comuna, "ID", "Nombre", cuentacorriente.Direccion.ComunaID);
            ViewBag.RegionID = new SelectList(db.Region, "ID", "Nombre", cuentacorriente.Direccion.Comuna.RegionID);
            return View(cuentacorriente);
        }

        //
        // POST: /CuentaCorriente/Edit/5

        [HttpPost]
        public ActionResult Edit(CuentaCorriente cuentacorriente)
        {
            if (ModelState.IsValid)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                Persona Persona = (Persona)Session["Persona"]; ;
                Direccion Direccion = db.Direccion.Find(Int32.Parse(Request.Form["DireccionID"].ToString()));
                Direccion.Calle = cuentacorriente.Direccion.Calle;
                Direccion.Numero = cuentacorriente.Direccion.Numero;
                Direccion.Depto = cuentacorriente.Direccion.Depto;
                Direccion.ComunaID = Int32.Parse(Request.Form["ComunaID"].ToString());
                db.Entry(Direccion).State = EntityState.Modified;
                db.SaveChanges();
                cuentacorriente.ProyectoID = Proyecto.ID;
                cuentacorriente.Direccion = Direccion;
                db.Entry(cuentacorriente).State = EntityState.Modified;
                db.SaveChanges();

                // Se acualiza información de session
                Session.Remove("CuentaCorriente");
                Session.Remove("InformacionPie");
                Session.Add("CuentaCorriente", cuentacorriente);
                Session.Add("InformacionPie", Proyecto.NombreLista + " (" + cuentacorriente.NumeroLista + ") | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());

                return RedirectToAction("Edit", new { id = cuentacorriente.ID });
            }
            ViewBag.EstablecimientoID = new SelectList(db.Proyecto, "ID", "Nombre", cuentacorriente.ProyectoID);
            ViewBag.DireccionID = new SelectList(db.Direccion, "ID", "Calle", cuentacorriente.DireccionID);
            ViewBag.BancoID = new SelectList(db.Banco, "ID", "Nombre", cuentacorriente.BancoID);
            return View(cuentacorriente);
        }

        //
        // GET: /CuentaCorriente/Delete/5
        /*
        public ActionResult Delete()
        {
            CuentaCorriente cuentacorriente = db.CuentaCorriente.Find(id);
            return View(cuentacorriente);
        }
        */
        //
        // POST: /CuentaCorriente/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            CuentaCorriente cuentacorriente = db.CuentaCorriente.Find(id);
            db.CuentaCorriente.Remove(cuentacorriente);
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