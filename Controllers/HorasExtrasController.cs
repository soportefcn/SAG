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

namespace SAG2.Controllers
{ 
    public class HorasExtrasController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        //
        // GET: /HorasExtras/

        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            return View(db.HorasExtras.Where(d => d.ProyectoID == Proyecto.ID).ToList());
        }

        //
        // GET: /HorasExtras/Details/5

        public ViewResult Details(int id)
        {
            HorasExtras horasextras = db.HorasExtras.Find(id);
            return View(horasextras);
        }

        //
        // GET: /HorasExtras/Create

        public ActionResult Create()
        {
            int Periodo = 0; int Mes = 0;

            Periodo = (int)Session["Periodo"];
            Mes = (int)Session["Mes"];
            ViewBag.periodo = Periodo;
            ViewBag.mes = Mes;
            return View();
        } 

        //
        // POST: /HorasExtras/Create

        [HttpPost]
        public ActionResult Create(HorasExtras horasextras)
        {
            if (ModelState.IsValid)
            {
                string vhextra = "";
                vhextra = Request.Form["MHorasExtras"].ToString();
                vhextra = vhextra.Replace(".", "");
                horasextras.MontoHorasExtras = Int32.Parse(vhextra);
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                horasextras.ProyectoID = Proyecto.ID;
                db.HorasExtras.Add(horasextras);
                db.SaveChanges();
                TempData["Message"] = "Grabado con exito ";
                return RedirectToAction("Create");
            }

            return View(horasextras);
        }
        
        //
        // GET: /HorasExtras/Edit/5
 
        public ActionResult Edit(int id)
        {
            HorasExtras horasextras = db.HorasExtras.Find(id);
            return View(horasextras);
        }

        //
        // POST: /HorasExtras/Edit/5

        [HttpPost]
        public ActionResult Edit(HorasExtras horasextras)
        {
            if (ModelState.IsValid)
            {
                string vhextra = "";
                vhextra = Request.Form["MHorasExtras"].ToString();
                vhextra = vhextra.Replace(".", "");
                horasextras.MontoHorasExtras = Int32.Parse(vhextra);
                db.Entry(horasextras).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Grabado con exito ";
                return RedirectToAction("Create");
            }
            return View(horasextras);
        }

        //
        // GET: /HorasExtras/Delete/5
 
        public ActionResult Delete(int id)
        {
            HorasExtras horasextras = db.HorasExtras.Find(id);
            db.HorasExtras.Remove(horasextras);
            db.SaveChanges();
            TempData["Message"] = "Eliminado con exito ";
            return RedirectToAction("Create");
        }

        //
        // POST: /HorasExtras/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            HorasExtras horasextras = db.HorasExtras.Find(id);
            db.HorasExtras.Remove(horasextras);
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