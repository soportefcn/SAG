using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;

namespace SAG2.Controllers
{ 
    public class ProveedoresController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();

        //
        // GET: /Proveedores/

        public ViewResult Index(string q = "")
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var rol = db.RolProveedor.Include(r => r.Proveedor).Where(r => r.ProyectoID == Proyecto.ID).Where(r => r.Proveedor.Nombre.Contains(q));
            var proveedor = from r in rol
                            select r.Proveedor;

            return View(proveedor.OrderBy(p => p.Nombre).ToList());
        }
        // [HttpPost]
        //public ViewResult PopUp(FormCollection form)
        //{
        //    string buscar = "";
        //    try
        //    {
        //        buscar = form["buscar"].ToString();
        //    }
        //    catch (Exception)
        //    {
        //        buscar = "";
        //    } 
        //    Proyecto Proyecto = (Proyecto)Session["Proyecto"];
        //    var rol = db.RolProveedor.Include(r => r.Proveedor).Where(r => r.ProyectoID == Proyecto.ID && r.Proveedor.Nombre.Contains(buscar));
        //    var proveedor = from r in rol
        //                    select r.Proveedor;

        //    return View(proveedor.OrderBy(p => p.Nombre).ToList());
        //}
        public ViewResult PopUp()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var rol = db.RolProveedor.Include(r => r.Proveedor).Where(r => r.ProyectoID == Proyecto.ID);
            var proveedor = from r in rol
                            select r.Proveedor;

            return View(proveedor.OrderBy(p => p.Nombre).ToList());
        }

        //
        // GET: /Proveedores/Details/5

        public ViewResult Details(int id)
        {
            Proveedor proveedor = db.Proveedor.Find(id);
            return View(proveedor);
        }

        //
        // GET: /Proveedores/Create
        public ActionResult IngresarPopUp()
        {
            ViewBag.RegionID = new SelectList(db.Region.OrderBy(a => a.Nombre), "ID", "Nombre");
            return View();
        }

        [HttpPost]
        public ActionResult IngresarPopUp(Proveedor proveedor)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            try
            {
                proveedor.DV = Request.Form["DVBuscar"].ToString();
            }
            catch (Exception)
            { }

            try
            {
                Proveedor Proveedor = db.Proveedor.Where(p => p.Rut == proveedor.Rut).Where(p => p.DV == proveedor.DV).Single();
                RolProveedor rol = new RolProveedor();
                rol.ProyectoID = Proyecto.ID;
                rol.ProveedorID = Proveedor.ID;
                db.RolProveedor.Add(rol);
                db.SaveChanges();
                return RedirectToAction("CerrarPopUp", new { @id = Proveedor.ID });
            }
            catch (Exception)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Direccion direccion = proveedor.Direccion;
                        direccion.Mostrar = 1;
                        direccion.ComunaID = 1;

                        if (Request.Form["ComunaID"] != null && !Request.Form["ComunaID"].ToString().Equals(""))
                        {
                            direccion.ComunaID = Int32.Parse(Request.Form["ComunaID"].ToString());
                        }

                        db.Direccion.Add(direccion);
                        db.SaveChanges();



                        proveedor.DireccionID = direccion.ID;
                        db.Proveedor.Add(proveedor);
                        db.SaveChanges();

                        RolProveedor rol = new RolProveedor();
                        rol.ProyectoID = Proyecto.ID;
                        rol.ProveedorID = proveedor.ID;
                        db.RolProveedor.Add(rol);
                        db.SaveChanges();

                        return RedirectToAction("CerrarPopUp", new { @id = proveedor.ID });
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = utils.mensajeError("Ha ocurrido un error (" + utils.Mensaje(e) + ")");
                }
            }

            ViewBag.RegionID = new SelectList(db.Region.OrderBy(a => a.Nombre), "ID", "Nombre", Int32.Parse(Request.Form["RegionID"].ToString()));
            ViewBag.ComunaID = Int32.Parse(Request.Form["ComunaID"].ToString()).ToString();
            return View(proveedor);
        }

        public ActionResult CerrarPopUp(int id)
        {
            Proveedor Proveedor = db.Proveedor.Find(id);
            return View(Proveedor);
        }

        public ActionResult Create()
        {
            ViewBag.RegionID = new SelectList(db.Region.OrderBy(a => a.Nombre), "ID", "Nombre");
            return View();
        } 

        //
        // POST: /Proveedores/Create

        [HttpPost]
        public ActionResult Create(Proveedor proveedor)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            int rutv = Int32.Parse(Request.Form["rut"].ToString());
            string digi = utils.Digito(rutv);

            if (digi !=  Request.Form["DVBuscar"].ToString())
            {
                TempData["Message"] = "Error en el Rut";
                return RedirectToAction("Create");
            }
            else
            {
            try
            {
                proveedor.DV = Request.Form["DVBuscar"].ToString();
            }
            catch (Exception)
            { }

            try
            {
                Proveedor Proveedor = db.Proveedor.Where(p => p.Rut == proveedor.Rut).Where(p => p.DV == proveedor.DV).Single();
                RolProveedor rol = new RolProveedor();
                rol.ProyectoID = Proyecto.ID;
                rol.ProveedorID = Proveedor.ID;
                db.RolProveedor.Add(rol);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + Proveedor.Nombre;
                return RedirectToAction("Create");
            }
            catch (Exception)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        Direccion direccion = proveedor.Direccion;
                        direccion.Mostrar = 1;
                        direccion.ComunaID = Int32.Parse(Request.Form["ComunaID"].ToString());
                        db.Direccion.Add(direccion);
                        db.SaveChanges();
                        db.Proveedor.Add(proveedor);
                        db.SaveChanges();

                        RolProveedor rol = new RolProveedor();
                        rol.ProyectoID = Proyecto.ID;
                        rol.ProveedorID = proveedor.ID;
                        db.RolProveedor.Add(rol);
                        db.SaveChanges();

                        return RedirectToAction("Create");
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = utils.mensajeError("Ha ocurrido un error (" + e.StackTrace + ")");
                }
            }
            ViewBag.RegionID = new SelectList(db.Region.OrderBy(a => a.Nombre), "ID", "Nombre", Int32.Parse(Request.Form["RegionID"].ToString()));
            ViewBag.ComunaID = Int32.Parse(Request.Form["ComunaID"].ToString()).ToString();
            return View(proveedor);
        }
        }
        
        //
        // GET: /Proveedores/Edit/5
 
        public ActionResult Edit(int id)
        {
            Proveedor proveedor = db.Proveedor.Find(id);
            ViewBag.RegionID = new SelectList(db.Region.OrderBy(a => a.Nombre), "ID", "Nombre", proveedor.Direccion.Comuna.RegionID);
            return View(proveedor);
        }

        //
        // POST: /Proveedores/Edit/5

        [HttpPost]
        public ActionResult Edit(Proveedor proveedor)
        {

            try
            {
                proveedor.DV = Request.Form["DVBuscar"].ToString();
            }
            catch (Exception)
            { }

            if (ModelState.IsValid)
            {
                Direccion Direccion = db.Direccion.Find(Int32.Parse(Request.Form["DireccionID"].ToString()));
                Direccion.Calle = proveedor.Direccion.Calle;
                Direccion.Numero = proveedor.Direccion.Numero;
                Direccion.Depto = proveedor.Direccion.Depto;
                Direccion.ComunaID = Int32.Parse(Request.Form["ComunaID"].ToString());
                db.Entry(Direccion).State = EntityState.Modified;
                db.SaveChanges();
                proveedor.Direccion = Direccion;
                db.Entry(proveedor).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + proveedor.Nombre;
                return RedirectToAction("Create");
            }

            ViewBag.RegionID = new SelectList(db.Region.OrderBy(a => a.Nombre), "ID", "Nombre");
            return View(proveedor);
        }

        //
        // GET: /Proveedores/Delete/5
        /*
        public ActionResult Delete()
        {
            Proveedor proveedor = db.Proveedor.Find(id);
            return View(proveedor);
        }
        */
        //
        // POST: /Proveedores/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            try
            {
                RolProveedor rol = db.RolProveedor.Where(r => r.ProveedorID == id).Where(r => r.ProyectoID == Proyecto.ID).Single();
                db.RolProveedor.Remove(rol);
                db.SaveChanges();

            }
            catch (Exception)
            {}
            //Proveedor proveedor = db.Proveedor.Find(id);
            //db.Proveedor.Remove(proveedor);
            //db.SaveChanges();
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}