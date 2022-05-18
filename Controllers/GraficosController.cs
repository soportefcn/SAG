using SAG2.Classes;
using SAG2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAG2.Controllers
{
    public class GraficosController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Constantes ctes = new Constantes();

        //
        // GET: /Graficos/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Comprobantes()
        {
            return View();
        }

        public ActionResult ComprobantesData()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];

            ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos);
            ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos);
            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == Proyecto.ID).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto);

            return View();
        }

    }
}
