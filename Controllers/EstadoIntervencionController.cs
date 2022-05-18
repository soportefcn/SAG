using SAG_5.Models;
using SAG2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAG_5.Controllers
{
    public class EstadoIntervencionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        public ActionResult Index()
        {
            var estado = db.EstadoIntervencion;
            return View(estado.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            EstadoIntervencion model = db.EstadoIntervencion.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Save(EstadoIntervencion model)
        {
            try
            {
                db.EstadoIntervencion.Add(model);
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
        public ActionResult Update(EstadoIntervencion model)
        {
            try
            {
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
                EstadoIntervencion model = db.EstadoIntervencion.Find(id);
                model.Estado = 1;
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