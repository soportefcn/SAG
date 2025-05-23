﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using SAG2.Comun; 
using System.Web.Script.Serialization;

namespace SAG2.Controllers
{ 
    public class EgresosController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();
        private Correo envioCorreo = new Correo();

        //
        // GET: /Egresos/
        [HttpPost]
        public ViewResult Index(FormCollection form)
        {
            int buscarPeriodo = 0;
            int buscarNdoc = 0;
            string Buscarbeneficiario = "";
            try
            {
                 buscarPeriodo = int.Parse(form["busqueda"].ToString());
            }
            catch (Exception )
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
            try
            {
                Buscarbeneficiario = form["bbene"].ToString();
            }
            catch (Exception)
            {
                Buscarbeneficiario = "";
            } 
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            
            var movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo.Equals(buscarPeriodo)).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
             if (buscarNdoc != 0){
                 movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo.Equals(buscarPeriodo) && a.NumeroComprobante.Equals(buscarNdoc)).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
             }
             if (Buscarbeneficiario != ""){
                 movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo.Equals(buscarPeriodo) && a.Beneficiario.Contains(Buscarbeneficiario)).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
             }

            return View(movimiento.ToList());
        }
        public ViewResult Index()
        {
            int periodo = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo == periodo ).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            return View(movimiento.ToList());
        }

        public ViewResult FondoFijo()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var ffg = db.FondoFijoGrupo.Where(f => f.ProyectoID == Proyecto.ID).OrderByDescending(f => f.ID);
            return View(ffg.ToList());
        }

        //
        // GET: /Egresos/Create

        public ActionResult Create()
        {
            int Iva = 0;
            int cuentaIva = 0;
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];

            Session.Remove("DetalleEgreso");
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.NroComprobante = "1";
            ViewBag.GestionIva = Proyecto.MI;

           
            try
            {
                var ReferenciaIva = db.Referencia.Where(d => d.PREDETERMINADO == 1 && d.GRUPO.Equals("IVA")).FirstOrDefault();

                Iva = int.Parse(ReferenciaIva.VALOR.ToString());
            }
            catch (Exception)
            {
                Iva = 19;
            }
            ViewBag.Iva = Iva;
            try {
                var cIva = db.Cuenta.Where(d => d.Equals("E") && d.CuentaIva == 3).FirstOrDefault();
                cuentaIva = cIva.ID;
            
            }
            catch (Exception) { }
            ViewBag.CuentaIva = cuentaIva;
            bool senainfo = false;

            if (senainfo)
            {
                if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID != ctes.tipoIngreso).Where(a => a.Periodo == periodo).Count() > 0)
                {
                    ViewBag.NroComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID != ctes.tipoIngreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
                }
            }
            else
            {
                if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoEgreso).Where(a => a.Periodo == periodo).Count() > 0)
                {
                    ViewBag.NroComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoEgreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
                }
            }

            ViewBag.TipoPagoID = new SelectList(db.TipoPago.Where(d => d.Estado == 1).ToList(), "TipoPagoID", "Nombre",1);
            ViewBag.DocumentoIDD = new SelectList(db.Documento, "ID", "NombreLista");
            ViewBag.ItemGastoID = new SelectList(db.ItemGasto, "ID", "NombreLista");
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
            var rol = db.Rol.Include(r => r.Persona).Include(r => r.TipoRol).Where(r => r.ProyectoID == Proyecto.ID);
            var persona = from r in rol
                          select r.Persona;
            ViewBag.PersonaID = new SelectList(persona, "ID", "NombreLista");
            var rolp = db.RolProveedor.Include(r => r.Proveedor).Where(r => r.ProyectoID == Proyecto.ID);
            var proveedor = from r in rolp
                            select r.Proveedor;
            ViewBag.ProveedorID = new SelectList(proveedor.OrderBy(p => p.Nombre), "ID", "NombreLista");
            return View();
        } 

        //
        // POST: /Egresos/Create

        [HttpPost]
        public ActionResult Create(Movimiento egreso)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            egreso.NumeroComprobante = 1;
            bool senainfo = false;

            if (senainfo)
            {
                if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID != ctes.tipoIngreso).Where(a => a.Periodo == periodo).Count() > 0)
                {
                    egreso.NumeroComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID != ctes.tipoIngreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
                }
            }
            else
            {
                if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoEgreso).Where(a => a.Periodo == periodo).Count() > 0)
                {
                    egreso.NumeroComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoEgreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
                }
            }

            egreso.ProyectoID = Proyecto.ID;
            egreso.CuentaCorrienteID = CuentaCorriente.ID;
            egreso.Mes = (int)Session["Mes"];
            egreso.Periodo = (int)Session["Periodo"];
            egreso.TipoComprobanteID = ctes.tipoEgreso;
            egreso.Descripcion = egreso.Descripcion.ToUpper();
            egreso.NDocumento = 0;

            if (Request.Form["tipoBeneficiario"].ToString().Equals("1"))
            {
                egreso.ProveedorID = null;
                egreso.Rut = null;
                egreso.DV = null;
            }
            else if (Request.Form["tipoBeneficiario"].ToString().Equals("2"))
            {
                egreso.PersonaID = null;
                egreso.Rut = null;
                egreso.DV = null;
            }
            else if (Request.Form["tipoBeneficiario"].ToString().Equals("3"))
            {
                egreso.ProveedorID = null;
                egreso.PersonaID = null;
            }
            else
            {
                throw new Exception("El beneficiario seleccionado no es válido.");
            }
            
            try
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                egreso.UsuarioID = Usuario.ID;
                egreso.FechaCreacion = DateTime.Now;

                if (egreso.NDocumento == null || egreso.NDocumento <= 0)
                {
                    egreso.NDocumento = 1;
                }

                if (ModelState.IsValid)
                {
                    db.Movimiento.Add(egreso);
                    db.SaveChanges();
                    //return RedirectToAction("Create");  
                }
                else 
                {
                    throw new Exception("Ocurrió un error al registrar el egreso.");
                }

                int egresoID = egreso.ID;
                int monto_egresos = 0;
                bool fondo_fijo = false;
                // Guardar detalle del egreso
                
                if (Session["DetalleEgreso"] != null)
                {
                    List<DetalleEgreso> lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                    foreach (DetalleEgreso detalle in lista)
                    {
                        monto_egresos += detalle.Monto;
                        if (detalle.FondoFijoID != null)
                        {

                            FondoFijo ff = db.FondoFijo.Find(detalle.FondoFijoID);
                            ff.EgresoID = egresoID;
                            db.Entry(ff).State = EntityState.Modified;
                            db.SaveChanges();
        

                            fondo_fijo = true;
                        } 
                        else if (detalle.DeudaPendienteID != null)
                        {
                    
                            DeudaPendiente dp = db.DeudaPendiente.Find(detalle.DeudaPendienteID);
                            dp.EgresoID = egresoID;
                            db.Entry(dp).State = EntityState.Modified;
                            db.SaveChanges();
                          
                        }
                        else if (detalle.BoletaHonorarioID != null)
                        {
                 
                            BoletaHonorario bh = db.BoletaHonorario.Find(detalle.BoletaHonorarioID);
                            bh.EgresoID = egresoID;
                            db.Entry(bh).State = EntityState.Modified;
                            db.SaveChanges();
                  
                        }

                        if (detalle.NDocumento == null || detalle.NDocumento <= 0)
                        {
                            detalle.NDocumento = 1;
                        }

                        detalle.MovimientoID = egresoID;

                        db.DetalleEgreso.Add(detalle);
                        db.SaveChanges();

                        // Aqui Anexo Egreso !!!!
                        //if (detalle.Anexo == 1) {
                        //    int MontoAnexo = 0;
                        //    int MontoRegistro = 0;
                        //    List<AnexoDetalle> Lista = new List<AnexoDetalle>();
                        //    Lista = db.AnexoDetalle.Where(d => d.anexo.ProyectoID == Proyecto.ID && d.CuentaID == detalle.CuentaID && d.Monto != d.MontoEgreso && d.anexo.Duracion > detalle.FechaEmision ).ToList();
                        //    MontoAnexo = detalle.Monto;
                        //    foreach (var Registro in Lista) { 
                        //        if ( MontoAnexo >= 0 ){
                        //            if (MontoAnexo <= Registro.Monto){
                        //                MontoRegistro = MontoAnexo;
                        //            }else {
                        //                MontoRegistro = Registro.Monto ;                                    
                        //            }
                        //            AnexoEgreso Dato = new AnexoEgreso();
                        //            Dato.AnexoDetalleID = Registro.ID;
                        //            Dato.DetalleEgresoID = detalle.ID;
                        //            Dato.Monto = MontoRegistro;
                        //            db.AnexoEgreso.Add(Dato);
                        //            db.SaveChanges();

                        //            Registro.MontoEgreso = Registro.MontoEgreso + MontoRegistro;
                        //            db.Entry(Registro).State = EntityState.Modified;
                        //            db.SaveChanges();

                        //            MontoAnexo = MontoAnexo - MontoRegistro;
                        //        }                            
                        //    }                                              
                        //}


                    }
                    Session.Remove("DetalleEgreso");
                    egreso.Monto_Egresos = monto_egresos;
                    if (fondo_fijo)
                    {
                        egreso.FondoFijo = "S";
                    }
                    db.Entry(egreso).State = EntityState.Modified;
                    db.SaveChanges();

                    if (Request.Form["FondoFijoGrupoID"] != null && !Request.Form["FondoFijoGrupoID"].ToString().Equals(""))
                    {
                        int fondoFijoGrupoID = Convert.ToInt32(Request.Form["FondoFijoGrupoID"].ToString());
                        FondoFijoGrupo ffg = db.FondoFijoGrupo.Find(fondoFijoGrupoID);
                        ffg.Modificacion = DateTime.Now;
                        ffg.Monto = monto_egresos;
                        ffg.Activo = "N";
                        ffg.EgresoID = egresoID;
                        db.Entry(ffg).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    db.Movimiento.Remove(egreso);
                    db.SaveChanges();
                    throw new Exception("No existe detalle para el egreso.");
                }

                if (!utils.ingresarSaldoIngreso(egreso, ModelState))
                {
                    throw new Exception("Ocurrio un error al actualiza el saldo de la cuenta corriente.");
                }

             //   if (Request.Form["ImprimirComprobante"].ToString().Equals("true"))
              //  {
                    return RedirectToAction("Edit", new { @id = egreso.ID, @imprimir = "true" });
              //  }
              //  else
               // {
                //    return RedirectToAction("Create");
                //}
            }
            catch(Exception e)
            {
                ViewBag.Mensaje = utils.mensajeError(utils.Mensaje(e));
                ViewBag.NroComprobante = egreso.NumeroComprobante.ToString();
                var rol = db.Rol.Include(r => r.Persona).Include(r => r.TipoRol).Where(r => r.ProyectoID == Proyecto.ID);
                var persona = from r in rol
                              select r.Persona;
                ViewBag.PersonaID = new SelectList(persona, "ID", "NombreLista", egreso.PersonaID);
                var rolp = db.RolProveedor.Include(r => r.Proveedor).Where(r => r.ProyectoID == Proyecto.ID);
                var proveedor = from r in rolp
                                select r.Proveedor;
                ViewBag.ProveedorID = new SelectList(proveedor.OrderBy(p => p.Nombre), "ID", "NombreLista", egreso.ProveedorID);
                ViewBag.DocumentoIDD = new SelectList(db.Documento, "ID", "NombreLista");
                ViewBag.ItemGastoID = new SelectList(db.ItemGasto, "ID", "NombreLista");
                ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
                return View(egreso);
            }
        }

        // GET: /Egresos/Detalle

        public ActionResult Detalle(int egresoID = 0)
        {
            return View();
        } 

        //
        // GET: /Egresos/Edit/5

        public ActionResult Edit(int id, string imprimir = "")
        {
            Session.Remove("DetalleEgreso");
            Movimiento egreso = db.Movimiento.Find(id);
            if (egreso.PersonaID != null)
            {
                egreso.ProveedorID = 0;
                egreso.Rut = egreso.Persona.Rut;
                egreso.DV = egreso.Persona.DV;
                egreso.Beneficiario = egreso.Persona.NombreCompleto;
            }
            else if (egreso.ProveedorID != null)
            {
                egreso.PersonaID = 0;
                egreso.Rut = egreso.Proveedor.Rut;
                egreso.DV = egreso.Proveedor.DV;
                egreso.Beneficiario = egreso.Proveedor.Nombre;
            }
            else
            {
                egreso.PersonaID = 0;
                egreso.ProveedorID = 0;
            }
            int periodo = (int)Session["Periodo"];
            ViewBag.DocumentoIDD = new SelectList(db.Documento, "ID", "NombreLista");
            ViewBag.ItemGastoID = new SelectList(db.ItemGasto, "ID", "NombreLista");
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
            ViewBag.Imprimir = imprimir;
            ViewBag.TipoPagoID = new SelectList(db.TipoPago.Where(d => d.Estado == 1).ToList(), "TipoPagoID", "Nombre", egreso.TipoPagoID);

            try
            {
                ViewBag.FondoFijoGrupoID = db.FondoFijoGrupo.Where(f => f.EgresoID == id).Single().ID.ToString();
            }
            catch (Exception)
            {
                ViewBag.FondoFijoGrupoID = "";
            }

            try
            {
                ViewBag.UltimoIdentificador = db.Movimiento.Where(m => m.auto == 0).Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.TipoComprobanteID == ctes.tipoEgreso).Where(m => m.ProyectoID == egreso.ProyectoID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null)).Max(a => a.ID).ToString();
                //ViewBag.UltimoIdentificador = db.Movimiento.Where(m => m.ProyectoID == egreso.ProyectoID).Where(a => a.TipoComprobanteID == ctes.tipoEgreso).Max(a => a.ID).ToString();
            }
            catch (Exception)
            {
                ViewBag.UltimoIdentificador = "0";
            }

            ViewBag.MaxComprobante = "0";
            try
            {
                ViewBag.MaxComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == egreso.ProyectoID && m.TipoComprobanteID == ctes.tipoEgreso && m.Periodo == periodo).Max(a => a.NumeroComprobante).ToString();
            }
            catch (Exception)
            { }

            try
            {
                ViewBag.MontoEgreso = db.DetalleEgreso.Where(d => d.MovimientoID == id).Sum(d => d.Monto);
            }
            catch (Exception)
            {
                ViewBag.MontoEgreso = "0";
            }

            if (egreso.Nulo != null && egreso.Nulo.Equals("S"))
            {
                ViewBag.MontoEgreso = egreso.Monto_Egresos;
            }
            // Detalle Egreso 
            List<DetalleEgreso> lista = new List<DetalleEgreso>();
            lista = db.DetalleEgreso.Where(d => d.MovimientoID == id).ToList();
            Session.Add("DetalleEgreso", lista);
            // Verificar Periodo 
            var Periododocumento = egreso.Periodo;
            var MesDocumento = egreso.Mes;
            var ProyectoDocumento = egreso.ProyectoID;
            ViewBag.GestionIva = db.Proyecto.Find(ProyectoDocumento).MI;
            int PeriodoEstado = 0;
            var DatosPeriodo = db.Periodo.Where(d => d.Mes == MesDocumento && d.Ano == Periododocumento && d.ProyectoID == ProyectoDocumento).Count();
            if (DatosPeriodo != 0) {
                PeriodoEstado = 1;
            }
            ViewBag.PeriodoEstado = PeriodoEstado;
            return View(egreso);
        }

        //
        // POST: /Egresos/Edit/5

        [HttpPost]
        public ActionResult Edit(Movimiento egreso)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Persona persona = (Persona)Session["Persona"];
            int originalID = egreso.ID;
            int? modificadoID = 0;
            Usuario Usuario = (Usuario)Session["Usuario"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            egreso.Descripcion = egreso.Descripcion.ToUpper();
            int GestionIva = 0;
            try
            {
                GestionIva = db.Proyecto.Find(egreso.ProyectoID).MI;
            }
            catch (Exception)
            {
                GestionIva = 0;
            }
            ViewBag.GestionIva = GestionIva;
            try
            {
                ViewBag.UltimoIdentificador = db.Movimiento.Where(m => m.ProyectoID == egreso.ProyectoID).Where(a => a.TipoComprobanteID == ctes.tipoEgreso).Max(a => a.ID).ToString();
            }
            catch (Exception)
            {
                ViewBag.UltimoIdentificador = "0";
            }

            // Solo enviar a autorización si el usuario no es administrador ni supervisor
            if ((!egreso.Periodo.Equals(periodo) || !egreso.Mes.Equals(mes)) && (Usuario.esUsuario))
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
                    db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + modificadoID);
                    db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + modificadoID);
                    db.Database.ExecuteSqlCommand("UPDATE DeudaPendiente SET EgresoID = NULL WHERE EgresoID = " + modificadoID);
                    db.Database.ExecuteSqlCommand("UPDATE FondoFijoGrupo SET EgresoID = NULL, Activo = 'S' WHERE EgresoID = " + modificadoID);
                    db.Database.ExecuteSqlCommand("UPDATE FondoFijo SET EgresoID = NULL WHERE EgresoID = " + modificadoID);
                    db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + modificadoID);
                    db.Database.ExecuteSqlCommand("DELETE FROM Movimiento WHERE ID = " + modificadoID);

                }
                catch (Exception)
                { }

                // Edicion del movimiento debe autorizarse, cambios se registran de forma temporal.

                // Se eliminan modificaciones anteriores
               //try
               //{
                List<Movimiento> tmp = db.Movimiento.Where(m => m.Temporal.Equals("S") && m.TipoComprobanteID == 2 && m.NumeroComprobante != ctes.tipoIngreso && m.ProyectoID == Proyecto.ID).ToList();
                    foreach (Movimiento mtmp in tmp)
                    {
                        db.Database.ExecuteSqlCommand("DELETE FROM Autorizaciones WHERE ModificadoID = " + mtmp.ID);
                        db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + mtmp.ID);
                        db.Database.ExecuteSqlCommand("DELETE FROM Movimiento WHERE ID = " + mtmp.ID);
                    }
                   //db.Database.ExecuteSqlCommand("DELETE FROM Movimiento WHERE Temporal = 'S' AND TipoComprobanteID = 2 AND NumeroComprobante = '" + egreso.NumeroComprobante + "' AND ProyectoID = '" + Proyecto.ID + "';");
               //}
               //catch (Exception)
               //{ }
  
                egreso.ID = 0;
                egreso.Temporal = "S";
                egreso.Eliminado = "N";

                if (Request.Form["tipoBeneficiario"].ToString().Equals("1"))
                {
                    egreso.ProveedorID = null;
                    egreso.Rut = null;
                    egreso.DV = null;
                }
                else if (Request.Form["tipoBeneficiario"].ToString().Equals("2"))
                {
                    egreso.PersonaID = null;
                    egreso.Rut = null;
                    egreso.DV = null;
                }
                else if (Request.Form["tipoBeneficiario"].ToString().Equals("3"))
                {
                    egreso.ProveedorID = null;
                    egreso.PersonaID = null;
                }
                else
                {
                    throw new Exception("El beneficiario seleccionado no es válido.");
                }

                //Usuario Usuario = (Usuario)Session["Usuario"];
                egreso.UsuarioID = Usuario.ID;

                db.Movimiento.Add(egreso);
                db.SaveChanges();

                int egresoID = egreso.ID;

                if (Session["DetalleEgreso"] != null)
                {
                    List<DetalleEgreso> lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                    foreach (DetalleEgreso detalle in lista)
                    {
                        detalle.MovimientoID = egresoID;
                        detalle.Temporal = "S";
                        db.DetalleEgreso.Add(detalle);
                        db.SaveChanges();
                    }
                    Session.Remove("DetalleEgreso");
                }
                else
                {
                    throw new Exception("No existe detalle para el egreso.");
                }

                // Se registra en la tabla de Autorizaciones
                Autorizacion autorizacion = new Autorizacion();
                autorizacion.OriginalID = originalID;
                autorizacion.ModificadoID = egreso.ID;
                autorizacion.SolicitaID = persona.ID;
                autorizacion.Tipo = "Modificación";
                autorizacion.FechaSolicitud = DateTime.Now;
                db.Autorizacion.Add(autorizacion);
                db.SaveChanges();

    


                ViewBag.Mensaje = utils.mensajeAdvertencia("La modificación ha sido solicitada al Supervisor.");
            }
            else
            {
                //CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
                int montoOriginal = Int32.Parse(Request.Form["MontoOriginal"].ToString());
                egreso.FondoFijo = null;

                if (Request.Form["tipoBeneficiario"].ToString().Equals("1"))
                {
                    egreso.ProveedorID = null;
                    egreso.Rut = null;
                    egreso.DV = null;
                }
                else if (Request.Form["tipoBeneficiario"].ToString().Equals("2"))
                {
                    egreso.PersonaID = null;
                    egreso.Rut = null;
                    egreso.DV = null;
                }
                else if (Request.Form["tipoBeneficiario"].ToString().Equals("3"))
                {
                    egreso.ProveedorID = null;
                    egreso.PersonaID = null;
                }
                else
                {
                    throw new Exception("El beneficiario seleccionado no es válido.");
                }

                try
                {
                    //Usuario Usuario = (Usuario)Session["Usuario"];
                    egreso.UsuarioID = Usuario.ID;

                    if (egreso.NDocumento == null || egreso.NDocumento <= 0)
                    {
                        egreso.NDocumento = 1;
                    }

                    if (ModelState.IsValid)
                    {
                        db.Entry(egreso).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Ocurrió un error al modificar el egreso.");
                    }

                    int monto_egresos = 0;
                    int egresoID = egreso.ID;
                    bool fondo_fijo = false;
                    // Guardar detalle del egreso
                    if (Session["DetalleEgreso"] != null)
                    {

                        List<DetalleEgreso> Detalle = new List<Models.DetalleEgreso>();

                        // Carga tabla de Base de Datos
                        Detalle = db.DetalleEgreso.Where(d => d.MovimientoID == egresoID).ToList();

                        // Libera Relaciones
                        db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + egresoID);
                        db.Database.ExecuteSqlCommand("UPDATE DeudaPendiente SET EgresoID = NULL WHERE EgresoID = " + egresoID);
                        db.Database.ExecuteSqlCommand("UPDATE FondoFijoGrupo SET EgresoID = NULL, Activo = 'S' WHERE EgresoID = " + egresoID);
                        db.Database.ExecuteSqlCommand("UPDATE FondoFijo SET EgresoID = NULL WHERE EgresoID = " + egresoID);

                        // Carga tabla Temporal
                        List<DetalleEgreso> lista = (List<DetalleEgreso>)Session["DetalleEgreso"];

                        // Modifica registros 

                        foreach (DetalleEgreso data in Detalle )
                        {
                            int ID = data.ID;

                            var DataTemporal = lista.Where(d => d.ID == ID).FirstOrDefault();
                            if (DataTemporal != null)
                            {
                                monto_egresos += DataTemporal.Monto;
                                if (DataTemporal.FondoFijoID != null)
                                {
                                    FondoFijo ff = db.FondoFijo.Find(DataTemporal.FondoFijoID);
                                    ff.EgresoID = egresoID;
                                    db.Entry(ff).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //detalle.FondoFijoID = ff.ID;
                                    fondo_fijo = true;
                                }
                                else if (DataTemporal.DeudaPendienteID != null)
                                {
                                    DeudaPendiente dp = db.DeudaPendiente.Find(DataTemporal.DeudaPendienteID);
                                    dp.EgresoID = egresoID;
                                    db.Entry(dp).State = EntityState.Modified;
                                    db.SaveChanges();

                                }
                                else if (DataTemporal.BoletaHonorarioID != null)
                                {
                                    BoletaHonorario bh = db.BoletaHonorario.Find(DataTemporal.BoletaHonorarioID);
                                    bh.EgresoID = egresoID;
                                    db.Entry(bh).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                if (DataTemporal.NDocumento == null || DataTemporal.NDocumento <= 0)
                                {
                                    DataTemporal.NDocumento = 1;
                                }



                                data.Monto = DataTemporal.Monto;
                                data.BoletaHonorarioID = DataTemporal.BoletaHonorarioID;
                                data.CuentaID = DataTemporal.CuentaID;
                                data.DeudaPendienteID = DataTemporal.DeudaPendienteID;
                                data.DocumentoID = DataTemporal.DocumentoID;
                                data.FechaEmision = DataTemporal.FechaEmision;
                                data.FechaVencimiento = DataTemporal.FechaVencimiento;
                                data.Glosa = DataTemporal.Glosa;
                                data.FondoFijoID = DataTemporal.FondoFijoID;
                                data.NComprobanteDP = DataTemporal.NComprobanteDP;
                                data.NDocumento = DataTemporal.NDocumento;
                                db.Entry(data).State = EntityState.Modified;
                                db.SaveChanges();

                            }
                            else {
                                db.DetalleEgreso.Remove(data);
                                db.SaveChanges();
                            
                            }
                            
                        
                        }
                   // Ingresar Nuevas Filas
                        var DataTemporalDetalle = lista.Where(d => d.ID == 0).ToList();
                        foreach (DetalleEgreso data in DataTemporalDetalle)
                        {
                            int ID = data.ID;
                
                                monto_egresos += data.Monto;
                                if (data.FondoFijoID != null)
                                {
                                    FondoFijo ff = db.FondoFijo.Find(data.FondoFijoID);
                                    ff.EgresoID = egresoID;
                                    db.Entry(ff).State = EntityState.Modified;
                                    db.SaveChanges();
                                    //detalle.FondoFijoID = ff.ID;
                                    fondo_fijo = true;
                                }
                                else if (data.DeudaPendienteID != null)
                                {
                                    DeudaPendiente dp = db.DeudaPendiente.Find(data.DeudaPendienteID);
                                    dp.EgresoID = egresoID;
                                    db.Entry(dp).State = EntityState.Modified;
                                    db.SaveChanges();
                                    
                                }
                                else if (data.BoletaHonorarioID != null)
                                {
                                    BoletaHonorario bh = db.BoletaHonorario.Find(data.BoletaHonorarioID);
                                    bh.EgresoID = egresoID;
                                    db.Entry(bh).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                if (data.NDocumento == null || data.NDocumento <= 0)
                                {
                                    data.NDocumento = 1;
                                }
                                data.MovimientoID = egresoID;
                                db.DetalleEgreso.Add(data);
                            }






                        Session.Remove("DetalleEgreso");

                        egreso.Monto_Egresos = monto_egresos;
                        if (fondo_fijo)
                        {
                            egreso.FondoFijo = "S";
                        }
                        
                        db.Entry(egreso).State = EntityState.Modified;
                        db.SaveChanges();

                        if (Request.Form["FondoFijoGrupoID"] != null && !Request.Form["FondoFijoGrupoID"].ToString().Equals(""))
                        {
                            int fondoFijoGrupoID = Convert.ToInt32(Request.Form["FondoFijoGrupoID"].ToString());
                            FondoFijoGrupo ffg = db.FondoFijoGrupo.Find(fondoFijoGrupoID);
                            ffg.Modificacion = DateTime.Now;
                            ffg.Monto = monto_egresos;
                            ffg.Activo = "N";
                            ffg.EgresoID = egresoID;
                            db.Entry(ffg).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        throw new Exception("No existe detalle para el egreso.");
                    }

                    if (!utils.actualizarSaldoIEgreso(egreso, ModelState, montoOriginal))
                    {
                        throw new Exception("Ocurrio un error al actualiza el saldo de la cuenta corriente.");
                    }

                    // Actualización de saldos en caso de periodos distintos
                    if ((egreso.Periodo != periodo || egreso.Mes != mes) && (!Usuario.esUsuario))
                    {
                        utils.RecalcularSaldos(egreso.Periodo, periodo, egreso.Mes, mes, Proyecto, CuentaCorriente);
                    }

                    return RedirectToAction("Create");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = utils.mensajeError(e.Message + " " + e.StackTrace.ToString());
                    if (e.InnerException != null)
                    {
                        ViewBag.Mensaje = utils.mensajeError(e.InnerException.Message + " " + e.StackTrace.ToString());
                    }

                    if (egreso.PersonaID != null)
                    {
                        egreso.ProveedorID = 0;
                    }
                    else if (egreso.ProveedorID != null)
                    {
                        egreso.PersonaID = 0;
                    }
                    else
                    {
                        egreso.PersonaID = 0;
                        egreso.ProveedorID = 0;
                    }

                    ViewBag.DocumentoIDD = new SelectList(db.Documento, "ID", "NombreLista");
                    ViewBag.ItemGastoID = new SelectList(db.ItemGasto, "ID", "NombreLista");
                    ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
  
                }
                
            
            }

       
            
            egreso = null;
            egreso = db.Movimiento.Find(originalID);

            ViewBag.DocumentoID = new SelectList(db.Documento, "ID", "NombreLista");
            ViewBag.ItemGastoID = new SelectList(db.ItemGasto, "ID", "NombreLista");
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
            var rol1 = db.Rol.Include(r => r.Persona).Include(r => r.TipoRol).Where(r => r.ProyectoID == Proyecto.ID);
            var persona1 = from r in rol1
                          select r.Persona;
            ViewBag.PersonaID = new SelectList(persona1, "ID", "NombreLista", egreso.PersonaID);
            var rolp1 = db.RolProveedor.Include(r => r.Proveedor).Where(r => r.ProyectoID == Proyecto.ID);
            var proveedor1 = from r in rolp1
                            select r.Proveedor;
            ViewBag.ProveedorID = new SelectList(proveedor1.OrderBy(p => p.Nombre), "ID", "NombreLista", egreso.ProveedorID);

            try
            {
                ViewBag.MontoEgreso = db.DetalleEgreso.Where(d => d.MovimientoID == originalID).Sum(d => d.Monto);
            }
            catch (Exception)
            {
                ViewBag.MontoEgreso = "0";
            }

            return View(egreso);
        }

        //
        // GET: /Egresos/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Movimiento movimiento = db.Movimiento.Find(id); 
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int maxComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoEgreso).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante);

            if (movimiento.NumeroComprobante == maxComprobante)
            {
                List<DetalleEgreso> detalles = db.DetalleEgreso.Where(d => d.MovimientoID == id).ToList();
                foreach (DetalleEgreso detalle in detalles)
                {
                    if (detalle.DeudaPendienteID != null)
                    {
                        //SAG2DB tmp = new SAG2DB();
                        DeudaPendiente dp = db.DeudaPendiente.Find(detalle.DeudaPendienteID);
                        dp.EgresoID = null;
                        db.Entry(dp).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (detalle.FondoFijoID != null)
                    {
                        //SAG2DB tmp = new SAG2DB();
                        FondoFijo ff = db.FondoFijo.Find(detalle.FondoFijoID);
                        ff.EgresoID = null;
                        db.Entry(ff).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else if (detalle.BoletaHonorarioID != null)
                    {
                        // SAG2DB tmp = new SAG2DB();
                        BoletaHonorario bh = db.BoletaHonorario.Find(detalle.BoletaHonorarioID);
                        bh.EgresoID = null;
                        db.Entry(bh).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    db.DetalleEgreso.Remove(detalle);
                    db.SaveChanges();
                }

                try
                {
                    db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + id);
                    db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + id);
                    db.Database.ExecuteSqlCommand("UPDATE DeudaPendiente SET EgresoID = NULL WHERE EgresoID = " + id);
                    db.Database.ExecuteSqlCommand("UPDATE FondoFijoGrupo SET EgresoID = NULL, Activo = 'S' WHERE EgresoID = " + id);
                    db.Database.ExecuteSqlCommand("UPDATE FondoFijo SET EgresoID = NULL WHERE EgresoID = " + id);
                    db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + id);
                    db.Database.ExecuteSqlCommand("DELETE FROM BienMovimientoInventario WHERE BienID in  ( SELECT ID  FROM  BienModInventario where MovimientoID  =  " + id + " )");
                    db.Database.ExecuteSqlCommand("DELETE FROM BienModInventario WHERE MovimientoID  =  " + id + " ");
                }
                catch (Exception)
                { }

                int monto = movimiento.Monto_Egresos;

                if (ModelState.IsValid)
                {
                    movimiento.Nulo = "S";
                    movimiento.Monto_Egresos = 0;
                    db.Entry(movimiento).State = EntityState.Modified;
                    db.SaveChanges();
                }

                utils.anularSaldoEgreso(movimiento, ModelState, monto);
                db.Movimiento.Remove(movimiento);
                db.SaveChanges();
            }

            return RedirectToAction("Create");
        }

        [HttpPost]
        public string GuardarLinea(string Origen, int Monto, string FechaEmision, int CuentaID, string Glosa, string FechaVencimiento = null, int? NComprobanteDP = null, int? DocumentoIDD = null, long? NDocumentoD = null, string BoletaHonorarioID = "", string DeudaPendienteID = "", int DetalleEgresoID = 0, int DetalleEgresoIndex = -1)
        {
            //int ValorNeto = 0;
            //int ValorIva = 0;
            int cuentaIva = 0;
            int gesIva = 0;
         //   int Anexo = 0;

            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            gesIva = Proyecto.MI;
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];

            try
            {
                Saldo Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == CuentaCorriente.ID && s.Periodo == periodo && s.Mes == mes).Single();
                if (Saldo.SaldoFinal < Monto)
                {
                    return "No hay saldo suficiente en la Cuenta Corriente para registrar este detalle. Saldo actual: $" + Saldo.SaldoFinal.ToString("#,##0");
                }
            }
            catch (Exception)
            {
                return "No hay saldo definido para este Proyecto.";
            }

            try
            {
                //ValorNeto = Int32.Parse(Request.Form["ValorNeto"].ToString());
                //ValorIva = Int32.Parse(Request.Form["ValorIva"].ToString());
                //DocumentoIDD = Int32.Parse(Request.Form["DocumentoIDD"].ToString());
                Cuenta CIva = new Cuenta();
               CIva = db.Cuenta.Where(d => d.CuentaIva == 3 && d.Tipo.Equals("E")).FirstOrDefault();
                cuentaIva = CIva.ID;
            }
            catch (Exception)
            {

            }

            List<DetalleEgreso> lista = null;

            if (Session["DetalleEgreso"] != null)
            {
                lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                if (lista.Count == 0)
                {
                    lista = new List<DetalleEgreso>();
                    DetalleEgresoIndex = -1;
                }
                if (DocumentoIDD == 1 && gesIva == 1 && DetalleEgresoID == 0)
                {
                      var   detalleIva = lista.Where(d => d.CuentaID == cuentaIva).FirstOrDefault();          
                      lista.Remove(detalleIva);
                }
    
            }
            else
            {
                lista = new List<DetalleEgreso>();
                DetalleEgresoIndex = -1;
            }

            // Se limpia la lista temporal
            //Session.Remove("DetalleEgreso");

            try
            {
                if (FechaVencimiento == null)
                {
                    FechaVencimiento = FechaEmision;
                }

                DetalleEgreso detalle = new DetalleEgreso();
               
                if (DetalleEgresoID > 0) 
                { 

                    detalle = lista.Where(d => d.ID == DetalleEgresoID).Single();
                }
                else if (DetalleEgresoIndex > -1)
                {

                    detalle = lista.ElementAt(DetalleEgresoIndex - 1);
                }
              //  detalle.Anexo = Anexo;
                detalle.NComprobanteDP = NComprobanteDP;
                detalle.DocumentoID = DocumentoIDD;

                if (NDocumentoD == null || NDocumentoD == 0)
                {
                    detalle.NDocumento = 1;
                }
                else
                {
                    detalle.NDocumento = NDocumentoD;
                }
                if (DocumentoIDD == 1 && gesIva == 1)
                {
                    detalle.Monto = Monto;
                    detalle.Iva = 1;
                }
                else
                {
                    detalle.Monto = Monto;
                    detalle.Iva = 0;
                }
                detalle.FechaEmision = DateTime.Parse(FechaEmision);
                detalle.FechaVencimiento = DateTime.Parse(FechaVencimiento);
                detalle.CuentaID = CuentaID;
                detalle.Glosa = Glosa;

                if (DocumentoIDD != null)
                {
                    //detalle.Documento = db.Documento.Find(DocumentoID);
                }

                if (DeudaPendienteID != "")
                {
                    // Si es DP, la modificamos de acuerdo a los datos ingresados.
                    try
                    {
                        int tmp = Int32.Parse(DeudaPendienteID);
                        DeudaPendiente dp = db.DeudaPendiente.Find(tmp);
                        dp.Monto = Monto;
                        dp.CuentaID = CuentaID;
                        dp.Glosa = Glosa;
                        dp.FechaEmision = DateTime.Parse(FechaEmision);
                        dp.FechaVencimiento = DateTime.Parse(FechaVencimiento);
                        dp.Periodo = periodo;
                        dp.Mes = mes;
                        db.Entry(dp).State = EntityState.Modified;
                        db.SaveChanges();

                        //detalle.DeudaPendiente = dp;
                        detalle.DeudaPendienteID = dp.ID;
                    }
                    catch (Exception)
                    {
                        return "Ocurrió un error al registrar la deuda pendiente.";
                    }
                }

                if (BoletaHonorarioID != "")
                {
                    // Si es DP, la modificamos de acuerdo a los datos ingresados.
                    try
                    {
                        int tmp = Int32.Parse(BoletaHonorarioID);
                        BoletaHonorario bh = db.BoletaHonorario.Find(tmp);
                        bh.Concepto = Glosa;

                        db.Entry(bh).State = EntityState.Modified;
                        db.SaveChanges();

                        //detalle.BoletaHonorario = bh;
                        detalle.BoletaHonorarioID = bh.ID;
                    }
                    catch (Exception)
                    {
                        return "Ocurrió un error al registrar la boleta de honorarios.";
                    }
                }                



                    if (DetalleEgresoID > 0)
                    {
                        // Edicion de detalle de egreso ya registrado
                        int index = lista.FindIndex(d => d.ID == DetalleEgresoID);
                        lista.RemoveAt(index);

                    }
                    else if (DetalleEgresoIndex > -1)
                    {
                        lista.RemoveAt(DetalleEgresoIndex -1);
                    }


                    lista.Add(detalle);

                    //if (DocumentoIDD == 1 && gesIva == 1 )
                    //{
                    //    DetalleEgreso detalleIva = new DetalleEgreso();
                    //    if (DetalleEgresoID == 0)
                    //    {
                    //       // DetalleEgreso detalleIva = new DetalleEgreso();
                    //        detalleIva.DocumentoID = detalle.DocumentoID;
                    //        detalleIva.Monto = ValorIva;
                    //        detalleIva.FechaEmision = detalle.FechaEmision;
                    //        detalleIva.FechaVencimiento = detalle.FechaVencimiento;
                    //        detalleIva.CuentaID = cuentaIva;
                    //        detalleIva.Glosa = "IVA CRÉDITO FISCAL";
                    //        detalleIva.NDocumento = detalle.NDocumento;
                    //        detalleIva.Iva = 2;
                    //    }
                    //    else {
                    //        detalleIva = lista.Where(d => d.CuentaID == cuentaIva).FirstOrDefault();
                    //        int DetalleEgresoIDIva = detalleIva.ID;
                    //        int index = lista.FindIndex(d => d.ID == DetalleEgresoIDIva);
                    //        lista.RemoveAt(index);
                    //        detalleIva.DocumentoID = detalle.DocumentoID;
                    //        detalleIva.Monto = ValorIva;
                    //        detalleIva.FechaEmision = detalle.FechaEmision;
                    //        detalleIva.FechaVencimiento = detalle.FechaVencimiento;
                    //        detalleIva.CuentaID = cuentaIva;
                    //        detalleIva.Glosa = "IVA CRÉDITO FISCAL";
                    //        detalleIva.NDocumento = detalle.NDocumento;
                    //        detalleIva.Iva = 2;
                        
                    //    }
                    //    lista.Add(detalleIva);

                    //}

                    Session.Add("DetalleEgreso", lista);

                return "OK";
            } 
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet]
        public string GuardarLinea(FormCollection data)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            // Buscar cuenta corriente segun movimiento !!!

            // Verificamos que haya saldo disponible para guardar el detalle de egreso
            try
            {
                Saldo Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == CuentaCorriente.ID && s.Periodo == periodo && s.Mes == mes).Single();
                if (Saldo.SaldoFinal < 0)
                {
                    return "No hay saldo suficiente en la Cuenta Corriente para registrar este detalle. Saldo actual: $" + Saldo.SaldoFinal.ToString("#,##0");
                }
            }
            catch (Exception)
            {
                return "No hay saldo definido para este Proyecto.";
            }

            List<DetalleEgreso> lista = null;

            if (Session["DetalleEgreso"] != null)
            {
                lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                if (lista.Count == 0)
                {
                    lista = new List<DetalleEgreso>();
 
                }

            }
            else
            {
                lista = new List<DetalleEgreso>();
 
            }

            // Se limpia la lista temporal
            //Session.Remove("DetalleEgreso");

                return "OK";
        }

        // Liberar comprobante de egreso
        public ActionResult LiberarEgreso(int id)
        {
            
            db.Database.ExecuteSqlCommand("UPDATE DeudaPendiente SET EgresoID = NULL WHERE EgresoID = " + id);
            db.Database.ExecuteSqlCommand("UPDATE FondoFijoGrupo SET EgresoID = NULL, Activo = 'S' WHERE EgresoID = " + id);
            db.Database.ExecuteSqlCommand("UPDATE FondoFijo SET EgresoID = NULL WHERE EgresoID = " + id);
            db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + id);
            db.Database.ExecuteSqlCommand("DELETE FROM BienMovimientoInventario WHERE BienID in  ( SELECT ID  FROM  BienModInventario where MovimientoID  =  " + id + " )");
            db.Database.ExecuteSqlCommand("DELETE FROM BienModInventario WHERE MovimientoID  =  " + id + " ");
            db.Database.ExecuteSqlCommand("DELETE FROM BoletaHonorario WHERE EgresoID = " + id);
            Movimiento Egreso = db.Movimiento.Find(id);
            int monto = Egreso.Monto_Egresos;

            if (ModelState.IsValid)
            {
                Egreso.Monto_Egresos = 0;
                db.Entry(Egreso).State = EntityState.Modified;
                db.SaveChanges();
            }

            utils.actualizarSaldoEgreso(Egreso, ModelState, monto);

            return RedirectToAction("Edit", new { id = id });
        }

        public string BorrarLinea(int DetalleEgresoID = 0, int DetalleEgresoIndex = -1)
        {
           // int? boletaID = null;

            try
            {
                if (Session["DetalleEgreso"] != null)
                {
                    List<DetalleEgreso> lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                    if (DetalleEgresoID > 0)
                    {
                        // Edicion de detalle de egreso ya registrado
                        int index = lista.FindIndex(d => d.ID == DetalleEgresoID);
                        int Iva = lista[index].Iva;         
                            //boletaID = lista[index].BoletaHonorarioID;
                            if (index >= 0)
                            {
                                lista.RemoveAt(index);
                            }
                            else
                            {
                                lista.RemoveAt(DetalleEgresoID - 1);
                            }
                        
                    }
                    else if (DetalleEgresoIndex > -1)
                    {                 
                      lista.RemoveAt(DetalleEgresoIndex - 1);          
                    }
                    int cuentaIva = 0;
                    try
                    {

                        Cuenta CIva = new Cuenta();
                        CIva = db.Cuenta.Where(d => d.CuentaIva == 3 && d.Tipo.Equals("E")).FirstOrDefault();
                        cuentaIva = CIva.ID;
                    }
                    catch (Exception)
                    {

                    }
                    var detalleIva = lista.Where(d => d.CuentaID == cuentaIva).FirstOrDefault();
                    lista.Remove(detalleIva);

                    Session.Remove("DetalleEgreso");
                    Session.Add("DetalleEgreso", lista);
                }

                try
                {
                    //BoletaHonorario bh = db.BoletaHonorario.Find(boletaID);
                    //db.BoletaHonorario.Remove(bh);
                    //db.SaveChanges();
                }
                catch (Exception)
                { }

                return "OK";
            } 
            catch (Exception e)
            {
                return e.Message;
            }
        }



        public string LineaIVA(int DetalleEgresoID = 0, int DetalleEgresoIndex = -1)
        {
            try
            {
                if (Session["DetalleEgreso"] != null)
                {
                    int Iva = 0;
                    try
                    {
                        var ReferenciaIva = db.Referencia.Where(d => d.PREDETERMINADO == 1 && d.GRUPO.Equals("IVA")).FirstOrDefault();

                        Iva = int.Parse(ReferenciaIva.VALOR.ToString());
                    }
                    catch (Exception)
                    {
                        Iva = 19;
                    }
                   int DocumentoIDD = Int32.Parse(Request.Form["DocumentoIDD"].ToString());
                   int gesIva = 0;
                   Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                   gesIva = Proyecto.MI;
 
                   if (DocumentoIDD == 1 && gesIva == 1)
                   {
                    List<DetalleEgreso> lista = null;
				   lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                   if (lista.Count > 0)
                   {
                       DetalleEgreso detalle = lista.FirstOrDefault();
                       var CIva = db.Cuenta.Where(d => d.CuentaIva == 3 && d.Tipo.Equals("E")).FirstOrDefault();
                       int cuentaIva = CIva.ID;
                       int ValorNeto = lista.Where(d => d.CuentaID != cuentaIva ).Sum(d => d.Monto);
                       double VIva = Iva * ValorNeto;
                       VIva = VIva / 100;
                       VIva = Math.Round(VIva);
                       int ValorIva = int.Parse(VIva.ToString());
                       DetalleEgreso detalleIva = new DetalleEgreso();
                       DetalleEgreso detalleIvaRev = new DetalleEgreso();
                       if (DetalleEgresoID == 0)
                       {
                           detalleIvaRev = lista.Where(d => d.CuentaID == cuentaIva).FirstOrDefault();
                           if (detalleIvaRev == null)
                           {
                               detalleIva.DocumentoID = detalle.DocumentoID;
                               detalleIva.Monto = ValorIva;
                               detalleIva.FechaEmision = detalle.FechaEmision;
                               detalleIva.FechaVencimiento = detalle.FechaVencimiento;
                               detalleIva.CuentaID = cuentaIva;
                               detalleIva.Glosa = "IVA CRÉDITO FISCAL";
                               detalleIva.NDocumento = detalle.NDocumento;
                               detalleIva.Iva = 2;
                               lista.Add(detalleIva);
                           }

                       }
                       else
                       {
                           detalleIva = lista.Where(d => d.CuentaID == cuentaIva).FirstOrDefault();
                           int DetalleEgresoIDIva = detalleIva.ID;
                           int index = lista.FindIndex(d => d.ID == DetalleEgresoIDIva);
                           lista.RemoveAt(index);
                           detalleIva.DocumentoID = detalle.DocumentoID;
                           detalleIva.Monto = ValorIva;
                           detalleIva.FechaEmision = detalle.FechaEmision;
                           detalleIva.FechaVencimiento = detalle.FechaVencimiento;
                           detalleIva.CuentaID = cuentaIva;
                           detalleIva.Glosa = "IVA CRÉDITO FISCAL";
                           detalleIva.NDocumento = detalle.NDocumento;
                           detalleIva.Iva = 2;
                            lista.Add(detalleIva);

                       }
                      
                       Session.Remove("DetalleEgreso");
                       Session.Add("DetalleEgreso", lista);
                   }

                   }




                }


                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet]
        public ActionResult ListarLineas(int? t = null)
        {
            
            if (Session["DetalleEgreso"] != null)
            {
                return View((List<DetalleEgreso>)Session["DetalleEgreso"]);
            }
            else
            {
                if (t == null)
                {
                    return View(new List<DetalleEgreso>());
                }
                else
                {
                    List<DetalleEgreso> lista = new List<DetalleEgreso>();
                    lista = db.DetalleEgreso.Where(d => d.MovimientoID == t).ToList();
                    Session.Add("DetalleEgreso", lista);
                    return View((List<DetalleEgreso>)Session["DetalleEgreso"]);
                }
            }
        }

        public ActionResult Anular(int id)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Movimiento movimiento = db.Movimiento.Find(id);
            Persona persona = (Persona)Session["Persona"];

            if (movimiento.Periodo != periodo || movimiento.Mes != mes)
            {
                movimiento.Monto_Egresos = 1;
                movimiento.Temporal = "S";
                movimiento.Nulo = "S";
                movimiento.Eliminado = "N";
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
                /* 20220601 - Aqui Enviar correo Autorizacion Supervisor
                string MensajeCorreo = "Se Solicita Autorizaci&oacute;n <br> Para : ";
               MensajeCorreo = MensajeCorreo + "<table><tr><td>Proyecto</td><td>" + movimiento.Proyecto.NombreLista + "</td></tr>";
               MensajeCorreo = MensajeCorreo + "<tr><td>Tipo Comp.</td><td>Egreso</td></tr><tr><td># Comp</td><td>" + movimiento.NumeroComprobante + "</td></tr>";
               MensajeCorreo = MensajeCorreo + "<tr><td>Solicitado Por </td><td>" + persona.NombreCompleto + "</td></tr><tr><td>tipo</td><td>Anulaci&oacute;n</td> </tr></table>"; 
              
                var supervisorCorreo = db.Rol.Where(d => d.TipoRolID == 4 && d.ProyectoID == movimiento.ProyectoID).ToList();
                foreach (var Scorreo in supervisorCorreo) {
                    string CorreoSup = db.Persona.Where(d => d.ID == Scorreo.PersonaID).FirstOrDefault().CorreoElectronico;

                    Correo.enviarCorreo(CorreoSup, MensajeCorreo, "Autorizacion Anulacion");  
                }
                */
                return RedirectToAction("Edit", new { id = @id, mensaje = "La anulación ha sido solicitada al Supervisor." });
            }

            try
            {
                // Se rescata la informacion del primer detalle para completar egreso
                DetalleEgreso rescateDetalle = db.DetalleEgreso.Where(d => d.MovimientoID == id).Take(1).Single();
                movimiento.CuentaID = rescateDetalle.CuentaID;
                movimiento.DocumentoID = rescateDetalle.DocumentoID;
                movimiento.NDocumento = rescateDetalle.NDocumento;
            }
            catch (Exception)
            { }

            List<DetalleEgreso> detalles = db.DetalleEgreso.Where(d => d.MovimientoID == id).ToList();
            foreach (DetalleEgreso detalle in detalles)
            {
                if (detalle.DeudaPendienteID != null)
                {
                    //SAG2DB tmp = new SAG2DB();
                    DeudaPendiente dp = db.DeudaPendiente.Find(detalle.DeudaPendienteID);
                    dp.EgresoID = null;
                    db.Entry(dp).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (detalle.FondoFijoID != null)
                {
                    //SAG2DB tmp = new SAG2DB();
                    FondoFijo ff = db.FondoFijo.Find(detalle.FondoFijoID);
                    ff.EgresoID = null;
                    db.Entry(ff).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (detalle.BoletaHonorarioID != null)
                {
                   // SAG2DB tmp = new SAG2DB();
                    BoletaHonorario bh = db.BoletaHonorario.Find(detalle.BoletaHonorarioID);
                    bh.EgresoID = null;
                    db.Entry(bh).State = EntityState.Modified;
                    db.SaveChanges();
                }

                db.DetalleEgreso.Remove(detalle);
                db.SaveChanges();
            }

            try
            {
                db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + id);
                db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + id);
                db.Database.ExecuteSqlCommand("UPDATE DeudaPendiente SET EgresoID = NULL WHERE EgresoID = " + id);
                db.Database.ExecuteSqlCommand("UPDATE FondoFijoGrupo SET EgresoID = NULL, Activo = 'S' WHERE EgresoID = " + id);
                db.Database.ExecuteSqlCommand("UPDATE FondoFijo SET EgresoID = NULL WHERE EgresoID = " + id);
                db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + id);
                db.Database.ExecuteSqlCommand("DELETE FROM BienMovimientoInventario WHERE BienID in  ( SELECT ID  FROM  BienModInventario where MovimientoID  =  " + id + " )");
                db.Database.ExecuteSqlCommand("DELETE FROM BienModInventario WHERE MovimientoID  =  " + id + " ");
            }
            catch (Exception)
            { }

            int monto = movimiento.Monto_Egresos;

            if (ModelState.IsValid)
            {
                movimiento.Nulo = "S";
                movimiento.Monto_Egresos = 1;
                db.Entry(movimiento).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (utils.anularSaldoEgreso(movimiento, ModelState, monto))
            {
                @ViewBag.Mensaje = utils.mensajeOK("Ingreso anulado con éxito!");
            }
            else
            {
                @ViewBag.Mensaje = utils.mensajeError("Ocurrió un error el anular el Ingreso");
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