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
    public class BienCondicionController : Controller
    {
        private SAG2DB db = new SAG2DB();
        public ActionResult Index()
        {
            var condicion = db.BienCondicion.ToList();
            return View(condicion);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            BienCondicion model = db.BienCondicion.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(BienCondicion model)
        {
            try
            {
                db.BienCondicion.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Datos Ingresados Correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al Guardar" + ex;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Update(BienCondicion model)
        {
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Datos Actualizados Correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al Actualizar" + ex;
                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                BienCondicion BienCondicion = db.BienCondicion.Find(id);
                db.BienCondicion.Remove(BienCondicion);
                db.SaveChanges();
                TempData["Success"] = "Datos Eliminados Correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al Eliminar" + ex;
                return RedirectToAction("Index");
            }
        }
    }
}
