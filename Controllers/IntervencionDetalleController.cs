using LumenWorks.Framework.IO.Csv;
using SAG2.Classes;
using SAG2.Models;
using SAG2.Comun; 
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SAG2.Controllers
{
    public class IntervencionDetalleController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Constantes ctes = new Constantes();
        private Util utils = new Util();
        private ControlLog logReg = new ControlLog();
        // GET: IntervencionDetalle
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetProgramaQ(int id)
        {
            var q = db.ProgramaQ.Where(x => x.ProyectoID == id );
            List<SelectListItem> listprogramaq = new List<SelectListItem>();

            foreach (var x in q)
            {
                listprogramaq.Add(new SelectListItem
                {
                    Text = string.Format("{0} - ${1} ",x.Codigo , x.Valor),
                    Value = x.ID.ToString()
                });
            }
            return Json(new SelectList(listprogramaq, "Value", "Text"));
        }

        public JsonResult GetDatos(int id)
        {
            string  Sigla = db.Proyecto.Where(x => x.ID == id).FirstOrDefault().TipoProyecto.Sigla.ToString();   
            List<Listado> listadatos = new List<Listado>();
            var q1 = db.ParametroUss.Where(d => d.Tipo.Equals(Sigla)).ToList();
            foreach (var x in q1)
            {
                Listado Datos = new Listado();
                Datos.ID = x.ID;
                Datos.Nombre = string.Format("Cantidad: {0} {1}/{2}", x.uss, x.Mes, x.Anio);
                Datos.Tipo = "Vbase";
                listadatos.Add(Datos);  
            }

            int comuna = db.Proyecto.Where(x => x.ID == id).FirstOrDefault().Direccion.ComunaID;
            var q2 = db.PorcentajeZona.Where(d => d.ComunaID.Equals(comuna)).ToList();
            foreach (var x in q2)
            {
                Listado Datos = new Listado();
                Datos.ID = x.ID;
                Datos.Nombre = x.Nombre ;
                Datos.Tipo = "VZona";
                listadatos.Add(Datos);
            }

            var q = db.ProgramaQ.Where(x => x.ProyectoID == id  );

            try
            {
                foreach (var x in q)
                {
                    if (x.Estado != 1)
                    {
                        Listado Datos = new Listado();
                        Datos.ID = x.ID;
                        Datos.Nombre = string.Format("{0} - ${1} ", x.Codigo, x.Valor);
                        Datos.Tipo = "VUSS";
                        listadatos.Add(Datos);
                    }
                }
            }catch(Exception ){} 
            return Json(listadatos);
        }
        public ActionResult Upload()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var q3 = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).OrderBy(a => a.CodCodeni).ToList();
            List<SelectListItem> listproyecto = new List<SelectListItem>();
            listproyecto.Add(new SelectListItem
            {
                Text = "Seleccione Un Proyecto",
                Value = ""
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

            string sigla = Proyecto.TipoProyecto.Sigla  ;

            var q2 = db.ParametroUss.Where(d => d.Tipo.Equals(sigla)).ToList();
            List<SelectListItem> listuss = new List<SelectListItem>();
            listuss.Add(new SelectListItem { Text = "Seleccione Parámetro USS", Value = "2" });

            foreach (var x in q2)
            {
                listuss.Add(new SelectListItem { Text = string.Format("Cantidad: {0} {1}/{2}", x.uss, x.Mes,x.Anio), Value = x.ID.ToString() });
            }
            ViewBag.listadouss = listuss;
            int comuna = Proyecto.Direccion.ComunaID;   
            var q5 = db.PorcentajeZona.Where(d=> d.ComunaID.Equals(comuna )).ToList();
            List<SelectListItem> listPorc = new List<SelectListItem>();
            listPorc.Add(new SelectListItem { Text = "Seleccione % de Zona", Value = "" });

            foreach (var x in q5)
            {
                listPorc.Add(new SelectListItem { Text = x.Nombre, Value = x.Valor.ToString() });
            }

            ViewBag.listadoPorcenZona = listPorc;


            return View();
        }


        public ActionResult DetalleImportacion(List<IntervencionDetalle> lista)
        {
            List<IntervencionDetalle> model = lista;

            return View(model);
        }

        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult Upload(IntervencionDetalle model, HttpPostedFileBase upload, int ParametroUSSID, int ProgramaQID, int ValorPorcZona)
        {

            try
            {
                List<IntervencionDetalle> lista = new List<IntervencionDetalle>();
                List<IntervencionDetalle> listaAux = new List<IntervencionDetalle>();
                ParametroUss parametroUSS = db.ParametroUss.Find(ParametroUSSID);
                ProgramaQ ProgramaQ = db.ProgramaQ.Find(ProgramaQID);
                string pathX = "";

                double VALORUSS = parametroUSS.uss;
                double Q = ProgramaQ.Valor;
                int PeriodoI = int.Parse(model.FechaIngreso.Year.ToString());
                int TotConvenio = db.Convenio.Where(d => d.ProyectoID == model.ProyectoID && d.Periodo == PeriodoI).FirstOrDefault().NroPlazas;          

                double valorIntervencion = ((VALORUSS*Q)/100)*(100+ValorPorcZona);

                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        if (upload.FileName.EndsWith(".csv"))
                        {
                            try
                            {
                                if (upload.ContentLength > 0)
                                {
                                    string name = String.Format("Informe de atención Mensual {0}.csv",DateTime.Now.ToString().Replace(":","."));
                                    string _path = Path.Combine(Server.MapPath("~/Archivos"), name);
                                    upload.SaveAs(_path);
                                    System.Threading.Thread.Sleep(1000);

                                    pathX = _path;
                                }
                            }
                            catch(Exception ex)
                            {
                                TempData["Message"] = "Error, FALLO GUARDADO." + ex.Message;
                            }
                            StreamReader reader = new StreamReader(pathX, Encoding.GetEncoding("iso-8859-1"));
                            List<string> list1 = new List<String>();
                            List<string> list2 = new List<String>();
                            List<string> list3 = new List<String>();
                            List<string> list4 = new List<String>();
                            List<string> list5 = new List<String>();
                            List<string> list6 = new List<String>();
                            List<string> list7 = new List<String>();
                            List<string> list8 = new List<String>();
                            List<string> list9 = new List<String>();
                            List<string> list10 = new List<String>();
                            List<string> list11 = new List<String>();
                            //string vara1, vara2, vara3, vara4;
                            int linea = 0;
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (!String.IsNullOrWhiteSpace(line))
                                {
                                    if (linea > 0) { 
                                    IntervencionDetalle inter = new IntervencionDetalle();
                                    inter.FechaIngreso = model.FechaIngreso;
                                    inter.ProyectoID = model.ProyectoID;
                                    inter.Anio = inter.FechaIngreso.Year;
                                    inter.Mes = inter.FechaIngreso.Month;
                                    inter.Proyecto = db.Proyecto.Find(inter.ProyectoID);
                                    inter.EstadoPago = 0;
                                    inter.ResumenID = 0;
                                    inter.Uss = VALORUSS;
                                    inter.UssQ = Q;

                                    string[] values = line.Split(';');
                                    if (values.Length == 11)
                                    {
                                       //string  tipoArchivo = "Mensual";
                                        list1.Add(values[0]); //Cod ingreso
                                        list2.Add(values[1]); //Apellido Paterno
                                        list3.Add(values[2]); //Apellido Materno
                                        list4.Add(values[3]); // Nombres
                                        list5.Add(values[4]); // Fecha ingreso
                                        list6.Add(values[5]); // Días Atentidos
                                        list7.Add(values[6]); // Numero Intervenciones no pagables
                                        list8.Add(values[7]); // numero intervenciones pagables
                                        list9.Add(values[8]); // calidad Juridica 80 bis                                   
                                        list10.Add(values[9]); // a pagar                                     
                                        list11.Add(values[10]); //Estado revisión
                                        inter.CodigoSename = values[0];
                                        inter.ApellidoPaterno = values[1];
                                        inter.ApellidoMaterno = values[2];
                                        inter.Nombre = values[3];
                                        inter.DiasAtencion = int.Parse(values[5]);
                                        inter.DiasAusente = int.Parse(values[7]);
                                        inter.NumInter = int.Parse(values[8]);
                                        inter.TotalIntervencionesAPagar = int.Parse(values[9]);
                                        inter.Bis = int.Parse(values[8]);
                                        inter.BisArch = int.Parse(values[8]);
                                        if (inter.Bis > 0 && inter.TotalIntervencionesAPagar > 0)
                                        {
                                            inter.Tipo = 2;
                                        }
                                        else if (inter.Bis > 0 && inter.TotalIntervencionesAPagar < 1)
                                        {
                                            inter.Tipo = 1;
                                        }
                                        else
                                        {
                                            inter.Tipo = 0;
                                        }

                                        inter.TotalPagar = (int)((inter.TotalIntervencionesAPagar) * valorIntervencion);
                                        inter.TotalPagarBis = (int)(inter.Bis * valorIntervencion);
                                        inter.TotalPagarNoBis = (int)(inter.TotalIntervencionesAPagar * valorIntervencion);
                                        listaAux.Add(inter);
                                    }
                                    if(values.Length == 9)
                                    {
                                      //  string tipoArchivo = "DAM";
                                        list1.Add(values[0]);
                                        list2.Add(values[1]);
                                        list3.Add(values[2]);
                                        list4.Add(values[3]);
                                        list5.Add(values[4]);
                                        list6.Add(values[5]);
                                        list7.Add(values[6]);
                                        list8.Add(values[7]);
                                        list9.Add(values[8]);
                                    }
                                        }
                                    linea++;
                                }
                            }

                           
  
                                int TotIntervenciones = listaAux.Sum(d => d.TotalIntervencionesAPagar);
                                int TotBis = listaAux.Where(d => d.TotalIntervencionesAPagar == 1).Sum(d => d.Bis);
                                int TotNobis = TotIntervenciones - TotBis;
                                int TbisP = TotIntervenciones - TotConvenio;
                                int Nnovis = 0;
                                int j = 1;
                                if (TotBis > TbisP)
                                {
                                    Nnovis = TotBis - TbisP;
                                }

                                var x2 = listaAux.Where(d => d.Bis == 0 && d.TotalIntervencionesAPagar == 1).ToList();
                                foreach (var dato2 in x2)
                                {
                                    lista.Add(dato2);
                                } 

                                var x = listaAux.Where(d => d.Bis == 1 && d.TotalIntervencionesAPagar == 1).ToList();
                                foreach (var dato in x)
                                {
                                    if (Nnovis >= j)
                                    {
                                        dato.Bis = 0;
                                        if (dato.Bis > 0 && dato.TotalIntervencionesAPagar > 0)
                                        {
                                            dato.Tipo = 2;
                                        }
                                        else if (dato.Bis > 0 && dato.TotalIntervencionesAPagar < 1)
                                        {
                                            dato.Tipo = 1;
                                        }
                                        else
                                        {
                                            dato.Tipo = 0;
                                        }

                                        dato.TotalPagar = (int)((dato.TotalIntervencionesAPagar) * valorIntervencion);
                                        dato.TotalPagarBis = (int)(dato.Bis * valorIntervencion);
                                        dato.TotalPagarNoBis = (int)(dato.TotalIntervencionesAPagar * valorIntervencion);
                                        lista.Add(dato);
                                        j++;
                                    }
                                    else
                                    {
                                        lista.Add(dato);
                                    }
                                }
                                

                                                   
                                return View("DetalleImportacion",lista);
                        }
                        else
                        {
                            TempData["Message"] += "Formato de archivo no soportado.";
                            return RedirectToAction("Upload");
                        }
                    }
                    else
                    {
                        TempData["Message"] += "Seleccione su archivo.";
                    }
                }
            }
            catch
            {
                TempData["Message"] += "Error, verifique los datos del archivo.";
            }
            return RedirectToAction("Upload");
        }

        public ActionResult ResumenAtenciones2(int idProyecto)
        {
            List<IntervencionResumen> model = db.IntervencionResumen.Where(a=> a.EstadoID == 2 && a.ProyectoID == idProyecto).ToList();
            

            return View(model);
        }

        public ActionResult ListarPagos()
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

        public ActionResult ReportePorProyecto()
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
        public ActionResult ListarPagos(int ProyectoID)
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
            List<IntervencionDetalle> model = db.IntervencionDetalle.Where(a =>a.ProyectoID == ProyectoID).ToList();
            
            return View(model);
        }

        public ActionResult eliminarIntervencion(int id)
        {
            Persona Persona = (Persona)Session["Persona"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Usuario usuario = (Usuario)Session["Usuario"];
            int periodo = (int)Session["Periodo"];
           int Mes = (int)Session["Mes"];
            IntervencionResumen model = db.IntervencionResumen.Find(id);

            int ProyectoID = model.ProyectoID;
            string Descripcion = "Se procede a eliminar Intervencion";
            int CLog = logReg.RegistraControl("Intervencion", Descripcion, periodo, Mes, usuario.ID, ProyectoID);

            string strsql = "insert into intervencion_resumenLog SELECT ProyectoID, RegionID, ComunaID, Anio, Mes, Uss, UssQ, Valor, Monto, Plaza,PlazaConvenio , Tipo, Descripcion, EstadoID, NumDocumento, CompIngreso, FechaIngreso, " + CLog + "";
            strsql = strsql + " FROM intervencion_resumen  WHERE ID = " + id;

            db.Database.ExecuteSqlCommand(strsql);
            db.Database.ExecuteSqlCommand("DELETE FROM intervencion_detalle WHERE ResumenID = " + id);
            db.Database.ExecuteSqlCommand("DELETE FROM intervencion_resumen WHERE ID = " + id);
            
           
 
            // Guardar en Log

            // Eliminar Intervencion !!!

   
            return RedirectToAction("ResumenAtenciones");

        }

        public ActionResult ResumenAtenciones()
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona persona = (Persona)Session["Persona"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }  
            List<IntervencionResumen> model = db.IntervencionResumen.Where(a => a.ProyectoID == Proyecto.ID).OrderByDescending(a => a.ID ).ToList();
            ViewBag.ProyectoID = Proyecto.ID;
            return View(model);
        }

        [HttpPost]
        public ActionResult ResumenAtenciones(int ProyectoID)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona persona = (Persona)Session["Persona"];
            ViewBag.ProyectoID = ProyectoID;
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }   
           List<IntervencionResumen> model = db.IntervencionResumen.Where(a => a.ProyectoID == ProyectoID).ToList();

            return View(model);
        }
        public ActionResult AsignarPagos()
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
        public ActionResult AsignarPagos(int ProyectoID)
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
            List<IntervencionDetalle> model = db.IntervencionDetalle.Where(a => a.ProyectoID == ProyectoID && a.EstadoPago == 0).ToList();

            return View(model);
        }

        public ActionResult AsignarPagadoP(int id)
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

            IntervencionDetalle inte = db.IntervencionDetalle.Find(id);
            inte.EstadoPago = 1;
            db.Entry(inte).State = EntityState.Modified;
            db.SaveChanges();
            
            List<IntervencionDetalle> model = db.IntervencionDetalle.Where(a => a.ProyectoID == inte.ProyectoID && a.EstadoPago ==0).ToList();
            
            return View("AsignarPagos",model);
        }

        public ActionResult AsignarPendienteP(int id)
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

            IntervencionDetalle inte = db.IntervencionDetalle.Find(id);
            inte.EstadoPago = 2;
            db.Entry(inte).State = EntityState.Modified;
            db.SaveChanges();


            List<IntervencionDetalle> model = db.IntervencionDetalle.Where(a => a.ProyectoID == inte.ProyectoID && a.EstadoPago == 0).ToList();

            return View("AsignarPagos", model);
        }



        public ActionResult AsignarPagado(int id)
        {
            IntervencionResumen model = db.IntervencionResumen.Find(id);
            model.EstadoID = 1;
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ResumenAtenciones2", new { idProyecto = model.ProyectoID  });

        }

        public ActionResult AsignarPendiente(int id)
        {
            IntervencionDetalle model = db.IntervencionDetalle.Find(id);
            model.EstadoPago = 2;
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("ResumenAtenciones2", new { idProyecto = model.ProyectoID  });
        }

        [HttpPost]
        public ActionResult RealizarIngreso(IntervencionResumen model, string Descripcion, long Monto)
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

            int cuentaID = 0;
            if (model.Tipo == 1)
            {
                cuentaID = 5;
            } else
            {
                cuentaID = 3;
            }

            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            int saldoFinal = 0;
            int periodo = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Session.Remove("DetalleIngreso");

            int NroComprobante = 1;

            if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Periodo == periodo).Count() > 0)
            {
                NroComprobante = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
            }

            Movimiento movimiento = new Movimiento();
            DateTime fecha = DateTime.Today;

            string cheque = String.Format("{0}{1}{2}",fecha.Day,fecha.Month,fecha.Year);

            movimiento.Descripcion = Descripcion;
            movimiento.Fecha = fecha;
            movimiento.ProyectoID = Proyecto.ID;
            movimiento.CuentaCorrienteID = CuentaCorriente.ID;
            movimiento.Mes = (int)Session["Mes"];
            movimiento.Periodo = (int)Session["Periodo"];
            movimiento.PersonaID = null;
            movimiento.DetalleEgresoID = null;
            movimiento.ProveedorID = null;
            movimiento.TipoComprobanteID = ctes.tipoIngreso;
            movimiento.CuentaID = cuentaID;
            movimiento.Monto_Ingresos = model.Monto;
            movimiento.Cheque = int.Parse(cheque);
            movimiento.Saldo = saldoFinal;
            movimiento.NDocumento = 0;
            movimiento.NumeroComprobante = NroComprobante;

            try
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                movimiento.UsuarioID = Usuario.ID;
                movimiento.FechaCreacion = DateTime.Now;
                
                if (ModelState.IsValid)
                {
                    db.Movimiento.Add(movimiento);
                    db.SaveChanges();

                    IntervencionResumen inter = db.IntervencionResumen.Find(model.ID);
                   

                    inter.Monto = model.Monto + inter.Monto ;
                    if ((inter.Valor - inter.Monto) <= 0)
                  //  if ( inter.Monto == 0)
                    {
                        inter.EstadoID = 1;
                    }
                    else
                    {
                        inter.EstadoID = 2;
                    }

                    //inter.EstadoID = 1;
                    
                    
                    db.Entry(inter).State = EntityState.Modified;
                    db.SaveChanges();

                    if (utils.ingresarSaldoIngreso(movimiento, ModelState))
                    {
                        @ViewBag.Mensaje = utils.mensajeOK("Ingreso registrado con éxito!");
                    }
                    else
                    {
                        throw new Exception("Ocurrio un error al actualiza el saldo de la cuenta corriente.");
                    }
                    TempData["Message"] = "Ingreso Creado Con éxito!";
                }
                else
                {
                    utils.erroresState(ModelState);
                    throw new Exception("Ocurrió un error al registrar el ingreso");
                }         
            }catch(Exception ex)
            {
                TempData["Message"] = "Error" + ex.Message ;
            }
             return View("ResumenAtenciones");
        }



        [HttpPost]
        public ActionResult GuardarDetalles(List<IntervencionDetalle> model)
        {
            int id = model[0].ProyectoID;
            try
            {
                Proyecto pro = db.Proyecto.Find(id);

                int  MontoMedPro = model.Sum(a => a.TotalPagar) - model.Sum(a => a.TotalPagarBis);
                int Monto80BIs = model.Sum(a => a.TotalPagarBis);

                int InterveBIS = model.Sum(a => a.TotalIntervencionesAPagar) - model.Sum(a => a.Bis); 
                int InterveMedPro = model.Sum(a => a.Bis);

                IntervencionResumen IMedPro = new IntervencionResumen();

                IMedPro.ProyectoID = id;
                IMedPro.EstadoID = 2;
                IMedPro.FechaIngreso = DateTime.Now;
                IMedPro.Mes = model[0].Mes;
                IMedPro.Anio = model[0].Anio;
                IMedPro.Descripcion = "Medida de Proteccion";
                IMedPro.Tipo = 0;
                IMedPro.Plaza = InterveMedPro;
                IMedPro.PlazaConvenio = pro.Convenio.NroPlazas;
                IMedPro.ComunaID = pro.Direccion.ComunaID;
                IMedPro.RegionID = pro.Direccion.Comuna.RegionID;
                IMedPro.Monto = 0;
                IMedPro.Valor = MontoMedPro;
                IMedPro.CompIngreso = 0;
                IMedPro.NumDocumento = 0;
                IMedPro.Uss = model[0].Uss;
                IMedPro.UssQ = model[0].UssQ;

                IntervencionResumen IBis = new IntervencionResumen();

                IBis.ProyectoID = id;
                IBis.EstadoID = 2;
                IBis.FechaIngreso = DateTime.Now;
                IBis.Mes = model[0].Mes;
                IBis.Anio = model[0].Anio;
                IBis.Tipo = 1;
                IBis.Descripcion = "80 Bis";
                IBis.Plaza = InterveBIS;
                IBis.PlazaConvenio = pro.Convenio.NroPlazas;
                IBis.ComunaID = pro.Direccion.ComunaID;
                IBis.RegionID = pro.Direccion.Comuna.RegionID;
                IBis.Monto = 0;
                IBis.Valor = Monto80BIs;
                IBis.CompIngreso = 0;
                IBis.NumDocumento = 0;
                IBis.Uss = model[0].Uss;
                IBis.UssQ = model[0].UssQ;

                db.IntervencionResumen.Add(IBis);
                db.SaveChanges();
                db.IntervencionResumen.Add(IMedPro);
                db.SaveChanges();
                List<IntervencionResumen> lista = db.IntervencionResumen.Where(a => a.ProyectoID == id).ToList();

                IntervencionResumen IbisSave = lista.Where(a => a.Tipo == 1).LastOrDefault();
                IntervencionResumen IMedProSave = lista.Where(a => a.Tipo == 0).LastOrDefault();

                foreach (var item in model)
                {

                    if(item.Tipo == 0)
                    {
                        item.ResumenID = IMedProSave.ID; ;
                    }

                    if (item.Tipo == 1)
                    {
                        item.ResumenID = IbisSave.ID;
                    }

                    db.IntervencionDetalle.Add(item);
                    db.SaveChanges();
                }
                TempData["Message"] = "Creado con exito " + model.Count() + " registros.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error" + ex.Message;
            }
            return RedirectToAction("ResumenAtenciones2", new {idProyecto = id });
        }

        public ActionResult Listado()
        {
            List<IntervencionDetalle> model = db.IntervencionDetalle.ToList();
            return View(model);
        }

        public ActionResult IngresarMonto(int id)
        {
            IntervencionResumen model = db.IntervencionResumen.Find(id);

            return View(model);
        }

        public ActionResult ResultadoPorProyecto(int id)
        {
            IntervencionResumen model = new IntervencionResumen();
            model.ProyectoID = id;
            return View("ReporteProyectoResultado", model);
        }
        public ActionResult ResultadoPorProyectoEX(int id)
        {
            IntervencionResumen model = new IntervencionResumen();
            model.ProyectoID = id;
            return View("ReporteProyectoResultadoEX", model);
        }
        public ActionResult ResultadoPorTipoProyecto(int id)
        {
            TipoProyecto model = new TipoProyecto();
            model.ID = id;
            return View("ReportePorTipoProyectoResultado", model);
        }

        public ActionResult ReporteTipoProyecto()
        {
            var q3 = db.TipoProyecto.ToList();
            List<SelectListItem> listproyecto = new List<SelectListItem>();
            listproyecto.Add(new SelectListItem
            {
                Text = "Seleccione Un Tipo De Proyecto",
                Value = "0"
            });

            foreach (var y in q3)
            {
                listproyecto.Add(new SelectListItem
                {
                    Text = y.Sigla,
                    Value = y.ID.ToString()
                });
            }
            ViewBag.listadoproyecto = listproyecto;

            return View();
        }

        public ActionResult ReporteTipoProyectoResultadoEX(int id)
        {
            TipoProyecto model = new TipoProyecto();
            model.ID = id;
            return View("ReporteTipoProyectoResultadoEX", model);
        }

        public ActionResult ReportePorLinea()
        {
            var q3 = db.LineasAtencion.ToList();
            List<SelectListItem> listproyecto = new List<SelectListItem>();
            listproyecto.Add(new SelectListItem
            {
                Text = "Seleccione Un Tipo De Proyecto",
                Value = "0"
            });

            foreach (var y in q3)
            {
                listproyecto.Add(new SelectListItem
                {
                    Text = y.Nombre,
                    Value = y.ID.ToString()
                });
            }
            ViewBag.listadoproyecto = listproyecto;

            return View();
        }
        public ActionResult ResultadoPorLinea(int id)
        {
            LineasAtencion model = new LineasAtencion();
            model.ID = id;
            return View("ReportePorLineaResultado", model);
        }

        public ActionResult ReporteLineaResultadoEX(int id)
        {
            LineasAtencion model = new LineasAtencion();
            model.ID = id;
            return View("ReporteLineaResultadoEX", model);
        }

        public ActionResult ReportePorEstado()
        {
            var q3 = db.EstadoIntervencion.ToList();
            List<SelectListItem> listproyecto = new List<SelectListItem>();
            listproyecto.Add(new SelectListItem
            {
                Text = "Seleccione Un Estado",
                Value = "0"
            });

            foreach (var y in q3)
            {
                listproyecto.Add(new SelectListItem
                {
                    Text = y.Nombre,
                    Value = y.ID.ToString()
                });
            }
            ViewBag.listadoproyecto = listproyecto;

            return View();
        }
        public ActionResult ResultadoPorEstado(int id)
        {
            EstadoIntervencion model = new EstadoIntervencion();
            model.ID = id;
            return View("ReporteEstadoResultado", model);
        }

        public ActionResult ReporteEstadoResultadoEX(int id)
        {
            LineasAtencion model = new LineasAtencion();
            model.ID = id;
            return View("ReporteEstadoResultadoEX", model);
        }
        public ActionResult ReportePorTipoIntervencion()
        {
            //var q3 = db.EstadoIntervencion.ToList();
            //List<SelectListItem> listproyecto = new List<SelectListItem>();
            //listproyecto.Add(new SelectListItem
            //{
            //    Text = "Seleccione Un Estado",
            //    Value = "0"
            //});

            //foreach (var y in q3)
            //{
            //    listproyecto.Add(new SelectListItem
            //    {
            //        Text = y.Nombre,
            //        Value = y.ID.ToString()
            //    });
            //}
            //ViewBag.listadoproyecto = listproyecto;

            return View();
        }
        public ActionResult ResultadoTipoIntervencion(int id)
        {
            IntervencionResumen model = new IntervencionResumen();
            model.Tipo = id;
            return View("ReportePorTipoIntervencionResultado", model);
        }
        public ActionResult ReporteTipoIntervencionResultadoEX(int id)
        {
            IntervencionResumen model = new IntervencionResumen();
            model.Tipo = id;
            return View("ReporteTipoIntervencionResultadoEX", model);
        }

        public ActionResult ReporteRegionComuna()
        {
            var q3 = db.Region.ToList();
            List<SelectListItem> listproyecto = new List<SelectListItem>();
            listproyecto.Add(new SelectListItem
            {
                Text = "Seleccione Una Región",
                Value = "0"
            });

            foreach (var y in q3)
            {
                listproyecto.Add(new SelectListItem
                {
                    Text = y.Nombre,
                    Value = y.ID.ToString()
                });
            }
            ViewBag.listadoproyecto = listproyecto;

            return View();
        }
        [HttpPost]
        public JsonResult GetComuna(int id)
        {
            
            var q = db.Comuna.Where(z => z.RegionID == id);
            List<SelectListItem> listDependenciasProyecto = new List<SelectListItem>();
            listDependenciasProyecto.Add(new SelectListItem
            {
                Text = "Seleccione una comuna",
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

        public ActionResult ResultadoRegion(int id)
        {
            Region model = new Region();
            model.ID = id;
            return View("ReporteRegionResultado", model);
        }

        public ActionResult ResultadoComuna(int id)
        {
            Comuna model = new Comuna();
            model.ID = id;
            return View("ReporteComunaResultado", model);
        }

        public ActionResult ReporteComunaResultadoEX(int id)
        {
            Comuna model = new Comuna();
            model.ID = id;
            return View("ReporteComunaResultadoEX", model);
        }

        public ActionResult ReporteRegionResultadoEX(int id)
        {
            Region model = new Region();
            model.ID = id;
            return View("ReporteRegionResultadoEX", model);
        }

        public ActionResult ReporteEntreFechas()
        {
            return View();
        }

        public ActionResult ResultadoEntreFechas(int mesInicio, int anioInicio, int mesHasta, int anioHasta)
        {
            //int[] fechas = new int[4] { mesInicio, anioInicio, mesHasta, anioHasta};
            //ViewData["fechas"] = fechas;
            ViewBag.mesInicio = mesInicio;
            ViewBag.mesHasta = mesHasta;
            ViewBag.anioInicio = anioInicio;
            ViewBag.anioHasta = anioHasta;
            EstadoIntervencion model = new EstadoIntervencion();
            return View(model);
        }

        public ActionResult ResultadoEntreFechasEX(int mesInicio, int anioInicio, int mesHasta, int anioHasta)
        {

            ViewBag.mesInicio = mesInicio;
            ViewBag.mesHasta = mesHasta;
            ViewBag.anioInicio = anioInicio;
            ViewBag.anioHasta = anioHasta;
            EstadoIntervencion model = new EstadoIntervencion();
            return View("ReporteEntreFechasEX", model);
        }

        public ActionResult ReporteAnual()
        {
            return View();
        }

        public ActionResult ResultadoAnual(int anioInicio, int anioHasta)
        {
            ViewBag.anioInicio = anioInicio;
            ViewBag.anioHasta = anioHasta;
            EstadoIntervencion model = new EstadoIntervencion();
            return View("ResultadoAnual",model);
        }

        public ActionResult ReporteAnualEX(int anioInicio, int anioHasta)
        {
            ViewBag.anioInicio = anioInicio;
            ViewBag.anioHasta = anioHasta;
            EstadoIntervencion model = new EstadoIntervencion();
            return View("ReporteAnualEX", model);
        }
    }
}