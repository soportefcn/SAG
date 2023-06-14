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
    public class TipoPagoController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /TipoPago/

        public ViewResult Index()
        {
            return View(db.TipoPago.Where(d => d.Estado == 1 ).ToList());
        }



        //
        // GET: /TipoPago/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /TipoPago/Create

        [HttpPost]
        public ActionResult Create(TipoPago tipopago)
        {
            if (ModelState.IsValid)
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                tipopago.UsuarioID = Usuario.ID;
                tipopago.Fecha = DateTime.Now;
                tipopago.Estado = 1;
                db.TipoPago.Add(tipopago);
                db.SaveChanges();
                return RedirectToAction("Edit", new { @id = tipopago.TipoPagoID });
            }

            return View(tipopago);
        }
        
        //
        // GET: /TipoPago/Edit/5
 
        public ActionResult Edit(int id)
        {
            TipoPago tipopago = db.TipoPago.Find(id);
            return View(tipopago);
        }

        //
        // POST: /TipoPago/Edit/5

        [HttpPost]
        public ActionResult Edit(TipoPago tipopago)
        {
            if (ModelState.IsValid)
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                tipopago.UsuarioID = Usuario.ID;
                tipopago.Fecha = DateTime.Now;
                tipopago.Estado = 1;
                db.Entry(tipopago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipopago);
        }

        //
        // GET: /TipoPago/Delete/5
 

        //
        // POST: /TipoPago/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario Usuario = (Usuario)Session["Usuario"];
            TipoPago tipopago = db.TipoPago.Find(id);
            tipopago.UsuarioID = Usuario.ID;
            tipopago.Fecha = DateTime.Now;
            tipopago.Estado = 1;
            db.Entry(tipopago).State = EntityState.Modified;
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