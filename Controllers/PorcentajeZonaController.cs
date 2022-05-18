using SAG_5.Models;
using SAG2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAG2.Controllers
{
    public class PorcentajeZonaController : Controller
    {
        private SAG2DB db = new SAG2DB();

        public ActionResult Index()
        {
            var PorcentajeZona = db.PorcentajeZona.ToList();
            return View(PorcentajeZona);
        }

        public ActionResult Create()
        {
            ViewBag.ComunaID = new SelectList(db.Comuna, "ID", "Nombre");
            return View();
        }

        public ActionResult Edit(int id)
        {
            PorcentajeZona model = db.PorcentajeZona.Find(id);
            ViewBag.ComunaID = new SelectList(db.Comuna, "ID", "Nombre");
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(PorcentajeZona model)
        {
            try
            {
                model.Nombre = model.Valor + " %";
                db.PorcentajeZona.Add(model);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + model.Nombre;
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error en la operación." + ex.Message;
            }
            return RedirectToAction("Create");
        }

        [HttpPost]
        public ActionResult Update(PorcentajeZona model)
        {
            try
            {
                model.Nombre = model.Valor + " %";
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Modificado con exito " + model.Nombre;
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Sin permiso para esta accion" + ex.Message;
            }
            return RedirectToAction("Create");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                PorcentajeZona model = db.PorcentajeZona.Find(id);
                model.estado = 1;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Eliminado con exito, N° " + model.ID;
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error en la operación. Información se encuentra en uso" + ex.Message;
            }

            return RedirectToAction("Create");
        }
    }
}