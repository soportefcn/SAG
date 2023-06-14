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
    public class ValorUFController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /ValorUF/

        public ViewResult Index()
        {
            return View(db.ValorUF.Where( d => d.Estado == 1 ).ToList());
        }

        //
        // GET: /ValorUF/Details/5


        //
        // GET: /ValorUF/Create

        public ActionResult Create()
        {
            int mes = (int)Session["Mes"];
            int periodo = (int)Session["Periodo"];
            ViewBag.periodo = periodo;
            ViewBag.mes = mes;
            return View();
        } 

        //
        // POST: /ValorUF/Create

        [HttpPost]
        public ActionResult Create(ValorUF valoruf)
        {
            if (ModelState.IsValid)
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                valoruf.UsuarioID = Usuario.ID;            
                valoruf.Fecha = DateTime.Now;
                valoruf.Estado = 1;
                db.ValorUF.Add(valoruf);
                db.SaveChanges();
                return RedirectToAction("Edit", new { @id = valoruf.ID });
            }

            return View(valoruf);
        }
        
        //
        // GET: /ValorUF/Edit/5
 
        public ActionResult Edit(int id)
        {
            ValorUF valoruf = db.ValorUF.Find(id);
            return View(valoruf);
        }

        //
        // POST: /ValorUF/Edit/5

        [HttpPost]
        public ActionResult Edit(ValorUF valoruf)
        {
            if (ModelState.IsValid)
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                valoruf.UsuarioID = Usuario.ID;
                valoruf.Fecha = DateTime.Now;
                valoruf.Estado = 1;
                db.Entry(valoruf).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }
            return View(valoruf);
        }

        //
        // GET: /ValorUF/Delete/5


        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            ValorUF valoruf = db.ValorUF.Find(id);
            Usuario Usuario = (Usuario)Session["Usuario"];
            valoruf.UsuarioID = Usuario.ID;
            valoruf.Fecha = DateTime.Now;
            valoruf.Estado = 0;
            db.Entry(valoruf).State = EntityState.Modified;
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