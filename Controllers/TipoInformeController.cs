using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;

namespace SAG_5.Controllers
{
    public class TipoInformeController : Controller
    {
        private SAG2DB db = new SAG2DB();

        // GET: TipoInformes
        public ActionResult Index()
        {
            return View(db.TipoInforme.ToList());
        }

        // GET: TipoInformes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
              //  return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoInforme tipoInforme = db.TipoInforme.Find(id);
            if (tipoInforme == null)
            {
                return HttpNotFound();
            }
            return View(tipoInforme);
        }

        // GET: TipoInformes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoInformes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Nombre,Eliminado")] TipoInforme tipoInforme)
        {
            tipoInforme.Eliminado = 0;
            if (ModelState.IsValid)
            {
                db.TipoInforme.Add(tipoInforme);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoInforme);
        }

        // GET: TipoInformes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
               // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoInforme tipoInforme = db.TipoInforme.Find(id);
            if (tipoInforme == null)
            {
                return HttpNotFound();
            }
            return View(tipoInforme);
        }

        // POST: TipoInformes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nombre,Eliminado")] TipoInforme tipoInforme)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoInforme).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoInforme);
        }

        // GET: TipoInformes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
               // return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoInforme tipoInforme = db.TipoInforme.Find(id);
            if (tipoInforme == null)
            {
                return HttpNotFound();
            }
            return View(tipoInforme);
        }

        // POST: TipoInformes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoInforme tipoInforme = db.TipoInforme.Find(id);
            db.TipoInforme.Remove(tipoInforme);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
