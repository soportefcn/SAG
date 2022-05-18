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

    public class BienProcedenciaController : Controller
    {

        private SAG2DB db = new SAG2DB();

        public ActionResult Index()
        {

            var procedencia = db.BienProcedencia.ToList();

            return View(procedencia);

        }



        public ActionResult Create()
        {

            return View();

        }



        public ActionResult Edit(int id)
        {

            BienProcedencia model = db.BienProcedencia.Find(id);

            return View(model);

        }



        [HttpPost]

        public ActionResult Save(BienProcedencia model)
        {

            try
            {

                db.BienProcedencia.Add(model);

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

        public ActionResult Update(BienProcedencia model)
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

                BienProcedencia BienProcedencia = db.BienProcedencia.Find(id);

                db.BienProcedencia.Remove(BienProcedencia);

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

