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
    public class DetalleInformesController : Controller
    {
        private SAG2DB db = new SAG2DB();

        // GET: DetalleInformes
        public ActionResult Index()
        {
            return View(db.DetalleInformes.ToList());
        }

        // GET: DetalleInformes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Create");
            }
            DetalleInformes detalleInformes = db.DetalleInformes.Find(id);
            if (detalleInformes == null)
            {
                return HttpNotFound();
            }
            return View(detalleInformes);
        }

        // GET: DetalleInformes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DetalleInformes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,informeID,cuentaID,tipoCuenta,valor,estado")] DetalleInformes detalleInformes)
        {
            detalleInformes.informeID = (int)Session["InformeID"];
            detalleInformes.fecha = DateTime.Now;
           // detalleInformes.tipoCuenta = db.Cuenta.Find(detalleInformes.cuentaID).Tipo;

            if (ModelState.IsValid)
            {
                db.DetalleInformes.Add(detalleInformes);
                db.SaveChanges();
                return Redirect("../Informe/ListarLineas/" + detalleInformes.informeID);
            }

            return Redirect("../Informe/ListarLineas/"+ detalleInformes.informeID);
        }

        // GET: DetalleInformes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Create");
            }
            DetalleInformes detalleInformes = db.DetalleInformes.Find(id);
            if (detalleInformes == null)
            {
                return HttpNotFound();
            }
            return View(detalleInformes);
        }

        // POST: DetalleInformes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,informeID,cuentaID,tipoCuenta,valor,fecha,estado")] DetalleInformes detalleInformes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detalleInformes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(detalleInformes);
        }

        // GET: DetalleInformes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Create");
            }
            DetalleInformes detalleInformes = db.DetalleInformes.Find(id);
            if (detalleInformes == null)
            {
                return HttpNotFound();
            }
            return View(detalleInformes);
        }

        // POST: DetalleInformes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetalleInformes detalleInformes = db.DetalleInformes.Find(id);
            db.DetalleInformes.Remove(detalleInformes);
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
