using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.Web.Script.Serialization;

namespace SAG2.Controllers
{ 
    public class ArticulosController : Controller
    {
     
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Articulos/

        public ViewResult Index()
        {
            var articulo = db.Articulo.Include(a => a.UnidadMedida);
            return View(articulo.ToList());
        }

        //
        // GET: /Articulos/Create

        public ActionResult Create()
        {
            ViewBag.CategoriaID = new SelectList(db.Categoria, "ID", "nombre");
            ViewBag.UnidadMedidaID = new SelectList(db.UnidadMedida, "ID", "Descripcion");
            return View();
        } 

        //
        // POST: /Articulos/Create

        [HttpPost]
        public ActionResult Create(Articulo articulo)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (ModelState.IsValid)
            {
                if (!Usuario.esUsuario)
                {
                    db.Articulo.Add(articulo);
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + articulo.Nombre;
                    // MessageBox.Show("Here is my message");
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Message"] = "Sin permiso para esta accion";
                }
            }
            ViewBag.CategoriaID = new SelectList(db.Categoria, "ID", "nombre", articulo.CategoriaID);
            ViewBag.UnidadMedidaID = new SelectList(db.UnidadMedida, "ID", "Descripcion", articulo.UnidadMedidaID);
            return View(articulo);
        }
        
        //
        // GET: /Articulos/Edit/5
 
        public ActionResult Edit(int id)
        {
            Articulo articulo = db.Articulo.Find(id);
            ViewBag.UnidadMedidaID = new SelectList(db.UnidadMedida, "ID", "Descripcion", articulo.UnidadMedidaID);
            ViewBag.CategoriaID = new SelectList(db.Categoria, "ID", "nombre", articulo.CategoriaID);
            return View(articulo);
        }

        //
        // POST: /Articulos/Edit/5

        [HttpPost]
        public ActionResult Edit(Articulo articulo)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (ModelState.IsValid)
            {
                if (!Usuario.esUsuario)
                {
                    db.Entry(articulo).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + articulo.Nombre;
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Message"] = "Sin permiso para esta accion";
                }
            }
            ViewBag.UnidadMedidaID = new SelectList(db.UnidadMedida, "ID", "Descripcion", articulo.UnidadMedidaID);
            return View(articulo);
        }

        //
        // GET: /Articulos/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (!Usuario.esUsuario)
            {
                try
                {
                    Articulo articulo = db.Articulo.Find(id);
                    db.Articulo.Remove(articulo);
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }
                catch (Exception)
                {
                    TempData["Message"] = "No se puede eliminar el articulo tiene movimiento en el Sistema de Bodega";
                    return RedirectToAction("Create");
                }
            }
            else
            {
                TempData["Message"] = "No se puede eliminar el articulo tiene movimiento en el Sistema de Bodega";
                return RedirectToAction("Create");
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        
        public string Articulos(int id)
        {
            var Articulos = (from c in db.Articulo
                           where c.CategoriaID == id
                           orderby c.Nombre
                           select new
                           {
                               Value = c.ID,
                               Text = c.Nombre
                           }).ToList();

            return new JavaScriptSerializer().Serialize(Articulos);
        }
        public string ArticuloSaldo(int id, int periodo, int mes)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int prID = Proyecto.ID;
            var Articulos = (from c in db.Articulo
                             join b in db.Bodega on c.ID equals b.ArticuloID
                             where c.CategoriaID == id && b.Periodo == periodo && b.Mes == mes && b.Saldo > 0 && b.ProyectoID == prID
                             orderby c.Nombre
                             select new
                             {
                                 Value = c.ID,
                                 Text = c.Nombre
                             }).ToList();

            return new JavaScriptSerializer().Serialize(Articulos);
        }
    }
}