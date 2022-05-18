using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Web.Script.Serialization;

namespace SAG2.Controllers
{ 
    public class AsignacionController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();

        //
        // GET: /Asignacion/

        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var inventario = db.Inventario.Include(i => i.Proyecto).Where(p => p.ProyectoID == Proyecto.ID && p.TipoID == 2);
            return View(inventario.ToList());
        }

        //
        // GET: /Asignacion/Details/5

        public ViewResult Details(int id)
        {
            InventarioBien inventariobien = db.InventarioBien.Find(id);
            return View(inventariobien);
        }

        //
        // GET: /Asignacion/Create

        public ActionResult Create()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
             int periodo = (int)Session["Periodo"];
            ViewBag.DependenciaDestino =  db.Dependencia.Where(a => a.ProyectoID == Proyecto.ID);
            ViewBag.InventarioID = 0;
            ViewBag.BienInventarioID = new SelectList(db.BienInventario.Where(b => b.ProyectoID == Proyecto.ID), "ID", "NombreCombo");  // , "ID", "NumeroInventario");
            ViewBag.NumCorre = utils.darcorrelativoinventario(periodo, 1, Proyecto.ID);
            return View();
        } 

        //
        // POST: /Asignacion/Create

        [HttpPost]
        public ActionResult Create(InventarioBien inventariobien)
        {
            Proyecto Pro = (Proyecto)Session["Proyecto"];
          
               
                    Inventario inventario = new Inventario();
                    
                   // int id = Int32.Parse(Request.Form["MovimientoID"].ToString());
                    inventario.TipoID = 2;
                    Usuario Usuario = (Usuario)Session["Usuario"];
                    inventario.ProyectoID = Pro.ID;
                    inventario.UsuarioID = Usuario.ID;
                    inventario.fechaGrabacion = DateTime.Now;
              
                    db.Inventario.Add(inventario);
                    db.SaveChanges();
                    inventariobien.InventarioID = inventario.ID;
                    inventariobien.DependenciaID_Origen = 1;
                    db.InventarioBien.Add(inventariobien);
                    db.SaveChanges();
              

                
               // return RedirectToAction("Index");  
            
            ViewBag.DependenciaDestino = db.Dependencia.Where(a => a.ProyectoID == Pro.ID);
            ViewBag.InventarioID = inventario.ID;
            ViewBag.BienInventarioID = new SelectList(db.BienInventario.Where(b => b.ProyectoID == Pro.ID), "ID", "NombreCombo"); 
            return View(inventariobien);
        }
        
        //
        // GET: /Asignacion/Edit/5
 
        public ActionResult Edit(int id)
        {
            InventarioBien inventariobien = db.InventarioBien.Find(id);
           // ViewBag.InventarioID = new SelectList(db.Inventario, "ID", "Numero", inventariobien.InventarioID);
            ViewBag.BienInventarioID = new SelectList(db.BienInventario, "ID", "NumeroInventario", inventariobien.BienInventarioID);
            return View(inventariobien);
        }

        //
        // POST: /Asignacion/Edit/5

        [HttpPost]
        public ActionResult Edit(InventarioBien inventariobien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventariobien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InventarioID = new SelectList(db.Inventario, "ID", "Numero", inventariobien.InventarioID);
            ViewBag.BienInventarioID = new SelectList(db.BienInventario, "ID", "NumeroInventario", inventariobien.BienInventarioID);
            return View(inventariobien);
        }

        //
        // GET: /Asignacion/Delete/5
 
        public ActionResult Delete(int id)
        {
            InventarioBien inventariobien = db.InventarioBien.Find(id);
            return View(inventariobien);
        }

        //
        // POST: /Asignacion/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            InventarioBien inventariobien = db.InventarioBien.Find(id);
            db.InventarioBien.Remove(inventariobien);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ListarBienInventario(int inventarioID)
        {
            var inventariobien = db.InventarioBien.Where(m => m.InventarioID == inventarioID).OrderByDescending(d => d.ID).ToList();
            return View(inventariobien);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}