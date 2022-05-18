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
    public class ParametroUSSController : Controller
    {
        private SAG2DB db = new SAG2DB();


        public ActionResult Index()
        {
            var parametroUss = db.ParametroUss.OrderBy(a => a.ID);

            return View(parametroUss.ToList().Where(e => e.estado != 1));
        }

        public ActionResult Create()
        {


            return View();
        }

        [HttpPost]
        public ActionResult Create(ParametroUss model)
        {
            try
            {
                model.Anio = model.FechaIngreso.Year;
                model.Mes = model.FechaIngreso.Month;


                db.ParametroUss.Add(model);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito USS: " + model.uss.ToString();
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error en la operación." + ex.Message;
            }
            return RedirectToAction("Create");
        }



        public ActionResult Edit(int id)
        {
            ParametroUss model = db.ParametroUss.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ParametroUss model)
        {
            try
            {
                model.Anio = model.FechaIngreso.Year;
                model.Mes = model.FechaIngreso.Month;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Modificado con exito, N° " + model.ID;
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error en la operación." + ex.Message;
            }
            return RedirectToAction("Create");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                ParametroUss model = db.ParametroUss.Find(id);
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