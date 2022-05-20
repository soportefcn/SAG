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
            @ViewBag.Roles = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == proyectoID).Where(r => r.TipoRolID != 9).OrderBy( r => r.TipoRol.Nombre).ToList()   ;
            @ViewBag.Auditorias = db.IndicadorCalidad.Where(d=> d.ProyectoID == proyectoID).Where(d => d.Periodo == periodo).Where(d => d.Tipo == 2).OrderByDescending(d => d.FechaInforme).ToList();
            @ViewBag.Supervisiones = db.IndicadorCalidad.Where(d => d.ProyectoID == proyectoID).Where(d => d.Periodo == periodo).Where(d => d.Tipo == 1).OrderByDescending(d => d.FechaInforme).ToList(); // Supervision.OrderByDescending(a => a.Fecha).ToList();
          //  Calidad.Where(d => d.Mes < 7).Where(d => d.Tipo == 1).Sum(d => d.GastoObjetado);
            // Rem aqui va cambiar supervisiones
            @ViewBag.Contratos = db.Contrato.Include(c => c.Servicio).Where(c => c.ProyectoID == proyectoID).OrderBy(c => c.Servicio.Nombre).ToList();

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
