using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.Web.Security;
using SAG2.Classes;

namespace SAG2.Controllers
{
    public class HomeController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();
        string[] Meses = new string[12]
        {
	        "Enero",
	        "Febrero",
	        "Marzo",
	        "Abril",
	        "Mayo",
	        "Junio",
	        "Julio",
	        "Agosto",
	        "Septiembre",
	        "Octubre",
	        "Noviembre",
	        "Diciembre"
	    };

        public ActionResult Index()
        {
            if (Session["Proyecto"] == null)
            {
                Session.Clear();
                Session.Add("InformacionPie", "Usuario no identificado.");
                Response.Redirect(FormsAuthentication.LoginUrl, false);
            }

            Proyecto proyecto = (Proyecto)Session["Proyecto"];
            int proyectoID = proyecto.ID;
            int cuentacorrienteID = ((CuentaCorriente)Session["CuentaCorriente"]).ID;
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Usuario Usuario = (Usuario)Session["Usuario"];
            @ViewBag.Alertas = "0";

            try
            {
                @ViewBag.Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == cuentacorrienteID).Where(s => s.Periodo == periodo).Where(s => s.Mes == mes).Single();
            }
            catch (Exception e)
            {
                utils.Log(2, "No existe saldo para periodo actual, se inicializa con CERO. " + e.Message);
                Saldo Saldo = new Saldo();
                Saldo.SaldoInicialCartola = 0;
                Saldo.SaldoFinal = 0;
                @ViewBag.Saldo = Saldo;
            }
            
            @ViewBag.CuentaCorriente = db.CuentaCorriente.Find(cuentacorrienteID);
            @ViewBag.Proyecto = db.Proyecto.Find(proyectoID);
            int Pinter = 1;
            try
            {
                Pinter = db.Proyecto.Find(proyectoID).Convenio.Tintervencion;
            }
            catch (Exception) { 
            
            }
            @ViewBag.Roles = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == proyectoID).Where(r => r.TipoRolID != 9).OrderBy( r => r.TipoRol.Nombre).ToList()   ;
            @ViewBag.Auditorias = db.IndicadorCalidad.Where(d=> d.ProyectoID == proyectoID).Where(d => d.Periodo == periodo).Where(d => d.Tipo == 2).OrderByDescending(d => d.FechaInforme).ToList();
            @ViewBag.Supervisiones = db.IndicadorCalidad.Where(d => d.ProyectoID == proyectoID).Where(d => d.Periodo == periodo).Where(d => d.Tipo == 1).OrderByDescending(d => d.FechaInforme).ToList(); // Supervision.OrderByDescending(a => a.Fecha).ToList();
          //  Calidad.Where(d => d.Mes < 7).Where(d => d.Tipo == 1).Sum(d => d.GastoObjetado);
            // Rem aqui va cambiar supervisiones
            @ViewBag.Contratos = db.Contrato.Include(c => c.Servicio).Where(c => c.ProyectoID == proyectoID).OrderBy(c => c.Servicio.Nombre).ToList();
            @ViewBag.Resolucion = db.Resolucion.Where(d => d.ProyectoID == proyectoID).OrderByDescending(d => d.ID).ToList();
            @ViewBag.ResolucionDoc = db.ResolucionDescarga.Where(d => d.Resolucion.ProyectoID == proyectoID).ToList(); 
            
            string ValorUSS = "";
            string FechaUSS = "";

            if (Pinter == 1)
            {
                try {
                    ValorUSS = db.ProgramaQ.Where(d => d.ProyectoID == proyectoID).OrderByDescending(d => d.ID).FirstOrDefault().Valor.ToString();
                }catch (Exception) {
                    ValorUSS = "";
                }
               
                try {
                    FechaUSS = db.ProgramaQ.Where(d => d.ProyectoID == proyectoID).OrderByDescending(d => d.ID).FirstOrDefault().FechaIngreso.ToShortDateString();
                } catch (Exception) {
                    FechaUSS = "";
               }
            }
            if (Pinter == 2)
            {
                try
                {
                    ValorUSS = db.ValorUF.Where(d => d.Predeterminado == 1).OrderByDescending(d => d.ID).FirstOrDefault().Valor.ToString();
                }
                catch (Exception)
                {
                    ValorUSS = "";
                }

                try
                {
                    FechaUSS = db.ValorUF.Where(d => d.Predeterminado == 1).OrderByDescending(d => d.ID).FirstOrDefault().FechaIngreso.ToShortDateString();
                }
                catch (Exception)
                {
                    FechaUSS = "";
                }
            }

            @ViewBag.ValorUSS = ValorUSS;
            @ViewBag.FechaUSS = FechaUSS;

            int PZona = 0;
            int ComunaID = 0;
            try
            {
                ComunaID = db.Proyecto.Where(d => d.ID == proyectoID).FirstOrDefault().Direccion.ComunaID;
                PZona = db.PorcentajeZona.Where(d => d.ComunaID == ComunaID).OrderByDescending(d => d.ID).FirstOrDefault().Valor ;
            }
            catch (Exception)
            {
                PZona = 0;
                ComunaID = 0;
            }
            @ViewBag.PZona = PZona;

            double Pbase = 0;
            string tipBa = "";
            try
            {
                tipBa = db.Proyecto.Where(d => d.ID == proyectoID).FirstOrDefault().TipoProyecto.Sigla;
                Pbase = db.ParametroUss.Where(d => d.Tipo == tipBa).OrderByDescending(d => d.ID).FirstOrDefault().uss;
            }
            catch (Exception)
            {
                Pbase = 0;
            }
            @ViewBag.Pbase = Pbase;
            try
            {
                @ViewBag.FondoFijoUtilizado = db.FondoFijo.Where(f => f.EgresoID == null).Where(f => f.ProyectoID == proyectoID).Sum(f => f.Monto) / (ctes.porcentajeFondoFijo * ctes.montoFondoFijo) * 100;
            }
            catch (Exception)
            {
                @ViewBag.FondoFijoUtilizado = 0;
            }

            Session.Add("FondoFijoUtilizado", ViewBag.FondoFijoUtilizado);

            // Modificar para que autorizaciones solo sean para Supervisoras filtradas por proyecto
            try
            {
                @ViewBag.AutorizacionesPendientes = 0;
                if (Usuario.esAdministrador)
                {
                    @ViewBag.AutorizacionesPendientes = db.Autorizacion.Where(a => a.Autorizado == null).Count();
                }
                else if (Usuario.esSupervisor)
                {
                    @ViewBag.AutorizacionesPendientes = db.Autorizacion.Where(a => a.Autorizado == null).Count();
                }
                @ViewBag.PreguntasPendientes = 0;
                if (Usuario.esAdministrador)
                {
                    @ViewBag.PreguntasPendientes = db.PreguntaFrecuente.Where(a => a.Respuesta == null).Count();
                }
                else if (Usuario.esSupervisor)
                {
                    @ViewBag.PreguntasPendientes = db.PreguntaFrecuente.Where(a => a.Respuesta == null).Count();
                }
                @ViewBag.Alertas = "1";
            }
            catch (Exception)
            {
                @ViewBag.AutorizacionesPendientes = 0;
            }

            Session.Add("AutorizacionesPendientes", ViewBag.AutorizacionesPendientes);

            if (proyecto.MesInicio != null && proyecto.PeriodoInicio != null && (proyecto.PeriodoInicio > periodo || (proyecto.PeriodoInicio == periodo && proyecto.MesInicio > mes))) 
            {
                @ViewBag.Script = "$(document).ready(function () {$(\"#MenuRendicion\").hide();});";
                @ViewBag.Mensaje = utils.mensajeError("Este Proyecto está programado para comenzar en "+ Meses[(int)proyecto.MesInicio - 1] + " del "+ proyecto.PeriodoInicio + ", el menú de Rendición de Cuentas no estará habilitado hasta ese período.");
            }

            // Fecha entrega respuesta auditoria
            @ViewBag.RespuestaAuditoria = 0;
            try
            {
                if (!Usuario.esUsuario)
                {
                    DateTime fechaFutura = DateTime.Now.AddDays(5);
                    //@ViewBag.RespuestaAuditoria = db.InformeAuditoria.Where(i => i.ProyectoID == proyecto.ID && i.VIII_FechaEntrega != null && (TimeSpan.Parse(i.VIII_FechaEntrega.Value.ToString()) - TimeSpan.Parse(fechaActual.ToString())).Days <= 5).Count();
                    @ViewBag.RespuestaAuditoria = db.ProgramaAnualAuditorias.Include(p => p.Proyecto).Include(p => p.Auditor).Where(p => p.PersonaID == Usuario.PersonaID && p.FechaProgramada <= fechaFutura && p.FechaReal == null).Count();
                    @ViewBag.Alertas = "1";
                }
                else
                {
                    @ViewBag.RespuestaAuditoria = 0;   
                }
            }
            catch (Exception)
            {
                @ViewBag.RespuestaAuditoria = 0;   
            }

            Session.Add("Alertas", ViewBag.Alertas);
            Session.Add("RespuestaAuditoria", ViewBag.RespuestaAuditoria);
            // REvisar Fecha de Terminp
            DateTime FechaTermino = new DateTime();
            var resol = db.Resolucion.Where(d => d.ProyectoID == proyecto.ID && d.Estado == 1).FirstOrDefault();

            if (resol == null)
            {
                try
                {
                    FechaTermino = DateTime.Parse(proyecto.Convenio.FechaTermino.ToString());
                }
                catch {

                    FechaTermino = DateTime.Now;
                }
            }
            else
            {
                FechaTermino = DateTime.Parse(resol.FechaTermino.ToString());
            }
            int dias = (FechaTermino - DateTime.Now).Days;
            int fin = 30;
            string MensajeFechafin = "";
            if (FechaTermino > DateTime.Now)
            {
                if (FechaTermino.Year == DateTime.Now.Year)
                {
                    if (FechaTermino.Month == DateTime.Now.Month)
                    {
                        MensajeFechafin = "El Programa finaliza mes en curso ";

                    }
                }
            }
            else
            {
                MensajeFechafin = "La fecha de Termino del convenio caduco ";
                dias = dias + fin;
            }
            ViewBag.FechaMensaje = MensajeFechafin;
            Session.Add("FechaMensaje", MensajeFechafin);
            ViewBag.DiasMensaje = dias;
            Session.Add("DiasMensaje", dias);

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Error(Inventario inventario)
        {
          
            return RedirectToAction("Index");
        }

        public RedirectResult CambiarTipo(string tipo)
        {
            string referrer = Request.UrlReferrer.ToString();

            if (tipo.Equals("Usuario"))
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                Usuario.Administrador = "N";
                Usuario.Supervisor = "N";
                Session.Remove("Usuario");
                Session.Add("Usuario", Usuario);
                Session.Add("CambioTipo", true);
            }
            else if (tipo.Equals("Administrador"))
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                Usuario.Administrador = "S";
                Usuario.Supervisor = "N";
                Session.Remove("Usuario");
                Session.Add("Usuario", Usuario);
                Session.Add("CambioTipo", true);
            }
            else if (tipo.Equals("Supervisor"))
            {
                Usuario Usuario = (Usuario)Session["Usuario"];
                Usuario.Administrador = "N";
                Usuario.Supervisor = "S";
                Session.Remove("Usuario");
                Session.Add("Usuario", Usuario);
                Session.Add("CambioTipo", true);
            }

            return Redirect(referrer);
        }
    }
   

}
