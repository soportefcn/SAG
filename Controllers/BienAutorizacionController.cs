using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Comun; 


namespace SAG2.Controllers
{

    public class BienAutorizacionController : Controller
    {

        private SAG2DB db = new SAG2DB();

        public ActionResult Index( int? ProyectoID)
        {
            if (ProyectoID == null)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                ProyectoID = Proyecto.ID;
            }
            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null), "ID", "NombreLista", ProyectoID);
         
            return View();
        }



        public ActionResult Lista(int id)
        {

            BienModInventarioVM model = new BienModInventarioVM();



            var q = db.Proyecto.ToList();

            List<SelectListItem> listproyecto = new List<SelectListItem>();

            listproyecto.Add(new SelectListItem

            {

                Text = "Seleccione Un Proyecto",

                Value = "0"

            });



            foreach (var x in q)
            {

                listproyecto.Add(new SelectListItem

                {

                    Text = x.NombreEstado,

                    Value = x.ID.ToString()

                });

            }



            List<BienModInventarioVM> lista = new List<BienModInventarioVM>();



            List<BienMovimiento> listaMov = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 0).ToList();





            foreach (var item in listaMov)
            {

                BienModInventario bmi = db.BienModInventario.Find(item.BienID);

                if (item.EstadoID == 3)
                {

                    bmi = db.BienModInventario.Find(item.bienAnteriorID);

                }









                if (bmi.ProyectoID == id)
                {

                    BienModInventarioVM bmivm = new BienModInventarioVM();





                    string ruta = item.RutaArchivo;



                    bmivm.ID = bmi.ID;

                    bmivm.MovimientoID = item.ID;

                    bmivm.ProyectoID = bmi.ProyectoID;

                    bmivm.Detalle = item.Detalle;

                    bmivm.DescripcionBien = bmi.DescripcionBien;

                    bmivm.CondicionText = item.Estado.Nombre;



                    bmivm.Cantidad = item.Cantidad;

                    bmivm.RutaArchivo = item.RutaArchivo;



                 /*   if (bmi.Egreso != null) // Error modelo
                    {

                        bmivm.SubFamilia = bmi.Egreso.Monto.ToString("#,##0");

                        bmivm.Familia = bmi.Egreso.NDocumento.ToString();

                    }

                    if (bmi.Reintegro != null) // Error MOdelo
                    {

                        bmivm.SubFamilia = bmi.Reintegro.Monto.ToString("#,##0");

                        bmivm.Familia = bmi.Reintegro.NDocumento.ToString();

                    } */

                    lista.Add(bmivm);

                }

                else
                {

                } 

            }

            ViewBag.listadoproyecto = listproyecto;

            model.ProyectoID = id;



            model.lista = lista;



            return View(model);

        }


        public ActionResult BienesAnulados()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            List<BienMovimiento> listmov = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 2 && a.Bien.ProyectoID == Proyecto.ID ).ToList();
            List<BienModInventario> listBien = new List<BienModInventario>();
            BienModInventarioVM model = new BienModInventarioVM();
            model.lista = new List<BienModInventarioVM>();
            foreach (var item in listmov)
            {
                BienModInventario bien = db.BienModInventario.Find(item.BienID);
                listBien.Add(bien);
                BienModInventarioVM bienVM = new BienModInventarioVM();

                bienVM.Proyecto = bien.Proyecto.NombreEstado;
                bienVM.Usuario = item.Auditor.Persona.NombreCompleto;
                bienVM.Familia = bien.Familia.Nombre + " - " + bien.SubFamilia.Nombre;
                bienVM.DescripcionBien = bien.DescripcionBien;
                bienVM.Detalle = item.Detalle;
                bienVM.Estado = item.Estado.Nombre;
                bienVM.Cantidad = item.Cantidad;
                bienVM.Procedencia = item.ComentarioAuditor;
                bienVM.CondicionText = bien.Condicion.Nombre;
                if (bien.Egreso != null) 
                {
                    bienVM.NDocumento = bien.Egreso.NDocumento.ToString();
                    bienVM.Monto = bien.Egreso.Monto.ToString();
                }

                if (bien.Reintegro != null)
                {
                    bienVM.NDocumento = bien.Reintegro.NDocumento.ToString();
                    bienVM.Monto = bien.Reintegro.Monto.ToString();
                } 
                model.lista.Add(bienVM);
            }
            return View(model);
        }



        public FileResult Download(string ImageName)
        {

            return File(ImageName, System.Net.Mime.MediaTypeNames.Application.Octet, string.Format("DocumentoAdjunto{0}", ImageName));

        }



        public ActionResult Permitir(int id)
        {



            BienMovimiento bien = db.BienMovimiento.Find(id);
            BienModInventario bienInv = db.BienModInventario.Find(bien.BienID);

            var proyectoUbicacion = db.Dependencia.Find(int.Parse(bien.NuevaUbicacion));

            if (bien.EstadoID == 3)
            {

                bienInv = db.BienModInventario.Find(bien.bienAnteriorID);

            }

            //try 

            //{

            Usuario usuario = (Usuario)Session["Usuario"];
            bien.AutorizacionAuditor = 1;
            bien.AuditorID = usuario.ID;

            var proyectoIDAn = bienInv.ProyectoAnteriorID.HasValue ? bienInv.ProyectoAnteriorID.Value : 0;



            if (bien.EstadoID == 3)
            {

                BienModInventario bienTraspaso = new BienModInventario();
                bienTraspaso.ID = bien.BienID;
                bienTraspaso.Fecha = bienInv.Fecha;
                bienTraspaso.FamiliaID = bienInv.FamiliaID;
                bienTraspaso.SubFamiliaID = bienInv.SubFamiliaID;
                bienTraspaso.ProcedenciaID = bienInv.ProcedenciaID;
                bienTraspaso.DescripcionBien = bienInv.DescripcionBien;
                bienTraspaso.Cantidad = bien.Cantidad;
                bienTraspaso.Monto = bienInv.Monto;
                bienTraspaso.Ubicacion = bien.NuevaUbicacion;
                bienTraspaso.EstadoID = 3;
                bienTraspaso.ProyectoAnteriorID = bienInv.ProyectoID;
                bienTraspaso.ProyectoID = proyectoUbicacion.ProyectoID;
                bienTraspaso.UsuarioID = usuario.ID;
                bienTraspaso.EgresoID = bienInv.EgresoID;
                bienTraspaso.CondicionID = bienInv.CondicionID;
                bienTraspaso.ReintegroID = bienInv.ReintegroID;
                bienTraspaso.MovimientoID = bienInv.MovimientoID;



                BienMovimiento mov = new BienMovimiento();
                int ultimaID = db.BienModInventario.ToList().Last().ID;
                mov.EstadoID = 1;
                mov.Detalle = bien.Detalle;
                mov.Cantidad = bien.Cantidad;
                mov.NuevaUbicacion = bien.NuevaUbicacion;
                mov.FechaMovimiento = DateTime.Now;
                mov.UsuarioID = bien.UsuarioID;
                mov.CondicionID = bien.CondicionID;
                mov.BienID = ultimaID + 1;
                if (db.BienMovimiento.ToList().Where(z => z.BienID == bienTraspaso.ID).Count() > 1)
                {
                    bienTraspaso.ID = ultimaID + 1;
                    mov.BienID = bienTraspaso.ID;
                }

                mov.ComentarioAuditor = "";
                mov.AutorizacionAuditor = 1;
                mov.AuditorID = 1;
                mov.RutaArchivo = bien.RutaArchivo;
                mov.bienAnteriorID = bien.BienID;
                db.BienMovimiento.Add(mov);
                db.BienModInventario.Add(bienTraspaso);

            }

            // db.Entry(bien).State = EntityState.Modified;

            db.SaveChanges();
            // correo 
            var Proyecto = db.Proyecto.Where(d => d.ID == bienInv.ProyectoID).FirstOrDefault().NombreLista;
            string MensajeCorreo = "El Bien se  Aprobado Correctamente <br> Para : ";
            MensajeCorreo = MensajeCorreo + "<table style='border: 1px solid black;'><tr><td>Proyecto</td><td>" + Proyecto + "</td></tr>";
            MensajeCorreo = MensajeCorreo + "<tr><td>Bien</td><td>" + bien.Detalle + "</td></tr> </table>";


            var supervisorCorreo = db.Rol.Where(d => d.TipoRolID == 9 && d.ProyectoID == bienInv.ProyectoID).ToList();
            foreach (var Scorreo in supervisorCorreo)
            {
                string CorreoSup = db.Persona.Where(d => d.ID == Scorreo.PersonaID).FirstOrDefault().CorreoElectronico;
                Correo.enviarCorreo(CorreoSup, MensajeCorreo, "Autorizacion Modificación");
            }
            TempData["Message"] = "Bien Aprobado Correctamente";

            //}



            //catch (Exception ex)

            //{

            //    TempData["Message"] = "Error al Aprobar el Bien:" + ex;

            //}



            return RedirectToAction("Index");



        }



        public ActionResult Anular(int id)
        {

            BienMovimiento model = db.BienMovimiento.Find(id);

            return View(model);

        }



        [HttpPost]

        public JsonResult GetBienes(int id)
        {

            List<BienModInventarioVM> lista = new List<BienModInventarioVM>();
            List<BienMovimiento> listaMov = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 0 && a.Bien.ProyectoID.Equals(id) ).ToList();

            foreach (var item in listaMov)
            {

                BienModInventario bmi = db.BienModInventario.Where(d => d.ID.Equals(item.BienID)).FirstOrDefault();  
                if (item.EstadoID == 3)
                {
                    bmi = db.BienModInventario.Find(item.bienAnteriorID);
                }

                if (bmi.ProyectoID == id )
                {

                    BienModInventarioVM bmivm = new BienModInventarioVM();





                    string ruta = item.RutaArchivo;



                    ruta.Replace("/", "%5C");

                    ruta.Replace(":", "%3C");

                    if (db.BienMovimiento.ToList().Where(z => z.BienID == bmi.ID).Count() > 1)
                    {

                        bmi = db.BienModInventario.Find(item.BienID);

                    }

                    bmivm.ID = bmi.ID;

                    bmivm.MovimientoID = item.ID;

                    bmivm.ProyectoID = bmi.ProyectoID;

                    bmivm.Detalle = item.Detalle;

                    bmivm.DescripcionBien = bmi.DescripcionBien;

                    bmivm.CondicionText = item.Estado.Nombre;



                    bmivm.Cantidad = item.Cantidad;

                    bmivm.RutaArchivo = item.RutaArchivo;

                    bmivm.SubFamilia = "$" + bmi.Monto.ToString("#,##0");
                    try
                    {
                        if (bmi.Movimiento.TipoComprobanteID == 2)
                        {
                            bmivm.Familia = bmi.Egreso.NDocumento.ToString();
                        }
                        else {
                            bmivm.Familia = bmi.Reintegro.NDocumento.ToString();
                        }
                        bmivm.Ubicacion = bmi.Movimiento.NumeroComprobante.ToString(); ;
                    }
                    catch {
                        bmivm.Familia = " ";
                        bmivm.Ubicacion = " ";
                    }






                    lista.Add(bmivm);

                }

                else
                {



                }

            }



            return Json(lista);

        }


        public ActionResult AnularSave(BienMovimiento model)
        {

            BienMovimiento bienmov = db.BienMovimiento.Find(model.ID);
            BienModInventario bien = db.BienModInventario.Find(bienmov.BienID);

            try
            {

                Usuario usuario = (Usuario)Session["Usuario"];
                bienmov.AutorizacionAuditor = 2;
                bienmov.ComentarioAuditor = model.ComentarioAuditor;
                bienmov.AuditorID = usuario.ID;
                db.Entry(bienmov).State = EntityState.Modified;
                db.SaveChanges();

                // correo

                TempData["Message"] = "Bien Anulado Correctamente";

            }

            catch (Exception ex)
            {

                TempData["Message"] = "Error al Anular el Bien:" + ex;

            }



            return RedirectToAction("Index", new { ProyectoID = bien.ProyectoID });

        }

    }

}

