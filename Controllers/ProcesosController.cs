using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Data;

namespace SAG2.Controllers
{
    public class ProcesosController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        //
        // GET: /Procesos/
        public ActionResult Index()
        {
            return AperturaMes();
        }


        [HttpPost]
        public ActionResult AperturaMes(FormCollection collection)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];

            int periodo_siguiente = 0;
            int mes_siguiente = 0;

            // Si el periodo actual es diciembre, cambiamos año.
            if (mes == 12)
            {
                mes_siguiente = 1;
                periodo_siguiente = periodo + 1;
            }
            else
            {
                mes_siguiente = mes + 1;
                periodo_siguiente = periodo;
            }

            try
            {
                // Chequeamos si existe periodo en la tabla, si existe el periodo ya esta cerrado.
                Periodo Periodo = (from p in db.Periodo
                                   where p.Ano == periodo && p.Mes == mes
                                   select p).Single();
            }
            catch (Exception e)
            {
                utils.Log(1, "Periodo " + mes + "-" + periodo + " esta abierto y se procede a cerrar. " + e.Message);

                // Obtenemos todos los movimientos del periodo
                List<Movimiento> Movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).ToList();

                // Recorremos lista y vamos cerrando los movimientos
                foreach (Movimiento movimiento in Movimientos)
                {
                    movimiento.Cerrado = "S";
                    if (ModelState.IsValid)
                    {
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        utils.erroresState(ModelState);
                    }
                }

                Periodo Periodo = new Periodo();
                Periodo.Ano = periodo;
                Periodo.Mes = mes;

                if (ModelState.IsValid)
                {
                    db.Periodo.Add(Periodo);
                    db.SaveChanges();
                }

                Session.Add("Periodo", periodo_siguiente);
                Session.Add("Mes", mes_siguiente);
                Session.Add("InformacionPie", ((Proyecto)Session["Proyecto"]).NombreLista + " (" + ((CuentaCorriente)Session["CuentaCorriente"]).NumeroLista + ") | " + ((Persona)Session["Persona"]).NombreCompleto + " | " + mes_siguiente + "-" + periodo_siguiente);
            }

            return AperturaMes();
        }
        //
        // GET: /Procesos/AperturaMes/
        public ActionResult AperturaMes()
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];

            // Si el periodo actual es diciembre, cambiamos año.
            if (mes == 12)
            {
                Session.Add("mes_siguiente", 1);
                Session.Add("periodo_siguiente", periodo + 1);
            }
            else
            {
                Session.Add("mes_siguiente", mes + 1);
                Session.Add("periodo_siguiente", periodo);
            }
            return View();
        }


        //
        // GET: /Procesos/CierreMes/
        [HttpPost]
        public ActionResult CierreMes(FormCollection collection)
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];

            int periodo_siguiente = 0;
            int mes_siguiente = 0;

            // Si el periodo actual es diciembre, cambiamos año.
            if (mes == 12)
            {
                mes_siguiente = 1;
                periodo_siguiente = periodo + 1;
            }
            else
            {
                mes_siguiente = mes + 1;
                periodo_siguiente = periodo;
            }

            try 
            {
                // Chequeamos si existe periodo en la tabla, si existe el periodo ya esta cerrado.
                Periodo Periodo = (from p in db.Periodo
                                   where p.Ano == periodo && p.Mes == mes
                                   select p).Single();
            }
            catch (Exception e)
            {
                utils.Log(1, "Periodo "+mes+"-"+periodo+" esta abierto y se procede a cerrar. " + e.Message);

                // Obtenemos todos los movimientos del periodo
                List<Movimiento> Movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).ToList();

                // Recorremos lista y vamos cerrando los movimientos
                foreach (Movimiento movimiento in Movimientos)
                {
                    movimiento.Cerrado = "S";
                    if (ModelState.IsValid)
                    {
                        db.Entry(movimiento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        utils.erroresState(ModelState);
                    }
                }

                Periodo Periodo = new Periodo();
                Periodo.Ano = periodo;
                Periodo.Mes = mes;

                if (ModelState.IsValid)
                {
                    db.Periodo.Add(Periodo);
                    db.SaveChanges();
                }

                Session.Add("Periodo", periodo_siguiente);
                Session.Add("Mes", mes_siguiente);
                Session.Add("InformacionPie", ((Proyecto)Session["Proyecto"]).NombreLista + " (" + ((CuentaCorriente)Session["CuentaCorriente"]).NumeroLista + ") | " + ((Persona)Session["Persona"]).NombreCompleto + " | " + mes_siguiente + "-" + periodo_siguiente);
            }

            return CierreMes();
        }

        //
        // GET: /Procesos/CierreMes/
        public ActionResult CierreMes()
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];

            // Si el periodo actual es diciembre, cambiamos año.
            if (mes == 12)
            {
                Session.Add("mes_siguiente", 1);
                Session.Add("periodo_siguiente", periodo + 1);
            }
            else
            {
                Session.Add("mes_siguiente", mes + 1);
                Session.Add("periodo_siguiente", periodo);
            }
            return View();
        }
    }
}
