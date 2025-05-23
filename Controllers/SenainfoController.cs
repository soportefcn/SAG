﻿using SAG2.Classes;
using SAG2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAG2.Controllers
{
    public class SenainfoController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();
        //
        // GET: /Senainfo/

        public ActionResult Index()
        {
            int largoCodigoSename = 7;
            ViewBag.Proyec = db.Proyecto.Where(p => p.CodSename.Length != largoCodigoSename && p.Cerrado == null && p.Eliminado == null);
            ViewBag.Proy = db.Proyecto.Where(p => p.CodSename.Length == null && p.Cerrado == null && p.Eliminado == null);
            ViewBag.LineasAtencionID = new SelectList(db.LineasAtencion, "ID", "NombreLista");
            return View();
        }

        [HttpPost]
        public ActionResult ExportarLinea(int LineasAtencionID, int periodo, int mes)
        {
            ViewBag.mes = mes;
            ViewBag.periodo = periodo;
            ViewBag.Movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).Where(a => a.Proyecto.TipoProyecto.LineasAtencionID == LineasAtencionID).OrderBy(m => m.ProyectoID).ThenBy(m => m.TipoComprobanteID).ThenBy(m => m.NumeroComprobante).ToList();

            string sigla = db.LineasAtencion.Find(LineasAtencionID).Sigla;
            ViewBag.sigla = sigla;
            int largoCodigoSename = 7;
            var Proyectos = db.Proyecto.Where(p => p.Cerrado == null && p.Eliminado == null && p.CodSename != null && !p.CodSename.Equals("") && p.CodSename.Length == largoCodigoSename).Where(p => p.TipoProyecto.LineasAtencionID == LineasAtencionID).OrderBy(p => p.ID);

            return View(Proyectos.ToList());
        }


        [HttpPost]
        public ActionResult ExportarArchivo(int periodo, int mes)
        {
            /*int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];*/

            ViewBag.mes = mes;
            ViewBag.periodo = periodo;

            var movimiento = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.TipoComprobanteID).ThenBy(m => m.NumeroComprobante);
            return View(movimiento.ToList());
        }

        [HttpPost]
        public ActionResult Exportar(int periodo, int mes)
        {
            ViewBag.mes = mes;
            ViewBag.periodo = periodo;
            ViewBag.Movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.TipoComprobanteID).ThenBy(m => m.NumeroComprobante).ToList();

            // Largo del codigo de Sename
            int largoCodigoSename = 7;

            var Proyectos = db.Proyecto.Where(p => p.Cerrado == null && p.Eliminado == null && p.CodSename != null && !p.CodSename.Equals("") && p.CodSename.Length == largoCodigoSename).OrderBy(p => p.ID);
           // var Proyectos = db.Proyecto.Where(p => p.ID == 148);
            return View(Proyectos.ToList());
        }

        [HttpPost]
        public ActionResult ExportarOrdenado(int periodo, int mes)
        {
            ViewBag.mes = mes;
            ViewBag.periodo = periodo;
            ViewBag.Movimientos = db.Movimiento.Where(m => m.auto == 0).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(a => a.Temporal == null && a.Eliminado == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.Mes).ThenBy(m => m.ProyectoID).ThenBy(m => m.TipoComprobanteID).ThenBy(m => m.NumeroComprobante).ToList();
            //var Proyectos = db.Proyecto.Where(p => p.Cerrado == null && p.Eliminado == null && p.CodSename != null && !p.CodSename.Equals("")).OrderBy(p => p.ID);
            //return View(Proyectos.ToList());
            return View();
        }

    }
}
