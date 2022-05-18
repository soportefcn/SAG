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
    public class ProgramaQController : Controller
    {
        private SAG2DB db = new SAG2DB();

        // GET: ProgramaQ
        public ActionResult Index()
        {
            var programaq = db.ProgramaQ;
            return View(programaq.ToList());
        }

        public ActionResult Create()
        {
            var q3 = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).OrderBy(a => a.CodCodeni).ToList();
            List<SelectListItem> listproyecto = new List<SelectListItem>();
            listproyecto.Add(new SelectListItem
            {
                Text = "Seleccione Un Proyecto",
                Value = "0"
            });

            foreach (var y in q3)
            {
                listproyecto.Add(new SelectListItem
                {
                    Text = y.NombreEstado,
                    Value = y.ID.ToString()
                });
            }
            ViewBag.listadoproyecto = listproyecto;

            return View();
        }
        [HttpPost]
        public ActionResult Create(ProgramaQ model)
        {
            try
            {
                db.ProgramaQ.Add(model);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito: " + model.Codigo;
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error en la operación." + ex.Message;
            }
            return RedirectToAction("Create");
        }

        public ActionResult Edit(int id)
        {
            var q3 = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).OrderBy(a => a.CodCodeni).ToList();
            List<SelectListItem> listproyecto = new List<SelectListItem>();
            listproyecto.Add(new SelectListItem
            {
                Text = "Seleccione Un Proyecto",
                Value = "0"
            });

            foreach (var y in q3)
            {
                listproyecto.Add(new SelectListItem
                {
                    Text = y.NombreEstado,
                    Value = y.ID.ToString()
                });
            }
            ViewBag.listadoproyecto = listproyecto;

            ProgramaQ model = db.ProgramaQ.Find(id);
            return View(model);
        }

        [HttpPost]
        public JsonResult GetDatosProyecto(int id)
        {
            if (id > 0)
            {
                var q = db.Proyecto.Where(x => x.ID == id).First();
                Proyecto proyecto = q;
                if(proyecto.CodCodeni == null)
                {
                    proyecto.CodCodeni = string.Empty;
                }
                if(proyecto.CodSename == null)
                {
                    proyecto.CodSename = string.Empty;
                }

                ProyectoVM bien = new ProyectoVM();

                bien.Codigo = proyecto.CodCodeni;
                bien.CodigoSename = proyecto.CodSename;

                return Json(bien);
            }
            else
            {

                ProyectoVM bien = new ProyectoVM();

                return Json(bien);
            }


        }

        [HttpPost]
        public ActionResult Edit(ProgramaQ model)
        {
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Modificado con exito: " + model.Codigo;
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
                ProgramaQ model = db.ProgramaQ .Find(id);
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