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
    public class CuentasPadresController : Controller
    {
        private SAG2DB db = new SAG2DB();

        // GET: CuentasPadres
        public ActionResult Index()
        {
            return View(db.CuentasPadres.ToList());
        }

        // GET: CuentasPadres/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            CuentasPadres cuentasPadres = db.CuentasPadres.Find(id);
            if (cuentasPadres == null)
            {
                return HttpNotFound();
            }
            return View(cuentasPadres);
        }

        // GET: CuentasPadres/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CuentasPadres/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,CuentaID,NombreGrupo,estado")] CuentasPadres cuentasPadres)
        {
            if (ModelState.IsValid)
            {
                db.CuentasPadres.Add(cuentasPadres);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cuentasPadres);
        }

        // GET: CuentasPadres/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            CuentasPadres cuentasPadres = db.CuentasPadres.Find(id);
            if (cuentasPadres == null)
            {
                return HttpNotFound();
            }
            return View(cuentasPadres);
        }

        // POST: CuentasPadres/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,CuentaID,NombreGrupo,estado")] CuentasPadres cuentasPadres)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cuentasPadres).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuentasPadres);
        }

        // GET: CuentasPadres/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            CuentasPadres cuentasPadres = db.CuentasPadres.Find(id);
            if (cuentasPadres == null)
            {
                return HttpNotFound();
            }
            return View(cuentasPadres);
        }

        // POST: CuentasPadres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuentasPadres cuentasPadres = db.CuentasPadres.Find(id);
            db.CuentasPadres.Remove(cuentasPadres);
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
