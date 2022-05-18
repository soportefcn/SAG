using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Data.SqlClient;

namespace SAG2.Controllers
{ 
    public class IndicadorCalidadController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        //
        // GET: /IndicadorCalidad/

        public ViewResult Index()
        {

            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var movimiento = db.IndicadorCalidad.Where(m => m.ProyectoID == Proyecto.ID);
            return View(movimiento.ToList());

           // return View(db.IndicadorCalidad.ToList());
        }

        //
        // GET: /IndicadorCalidad/Details/5

        public ViewResult Details(int id)
        {
            IndicadorCalidad indicadorcalidad = db.IndicadorCalidad.Find(id);
            return View(indicadorcalidad);
        }

        //
        // GET: /IndicadorCalidad/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /IndicadorCalidad/Create

        [HttpPost]
        public ActionResult Create(IndicadorCalidad indicadorcalidad, HttpPostedFileBase file)
        {
           
            if (ModelState.IsValid)
            {
                indicadorcalidad.AdjuntarInforme = file.FileName;
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                if (indicadorcalidad.Tipo == 3)
                {
                    indicadorcalidad.GastoObjetado = 0;
                    indicadorcalidad.GastoRechazado = 0;
                }
                string valorGObj = "";
                string valorGRecha = "";
                valorGObj = Request.Form["GaObjetado"].ToString();// indicadorcalidad.GastoObjetado;GaRechazado
                if (valorGObj == "")
                {
                    valorGObj = "0";
                }
                else
                {
                    valorGObj = valorGObj.Replace(".", "");
                }
                
                valorGRecha = Request.Form["GaRechazado"].ToString();// indicadorcalidad.GastoObjetado;GaRechazado
                if (valorGObj == "")
                {
                    valorGRecha = "0";
                }
                else
                {
                    valorGRecha = valorGObj.Replace(".", "");
                }


                indicadorcalidad.GastoObjetado = Int32.Parse(valorGObj);
                indicadorcalidad.GastoRechazado = Int32.Parse(valorGRecha);

                Usuario Usuario = (Usuario)Session["Usuario"];
                indicadorcalidad.UsuarioID = Usuario.ID;
          
                indicadorcalidad.ProyectoID = Proyecto.ID;
                indicadorcalidad.FechaSistema = DateTime.Now;

                db.IndicadorCalidad.Add(indicadorcalidad);
               
                db.SaveChanges();
                string filename = System.IO.Path.GetFileName(file.FileName);
                /*  Saving the file in server folder*/
                file.SaveAs(Server.MapPath("~/pdf/" + filename));
                string filepathtosave = "pdf/" + filename;
                /*
                 * HERE WILL BE YOUR CODE TO SAVE THE FILE DETAIL IN DATA BASE
                 *
                 */

                ViewBag.Message = "File Uploaded successfully.";
                TempData["Message"] = "Grabado con exito ";
                return RedirectToAction("Create");
                
            }

            return View(indicadorcalidad);
        }
        public ActionResult Create2()
        {
            return View();
        }

        //
        // POST: /IndicadorCalidad/Create

        [HttpPost]
        public ActionResult Create2(IndicadorCalidad indicadorcalidad, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {

                indicadorcalidad.AdjuntarInforme = file.FileName;
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                 indicadorcalidad.GastoObjetado = 0;
                indicadorcalidad.GastoRechazado = 0;
             
                Usuario Usuario = (Usuario)Session["Usuario"];
                indicadorcalidad.UsuarioID = Usuario.ID;
               
                indicadorcalidad.ProyectoID = Proyecto.ID;
                indicadorcalidad.FechaSistema = DateTime.Now;

                db.IndicadorCalidad.Add(indicadorcalidad);

                db.SaveChanges();
                string filename = System.IO.Path.GetFileName(file.FileName);
                /*  Saving the file in server folder*/
                file.SaveAs(Server.MapPath("~/pdf/" + filename));
                string filepathtosave = "pdf/" + filename;
                /*
                 * HERE WILL BE YOUR CODE TO SAVE THE FILE DETAIL IN DATA BASE
                 *
                 */

                ViewBag.Message = "File Uploaded successfully.";
                TempData["Message"] = "Grabado con exito ";
                return RedirectToAction("Create2");

            }

            return View(indicadorcalidad);
        }
        //
        // GET: /IndicadorCalidad/Edit/5
 
        public ActionResult Edit(int id)
        {
            IndicadorCalidad indicadorcalidad = db.IndicadorCalidad.Find(id);
            if (indicadorcalidad.Tipo == 1)
            {
                ViewBag.nombre = "Sename";
            }
            else {
                ViewBag.nombre = "Codeni";
            }


            return View(indicadorcalidad);
        }

        //
        // POST: /IndicadorCalidad/Edit/5

        [HttpPost]
        public ActionResult Edit(IndicadorCalidad indicadorcalidad, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                indicadorcalidad.AdjuntarInforme = file.FileName;

                string valorGObj = "";
                string valorGRecha = "";
                valorGObj = Request.Form["GaObjetado"].ToString();// indicadorcalidad.GastoObjetado;GaRechazado
                valorGObj = valorGObj.Replace(".", "");
                valorGRecha = Request.Form["GaRechazado"].ToString();// indicadorcalidad.GastoObjetado;GaRechazado
                valorGRecha = valorGRecha.Replace(".", "");

                indicadorcalidad.GastoObjetado = Int32.Parse(valorGObj);
                indicadorcalidad.GastoObjetado = Int32.Parse(valorGRecha);


                db.Entry(indicadorcalidad).State = EntityState.Modified;

                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + indicadorcalidad.NumeroInforme;
                string filename = System.IO.Path.GetFileName(file.FileName);
               /*  Saving the file in server folder*/
                file.SaveAs(Server.MapPath("~/pdf/" + filename));
                string filepathtosave = "pdf/" + filename;
                /*
                 * HERE WILL BE YOUR CODE TO SAVE THE FILE DETAIL IN DATA BASE
                 *
                 */

                ViewBag.Message = "File Uploaded successfully.";
                return RedirectToAction("Create");
            }
            return View(indicadorcalidad);
        }

        //
        // GET: /IndicadorCalidad/Delete/5
 
        public ActionResult Delete(int id)
        {
            IndicadorCalidad indicadorcalidad = db.IndicadorCalidad.Find(id);
            db.IndicadorCalidad.Remove(indicadorcalidad);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        //
        // POST: /IndicadorCalidad/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            IndicadorCalidad indicadorcalidad = db.IndicadorCalidad.Find(id);
            db.IndicadorCalidad.Remove(indicadorcalidad);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}