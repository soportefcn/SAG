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
    public class DepositoPlazoController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /DepositoPlazo/

        public ViewResult Index()
        {
            var depositoplazo = db.DepositoPlazo.Include(d => d.Proyecto).Include(d => d.CuentaCorriente);
            return View(depositoplazo.ToList());
        }

        //
        // GET: /DepositoPlazo/Details/5

        public ViewResult Details(int id)
        {
            DepositoPlazo depositoplazo = db.DepositoPlazo.Find(id);
            return View(depositoplazo);
        }

        //
        // GET: /DepositoPlazo/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /DepositoPlazo/Create

        [HttpPost]
        public ActionResult Create(DepositoPlazo depositoplazo)
        {
            depositoplazo.ProyectoID = (int)((Proyecto)Session["Proyecto"]).ID;
            depositoplazo.CuentaCorrienteID = (int)((CuentaCorriente)Session["CuentaCorriente"]).ID;
            depositoplazo.Periodo = (int)Session["Periodo"];
            depositoplazo.Mes = (int)Session["Mes"];

            if (ModelState.IsValid)
            {
                db.DepositoPlazo.Add(depositoplazo);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            return View(depositoplazo);
        }
        
        //
        // GET: /DepositoPlazo/Edit/5
 
        public ActionResult Edit(int id)
        {
            DepositoPlazo depositoplazo = db.DepositoPlazo.Find(id);
            @ViewBag.interes = depositoplazo.Interes.ToString();
            return View(depositoplazo);
        }

        //
        // POST: /DepositoPlazo/Edit/5

        [HttpPost]
        public ActionResult Edit(DepositoPlazo depositoplazo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(depositoplazo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            return View(depositoplazo);
        }

        //
        // GET: /DepositoPlazo/Delete/5
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            DepositoPlazo depositoplazo = db.DepositoPlazo.Find(id);
            db.DepositoPlazo.Remove(depositoplazo);
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