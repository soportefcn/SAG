using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using SAG2.Comun; 

namespace SAG2.Controllers
{ 
    public class CuentaGlosaController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();
        //
        // GET: /CuentaGlosa/

        public ViewResult Index()
        {
            ViewBag.Cuenta = db.Cuenta.ToList();
            return View(db.CuentaGlosa.Where(d => d.Estado == 1).OrderBy(d => d.CuentaID).ToList());
        }

        //
        // GET: /CuentaGlosa/Details/5

        public ViewResult Details(int id)
        {
            CuentaGlosa cuentaglosa = db.CuentaGlosa.Find(id);
            return View(cuentaglosa);
        }

        //
        // GET: /CuentaGlosa/Create

        public ActionResult Create()
        {
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "NombreLista");
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            return View();
        } 

        //
        // POST: /CuentaGlosa/Create

        [HttpPost]
        public ActionResult Create(CuentaGlosa cuentaglosa)
        {
            Usuario usuario = (Usuario)Session["Usuario"];

            if (ModelState.IsValid)
            {
                int xcuenta = cuentaglosa.CuentaID;
                db.Database.ExecuteSqlCommand("UPDATE CuentaGlosa SET Estado =  0 where  CuentaID = " + xcuenta );
                cuentaglosa.UsuarioID = usuario.ID;
                cuentaglosa.Fecha = DateTime.Now;
                cuentaglosa.Estado = 1;
                db.CuentaGlosa.Add(cuentaglosa);
                db.SaveChanges();
                ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "NombreLista");
                TempData["Message"] = "Creado con exito ";
                return RedirectToAction("Create");
            }
            return View();
           
        }
        
        //
        // GET: /CuentaGlosa/Edit/5
 
        public ActionResult Edit(int id)
        {
            CuentaGlosa cuentaglosa = db.CuentaGlosa.Find(id);
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "NombreLista",cuentaglosa.CuentaID);
            return View(cuentaglosa);
        }

        //
        // POST: /CuentaGlosa/Edit/5

        [HttpPost]
        public ActionResult Edit(CuentaGlosa cuentaglosa)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            if (ModelState.IsValid)
            {
                cuentaglosa.UsuarioID = usuario.ID;
                cuentaglosa.Fecha = DateTime.Now;
                cuentaglosa.Estado = 1;
                db.Entry(cuentaglosa).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "NombreLista");
            }
            return View(cuentaglosa);
        }


      
        public ActionResult Delete(int id)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            CuentaGlosa cuentaglosa = db.CuentaGlosa.Find(id);
            cuentaglosa.UsuarioID = usuario.ID;
            cuentaglosa.Fecha = DateTime.Now;
            cuentaglosa.Estado = 0;
            db.Entry(cuentaglosa).State = EntityState.Modified;
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