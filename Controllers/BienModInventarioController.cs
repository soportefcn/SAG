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

    public class BienModInventarioController : Controller
    {

        private SAG2DB db = new SAG2DB();

        public ActionResult Index()
        {

            BienModInventarioVM model = new BienModInventarioVM();

            model.lista = new List<BienModInventarioVM>();





            try
            {

                Usuario usuario = (Usuario)Session["Usuario"];

                Proyecto proyecto = (Proyecto)Session["Proyecto"];



                List<BienMovimiento> listaMov = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1 && a.EstadoID == 1).ToList();



                if (listaMov.Count() > 0)
                {

                    List<BienModInventario> listaModel = new List<BienModInventario>();





                    foreach (var item in listaMov)
                    {

                        List<BienMovimiento> listTransf = db.BienMovimiento.Where(a => a.BienID == item.BienID && a.AutorizacionAuditor == 1 && a.EstadoID == 3).ToList();

                        List<BienMovimiento> listBaja = db.BienMovimiento.Where(a => a.BienID == item.BienID && a.AutorizacionAuditor == 1 && a.EstadoID == 2).ToList();





                        if (listTransf.Count() == 0 && listBaja.Count() == 0)
                        {

                            item.Bien.MovimientoID = item.ID;



                            listaModel.Add(item.Bien);

                        }

                    }

                    foreach (var item in listaModel)
                    {

                        if (item.ProyectoID == proyecto.ID)
                        {

                            BienModInventarioVM obj = new BienModInventarioVM();

                            obj.ID = item.ID;

                            obj.Familia = item.Familia.Nombre + " - " + item.SubFamilia.Nombre;

                            obj.Estado = item.Estado.Nombre;

                            obj.DescripcionBien = item.DescripcionBien;

                            obj.MovimientoID = item.MovimientoID;

                            obj.Cantidad = item.Cantidad;

                            obj.CondicionText = item.Condicion.Nombre;



                            obj.MontoInt = item.Monto;



                            if (item.Egreso != null) 
                            {

                                obj.ComentarioAuditor = item.Egreso.Egreso.NumeroComprobante.ToString();

                            }

                            if (item.Reintegro != null) 
                            {

                                obj.ComentarioAuditor = item.Reintegro.Reintegro.NumeroComprobante.ToString();

                            }
                            int dependencia = int.Parse(item.Ubicacion);
                            obj.Ubicacion = db.Dependencia.Where(x => x.ID == dependencia).FirstOrDefault().Nombre;

                            obj.Proyecto = item.Proyecto.Nombre;

                            //procedencia se usa para pasar id de movimiento de bienes

                            obj.ProcedenciaID = item.ProcedenciaID;

                            model.lista.Add(obj);

                        }

                    }

                }

            }
            catch (Exception ex)
            {

                TempData["Message"] = "Sin permiso para esta accion" + ex.Message;

            }

            return View(model);

        }



        public ActionResult Details(int id)
        {

            BienModInventario model = db.BienModInventario.Find(id);



            var q = db.BienFamilia.ToList();

            List<SelectListItem> listfamilia = new List<SelectListItem>();



            foreach (var x in q)
            {

                listfamilia.Add(new SelectListItem

                {

                    Text = x.Nombre,

                    Value = x.ID.ToString()

                });

            }

            ViewBag.listadofamilia = listfamilia;



            var q2 = db.BienSubFamilia.ToList();

            List<SelectListItem> listsubfamilia = new List<SelectListItem>();

            foreach (var x in q2)
            {

                listsubfamilia.Add(new SelectListItem

                {

                    Text = x.Nombre,

                    Value = x.ID.ToString()

                });

            }

            ViewBag.listadosubfamilia = listsubfamilia;



            var q3 = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).OrderBy(a => a.CodCodeni).ToList();

            List<SelectListItem> listproyecto = new List<SelectListItem>();

            foreach (var y in q3)
            {

                listproyecto.Add(new SelectListItem

                {

                    Text = y.NombreEstado,

                    Value = y.ID.ToString()

                });

            }

            ViewBag.listadoproyecto = listproyecto;



            var q4 = db.BienEstadoInventario.ToList();

            List<SelectListItem> listestado = new List<SelectListItem>();

            foreach (var z in q4)
            {

                listestado.Add(new SelectListItem

                {

                    Text = z.Nombre,

                    Value = z.ID.ToString()

                });

            }

            ViewBag.listadoestado = listestado;



            var q5 = db.BienCondicion.ToList();

            List<SelectListItem> listcondicion = new List<SelectListItem>();

            foreach (var b in q5)
            {

                listcondicion.Add(new SelectListItem

                {

                    Text = b.Nombre,

                    Value = b.ID.ToString()

                });

            }

            ViewBag.listacondicion = listcondicion;



            var q6 = db.BienProcedencia.ToList();

            List<SelectListItem> listorigen = new List<SelectListItem>();

            foreach (var c in q6)
            {

                listorigen.Add(new SelectListItem

                {

                    Text = c.Nombre,

                    Value = c.ID.ToString()

                });

            }

            ViewBag.listaorigen = listorigen;

            return View(model);

        }



        #region Create

        public ActionResult Create()
        {

            try
            {

                Proyecto Proyecto = (Proyecto)Session["Proyecto"];

                int id = 0;

                int largopre = 0;

                largopre = int.Parse(Proyecto.CodCodeni);

                int largo = largopre.ToString().Length;

                bool fromBD = false;

                try
                {

                    id = db.BienModInventario.Where(a => a.ProyectoID == Proyecto.ID).Max(a => a.ID);

                    fromBD = true;

                }

                catch (Exception ex)
                {
                    TempData["Message"] = "Sin permiso para esta accion" + ex.Message;
                    id = 0;

                }



                if ( id <= 0)
                {

                    id = int.Parse(string.Format("{0}{1}", Proyecto.CodCodeni, "01"));

                }



                int CodCodeniInt = int.Parse(Proyecto.CodCodeni);

                string CodCodeni = CodCodeniInt.ToString();

                string CodBien = (id + 1).ToString();

                string CodeniBien = "";
                try
                {
                    CodeniBien = CodBien.Substring(0, CodCodeni.Length);
                }
                catch {
                    CodeniBien = CodBien;
                }

                string aux = (int.Parse(Proyecto.CodCodeni + "01")).ToString();



                if (CodeniBien != CodCodeni)
                {

                    int cerosint = (id.ToString().Length - CodCodeni.Length);

                    string cerosstring = string.Empty;

                    for (int i = 0; i < cerosint; i++)
                    {

                        cerosstring += "0";

                    }

                    id = int.Parse(CodCodeni + "1" + cerosstring);

                }

                else if (id != int.Parse(aux) || id == int.Parse(aux) && fromBD == true)
                {

                    id = id + 1;

                }

                ViewBag.NroBien = id;



                var q = db.BienFamilia.ToList();

                List<SelectListItem> listfamilia = new List<SelectListItem>();

                listfamilia.Add(new SelectListItem

                {

                    Text = "Seleccione Una Familia",

                    Value = "0"

                });



                foreach (var x in q)
                {

                    listfamilia.Add(new SelectListItem

                    {

                        Text = x.Nombre,

                        Value = x.ID.ToString()

                    });

                }

                ViewBag.listadofamilia = listfamilia;



                var u = db.Usuario.ToList();



                List<SelectListItem> listUsuario = new List<SelectListItem>();



                listUsuario.Add(new SelectListItem

                {

                    Text = "Seleccione Un Usuario",

                    Value = "0"

                });



                foreach (var item in u)
                {

                    listUsuario.Add(new SelectListItem

                    {

                        Text = string.Format("{0} {1} {2}", item.Persona.Nombres, item.Persona.ApellidoParterno, item.Persona.ApellidoMaterno),

                        Value = item.ID.ToString()

                    });

                }

                ViewBag.listadoUsuario = listUsuario;



                var q2 = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).OrderBy(a => a.CodCodeni).ToList();

                List<SelectListItem> listproyecto = new List<SelectListItem>();

                foreach (var y in q2)
                {

                    listproyecto.Add(new SelectListItem

                    {

                        Text = y.NombreEstado,

                        Value = y.ID.ToString()

                    });

                }

                ViewBag.listadoproyecto = listproyecto;



                var q3 = db.BienEstadoInventario.ToList();

                List<SelectListItem> listestado = new List<SelectListItem>();

                foreach (var z in q3)
                {

                    listestado.Add(new SelectListItem

                    {

                        Text = z.Nombre,

                        Value = z.ID.ToString()

                    });

                }

                ViewBag.listadoestado = listestado;



                var q4 = db.BienCondicion.ToList();

                List<SelectListItem> listcondicion = new List<SelectListItem>();

                foreach (var a in q4)
                {

                    listcondicion.Add(new SelectListItem

                    {

                        Text = a.Nombre,

                        Value = a.ID.ToString()

                    });

                }

                ViewBag.listacondicion = listcondicion;



                var q5 = db.BienProcedencia.ToList();

                List<SelectListItem> listorigen = new List<SelectListItem>();

                foreach (var b in q5)
                {

                    listorigen.Add(new SelectListItem

                    {

                        Text = b.Nombre,

                        Value = b.ID.ToString()

                    });

                }

                ViewBag.listaorigen = listorigen;



            }
            catch (Exception ex)
            {

                TempData["Message"] = "Sin permiso para esta accion" + ex.Message;

            }

            return View();

        }



        [HttpPost]

        public ActionResult Save(BienModInventarioVM model, HttpPostedFileBase archivo, int tipo)
        {

            Usuario usuario = (Usuario)Session["Usuario"];

            Proyecto proyecto = (Proyecto)Session["Proyecto"];
            int ultimaID = 0;
            try
            {
                try
                {
                     ultimaID = db.BienModInventario.ToList().Last().ID;
                }
                catch {
                    ultimaID = int.Parse(string.Format("{0}{1}", proyecto.CodCodeni, "01"));
                }
                BienModInventario bien = new BienModInventario();
                BienMovimiento mov = new BienMovimiento();

     
                bien.ID = ultimaID + 1;
                bien.Monto = model.MontoInt;
                bien.UsuarioID = model.UsuarioID;
                bien.ProyectoID = proyecto.ID;
                bien.Fecha = model.Fecha;
                bien.Cantidad = model.Cantidad;
                bien.DescripcionBien = model.DescripcionBien;
                bien.Ubicacion = model.Ubicacion;
                bien.CondicionID = model.CondicionID;
                bien.FamiliaID = model.FamiliaID;
                bien.SubFamiliaID = model.SubFamiliaID;
                bien.EstadoID = 1;
                bien.ProcedenciaID = model.ProcedenciaID;
                bien.MovimientoID = model.MovimientoID;

                if (tipo == 1)
                {
                 bien.EgresoID = model.EgresoID;
                 bien.ReintegroID = 0; 
                }

                else if (tipo == 2)
                {

                    bien.ReintegroID = model.EgresoID;

                }



                try
                {

                    if (archivo.ContentLength > 0)
                    {
                        string name = string.Format("Archivo-{0}-{1}-{2}{3}{4}.{5}", bien.ProyectoID, bien.ID, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, Path.GetFileName(archivo.FileName));
                        string _path = Path.Combine(Server.MapPath("~/Archivos"), name);

                        mov.RutaArchivo = _path;

                        archivo.SaveAs(_path);

                    }

                    if (archivo == null || mov.RutaArchivo == null)
                    {

                        throw new Exception();

                    }



                    //db

                    db.Database.ExecuteSqlCommand("PABien " + bien.ID + ", " + bien.FamiliaID + ", " + bien.SubFamiliaID + ", " + bien.ProcedenciaID + ", '" + bien.DescripcionBien + "', " + bien.Cantidad + ", " + bien.Monto + ", '" + bien.Ubicacion + "', " + bien.EstadoID + ", " + bien.ProyectoID + ", " + bien.ProyectoID + ", " + bien.UsuarioID + ", " + bien.EgresoID + ", " + bien.CondicionID + ", " + bien.ReintegroID + ", " + bien.MovimientoID );                   

                    

                    mov.UsuarioID = usuario.ID;
                    mov.CondicionID = model.CondicionID;
                    mov.AutorizacionAuditor = 0;
                    mov.Cantidad = model.Cantidad;
                    mov.EstadoID = 1;
                    mov.Detalle = model.Detalle;
                    mov.NuevaUbicacion = bien.Ubicacion;
                    mov.FechaMovimiento = DateTime.Now;
                    mov.ComentarioAuditor = "";
                    mov.BienID = bien.ID;
                    db.BienMovimiento.Add(mov);
                    db.SaveChanges();



                    TempData["Message"] += "Creado con exito " + model.DescripcionBien;



                }

                catch (Exception ex)
                {

                    TempData["Message"] = "Error en la carga de archivo, seleccione un archivo válido." + ex.Message;

                    //TempData["Message"] += "Error en la carga de archivo, seleccione un archivo válido.";

                }

            }

            catch (Exception ex)
            {

                TempData["Message"] += "Error al guardar" + ex.Message;
                     

            }

            return RedirectToAction("Create");

        }

        #endregion



        public ActionResult DetailAlta(int id)
        {

            BienModInventarioVM model = new BienModInventarioVM();

            Dependencia model2 = new Dependencia();





            BienMovimiento bienMovimiento = db.BienMovimiento.Where(a => a.BienID == id && a.EstadoID == 1).First();



            try
            {

                BienModInventario bien = db.BienModInventario.Find(id);

                model.ID = bien.ID;

                model.Proyecto = bien.Proyecto.Nombre;

                model.Familia = bien.Familia.Nombre;

                model.SubFamilia = bien.SubFamilia.Nombre;

                model.CondicionText = bien.Condicion.Nombre;

                model.Estado = bien.Estado.Nombre;

                model.RutaArchivo = bienMovimiento.RutaArchivo;

                model.Ubicacion = bien.Ubicacion;

                model.Usuario = bien.Usuario.Persona.NombreCompleto;

                model.DescripcionBien = bien.DescripcionBien;

                model.Fecha = bien.Fecha;

                if (bien.Reintegro != null) 
                {

                    model.Reintegro = bien.Reintegro;

                }

                if (bien.Egreso != null)
                {

                    model.Egreso = bien.Egreso;

                } 

                return View(model);

            }

            catch (Exception ex)
            {
                TempData["Message"] += "Error al guardar" + ex.Message;
            }



            return View(model);

        }





        #region Edit

        public ActionResult Edit(int id)
        {

            BienModInventario model = db.BienModInventario.Find(id);

            BienMovimiento bienMovimiento = db.BienMovimiento.Where(a => a.BienID == id && a.EstadoID == 1).First();





            ViewBag.NroBien = id;



            var u = db.Usuario.ToList();

            List<SelectListItem> listUsuario = new List<SelectListItem>();

            Usuario seleccionado = db.Usuario.Where(a => a.ID == model.UsuarioID).First();





            listUsuario.Add(new SelectListItem

            {

                Text = string.Format("{0} {1} {2}", seleccionado.Persona.Nombres, seleccionado.Persona.ApellidoParterno, seleccionado.Persona.ApellidoMaterno),



                Value = seleccionado.ID.ToString()

            });



            foreach (var item in u)
            {

                listUsuario.Add(new SelectListItem

                {

                    Text = string.Format("{0} {1} {2}", item.Persona.Nombres, item.Persona.ApellidoParterno, item.Persona.ApellidoMaterno),

                    Value = item.ID.ToString()

                });

            }

            ViewBag.listadoUsuario = listUsuario;



            var q = db.BienFamilia.ToList();

            List<SelectListItem> listfamilia = new List<SelectListItem>();

            listfamilia.Add(new SelectListItem

            {

                Text = "Seleccione Una Familia",

                Value = "0"

            });



            foreach (var x in q)
            {

                listfamilia.Add(new SelectListItem

                {

                    Text = x.Nombre,

                    Value = x.ID.ToString()

                });

            }

            ViewBag.listadofamilia = listfamilia;



            var q2 = db.BienSubFamilia.ToList();

            List<SelectListItem> listsubfamilia = new List<SelectListItem>();

            listsubfamilia.Add(new SelectListItem

            {

                Text = "Seleccione Una Sub Familia",

                Value = "0"

            });

            foreach (var x in q2)
            {

                listsubfamilia.Add(new SelectListItem

                {

                    Text = x.Nombre,

                    Value = x.ID.ToString()

                });

            }

            ViewBag.listadosubfamilia = listsubfamilia;



            var q3 = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).OrderBy(a => a.CodCodeni).ToList();

            List<SelectListItem> listproyecto = new List<SelectListItem>();

            foreach (var y in q3)
            {

                listproyecto.Add(new SelectListItem

                {

                    Text = y.NombreEstado,

                    Value = y.ID.ToString()

                });

            }

            ViewBag.listadoproyecto = listproyecto;



            var q4 = db.BienEstadoInventario.ToList();

            List<SelectListItem> listestado = new List<SelectListItem>();

            foreach (var z in q4)
            {

                listestado.Add(new SelectListItem

                {

                    Text = z.Nombre,

                    Value = z.ID.ToString()

                });

            }

            ViewBag.listadoestado = listestado;



            var q5 = db.BienCondicion.ToList();

            List<SelectListItem> listcondicion = new List<SelectListItem>();

            foreach (var b in q5)
            {

                listcondicion.Add(new SelectListItem

                {

                    Text = b.Nombre,

                    Value = b.ID.ToString()

                });

            }

            ViewBag.listacondicion = listcondicion;



            var q6 = db.BienProcedencia.ToList();

            List<SelectListItem> listorigen = new List<SelectListItem>();

            foreach (var c in q6)
            {

                listorigen.Add(new SelectListItem

                {

                    Text = c.Nombre,

                    Value = c.ID.ToString()

                });

            }

            ViewBag.listaorigen = listorigen;



            return View(model);

        }



        [HttpPost]

        public ActionResult Update(BienModInventario model)
        {

            try
            {

                model.EstadoID = 1;





                Proyecto proyecto = (Proyecto)Session["Proyecto"];

                model.ProyectoID = proyecto.ID;

                db.Entry(model).State = EntityState.Modified;

                db.SaveChanges();

                TempData["Message"] = "Actualizado con exito " + model.DescripcionBien;

            }

            catch (Exception ex)
            {

                TempData["Message"] += "Error al guardar" + ex.Message;

            }

            return RedirectToAction("Create");

        }



        #endregion



        [HttpPost]

        public JsonResult GetSubFamilia(int id)
        {

            var q = db.BienSubFamilia.Where(x => x.FamiliaID == id);

            List<SelectListItem> listsubfamilia = new List<SelectListItem>();



            foreach (var x in q)
            {

                listsubfamilia.Add(new SelectListItem

                {

                    Text = x.Nombre,

                    Value = x.ID.ToString()

                });

            }

            return Json(new SelectList(listsubfamilia, "Value", "Text"));

        }



        public ActionResult Delete(int id)
        {

            try
            {

                BienModInventario BienModInventario = db.BienModInventario.Find(id);

                db.BienModInventario.Remove(BienModInventario);

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



        #region Traslados

        public ActionResult Traslado()
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



            var q4 = db.BienCondicion.ToList();

            List<SelectListItem> listcondicion = new List<SelectListItem>();

            listcondicion.Add(new SelectListItem

            {

                Text = "Seleccione Una Condicion",

                Value = "0"

            });



            foreach (var a in q4)
            {

                listcondicion.Add(new SelectListItem

                {

                    Text = a.Nombre,

                    Value = a.ID.ToString()

                });

            }

            ViewBag.listacondicion = listcondicion;



            var q5 = db.Usuario.ToList();

            List<SelectListItem> listusuario = new List<SelectListItem>();

            listusuario.Add(new SelectListItem

            {

                Text = "Seleccione Un Usuario",

                Value = "0"

            });



            foreach (var a in q5)
            {

                listusuario.Add(new SelectListItem

                {

                    Text = string.Format("{0} {1} {2}", a.Persona.Nombres, a.Persona.ApellidoParterno, a.Persona.ApellidoMaterno),

                    Value = a.ID.ToString()

                });

            }

            ViewBag.listadousuarios = listusuario;







            return View();

        }



        public ActionResult DetailTraslado(int id)
        {

            BienMovimiento mov = db.BienMovimiento.Find(id);

            BienModInventario bien = db.BienModInventario.Find(mov.BienID);

            BienModInventarioVM model = new BienModInventarioVM();

            try
            {

                model.ID = bien.ID;

                model.Cantidad = mov.Cantidad;

             //   model.Proyecto = bien.ProyectoAnterior.Nombre;

                model.Familia = bien.Familia.Nombre;

                model.SubFamilia = bien.SubFamilia.Nombre;

                model.Procedencia = bien.Ubicacion;

                model.CondicionText = mov.Condicion.Nombre;

                model.Ubicacion = mov.NuevaUbicacion;

                model.Usuario = bien.Usuario.Persona.NombreCompleto;

                model.DescripcionBien = bien.DescripcionBien;

                return View(model);

            }

            catch
            {



            }

            return View(model);

        }



        [HttpPost]

        public ActionResult Trasladar(BienModInventarioVM modelVM, string NuevaUbicacion)
        {

            //try

            //{

            BienMovimiento mov = new BienMovimiento();

            int ultimaID = db.BienModInventario.ToList().Last().ID;



            BienModInventario model = db.BienModInventario.Find(modelVM.ID);

            BienModInventario model2 = new BienModInventario();

            mov.EstadoID = 3;

            mov.Detalle = modelVM.Detalle;

            mov.Cantidad = modelVM.Cantidad;

            mov.NuevaUbicacion = modelVM.Ubicacion2;

            mov.FechaMovimiento = DateTime.Now;

            mov.UsuarioID = modelVM.UsuarioID;

            mov.CondicionID = modelVM.CondicionID;

            mov.BienID = ultimaID + 1;

            if (db.BienMovimiento.ToList().Where(z => z.BienID == mov.BienID).Count() > 1)
            {

                mov.BienID = mov.BienID + 1;

            }

            mov.ComentarioAuditor = "";

            mov.AutorizacionAuditor = 0;

            mov.bienAnteriorID = model.ID;



            BienMovimiento movAlta = db.BienMovimiento.Where(a => a.EstadoID == 1 && a.BienID == model.ID).First();



            db.BienMovimiento.Add(mov);

            db.SaveChanges();



            if (modelVM.Archivo.ContentLength > 0)
            {

                string name = string.Format("Traslado-{0}-{1}", model.Fecha.ToString().Replace(":", ""), Path.GetFileName(modelVM.Archivo.FileName));

                string _path = Path.Combine(Server.MapPath("~/Archivos"), name);

                mov.RutaArchivo = _path;

                modelVM.Archivo.SaveAs(_path);

            }



            //model.ProyectoID = modelVM.ProyectoAnteriorID;

            //model.ProyectoAnteriorID = modelVM.ProyectoID;

            //        model.Ubicacion = modelVM.Ubicacion2;

            //db.BienModInventario.Add(model2);

            //        db.Entry(model).State = EntityState.Modified;

            db.SaveChanges();





            TempData["Message"] = "Trasladado con éxito " + model.DescripcionBien;

            //}

            //catch (Exception ex)

            //{

            // TempData["Message"] = "Error en la operación.";

            //}

            return RedirectToAction("Traslado");

        }



        [HttpPost]

        public JsonResult GetBienesTraslado(int id)
        {

            List<BienModInventario> listInv = new List<BienModInventario>();



            List<SelectListItem> listaBienes = new List<SelectListItem>();

            List<BienMovimiento> listaMovAlta = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1 && a.EstadoID == 1).ToList();

            List<BienMovimiento> listaMovOtros = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1 && (a.EstadoID == 2 || a.EstadoID == 3)).ToList();





            foreach (var item in listaMovAlta)
            {

                BienModInventario bien = db.BienModInventario.Find(item.BienID);



                if (bien.ProyectoID == id)
                {

                    listInv.Add(bien);

                }

            }



            foreach (var item in listInv)
            {

                foreach (var item2 in listaMovOtros)
                {

                    if (item2.BienID == item.ID || item2.bienAnteriorID == item.ID)
                    {

                        item.Cantidad = item.Cantidad - item2.Cantidad;

                    }

                }





            }



            listaBienes.Add(new SelectListItem

            {

                Text = "Seleccione un Bien",

                Value = "-1"

            });



            foreach (var x in listInv)
            {

                if (x.Cantidad > 0)
                {

                    listaBienes.Add(new SelectListItem

                    {

                        Text = x.ID + " - " + x.DescripcionBien,

                        Value = x.ID.ToString()

                    });

                }

            }

            return Json(new SelectList(listaBienes, "Value", "Text"));

        }



        [HttpPost]

        public JsonResult GetBienesCreate(int id)
        {

            var q = db.BienModInventario.Where(x => x.ProyectoID == id).OrderBy(a => a.ID);

            List<SelectListItem> listbienes = new List<SelectListItem>();

            listbienes.Add(new SelectListItem

            {

                Text = "Seleccione Un Bien",

                Value = "0"

            });



            foreach (var x in q)
            {

                string valorNro = "";

                if (x.ID.ToString().Length == 1)
                {

                    valorNro = "2050" + x.ID;

                }

                else if (x.ID.ToString().Length > 2)
                {

                    valorNro = "205" + x.ID;

                }



                listbienes.Add(new SelectListItem

                {

                    Text = valorNro + " - " + x.DescripcionBien,

                    Value = x.ID.ToString()

                });

            }

            return Json(new SelectList(listbienes, "Value", "Text"));

        }



        [HttpPost]

        public JsonResult GetDatosBien(int id)
        {

            if (id > 0)
            {

                var q = db.BienModInventario.Where(x => x.ID == id).First();

                var listOtrosMovimientos = db.BienMovimiento.Where(a => a.BienID == id && a.EstadoID != 1).ToList();



                int cantidad = 0;

                foreach (var item in listOtrosMovimientos)
                {

                    cantidad = cantidad + item.Cantidad;

                }



                BienModInventario model = q;

                BienModInventarioVM bien = new BienModInventarioVM();



                var ubicacionID = int.Parse(model.Ubicacion);

                var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();

                Dependencia model2 = w;

                bien.Ubicacion = model2.Nombre;

                bien.Cantidad = model.Cantidad - cantidad;

                bien.Familia = model.Familia.Nombre;

                bien.SubFamilia = model.SubFamilia.Nombre;

                bien.Usuario = string.Format("{0}", model.Usuario.Persona.NombreCompleto);

                return Json(bien);

            }

            else
            {

                BienModInventarioVM bien = new BienModInventarioVM();

                return Json(bien);

            }



        }



        [HttpPost]

        public JsonResult GetProyectoBien(int id)
        {

            //var q = db.BienModInventario.Where(x => x.ProyectoID == id).OrderBy(a => a.ID);

            var q = db.Dependencia.Where(z => z.ProyectoID == id);

            List<SelectListItem> listDependenciasProyecto = new List<SelectListItem>();

            listDependenciasProyecto.Add(new SelectListItem

            {

                Text = "Seleccione una dependencia",

                Value = "0"

            });



            foreach (var x in q)
            {

                listDependenciasProyecto.Add(new SelectListItem

                {

                    Text = x.Nombre,

                    Value = x.ID.ToString()

                });

            }

            return Json(new SelectList(listDependenciasProyecto, "Value", "Text"));

        }



        public ActionResult Traslados()
        {

            BienModInventarioVM model = new BienModInventarioVM();

            model.lista = new List<BienModInventarioVM>();

            Proyecto proyecto = (Proyecto)Session["Proyecto"];

            List<BienMovimiento> listmov = db.BienMovimiento.Where(a => a.EstadoID == 3 && a.AutorizacionAuditor == 1).ToList();



            foreach (var item in listmov)
            {

                BienModInventario bien = db.BienModInventario.Find(item.BienID);

                BienModInventarioVM obj = new BienModInventarioVM();



                if (bien.ProyectoID == proyecto.ID)
                {

                    obj.ID = bien.ID;

                    obj.MovimientoBienID = item.ID;

                    obj.DescripcionBien = bien.DescripcionBien;

                    obj.Detalle = item.Detalle;

                    obj.Cantidad = item.Cantidad;

                    obj.Fecha = item.FechaMovimiento;

                    obj.Usuario = bien.Usuario.Persona.NombreCompleto;

                    obj.Familia = bien.Familia.Nombre + " - " + bien.SubFamilia.Nombre;

                    obj.CondicionText = bien.Condicion.Nombre;

                    obj.Ubicacion = item.NuevaUbicacion;

                 //   obj.Proyecto = bien.ProyectoAnterior.Nombre;

                    obj.Estado = item.Estado.Nombre;



                    if (bien.Egreso != null)
                    {

                        obj.Monto = bien.Egreso.Monto.ToString();

                        obj.NDocumento = bien.Egreso.NDocumento.ToString();

                    }



                    if (bien.Reintegro != null)
                    {

                        obj.Monto = bien.Reintegro.Monto.ToString();

                        obj.NDocumento = bien.Reintegro.NDocumento.ToString();

                    } 

                    model.lista.Add(obj);

                }

            }

            return View(model);

        }



        #endregion



        #region Bajas

        public ActionResult Baja()
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



            var q4 = db.BienCondicion.ToList();

            List<SelectListItem> listcondicion = new List<SelectListItem>();

            listcondicion.Add(new SelectListItem

            {

                Text = "Seleccione Una Condición",

                Value = "0"

            });



            foreach (var a in q4)
            {

                listcondicion.Add(new SelectListItem

                {

                    Text = a.Nombre,

                    Value = a.ID.ToString()

                });

            }

            ViewBag.listacondicion = listcondicion;



            return View();

        }



        [HttpPost]

        public ActionResult AsignarBaja(BienModInventarioVM modelVM)
        {

            try
            {

                Usuario usuario = (Usuario)Session["Usuario"];

                BienMovimiento mov = new BienMovimiento();

                BienModInventario model = db.BienModInventario.Find(modelVM.ID);

                mov.EstadoID = 2;

                mov.Detalle = modelVM.Detalle;

                mov.Cantidad = modelVM.Cantidad;

                mov.FechaMovimiento = DateTime.Now;

                mov.UsuarioID = usuario.ID;

                mov.CondicionID = modelVM.CondicionID;

                mov.ComentarioAuditor = "";

                mov.AutorizacionAuditor = 0;

                mov.BienID = model.ID;

                mov.NuevaUbicacion = model.Ubicacion;



                if (modelVM.Archivo.ContentLength > 0)
                {

                    string name = string.Format("Baja-{0}-{1}-{2}", model.ProyectoID, model.ID, Path.GetFileName(modelVM.Archivo.FileName));

                    string _path = Path.Combine(Server.MapPath("~/Archivos"), name);

                    mov.RutaArchivo = _path;

                    modelVM.Archivo.SaveAs(_path);

                }



                db.BienMovimiento.Add(mov);

                db.SaveChanges();

                TempData["Message"] = "Baja con éxito: " + model.DescripcionBien + "...En espera de aprobación";

            }

            catch (Exception ex)
            {

                TempData["Message"] += "Error al guardar" + ex.Message;

            }

            return RedirectToAction("Baja");

        }



        public ActionResult Bajas()
        {

            BienModInventarioVM model = new BienModInventarioVM();

            model.lista = new List<BienModInventarioVM>();



            Proyecto proyecto = (Proyecto)Session["Proyecto"];



            List<BienMovimiento> listmov = db.BienMovimiento.Where(a => a.EstadoID == 2 && a.AutorizacionAuditor == 1).ToList();





            foreach (var item in listmov)
            {

                BienModInventario bien = db.BienModInventario.Find(item.BienID);



                BienModInventarioVM obj = new BienModInventarioVM();



                if (bien.ProyectoID == proyecto.ID)
                {

                    obj.ID = bien.ID;

                    obj.MovimientoBienID = item.ID;

                    obj.DescripcionBien = bien.DescripcionBien;

                    obj.Detalle = item.Detalle;

                    obj.Cantidad = item.Cantidad;

                    obj.Fecha = item.FechaMovimiento;

                    obj.RutaArchivo = item.RutaArchivo;

                    obj.Usuario = bien.Usuario.Persona.NombreCompleto;

                    obj.Familia = bien.Familia.Nombre + " - " + bien.SubFamilia.Nombre;

                    obj.CondicionText = item.Condicion.Nombre;

                    obj.Ubicacion = bien.Ubicacion;

                    obj.Proyecto = bien.Proyecto.Nombre;

                    obj.Estado = item.Estado.Nombre;



                    if (bien.Egreso != null) 
                    {

                        obj.Monto = bien.Egreso.Monto.ToString();

                        obj.NDocumento = bien.Egreso.NDocumento.ToString();

                    }



                    if (bien.Reintegro != null)
                    {

                        obj.Monto = bien.Reintegro.Monto.ToString();

                        obj.NDocumento = bien.Reintegro.NDocumento.ToString();

                    } 

                    model.lista.Add(obj);

                }

            }

            return View(model);

        }



        public ActionResult DetailBaja(int id, int movimientoID)
        {



            BienModInventarioVM model = new BienModInventarioVM();

            BienMovimiento bienMovimiento = db.BienMovimiento.Find(movimientoID);



            try
            {

                BienModInventario bien = db.BienModInventario.Find(id);

                model.ID = bien.ID;

                model.Proyecto = bien.Proyecto.Nombre;

                model.Auditor = bienMovimiento.Auditor;

                model.Detalle = bienMovimiento.Detalle;

                model.Familia = bien.Familia.Nombre;

                model.SubFamilia = bien.SubFamilia.Nombre;

                model.CondicionText = bienMovimiento.Condicion.Nombre;

                model.Estado = bienMovimiento.Estado.Nombre;

                model.RutaArchivo = bienMovimiento.RutaArchivo;

                model.Ubicacion = bien.Ubicacion;

                model.Usuario = bien.Usuario.Persona.NombreCompleto;

                model.DescripcionBien = bien.DescripcionBien;

                return View(model);

            }

            catch (Exception ex)
            {
                TempData["Message"] += "Error al guardar" + ex.Message;
            }



            return View(model);



        }



        public JsonResult GetBienesBaja(int id)
        {



            List<BienModInventario> listInv = new List<BienModInventario>();



            List<SelectListItem> listaBienes = new List<SelectListItem>();

            List<BienMovimiento> listaMovAlta = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1 && a.EstadoID == 1).ToList();

            List<BienMovimiento> listaMovOtros = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1 && (a.EstadoID == 2 || a.EstadoID == 3)).ToList();



            List<SAG2.Models.Dependencia> deps = new List<SAG2.Models.Dependencia>().ToList();

            SAG2.Models.Dependencia dependencias = new SAG2.Models.Dependencia();

            int Proyecto = ((SAG2.Models.Proyecto)Session["Proyecto"]).ID;

            ViewBag.Dependencia = new SelectList(deps, "ID", "Nombre", Proyecto);



            using (SAG2.Models.SAG2DB depen = new SAG2.Models.SAG2DB())
            {

                var ListaDepen = new SelectList(depen.Dependencia.ToList().Where(p => p.ProyectoID == Proyecto), "ID", "Nombre");

                ViewData["ListaDependencia"] = ListaDepen;

            }







            foreach (var item in listaMovAlta)
            {

                BienModInventario bien = db.BienModInventario.Find(item.BienID);



                if (bien.ProyectoID == id)
                {
                   
                        var listOtrosMovimientos = db.BienMovimiento.Where(a => a.BienID == bien.ID   && a.EstadoID != 1).ToList();



                        int cantidad = 0;

                        foreach (var item2 in listOtrosMovimientos)
                        {

                            cantidad = cantidad + item2.Cantidad;

                        }

                        int x = bien.Cantidad - cantidad; 
                    if(x>0){

                        listInv.Add(bien);
                    }
                }

            }





            foreach (var item in listInv)
            {

                foreach (var item2 in listaMovOtros)
                {

                    if (item2.BienID == item.ID)
                    {

                        item.Cantidad = item.Cantidad - item2.Cantidad;

                    }

                }





            }



            listaBienes.Add(new SelectListItem

            {

                Text = "Seleccione un Bien",

                Value = "-1"

            });



            foreach (var x in listInv)
            {

                if (x.Cantidad > 0)
                {

                    listaBienes.Add(new SelectListItem

                    {

                        Text = x.ID + " - " + x.DescripcionBien,

                        Value = x.ID.ToString()

                    });

                }

            }

            return Json(new SelectList(listaBienes, "Value", "Text"));

        }

        public FileResult Download(string ImageName)
        {









            return File(ImageName, System.Net.Mime.MediaTypeNames.Application.Octet, string.Format("DocumentoAdjunto{0}", ImageName));

        }



        #endregion



        public ActionResult ListadoDetalles()
        {



            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            int periodo = (int)Session["Periodo"];

            int mes = (int)Session["Mes"];



            var egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Nulo == null && a.Cuenta.Codigo.StartsWith("8")).OrderByDescending(d => d.Egreso.ID).ToList();

            return View(egresos);

        }



        public ActionResult ListadoDetalles2()
        {



            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            int periodo = (int)Session["Periodo"];

            int mes = (int)Session["Mes"];



            var egresos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == Proyecto.ID).OrderByDescending(d => d.Reintegro.ID).ToList();

            return View(egresos);

        }







    }

}

