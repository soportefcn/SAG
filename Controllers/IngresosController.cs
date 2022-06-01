using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using SAG2.Comun; 
using System.Data.SqlClient;

namespace SAG2.Controllers
{ 
    public class IngresosController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();

        //
        // GET: /Ingresos/
        [HttpGet]


        public ActionResult ListarLineas(Int32? t = null)
        {

            try
            {
              
                    List<DetalleIngreso> lista = new List<DetalleIngreso>();
                    lista = db.DetalleIngreso.Where(d => d.MovimientoID == 1).ToList();
                    Session.Add("DetalleIngreso", lista);
                
            }
            catch { }
            return View((List<DetalleIngreso>)Session["DetalleIngreso"]);
        }
        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int periodo = (int)Session["Periodo"];
          //  var movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.CuentaCorriente).Where(a => a.TipoComprobanteID == ctes.tipoIngreso && a.ProyectoID == Proyecto.ID && a.Temporal == null && a.CuentaID != 1 && a.Eliminado == null && a.Periodo == periodo).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            var movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.CuentaCorriente).Where(a => a.TipoComprobanteID == ctes.tipoIngreso && a.ProyectoID == Proyecto.ID && a.Temporal == null && a.CuentaID != 1 && a.Eliminado == null ).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            return View(movimiento.ToList());
        }
        [HttpPost]
        public ViewResult Index(FormCollection form)
        {
            int buscarPeriodo = 0;
            int buscarNdoc = 0;
          //  string Buscarbeneficiario = "";
            try
            {
                buscarPeriodo = int.Parse(form["busqueda"].ToString());
            }
            catch (Exception)
            {
                buscarPeriodo = (int)Session["Periodo"];
            }
            try
            {
                buscarNdoc = int.Parse(form["bEgreso"].ToString());
            }
            catch (Exception)
            {
                buscarNdoc = 0;
            }

            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            var movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo.Equals(buscarPeriodo)).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            if (buscarNdoc != 0)
            {
                movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo.Equals(buscarPeriodo) && a.NumeroComprobante.Equals(buscarNdoc)).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            }

            return View(movimiento.ToList());
        }
        //
        // GET: /Ingresos/Details/5

        public ViewResult Details(int id)
        {
            Movimiento movimiento = db.Movimiento.Find(id);
            return View(movimiento);
        }

        //
        // GET: /Ingresos/Create

        public ActionResult Create()
        {
            int periodo = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Session.Remove("DetalleIngreso");
            ViewBag.NroComprobante = "1";

            if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Periodo == periodo).Count() > 0)
            {
                ViewBag.NroComprobante = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
            }

            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaIngresos));
            ViewBag.tipoProyecto = Proyecto.TipoProyecto.Sigla;
            return View();
        } 

        //
        // POST: /Ingresos/Create

        [HttpPost]
        public ActionResult Create(Movimiento movimiento)
        {
            utils.Log(1, "Inicio proceso de ingreso");
            int entro = 1;
            int saldoFinal = 0;
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            int cuentaID = Int32.Parse(Request.Form["CuentaID"].ToString());
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            if (Proyecto.TipoProyecto.Sigla == "AC")
            {
                if (movimiento.CuentaID == 10)
                {
                    if (Session["DetalleIngreso"] != null) {
                        entro = 1;
                    } else {
                        entro = 0;
                    }
                }
            }

            if (entro == 1)
            {
                movimiento.NumeroComprobante = 1;
                movimiento.Descripcion = movimiento.Descripcion.ToUpper();

                if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Periodo == periodo).Count() > 0)
                {
                    movimiento.NumeroComprobante = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
                }

                movimiento.ProyectoID = Proyecto.ID;
                movimiento.CuentaCorrienteID = CuentaCorriente.ID;
                movimiento.Mes = (int)Session["Mes"];
                movimiento.Periodo = (int)Session["Periodo"];
                movimiento.PersonaID = null;
                movimiento.DetalleEgresoID = null;
                movimiento.ProveedorID = null;
                movimiento.TipoComprobanteID = ctes.tipoIngreso;
                movimiento.CuentaID = cuentaID;
                ViewBag.NroComprobante = movimiento.NumeroComprobante.ToString();
                ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaIngresos), cuentaID);
                movimiento.Saldo = saldoFinal;
                movimiento.NDocumento = 0;

                try
                {
                    Usuario Usuario = (Usuario)Session["Usuario"];
                    movimiento.UsuarioID = Usuario.ID;
                    movimiento.FechaCreacion = DateTime.Now;

                    if (ModelState.IsValid)
                    {
                        db.Movimiento.Add(movimiento);
                        db.SaveChanges();

                        int IngresoID = movimiento.ID;

                        if (Session["DetalleIngreso"] != null)
                        {
                            List<DetalleIngreso> lista = (List<DetalleIngreso>)Session["DetalleIngreso"];
                            foreach (DetalleIngreso detalle in lista)
                            {
                                //  monto_egresos += detalle.Monto;

                                detalle.MovimientoID = IngresoID;
                                db.DetalleIngreso.Add(detalle);
                                db.SaveChanges();
                            }
                            Session.Remove("DetalleReintegro");

                        }

                    }
                    else
                    {
                        utils.erroresState(ModelState);
                        throw new Exception("Ocurrió un error al registrar el ingreso");
                    }

                    if (utils.ingresarSaldoIngreso(movimiento, ModelState))
                    {
                        @ViewBag.Mensaje = utils.mensajeOK("Ingreso registrado con éxito!");
                    }
                    else
                    {
                        throw new Exception("Ocurrio un error al actualiza el saldo de la cuenta corriente.");
                    }
                }
                catch (SqlException e)
                {
                    @ViewBag.Mensaje = utils.mensajeError(utils.Mensaje(e));
                    return View(movimiento);
                }
                catch (Exception e)
                {
                    @ViewBag.Mensaje = utils.mensajeError(utils.Mensaje(e));
                    return View(movimiento);
                }

                utils.Log(1, "Fin proceso de ingreso");
                if (Request.Form["ImprimirComprobante"].ToString().Equals("true"))
                {
                    return RedirectToAction("Edit", new { @id = movimiento.ID, @imprimir = "true" });
                }
                else
                {
                    return RedirectToAction("Create");
                }
                //return View(movimiento);
            }else 
                {
                    @ViewBag.Mensaje = "Existe un error en la carga de la planilla";
                    return RedirectToAction("Create");
                }
        }
        
        //
        // GET: /Ingresos/Edit/5

        public ActionResult Edit(int id, string imprimir = "", string mensaje = "")
        {
            Movimiento movimiento = db.Movimiento.Find(id);
            int periodo = (int)Session["Periodo"];
            ViewBag.NroComprobante = movimiento.NumeroComprobante.ToString();
            ViewBag.Proyecto = db.Proyecto.Find(movimiento.ProyectoID).NombreLista;
            ViewBag.CuentaCorriente = db.CuentaCorriente.Find(movimiento.CuentaCorrienteID).NumeroLista;
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaIngresos), movimiento.CuentaID);
            ViewBag.Imprimir = imprimir;
            ViewBag.tipoProyecto = movimiento.Proyecto.TipoProyecto.Sigla;
            try
            {
                //ViewBag.UltimoIdentificador = db.Movimiento.Where(m => m.ProyectoID == movimiento.ProyectoID && m.TipoComprobanteID == ctes.tipoIngreso).Max(a => a.ID).ToString();
                ViewBag.UltimoIdentificador =  db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.CuentaCorriente).Where(a => a.TipoComprobanteID == ctes.tipoIngreso && a.ProyectoID == movimiento.ProyectoID && a.Temporal == null && a.CuentaID != 1 && a.Eliminado == null).Max(a => a.ID).ToString();
            }
            catch (Exception)
            {
                ViewBag.UltimoIdentificador = "0";
            }

            if (!mensaje.Equals(""))
                ViewBag.Mensaje = utils.mensajeAdvertencia(mensaje);

            return View(movimiento);
        }

        //
        // POST: /Ingresos/Edit/5

        [HttpPost]
        public ActionResult Edit(Movimiento movimiento)
        {
            Usuario Usuario = (Usuario)Session["Usuario"];
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Persona persona = (Persona)Session["Persona"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            int montoOriginal = Int32.Parse(Request.Form["MontoOriginal"].ToString());
            utils.Log(1, "Ingreso a modificar ID: " + movimiento.ID);
            movimiento.CuentaID = Int32.Parse(Request.Form["CuentaID"].ToString());
            movimiento.PersonaID = null;
            movimiento.DetalleEgresoID = null;
            movimiento.ProveedorID = null;
            movimiento.TipoComprobanteID = 1;
            int originalID = movimiento.ID;
            movimiento.Descripcion = movimiento.Descripcion.ToUpper();
            int? modificadoID = 0;

            try
            {
                ViewBag.UltimoIdentificador = db.Movimiento.Where(m => m.ProyectoID == movimiento.ProyectoID && m.TipoComprobanteID == ctes.tipoIngreso).Max(a => a.ID).ToString();
            }
            catch (Exception)
            {
                ViewBag.UltimoIdentificador = "0";
            }

            if (!movimiento.Periodo.Equals(periodo) || !movimiento.Mes.Equals(mes))
            {
                if (Usuario.esAdministrador || Usuario.esSupervisor)
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            db.Movimiento.Attach(movimiento);
                            db.Entry(movimiento).State = EntityState.Modified;
                            db.SaveChanges();

                            // Se deben actualizar los saldos

                            int mes_comprobante = movimiento.Mes;
                            int periodo_comprobante = movimiento.Periodo;
                            int mes_proyecto = mes;
                            int periodo_proyecto = periodo;

                            utils.RecalcularSaldos(periodo_comprobante, periodo_proyecto, mes_comprobante, mes_proyecto, Proyecto, CuentaCorriente);
                        }
                        catch (Exception e)
                        {
                            utils.Log(2, e.Message);
                            ViewBag.Mensaje = utils.mensajeError("No fue posible modificar este Ingreso, intente nuevamente.");
                        }
                    }
                }
                else
                {
                    // Se elimina cualquier modificación anterior, solo queda la última.
                    try
                    {
                        Autorizacion autorizaciontmp = db.Autorizacion.Where(a => a.OriginalID == originalID).Single();
                        modificadoID = autorizaciontmp.ModificadoID;
                        db.Autorizacion.Remove(autorizaciontmp);
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }

                    try
                    {
                        Movimiento tmp = db.Movimiento.Find(modificadoID);
                        db.Movimiento.Remove(tmp);
                        db.SaveChanges();
                    }
                    catch (Exception)
                    { }

                    // Edicion del movimiento debe autorizarse, cambios se registran de forma temporal.
                    movimiento.ID = 0;
                    movimiento.Temporal = "S";
                    movimiento.Eliminado = "N";

                    movimiento.UsuarioID = Usuario.ID;
                    db.Movimiento.Add(movimiento);
                    db.SaveChanges();

                    // Se registra en la tabla de Autorizaciones
                    Autorizacion autorizacion = new Autorizacion();
                    autorizacion.OriginalID = originalID;
                    autorizacion.ModificadoID = movimiento.ID;
                    autorizacion.SolicitaID = persona.ID;
                    autorizacion.Tipo = "Modificación";
                    autorizacion.FechaSolicitud = DateTime.Now;
                    db.Autorizacion.Add(autorizacion);
                    db.SaveChanges();

                    string MensajeCorreo = "Se Solicita Autorizacion <br> Para : ";
                    MensajeCorreo = MensajeCorreo + "<table><tr><td><td>Proyecto</td><td>" + movimiento.Proyecto.NombreLista + "</td>";
                    MensajeCorreo = MensajeCorreo + "<td>Tipo Comp.</td><td>Ingreso</td><td># Comp</td><td>" + movimiento.NumeroComprobante + "</td>";
                    MensajeCorreo = MensajeCorreo + "<td>Solicitado Por </td><td>" + persona.NombreCompleto + "</td><td>tipo</td><td>Modificación</td> </table>";
                    int prmov = movimiento.ProyectoID;
                    var supervisorCorreo = db.Rol.Where(d => d.TipoRolID == 4 && d.ProyectoID == prmov).ToList();
                    foreach (var Scorreo in supervisorCorreo)
                    {
                        string CorreoSup = db.Persona.Where(d => d.ID == Scorreo.PersonaID).FirstOrDefault().CorreoElectronico;

                        Correo.enviarCorreo(CorreoSup, MensajeCorreo, "Autorizacion anulacion");
                    }

                    ViewBag.Mensaje = utils.mensajeAdvertencia("La modificación ha sido solicitada al Supervisor.");
                }
            }
            else
            {
                utils.Log(1, "Modificacion dentro del periodo");
                movimiento.UsuarioID = Usuario.ID;
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Movimiento.Attach(movimiento);
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        utils.Log(2, e.Message);
                        ViewBag.Mensaje = utils.mensajeError("No fue posible modificar este Ingreso, intente nuevamente.");
                    }
                }

                if (utils.actualizarSaldoIngreso(movimiento, ModelState, montoOriginal))
                {
                    @ViewBag.Mensaje = utils.mensajeOK("Ingreso modificado exitosamente.");
                }
                else
                {
                    @ViewBag.Mensaje = utils.mensajeError("Ocurrió un error el actualizar los saldos");
                }
            }

            ViewBag.Proyecto = db.Proyecto.Find(movimiento.ProyectoID).NombreLista;
            ViewBag.CuentaCorriente = db.CuentaCorriente.Find(movimiento.CuentaCorrienteID).NumeroLista;
            ViewBag.NroComprobante = movimiento.NumeroComprobante.ToString();
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaIngresos), movimiento.CuentaID);
            ViewBag.UltimoIdentificador = "0";
            try
            {
                ViewBag.UltimoIdentificador = db.Movimiento.Where(m => m.ProyectoID == movimiento.ProyectoID && m.TipoComprobanteID == ctes.tipoIngreso && m.Periodo == periodo).Max(a => a.NumeroComprobante).ToString();
            }
            catch (Exception)
            {
                ViewBag.UltimoIdentificador = "0";
            } 
            //ViewBag.MaxComprobante = db.Movimiento.Where(m => m.ProyectoID == movimiento.ProyectoID).Where(a => a.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante);
            return View(movimiento);
        }

        public ActionResult Anular(int id)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Persona persona = (Persona)Session["Persona"];
            Movimiento movimiento = db.Movimiento.Find(id);

            if (movimiento.Periodo != periodo || movimiento.Mes != mes)
            {
                // La anulacion del movimiento debe autorizarse, cambios se registran de forma temporal.
                //movimiento.ID = 0;
                movimiento.Temporal = "S";
                movimiento.Eliminado = "N";
                movimiento.Nulo = "S";
                //movimiento.Monto_Ingresos = 0;
                db.Movimiento.Add(movimiento);
                db.SaveChanges();

                // Se registra en la tabla de Autorizaciones
                Autorizacion autorizacion = new Autorizacion();
                autorizacion.OriginalID = id;
                autorizacion.ModificadoID = movimiento.ID;
                autorizacion.SolicitaID = persona.ID;
                autorizacion.Tipo = "Anulación";
                autorizacion.FechaSolicitud = DateTime.Now;
                db.Autorizacion.Add(autorizacion);
                db.SaveChanges();

                string MensajeCorreo = "Se Solicita Autorizacion <br> Para : ";
                MensajeCorreo = MensajeCorreo + "<table><tr><td><td>Proyecto</td><td>" + movimiento.Proyecto.NombreLista + "</td>";
                MensajeCorreo = MensajeCorreo + "<td>Tipo Comp.</td><td>Ingreso</td><td># Comp</td><td>" + movimiento.NumeroComprobante + "</td>";
                MensajeCorreo = MensajeCorreo + "<td>Solicitado Por </td><td>" + persona.NombreCompleto + "</td><td>tipo</td><td>Anulacion</td> </table>";
                int prmov = movimiento.ProyectoID;
                var supervisorCorreo = db.Rol.Where(d => d.TipoRolID == 4 && d.ProyectoID == prmov).ToList();
                foreach (var Scorreo in supervisorCorreo)
                {
                    string CorreoSup = db.Persona.Where(d => d.ID == Scorreo.PersonaID).FirstOrDefault().CorreoElectronico;

                    Correo.enviarCorreo(CorreoSup, MensajeCorreo, "Autorizacion anulacion");
                }

                return RedirectToAction("Edit", new { id = @id, mensaje = "La anulación ha sido solicitada al Supervisor." });
            }
            
            // Registro de Anulacion
            int? cuentaID = movimiento.CuentaID;
            int monto = movimiento.Monto_Ingresos;


            if (ModelState.IsValid)
            {
                movimiento.Nulo = "S";
                db.Entry(movimiento).State = EntityState.Modified;
                //movimiento.Monto_Ingresos = 0;
                db.SaveChanges();
            }

            if (utils.anularSaldoIngreso(movimiento, ModelState, monto))
            {
                @ViewBag.Mensaje = utils.mensajeOK("Ingreso anulado con éxito!");
            }
            else
            {
                @ViewBag.Mensaje = utils.mensajeError("Ocurrió un error el anular el Ingreso");
            } 

            return RedirectToAction("Create");
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Movimiento movimiento = db.Movimiento.Find(id); 
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int maxComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoIngreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante);

            if (movimiento.NumeroComprobante == maxComprobante)
            {
                int monto = movimiento.Monto_Ingresos;

                if (ModelState.IsValid)
                {
                    movimiento.Nulo = "S";
                    db.Entry(movimiento).State = EntityState.Modified;
                    movimiento.Monto_Ingresos = 0;
                    db.SaveChanges();
                }

                utils.anularSaldoIngreso(movimiento, ModelState, monto);
                db.Movimiento.Remove(movimiento);
                db.SaveChanges();
            }

            return RedirectToAction("Create");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}