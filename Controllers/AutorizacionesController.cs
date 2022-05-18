using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.Data;
using SAG2.Classes;

namespace SAG2.Controllers
{
    public class AutorizacionesController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Constantes ctes = new Constantes();
        private Util utils = new Util();

        public ActionResult Pendientes()
        {
            var autorizaciones = db.Autorizacion.OrderByDescending(a => a.FechaSolicitud);
            return View(autorizaciones.ToList());
        }

        [HttpGet]
        public ActionResult Opciones()
        {
            var opciones = db.OpcionesSupervision;
            return View(opciones.ToList());
        }

        [HttpPost]
        public ActionResult Opciones(FormCollection Form)
        {
            int numeroOpciones = db.OpcionesSupervision.Count();

            for (int i = 1; i <= numeroOpciones; i++)
            {
                string valor = Form["Opcion" + i].ToString();
                OpcionesSupervision OpcionesSupervision = db.OpcionesSupervision.Find(i);
                OpcionesSupervision.Opcion = null;

                if (valor.Equals("true"))
                {
                    OpcionesSupervision.Opcion = "S";
                }

                db.Entry(OpcionesSupervision).State = EntityState.Modified;
                db.SaveChanges();
            }

            var opciones = db.OpcionesSupervision;
            return View(opciones.ToList());
        }

        public ActionResult Autorizar(int id)
        {
            Persona persona = (Persona)Session["Persona"];
            Autorizacion autorizacion = db.Autorizacion.Find(id);
            autorizacion.Autorizado = "S";
            autorizacion.AutorizaID = persona.ID;
            autorizacion.FechaAutorizacion = DateTime.Now;
            Movimiento original = db.Movimiento.Find(autorizacion.OriginalID);
            Movimiento modificado = db.Movimiento.Find(autorizacion.ModificadoID);
            //autorizacion.OriginalID = null;
            db.Entry(autorizacion).State = EntityState.Modified;
            db.SaveChanges();

            if (autorizacion.Tipo.Equals("Modificación"))
            {
                if (original.TipoComprobanteID == ctes.tipoIngreso || original.TipoComprobanteID == ctes.tipoReintegro)
                {
                    // Comprobante de Ingreso y Reintegro
                    original.Temporal = null;
                    original.Eliminado = "S";
                    // Mejor marcar como eliminado
                    utils.actualizarSaldoIngreso(modificado, ModelState, original.Monto_Ingresos);
                    db.Entry(original).State = EntityState.Modified;
                    //db.Movimiento.Remove(original);
                    db.SaveChanges();

                    modificado.Temporal = null;
                    modificado.Eliminado = null;
                    db.Entry(modificado).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (original.TipoComprobanteID == ctes.tipoEgreso)
                {
                    // Comprobante de Egreso
                    db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = " + modificado.ID + " WHERE EgresoID = " + original.ID);
                    db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = " + modificado.ID + " WHERE EgresoID = " + original.ID);
                    db.Database.ExecuteSqlCommand("UPDATE DeudaPendiente SET EgresoID = " + modificado.ID + " WHERE EgresoID = " + original.ID);
                    db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + original.ID);
                    db.Database.ExecuteSqlCommand("UPDATE DetalleEgreso SET Temporal = NULL WHERE MovimientoID = " + modificado.ID);

                    // Comprobante de Ingreso y Reintegro
                    utils.actualizarSaldoEgreso(modificado, ModelState, original.Monto_Egresos);
                    //db.Movimiento.Remove(original);

                    //db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + original.ID);
                    //db.Database.ExecuteSqlCommand("DELETE FROM Movimiento WHERE ID = " + original.ID);

                    original.Temporal = null;
                    original.Eliminado = "S";
                    db.Entry(original).State = EntityState.Modified;
                    db.Movimiento.Remove(original);
                    db.SaveChanges();                    
                    
                    modificado.Temporal = null;
                    modificado.Eliminado = null;
                    db.Entry(modificado).State = EntityState.Modified;
                    db.SaveChanges();

                    // Actualizamos Informacion de FF si EXISTE
                    try
                    {
                        DetalleEgreso de = db.DetalleEgreso.Where(d => d.MovimientoID == modificado.ID).Take(1).Single();
                        if (de.FondoFijo != null)
                        {
                            int ffgID = de.FondoFijo.FondoFijoGrupoID;
                            FondoFijoGrupo ffg = db.FondoFijoGrupo.Find(ffgID);
                            ffg.EgresoID = modificado.ID;
                            ffg.Activo = "N";
                            ffg.Modificacion = DateTime.Now;
                            db.Entry(ffg).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    catch (Exception)
                    { }

                }
            }
            else if (autorizacion.Tipo.Equals("Anulación"))
            {
                if (original.TipoComprobanteID == ctes.tipoIngreso || original.TipoComprobanteID == ctes.tipoReintegro)
                {
                    // Comprobante de Ingreso y Reintegro
                    int monto = original.Monto_Ingresos;
                    utils.anularSaldoIngreso(modificado, ModelState, monto);
                    original.Temporal = null;
                    original.Eliminado = "S";
                    db.Entry(original).State = EntityState.Modified;
                    //db.Movimiento.Remove(original);
                    db.SaveChanges();

                    modificado.Temporal = null;
                    modificado.Eliminado = null;
                    modificado.Nulo = "S";
                    db.Entry(modificado).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (original.TipoComprobanteID == ctes.tipoEgreso)
                {
                    // Comprobante de Egreso
                    db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + original.ID);
                    db.Database.ExecuteSqlCommand("UPDATE BoletaHonorario SET EgresoID = NULL WHERE EgresoID = " + original.ID);
                    db.Database.ExecuteSqlCommand("UPDATE DeudaPendiente SET EgresoID = NULL WHERE EgresoID = " + original.ID);
                    db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + original.ID);

                    int monto = original.Monto_Egresos;
                    utils.anularSaldoEgreso(modificado, ModelState, monto);
                    original.Temporal = null;
                    original.Nulo = "S";
                    db.Entry(original).State = EntityState.Modified;
                    //db.Movimiento.Remove(original);
                    db.SaveChanges();
                    db.SaveChanges();

                    modificado.Temporal = null;
                    modificado.Eliminado = null;
                    db.Entry(modificado).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            // Actualizar todos los saldos de todos los periodos
            int periodo_comprobante = modificado.Periodo;
            int mes_comprobante = modificado.Mes;


            return RedirectToAction("Pendientes");
        }

        public ActionResult Rechazar(int id)
        {
            Persona persona = (Persona)Session["Persona"];
            Autorizacion autorizacion = db.Autorizacion.Find(id);

            int? ModificadoID = autorizacion.ModificadoID;
            autorizacion.Autorizado = "N";
            autorizacion.AutorizaID = persona.ID;
            autorizacion.ModificadoID = autorizacion.OriginalID;
            autorizacion.FechaAutorizacion = DateTime.Now;
            db.Entry(autorizacion).State = EntityState.Modified;
            db.SaveChanges();

            try
            {
                db.Database.ExecuteSqlCommand("DELETE FROM DetalleEgreso WHERE MovimientoID = " + ModificadoID);
                db.Database.ExecuteSqlCommand("DELETE FROM Movimiento WHERE ID = " + ModificadoID);
            }
            catch (Exception)
            { }

            return RedirectToAction("Pendientes");
        }
    }
}
