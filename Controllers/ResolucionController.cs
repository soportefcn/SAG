using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.IO;

namespace SAG2.Controllers
{ 
    public class ResolucionController : Controller
    {
        private SAG2DB db = new SAG2DB();

        //
        // GET: /Resolucion/

        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];          
            return View(db.Resolucion.Where(d => d.ProyectoID == Proyecto.ID).OrderByDescending(d => d.ID).ToList());
        }
        public ViewResult Listado() {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.ResolucionDoc = db.ResolucionDescarga.Where(d => d.Resolucion.ProyectoID == Proyecto.ID).ToList();
            return View(db.Resolucion.Where(d => d.ProyectoID == Proyecto.ID).OrderByDescending(d => d.ID).ToList());
        }
        //
        // GET: /Resolucion/Details/5

        public ViewResult Details(int id)
        {
            Resolucion resolucion = db.Resolucion.Find(id);
            return View(resolucion);
        }



        public ActionResult Convenio()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int proyectoID = Proyecto.ID;
            int ConvenioId = Proyecto.ConvenioID;
            Convenio convemio = db.Convenio.Find(ConvenioId);
            FormConvenio ConvenioF = new FormConvenio();
            ConvenioF.ID = ConvenioId;
            ConvenioF.NroPlazas = convemio.NroPlazas;
            ConvenioF.ResEx = convemio.ResEx;

            ConvenioF.Comentarios = convemio.Comentarios;
            ConvenioF.FechaInicio = convemio.FechaInicio;
            ConvenioF.FechaTermino = convemio.FechaTermino;
            ViewBag.ConvenioArchivo = db.ConvenioDescarga.Where(d => d.ProyectoID == proyectoID && d.Estado == 1).FirstOrDefault();
            TempData["Message"] = null;
            return View(ConvenioF);
        }



        [HttpPost]
        public ActionResult Convenio(FormConvenio ConvenioF, HttpPostedFileBase file)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Usuario Usuario = (Usuario)Session["Usuario"];

            int proyectoID = Proyecto.ID;

            Convenio Registro = db.Convenio.Find(ConvenioF.ID);

            Registro.Comentarios = ConvenioF.Comentarios;
            Registro.FechaInicio = ConvenioF.FechaInicio;
            Registro.FechaTermino = ConvenioF.FechaTermino;
            Registro.ResEx = ConvenioF.ResEx;
            Registro.NroPlazas = ConvenioF.NroPlazas;
            db.Entry(Registro).State = EntityState.Modified;
            db.SaveChanges();


            try
            {
                if (file.ContentLength > 0)
                {
                    string fE = DateTime.Now.Year + "" + DateTime.Now.Month + "" + DateTime.Now.Day;
                    string filename = "Cn" + Proyecto.CodCodeni + "_" + fE + ".pdf";
                    string _path = Path.Combine(Server.MapPath("~/archivos"), filename);
                    file.SaveAs(_path);

                    db.Database.ExecuteSqlCommand("UPDATE ConvenioDescarga SET Estado = 2 WHERE ProyectoID = " + proyectoID);


                    ConvenioDescarga trDocumento = new ConvenioDescarga();
                    trDocumento.NombreArchivo = _path;
                    trDocumento.ProyectoID = proyectoID;
                    trDocumento.UsuarioID = Usuario.ID;
                    trDocumento.Fecha = DateTime.Now;
                    trDocumento.Estado = 1;
                    db.ConvenioDescarga.Add(trDocumento);

                    db.SaveChanges();
                }
            }
            catch (Exception)
            {

            }


            return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Resolucion/Create

        public ActionResult Create()
        {
            //
            DateTime FechaInicio =  new DateTime();
            DateTime FechaTermino = new DateTime();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                var resol = db.Resolucion.Where(d => d.ProyectoID == Proyecto.ID && d.Estado == 1).FirstOrDefault();
                FechaInicio = resol.FechaTermino.AddDays(1);
                FechaTermino = FechaInicio.AddMonths(6);               

            }
            catch (Exception) {
                var resol = db.Convenio.Find(Proyecto.ConvenioID);
                try
                {
                    FechaInicio = DateTime.Parse(resol.FechaTermino.ToString()).AddDays(1);
                    FechaTermino = FechaInicio.AddMonths(6);
                }catch(Exception){
                    FechaInicio = DateTime.Now.AddDays(1);
                    FechaTermino = FechaInicio.AddMonths(6);
                }
            }
            ViewBag.Desde = FechaInicio.ToShortDateString();
            ViewBag.Hasta = FechaTermino.ToShortDateString();

            return View();
        } 

        //
        // POST: /Resolucion/Create

        [HttpPost]
        public ActionResult Create(Resolucion resolucion, HttpPostedFileBase file)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (ModelState.IsValid)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];

                db.Database.ExecuteSqlCommand("UPDATE Resolucion SET estado = 0 WHERE ProyectoID = " + Proyecto.ID);

                resolucion.UsuarioID = Usuario.ID;
                resolucion.Fecha = DateTime.Now;
                resolucion.ProyectoID = Proyecto.ID;
                resolucion.Estado = 1;
       
                db.Resolucion.Add(resolucion);
                db.SaveChanges();

                try
                {
                    if (file.ContentLength > 0)
                    {
                        string filename = "Res" + resolucion.ProyectoID + "_" + resolucion.ID +  ".pdf";
                        string _path = Path.Combine(Server.MapPath("~/archivos"), filename);
                        file.SaveAs(_path);

                        ResolucionDescarga trDocumento = new ResolucionDescarga();
                        trDocumento.NombreArchivo = _path;
                        trDocumento.ResolucionID = resolucion.ID;
                        db.ResolucionDescarga.Add(trDocumento);
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {

                }

                TempData["Message"] = "Creado con exito " + resolucion.ResEx ;
                return RedirectToAction("Edit", new { id = resolucion.ID });  
            }

            return View(resolucion);
        }
        
        //
        // GET: /Resolucion/Edit/5
 
        public ActionResult Edit(int id)
        {
            Resolucion resolucion = db.Resolucion.Find(id);
            ViewBag.Archivo = "no";
            var doc = db.ResolucionDescarga.Where(d => d.ResolucionID == id).ToList();
            if (doc.Count() > 0)
            {
                ViewBag.Archivo = "si";
                ViewBag.TrasladoDocumento = doc;

            }
            return View(resolucion);
        }

        //
        // POST: /Resolucion/Edit/5

        [HttpPost]
        public ActionResult Edit(Resolucion resolucion, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var a = resolucion.Estado;
                var d = resolucion.ID;
 
                db.Entry(resolucion).State = EntityState.Modified;
                db.SaveChanges();

               
                try
                {
                    if (file.ContentLength > 0)
                    {
                        DateTime fec = DateTime.Now;
                        string FecX = fec.ToString("ddMMyyyy");
                        string filename = "Res" + resolucion.ProyectoID + "_" + resolucion.ID + "_" + FecX + ".pdf";
                        string _path = Path.Combine(Server.MapPath("~/archivos"), filename);
                        file.SaveAs(_path);
                        ResolucionDescarga trDocumento = db.ResolucionDescarga.Where(dx => dx.ResolucionID == resolucion.ID).FirstOrDefault();
                        
                        trDocumento.NombreArchivo = _path;
                        trDocumento.ResolucionID = resolucion.ID;
                        db.Entry(trDocumento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                catch (Exception)
                {

                }




                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }


        public FileResult Download(string ImageName)
        {
            return File(ImageName, System.Net.Mime.MediaTypeNames.Application.Octet, string.Format("Doc{0}", "Documento.pdf"));
        }

        //
        // GET: /Resolucion/Delete/5
 


        //
        // POST: /Resolucion/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Resolucion resolucion = db.Resolucion.Find(id);
    
                db.Resolucion.Remove(resolucion);
                db.SaveChanges();
            
            TempData["Message"] = "Modificado con Exito " ;
            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}