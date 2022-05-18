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
    public class BancoController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();

        //
        // GET: /Banco/

        public ViewResult Index(string q = "")
        {
            return View(db.Banco.OrderBy(a => a.Nombre).Where(a => a.Nombre.Contains(q)).ToList());
        }

        //
        // GET: /Banco/Details/5

        public ViewResult Details(int id)
        {
            Banco banco = db.Banco.Find(id);
            return View(banco);
        }

        //
        // GET: /Banco/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Banco/Create

        [HttpPost]
        public ActionResult Create(Banco banco)
        {
            if (ModelState.IsValid)
            {
            
                db.Banco.Add(banco);
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + banco.Nombre;
                return RedirectToAction("Create");
           
            }

            return View(banco);
        }
        
        //
        // GET: /Banco/Edit/5
 
        public ActionResult Edit(int id)
        {
            Banco banco = db.Banco.Find(id);
            return View(banco);
        }

        //
        // POST: /Banco/Edit/5

        [HttpPost]
        public ActionResult Edit(Banco banco)
        {
            if (ModelState.IsValid)
            {
                db.Entry(banco).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " + banco.Nombre;
                return RedirectToAction("Create");
            }
            return View(banco);
        }

        //
        // GET: /Banco/Delete/5
        /*
        public ActionResult Delete()
        {
            Banco banco = db.Banco.Find(id);
            db.Banco.Remove(banco);
            db.SaveChanges();
            return RedirectToAction("Create");
        }
        */
        //
        // POST: /Banco/Delete/5
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {  
        try
            {          
            Banco banco = db.Banco.Find(id);
            db.Banco.Remove(banco);
            db.SaveChanges();
            return RedirectToAction("Create");
            }
        catch (Exception)
        {
            TempData["Message"] = "No se puede eliminar el Banco tiene una Cuenta Corriente Asociada";
            return RedirectToAction("Create");
        }
        }

        public ActionResult Saldos()
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(a => a.Temporal == null && a.Eliminado == null && a.Nulo == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.NumeroComprobante);

            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyectoID != 13 && p.Cerrado == null).OrderBy(p => p.CodCodeni).ToList();

            }

            if (usuario.esSupervisor)
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

            }


            ViewBag.Saldos = db.Saldo.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).ToList();
            ViewBag.Mes = mes;
            ViewBag.periodo = periodo;
            return View(movimientos.ToList());
        }
        [HttpPost]
        public ActionResult Saldos(FormCollection form)
        {
            int periodo = Int32.Parse(form["Periodo"]);
            int mes = Int32.Parse(form["Mes"].ToString());
            var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(a => a.Temporal == null && a.Eliminado == null && a.Nulo == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.NumeroComprobante);
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyectoID != 13 && p.Cerrado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.Saldos = db.Saldo.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).ToList();
            ViewBag.Mes = mes;
            ViewBag.periodo = periodo;
            return View(movimientos.ToList());
        }
        public ActionResult Libro()
        { 
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];

            try
            {
                ViewBag.SaldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                ViewBag.SaldoInicial = 0;
            }

            //var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.Periodo).ThenBy(m => m.NumeroComprobante);
            var movimientos = db.Movimiento.Where( m => m.auto == 0).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.Fecha).ThenBy(m => m.NumeroComprobante);
            ViewBag.periodo = periodo;
            ViewBag.mes = mes;
            ViewBag.cuentaCorriente = CuentaCorriente;
            ViewBag.NumeroCuenta = CuentaCorriente.Numero;
            return View(movimientos.ToList());
        }

        [HttpPost]
        public ActionResult Libro(FormCollection form)
        {
            int periodo = Int32.Parse(form["Periodo"]);
            int mes = Int32.Parse(form["Mes"].ToString());
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];

            try
            {
                ViewBag.SaldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                ViewBag.SaldoInicial = 0;
            }

            //var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.Periodo).ThenBy(m => m.NumeroComprobante);
            var movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.Fecha).ThenBy(m => m.NumeroComprobante);
            ViewBag.periodo = periodo;
            ViewBag.mes = mes;
            ViewBag.cuentaCorriente = CuentaCorriente;
            ViewBag.NumeroCuenta = CuentaCorriente.Numero;
            return View(movimientos.ToList());
        }

        [HttpGet]
        public ActionResult Conciliacion()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Usuario Usuario = (Usuario)Session["Usuario"];

            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];

            // Periodo anterior
            try
            {
                ViewBag.SaldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                ViewBag.SaldoInicial = 0;
            }
            
            var movimientos = from m in db.Movimiento
                              where  (m.auto == 0) &&  (m.ProyectoID == Proyecto.ID) && ((m.Periodo == periodo && m.Mes == mes) || (m.Periodo < periodo) || (m.Periodo == periodo && m.Mes < mes)) && m.Temporal == null && m.Nulo == null && m.Eliminado == null && ((m.CuentaID != 1 && m.CuentaID != 6) || m.CuentaID == null)
                              orderby m.Periodo, m.Fecha, m.NumeroComprobante
                              select m;
            //var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID);
            @ViewBag.periodo = periodo;
            @ViewBag.mes = mes;
            @ViewBag.cuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            @ViewBag.NumeroCuenta = @ViewBag.cuentaCorriente.Numero;

            //ViewBag.ConciliacionID = "0";
            ViewBag.SaldoCartola = "0";
            ViewBag.GastosBancarios = "0";
            ViewBag.Depositos = "0";
            ViewBag.Fecha = DateTime.Now.ToShortDateString();

            try
            {
                //int ID = Int32.Parse(Request.Form["ConciliacionID"].ToString());
                Conciliacion Conciliacion = db.Conciliacion.Where(c => c.ProyectoID == Proyecto.ID).Where(c => c.Periodo == periodo).Where(c => c.Mes == mes).Single();
                //ViewBag.ConciliacionID = Conciliacion.ID;
                ViewBag.SaldoCartola = Conciliacion.SaldoCartola;
                ViewBag.GastosBancarios = Conciliacion.Gastos;
                ViewBag.Depositos = Conciliacion.Depositos;
                ViewBag.Fecha = Conciliacion.FechaCartola.ToShortDateString();
            }
            catch (Exception)
            { }

            ViewBag.esSupervisor = Usuario.esSupervisor;

            return View(movimientos.ToList());
        }

        [HttpPost]
        public ActionResult Conciliacion(FormCollection form)
        {
            ViewBag.ConciliacionID = "0";
            ViewBag.SaldoCartola = "0";
            ViewBag.GastosBancarios = "0";
            ViewBag.Depositos = "0";
            ViewBag.Fecha = DateTime.Now.ToShortDateString();

            int periodo = Int32.Parse(form["Periodo"]);
            int mes = Int32.Parse(form["Mes"].ToString());
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            
            // Periodo anterior
            try
            {
                ViewBag.SaldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                ViewBag.SaldoInicial = 0;
            }
            
            var movimientos = from m in db.Movimiento
                              where (m.auto == 0) && (m.ProyectoID == Proyecto.ID) && ((m.Periodo == periodo && m.Mes == mes) || (m.Periodo < periodo) || (m.Periodo == periodo && m.Mes < mes)) && m.Temporal == null && m.Nulo == null && m.Eliminado == null && ((m.CuentaID != 1 && m.CuentaID != 6) || m.CuentaID == null)
                              orderby m.Periodo, m.Fecha, m.NumeroComprobante
                              select m;
            //var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID);
            @ViewBag.periodo = periodo;
            @ViewBag.mes = mes;
            @ViewBag.cuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            @ViewBag.NumeroCuenta = @ViewBag.cuentaCorriente.Numero;

            if ((Request.Form["Guardar"] != null && Request.Form["Guardar"].ToString().Equals("Guardar")) || (Request.Form["ImprimirConciliacion"] != null && Request.Form["ImprimirConciliacion"].ToString().Equals("Imprimir")))
            {
                try
                {
                   
                   
                    Conciliacion Conciliacion = db.Conciliacion.Where(c => c.ProyectoID == Proyecto.ID).Where(c => c.Periodo == periodo).Where(c => c.Mes == mes).Single();

                    if (Request.Form["Depositos"] == null)
                    {
                        Conciliacion.Depositos = 0;
                    }
                    else
                    {
                        Conciliacion.Depositos = Int32.Parse(Request.Form["Depositos"].ToString());
                    }
                    Conciliacion.FechaCartola = DateTime.Parse(Request.Form["Fecha"].ToString());
                    Conciliacion.FechaConciliacion = DateTime.Now;
                    Conciliacion.Gastos = Int32.Parse(Request.Form["GastosBancarios"].ToString());
                    Conciliacion.SaldoCartola = Int32.Parse(Request.Form["SaldoCartola"].ToString());
                    Conciliacion.PersonaID = ((Persona)Session["Persona"]).ID;
                    db.Entry(Conciliacion).State = EntityState.Modified;
                    db.SaveChanges();

                    //ViewBag.ConciliacionID = Conciliacion.ID;
                    ViewBag.SaldoCartola = Conciliacion.SaldoCartola;
                    ViewBag.GastosBancarios = Conciliacion.Gastos;
                    ViewBag.Depositos = Conciliacion.Depositos;
                    ViewBag.Fecha = Conciliacion.FechaCartola.ToShortDateString();

                }
                catch (Exception)
                {
                    Conciliacion Conciliacion = new Conciliacion();
                    //ViewBag.ConciliacionID = Conciliacion.ID;
                    //Conciliacion Conciliacion = db.Conciliacion.Where(c => c.ProyectoID == Proyecto.ID).Where(c => c.Periodo == periodo).Where(c => c.Mes == mes).Single();
                    Conciliacion.Depositos = Int32.Parse(Request.Form["Depositos"].ToString());
                    Conciliacion.FechaCartola = DateTime.Parse(Request.Form["Fecha"].ToString());
                    Conciliacion.FechaConciliacion = DateTime.Now;
                    Conciliacion.Gastos = Int32.Parse(Request.Form["GastosBancarios"].ToString());
                    Conciliacion.SaldoCartola = Int32.Parse(Request.Form["SaldoCartola"].ToString());
                    Conciliacion.Periodo = periodo;
                    Conciliacion.Mes = mes;
                    Conciliacion.PersonaID = ((Persona)Session["Persona"]).ID;
                    Conciliacion.ProyectoID = Proyecto.ID;
                    db.Conciliacion.Add(Conciliacion);
                    db.SaveChanges();

                    //ViewBag.ConciliacionID = Conciliacion.ID;
                    ViewBag.SaldoCartola = Conciliacion.SaldoCartola;
                    ViewBag.GastosBancarios = Conciliacion.Gastos;
                    ViewBag.Depositos = Conciliacion.Depositos;
                    ViewBag.Fecha = Conciliacion.FechaCartola.ToShortDateString();
                }


            }

            try
            {
                Conciliacion Conciliacion = db.Conciliacion.Where(c => c.ProyectoID == Proyecto.ID).Where(c => c.Periodo == periodo).Where(c => c.Mes == mes).Single();
                //ViewBag.ConciliacionID = Conciliacion.ID;
                ViewBag.SaldoCartola = Conciliacion.SaldoCartola;
                ViewBag.GastosBancarios = Conciliacion.Gastos;
                ViewBag.Depositos = Conciliacion.Depositos;
                ViewBag.Fecha = Conciliacion.FechaCartola.ToShortDateString();
            }
            catch (Exception)
            { }

            if (Request.Form["ImprimirConciliacion"] != null && Request.Form["ImprimirConciliacion"].ToString().Equals("Imprimir"))
            {
                @ViewBag.Imprimir = "true";
            }
            else
            {
                @ViewBag.Imprimir = "false";
            }

            return View(movimientos.ToList());
        }

        [HttpPost]
        public string Conciliar(int detalleEgresoID = 0, int movimientoID = 0)
        {
            Persona Persona = (Persona)Session["Persona"];
            Usuario Usuario = (Usuario)Session["Usuario"];
            DateTime fecha = DateTime.Now;

            string respuesta = "";
            bool estaConciliado = false;

            try
            {
                if (movimientoID > 0)
                {
                    // Conciliacion de Ingreso o Reintegro
                    try
                    {
                        ConciliacionRegistro cr = db.ConciliacionRegistro.Where(c => c.MovimientoID == movimientoID).Single();
                        estaConciliado = true;
                    }
                    catch (Exception)
                    {
                        estaConciliado = false;
                    }
                    
                    Movimiento movimiento = db.Movimiento.Find(movimientoID);

                    // Si el movimiento no ha sido conciliado, seguimos.
                    if (!estaConciliado)
                    {
                        try
                        {
                            // Se crea el registro de conciliación
                            ConciliacionRegistro cr = new ConciliacionRegistro();
                            cr.MovimientoID = movimiento.ID;

                            // Dependiendo del nivel del usuario se define el periodo conciliado.
                            if (Usuario.esAdministrador)
                            {
                                cr.Periodo = movimiento.Periodo;
                                cr.Mes = movimiento.Mes;
                            }
                            else
                            {
                                cr.Periodo = (int)Session["Periodo"];
                                cr.Mes = (int)Session["Mes"];
                            }

                            cr.PersonaID = Persona.ID;
                            cr.Fecha = fecha;

                            db.ConciliacionRegistro.Add(cr);
                            db.SaveChanges();

                            try
                            {
                                // Se define el movimiento como conciliado
                                movimiento.Conciliado = "S";
                                db.Entry(movimiento).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception)
                            {
                                //return "No se pudo crear el registro de conciliacion: 385. " + e.StackTrace.ToString();
                            }
                        }
                        catch (Exception)
                        {
                            //return "No se pudo crear el registro de conciliacion: 390. " + e.StackTrace.ToString();
                        }

                        respuesta = "OK";
                    }
                    else
                    {
                        try
                        {
                            // Se elimina el registro de conciliación
                            ConciliacionRegistro cr = db.ConciliacionRegistro.Where(c => c.MovimientoID == movimiento.ID).Single();
                            db.Entry(cr).State = EntityState.Deleted;
                            db.SaveChanges();
                            // Al parecer aca falla 
                            /*La secuencia no contiene elementos en System.Linq.Enumerable.Single[TSource](IEnumerable`1 source) en System.Linq.Queryable.Single[TSource](IQueryable`1 source) en SAG2.Controllers.BancoController.Conciliar(Int32 detalleEgresoID, Int32 movimientoID)*/
                        }
                        catch (Exception)
                        {
                            //return "No se pudo encontrar el registro de conciliacion: 408. " + e.StackTrace.ToString();
                        }

                        try
                        {
                            // Se asigna como no conciliado al movimiento
                            movimiento.Conciliado = null;
                            db.Entry(movimiento).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        catch (Exception) 
                        {
                            //return "No se pudo encontrar el registro de conciliacion: 420. " + e.StackTrace.ToString();
                        }

                        respuesta = "OK2";
                    }
                }

                if (detalleEgresoID > 0)
                {
                    // Conciliacion de Egreso
                    DetalleEgreso detalle = db.DetalleEgreso.Find(detalleEgresoID);

                    try
                    {
                        ConciliacionRegistro cr = db.ConciliacionRegistro.Where(c => c.DetalleID == detalleEgresoID).Single();
                        estaConciliado = true;
                    }
                    catch (Exception)
                    {
                        estaConciliado = false;

                    }
                    // Si el movimiento no ha sido conciliado, seguimos.
                    if (!estaConciliado)
                    {
                        try
                        {
                            ConciliacionRegistro cr = new ConciliacionRegistro();
                            cr.DetalleID = detalle.ID;

                            if (Usuario.esAdministrador)
                            {
                                cr.Periodo = detalle.Egreso.Periodo;
                                cr.Mes = detalle.Egreso.Mes;
                            }
                            else
                            {
                                cr.Periodo = (int)Session["Periodo"];
                                cr.Mes = (int)Session["Mes"];
                            }

                            cr.PersonaID = Persona.ID;
                            cr.Fecha = fecha;

                            db.ConciliacionRegistro.Add(cr);
                            db.SaveChanges();

                            try
                            {
                                detalle.Conciliado = "S";
                                db.Entry(detalle).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception)
                            {
                                //return "No se pudo encontrar el registro de conciliacion: 465. " + e.StackTrace.ToString();
                            }

                            respuesta = "OK";
                        }
                        catch (Exception)
                        {
                            //return "No se pudo encontrar el registro de conciliacion: 472. " + e.StackTrace.ToString();
                        }
                    }
                    else
                    {
                        try
                        {
                            ConciliacionRegistro cr = db.ConciliacionRegistro.Where(c => c.DetalleID == detalle.ID).Single();
                            db.Entry(cr).State = EntityState.Deleted;
                            db.SaveChanges();
                            try
                            {
                                detalle.Conciliado = null;
                                db.Entry(detalle).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            catch (Exception)
                            {
                                //return "No se pudo encontrar el registro de conciliacion: 490. " + e.StackTrace.ToString();
                            }
                        }
                        catch (Exception)
                        {
                            //return "No se pudo encontrar el registro de conciliacion: 495. " + e.StackTrace.ToString();
                        }

                        respuesta = "OK2";
                    }

                    int cantidad = db.DetalleEgreso.Where(d => d.MovimientoID == detalle.MovimientoID).Where(d => d.Conciliado == null).Where(m => m.Temporal == null).Count();

                    // Si se conciliaron todos los detalles, se concilia el Egreso
                    if (cantidad == 0)
                    {
                        Movimiento movimiento = db.Movimiento.Find(detalle.MovimientoID);
                        movimiento.Conciliado = "S";
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        Movimiento movimiento = db.Movimiento.Find(detalle.MovimientoID);
                        movimiento.Conciliado = null;
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return respuesta;
            }
            catch (Exception e)
            {
                return e.StackTrace.ToString();
            }
        }

        [HttpGet]
        public ActionResult Exportar(int periodo, int mes)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Usuario Usuario = (Usuario)Session["Usuario"];

            /*int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];*/
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];

            // Periodo anterior
            try
            {
                ViewBag.SaldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                ViewBag.SaldoInicial = 0;
            }

            var movimientos = from m in db.Movimiento
                              where (m.ProyectoID == Proyecto.ID) && ((m.Periodo == periodo && m.Mes == mes) || (m.Periodo < periodo) || (m.Periodo == periodo && m.Mes < mes)) && m.Temporal == null && m.Nulo == null && m.Eliminado == null && ((m.CuentaID != 1 && m.CuentaID != 6) || m.CuentaID == null)
                              orderby m.Periodo, m.Fecha, m.NumeroComprobante
                              select m;
            //var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.ProyectoID == Proyecto.ID);
            @ViewBag.periodo = periodo;
            @ViewBag.mes = mes;
            @ViewBag.cuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            @ViewBag.NumeroCuenta = @ViewBag.cuentaCorriente.Numero;

            //ViewBag.ConciliacionID = "0";
            ViewBag.SaldoCartola = "0";
            ViewBag.GastosBancarios = "0";
            ViewBag.Depositos = "0";
            ViewBag.Fecha = DateTime.Now.ToShortDateString();

            try
            {
                //int ID = Int32.Parse(Request.Form["ConciliacionID"].ToString());
                Conciliacion Conciliacion = db.Conciliacion.Where(c => c.ProyectoID == Proyecto.ID).Where(c => c.Periodo == periodo).Where(c => c.Mes == mes).Single();
                //ViewBag.ConciliacionID = Conciliacion.ID;
                ViewBag.SaldoCartola = Conciliacion.SaldoCartola;
                ViewBag.GastosBancarios = Conciliacion.Gastos;
                ViewBag.Depositos = Conciliacion.Depositos;
                ViewBag.Fecha = Conciliacion.FechaCartola.ToShortDateString();
            }
            catch (Exception)
            { }

            ViewBag.esSupervisor = Usuario.esSupervisor;

            return View(movimientos.ToList());
        }

        public ActionResult ConciliacionMasiva()
        {
            @ViewBag.periodo = (int)Session["Periodo"];
            @ViewBag.mes = (int)Session["Mes"];
            return View();
        }

        public ActionResult ExportarConciliacionMasiva(int Periodo, int Mes)
        {
            @ViewBag.periodo = Periodo;
            @ViewBag.mes = Mes;
            List<Proyecto> proyecto = db.Proyecto.OrderBy(p => p.CodCodeni).ToList();
            return View(proyecto.ToList());
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}