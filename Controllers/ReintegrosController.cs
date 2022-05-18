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
    public class ReintegrosController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();

        //
        // GET: /Reintegros/
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
            try
            {
                Buscarbeneficiario = form["bbene"].ToString();
            }
            catch (Exception)
            {
                Buscarbeneficiario = "";
            }
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            var movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo.Equals(buscarPeriodo)).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            if (buscarNdoc != 0)
            {
                movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo.Equals(buscarPeriodo) && a.NumeroComprobante.Equals(buscarNdoc)).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            }
            if (Buscarbeneficiario != "")
            {
                movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 6 || a.CuentaID == null) || a.CuentaID == null) && a.Periodo.Equals(buscarPeriodo) && a.Beneficiario.Contains(Buscarbeneficiario)).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            }

            return View(movimiento.ToList());
        }
        public ViewResult Index()
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            var movimiento = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.auto == 0).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).OrderByDescending(a => a.Periodo).ThenByDescending(a => a.NumeroComprobante);
            return View(movimiento.ToList());
        }
        [HttpGet]
        public ActionResult ListarLineas(int? t = null)
        {

            if (Session["DetalleReintegro"] != null)
            {
                return View((List<DetalleReintegro>)Session["DetalleReintegro"]);
            }
            else
            {
                if (t == null)
                {
                    return View(new List<DetalleReintegro>());
                }
                else
                {
                    List<DetalleReintegro> lista = new List<DetalleReintegro>();
                    lista = db.DetalleReintegro.Where(d => d.MovimientoID == t).ToList();
                    Session.Add("DetalleReintegro", lista);
                    return View((List<DetalleReintegro>)Session["DetalleReintegro"]);
                }
            }
        }

        public ActionResult Create2()
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Session.Remove("DetalleReintegro");
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
           // ViewBag.NroComprobante = 1;
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
                if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Periodo == periodo).Count() > 0)
                {
                    try
                    {
                        ViewBag.NroComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Periodo == periodo).Max(a => a.NumeroComprobante) + 1;
                        if (ViewBag.NroComprobante == null)
                        {
                            ViewBag.NroComprobante = 1;
                        }
                    }catch{
                        ViewBag.NroComprobante = 1;
                    }
                }
            }
            ViewBag.DocumentoIDD = new SelectList(db.Documento, "ID", "NombreLista");
            ViewBag.ItemGastoID = new SelectList(db.ItemGasto, "ID", "NombreLista");
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
            return View();
        } 


        public ActionResult Edit(int id, string imprimir = "")
        {
            Movimiento movimiento = db.Movimiento.Find(id);
            int periodo = (int)Session["Periodo"];
            DetalleEgreso detalle = db.DetalleEgreso.Find(movimiento.DetalleEgresoID);
            ViewBag.detalle = detalle;
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos), movimiento.CuentaID);
            ViewBag.Imprimir = imprimir;
            ViewBag.UltimoIdentificador = "0";
            try
            {
                //ViewBag.UltimoIdentificador = db.Movimiento.Where(m => m.ProyectoID == movimiento.ProyectoID).Where(a => a.TipoComprobanteID == ctes.tipoReintegro).Max(a => a.ID).ToString();
                ViewBag.UltimoIdentificador = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.ProyectoID == movimiento.ProyectoID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).Max(a => a.ID).ToString();
            }
            catch (Exception)
            {
                ViewBag.UltimoIdentificador = "0";
            }


            return View(movimiento);
        }

        public ActionResult Edit2(int id, string imprimir = "")
        {
            Session.Remove("DetalleReintegro");
            Movimiento movimiento = db.Movimiento.Find(id);
            int periodo = (int)Session["Periodo"];
            DetalleEgreso detalle = db.DetalleEgreso.Find(movimiento.DetalleEgresoID);
            ViewBag.brut =movimiento.Rut;
            ViewBag.bnombre = movimiento.Beneficiario;
            ViewBag.detalle = detalle;
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos), movimiento.CuentaID);
            ViewBag.Imprimir = imprimir;
            ViewBag.UltimoIdentificador = "0";
            ViewBag.DocumentoIDD = new SelectList(db.Documento, "ID", "NombreLista");

            try
            {
                //ViewBag.UltimoIdentificador = db.Movimiento.Where(m => m.ProyectoID == movimiento.ProyectoID).Where(a => a.TipoComprobanteID == ctes.tipoReintegro).Max(a => a.ID).ToString();
                ViewBag.UltimoIdentificador = db.Movimiento.Include(m => m.Proyecto).Include(m => m.TipoComprobante).Include(m => m.Cuenta).Include(m => m.Persona).Include(m => m.Proveedor).Include(m => m.CuentaCorriente).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.ProyectoID == movimiento.ProyectoID).Where(a => a.Temporal == null && a.Eliminado == null && a.CuentaID != 1).Where(a => a.auto == 0).Max(a => a.ID).ToString();
            }
            catch (Exception)
            {
                ViewBag.UltimoIdentificador = "0";
            }


            return View(movimiento);
        }


        public int Guardar(string Beneficiario, int ComprobanteEgreso, int CuentaID, string DV, string Descripcion, int DetalleEgresoID, bool ImprimirComprobante, int Mes, int Monto_Ingresos, int NDocumento, int Periodo, bool PreguntarImprimir, string Rut, string fecha)
        {
            int egresoID = 0;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int rev = 0;
            try
            {
                if (CuentaID == 60)
                {
                    rev = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.DetalleEgresoID == DetalleEgresoID).Where(a => a.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Periodo == Periodo).Count();
                }
                else {
                    rev = 0;
                }
              }
            catch(Exception){
            rev = 0;
            }
            if (rev == 0)
            {

                CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
                Movimiento reintegro = new Movimiento();

                reintegro.NumeroComprobante = 1;

                if (db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Periodo == Periodo).Count() > 0)
                {
                    reintegro.NumeroComprobante = db.Movimiento.Where(m => m.auto == 0).Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Periodo == Periodo).Max(a => a.NumeroComprobante) + 1;
                }

                reintegro.ProyectoID = Proyecto.ID;
                reintegro.TipoComprobanteID = ctes.tipoReintegro;
                reintegro.CuentaID = CuentaID;
                reintegro.CuentaCorrienteID = CuentaCorriente.ID;
                reintegro.NDocumento = NDocumento;
                reintegro.Periodo = Periodo;
                reintegro.Mes = Mes;
                reintegro.Fecha = DateTime.Parse(fecha);
                reintegro.FechaCheque = DateTime.Parse(fecha);
                reintegro.Descripcion = Descripcion.ToUpper();
                reintegro.Monto_Ingresos = Monto_Ingresos;
                // reintegro.Descripcion = Descripcion;
                Usuario Usuario = (Usuario)Session["Usuario"];
                reintegro.UsuarioID = Usuario.ID;
                reintegro.FechaCreacion = DateTime.Now;
                reintegro.DV = DV;
                reintegro.Rut = Rut;
                reintegro.DetalleEgresoID = DetalleEgresoID;
                reintegro.Beneficiario = Beneficiario;
                db.Movimiento.Add(reintegro);
                db.SaveChanges();

                // detalle
                var monto_egresos = 0;
                egresoID = reintegro.ID;
                if (Session["DetalleReintegro"] != null)
                {
                    List<DetalleReintegro> lista = (List<DetalleReintegro>)Session["DetalleReintegro"];
                    foreach (DetalleReintegro detalle in lista)
                    {
                        monto_egresos += detalle.Monto;

                        detalle.MovimientoID = egresoID;
                        db.DetalleReintegro.Add(detalle);
                        db.SaveChanges();
                    }



                }

             
            }
            return egresoID;
        }

        //
        // POST: /Reintegros/Edit/5
        public string GuardarLinea(int DocumentoIDD, string NDocumentoD, int Monto, string FechaEmision, int CuentaIDD, int DetalleReintegroIndex = -1)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
            Usuario Usuario = (Usuario)Session["Usuario"];
         
            // Verificamos que haya saldo disponible para guardar el detalle de egreso
            List<DetalleReintegro> lista = null;
            if (Session["DetalleReintegro"] != null)
            {
                lista = (List<DetalleReintegro>)Session["DetalleReintegro"];
            }
            else
            {
                lista = new List<DetalleReintegro>();
            }
            // Se limpia la lista temporal
            Session.Remove("DetalleReintegro");
            try
            {
                DetalleReintegro detalle = new DetalleReintegro();
                if (DetalleReintegroIndex > -1)
                {

                    detalle = lista.ElementAt(DetalleReintegroIndex - 1);
                }     
                detalle.DocumentoID = DocumentoIDD;
                detalle.NDocumento = NDocumentoD;
                detalle.Monto = Monto;
                detalle.FechaEmision = DateTime.Parse(FechaEmision);       
                detalle.CuentaIDD = CuentaIDD;
                detalle.FechaCreacion = DateTime.Now;
                detalle.UsuarioID = Usuario.ID;
 if (DetalleReintegroIndex > -1)
                {
                    lista.RemoveAt(DetalleReintegroIndex - 1);
                }
                lista.Add(detalle);
                Session.Add("DetalleReintegro", lista);

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string BorrarLinea(int DetalleEgresoID = 0, int DetalleReintegroIndex = -1)
        {
          

            try
            {
                if (Session["DetalleReintegro"] != null)
                {
                    List<DetalleReintegro> lista = (List<DetalleReintegro>)Session["DetalleReintegro"];
                if (DetalleReintegroIndex > -1)
                    {

                        lista.RemoveAt(DetalleReintegroIndex - 1);
                    }

                    Session.Remove("DetalleReintegro");
                    Session.Add("DetalleReintegro", lista);
                }



                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
     
        [HttpPost]
        public ActionResult Edit2(Movimiento movimiento)
        {


        
            var monto_egresos = 0;
            int egresoID = movimiento.ID;
            var DetalleEgresoID = movimiento.DetalleEgresoID;
            int proyectoID = movimiento.ProyectoID;
            var CuentaID = movimiento.CuentaID;
            int CuentacorrienteID = movimiento.CuentaCorrienteID;
            int NumeroComprobante = movimiento.NumeroComprobante;
          
           string Descripcion = movimiento.Descripcion;
           var UsuarioID = movimiento.UsuarioID;
           string DV = movimiento.DV;
           string rut = movimiento.Rut;
           string Beneficiario = movimiento.Beneficiario;

           Usuario Usuario = (Usuario)Session["Usuario"];
           int periodo = (int)Session["Periodo"];
           int mes = (int)Session["Mes"];

           Persona persona = (Persona)Session["Persona"];
           Proyecto Proyecto = (Proyecto)Session["Proyecto"];
           CuentaCorriente CuentaCorriente = (CuentaCorriente)Session["CuentaCorriente"];
          // int montoOriginal = Int32.Parse(Request.Form["MontoOriginal"].ToString());
           movimiento.CuentaID = Int32.Parse(Request.Form["CuentaID"].ToString());
           movimiento.NumeroComprobante = Int32.Parse(Request.Form["NumeroComprobante"].ToString());
           //movimiento.Fecha = DateTime.Parse(fecha);
           movimiento.NDocumento = NumeroComprobante;
           movimiento.PersonaID = null;
           movimiento.ProveedorID = null;
           movimiento.TipoComprobanteID = ctes.tipoReintegro;
           int originalID = movimiento.ID;
           movimiento.Descripcion = movimiento.Descripcion.ToUpper();
           ViewBag.DocumentoIDD = new SelectList(db.Documento, "ID", "NombreLista");
           ViewBag.UltimoIdentificador = "0";

           db.Movimiento.Attach(movimiento);
           db.Entry(movimiento).State = EntityState.Modified;
           db.SaveChanges();

           // Se deben actualizar los saldos

           int mes_comprobante = movimiento.Mes;
           int periodo_comprobante = movimiento.Periodo;
           int mes_proyecto = mes;
           int periodo_proyecto = periodo;
           var NroComprobante3 = 10000 + NumeroComprobante;
           utils.RecalcularSaldos(periodo_comprobante, periodo_proyecto, mes_comprobante, mes_proyecto, Proyecto, CuentaCorriente);



            // Eliminar DetalleReintegro
            
            db.Database.ExecuteSqlCommand("DELETE FROM DetalleReintegro WHERE MovimientoID = " + egresoID);
            List<DetalleReintegro> lista2 = (List<DetalleReintegro>)Session["DetalleReintegro"];
            foreach (DetalleReintegro detalle in lista2)
            {
                monto_egresos += detalle.Monto;

                detalle.MovimientoID = egresoID;
                db.DetalleReintegro.Add(detalle);
                db.SaveChanges();
            }





                Session.Remove("DetalleReintegro");

            
     
            return RedirectToAction("Create2");
            //return View(movimiento);
        }
        public ActionResult Anular(int id)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Movimiento movimiento = db.Movimiento.Find(id);
            Persona persona = (Persona)Session["Persona"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var DetalleEgresoID = movimiento.DetalleEgresoID;
            if (movimiento.Periodo != periodo || movimiento.Mes != mes)
            {
               
                movimiento.Temporal = "S";
                movimiento.Nulo = "S";
                movimiento.Eliminado = "N";
                movimiento.Monto_Ingresos = 0;
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

      
                return RedirectToAction("Edit2", new { id = @id, mensaje = "La anulación ha sido solicitada al Supervisor." });
            }

            int monto = movimiento.Monto_Ingresos;

            if (ModelState.IsValid)
            {
                var NroComprobante3 = 10000 + movimiento.NumeroComprobante;
                movimiento.Nulo = "S";
                db.Entry(movimiento).State = EntityState.Modified;
                //movimiento.Monto_Ingresos = 0;
                db.SaveChanges();

                db.Database.ExecuteSqlCommand("DELETE FROM DetalleReintegro WHERE MovimientoID = " + id);
                db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = ( Select ID FROM  Movimiento WHERE Movimiento.auto = 1 and Movimiento.TipoComprobanteID = 2 and Movimiento.NumeroComprobante = " + NroComprobante3 + " and Movimiento.Periodo = " + periodo + " and Movimiento.ProyectoID = " + Proyecto.ID + ") ");
                db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = ( Select ID FROM  Movimiento WHERE Movimiento.auto = 1 and Movimiento.TipoComprobanteID = 3 and Movimiento.NumeroComprobante = " + NroComprobante3 + " and Movimiento.Periodo = " + periodo + " and Movimiento.ProyectoID = " + Proyecto.ID + ") ");

                // Eliminar Reintegro y egreso
                db.Database.ExecuteSqlCommand("DELETE FROM Movimiento WHERE Movimiento.auto = 1 and Movimiento.TipoComprobanteID = 2 and Movimiento.NumeroComprobante = " + NroComprobante3 + " and Movimiento.Periodo = " + periodo + " and Movimiento.ProyectoID = " + Proyecto.ID);
                db.Database.ExecuteSqlCommand("DELETE FROM Movimiento WHERE Movimiento.auto = 1 and Movimiento.TipoComprobanteID = 3 and Movimiento.NumeroComprobante = " + NroComprobante3 + " and Movimiento.Periodo = " + periodo + " and Movimiento.ProyectoID = " + Proyecto.ID);


            }

            if (utils.anularSaldoIngreso(movimiento, ModelState, monto))
            {
                @ViewBag.Mensaje = utils.mensajeOK("Ingreso anulado con éxito!");
            }
            else
            {
                @ViewBag.Mensaje = utils.mensajeError("Ocurrió un error el anular el Ingreso");
            }

            return RedirectToAction("Create2");
        }

        public ActionResult ListadoDetalles()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            return View(db.DetalleEgreso.Where(d => d.Egreso.auto == 0).Where(d => d.Egreso.ProyectoID == Proyecto.ID).Where(d => d.Temporal == null && d.Nulo == null).OrderByDescending(d => d.Egreso.Fecha).ThenByDescending(d => d.Egreso.NumeroComprobante).ToList());
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            int periodo = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Movimiento reintrego = db.Movimiento.Find(id);
            int maxComprobante = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(a => a.TipoComprobanteID == ctes.tipoReintegro).Where(a => a.Periodo == periodo).Where(a=> a.auto == 0).Max(a => a.NumeroComprobante);

            if (reintrego.NumeroComprobante == maxComprobante)
            {
                int monto = reintrego.Monto_Ingresos;

                if (ModelState.IsValid)
                {
                    var NroComprobante3 = 10000 + reintrego.NumeroComprobante;
                    reintrego.Nulo = "S";
                   var  DetalleEgresoID = reintrego.DetalleEgresoID;
                    db.Entry(reintrego).State = EntityState.Modified;
                    reintrego.Monto_Ingresos = 0;
                    
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("DELETE FROM DetalleReintegro WHERE MovimientoID = " + id);
             
                }

                utils.anularSaldoIngreso(reintrego, ModelState, monto);
                db.Movimiento.Remove(reintrego);
                db.SaveChanges();

            }
            if (reintrego.NumeroComprobante == maxComprobante)
            {

                return RedirectToAction("Create2");
            }
            else {
                return RedirectToAction("Edit2");

            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public int CuentaIDD { get; set; }
    }
}