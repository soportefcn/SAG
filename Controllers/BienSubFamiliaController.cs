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
    public class BienSubFamiliaController : Controller
    {
        private SAG2DB db = new SAG2DB();
        
        public ActionResult Index()
        {
            var q = (from sf in db.BienSubFamilia
                     join f in db.BienFamilia on sf.FamiliaID equals f.ID
                     select new { IDSubFamilia = sf.ID, NombreSubFamilia = sf.Nombre, NombreFamilia = f.Nombre}
                     );
            var lista = new List<BienSubFamiliaViewModel>();
            foreach(var t in q)
            {
                lista.Add(new BienSubFamiliaViewModel()
                {
                    ID = t.IDSubFamilia,
                    Nombre = t.NombreSubFamilia,
                    descFamilia = t.NombreFamilia
                });
            }

            return View(lista);
        }

        public ActionResult Create()
        {
            var q = (from z in db.BienFamilia
                     select new { z.ID, z.Nombre });
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach(var x in q)
            listItems.Add(new SelectListItem
            {
                Text = x.Nombre,
                Value = x.ID.ToString()
            });
            ViewBag.listadofamilia = listItems;
            return View();
        }

        public ActionResult Edit(int id)
        {
            var q = (from z in db.Familia
                     select new { z.ID, z.Descripcion });
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (var x in q)
                listItems.Add(new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.ID.ToString()
                });
            ViewBag.listadofamilia = listItems;
            BienSubFamilia model = db.BienSubFamilia.Find(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Save(BienSubFamilia model) 
        {
            try
            {
                db.BienSubFamilia.Add(model);
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
        public ActionResult Update(BienSubFamilia model)
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
                BienSubFamilia subFamilia = db.BienSubFamilia.Find(id);
                db.BienSubFamilia.Remove(subFamilia);
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
