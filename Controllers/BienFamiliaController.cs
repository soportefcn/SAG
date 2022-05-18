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
    public class BienFamiliaController : Controller
    {
        private SAG2DB db = new SAG2DB();
        public ActionResult Index()
        {
            var familia = db.BienFamilia.ToList();
            return View(familia);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            BienFamilia model = db.BienFamilia.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(BienFamilia model)
        {
            try
            {
                db.BienFamilia.Add(model);
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
        public ActionResult Update(BienFamilia model)
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
                BienFamilia Familia = db.BienFamilia.Find(id);
                db.BienFamilia.Remove(Familia);
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
