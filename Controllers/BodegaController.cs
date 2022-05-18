using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.Entity;

namespace SAG2.Controllers
{
    public class BodegaController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();

        public ActionResult Saldos(int Periodo = 0, int Mes = 0)
        {
            if (Periodo == 0 || Mes == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
            int Ppid;
            Ppid = ((Proyecto)Session["Proyecto"]).ID;
            List<Bodega> saldos = db.Bodega.Where(s => s.Mes == Mes).Where(s => s.Periodo == Periodo).Where(s=> s.ProyectoID == Ppid).ToList();

            ViewBag.periodo = Periodo;
            ViewBag.mes = Mes;
            ViewBag.proyectoID = ((Proyecto)Session["Proyecto"]).ID;

            return View(saldos);
        }

        public ActionResult Imprimir(int Periodo = 0, int Mes = 0)
        {
            if (Periodo == 0 || Mes == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
            int Ppid;
            Ppid = ((Proyecto)Session["Proyecto"]).ID;
            List<Bodega> saldos = db.Bodega.Where(s => s.Mes == Mes).Where(s => s.Periodo == Periodo).Where(s => s.ProyectoID == Ppid).ToList();

            ViewBag.periodo = Periodo;
            ViewBag.mes = Mes;
            ViewBag.proyectoID = ((Proyecto)Session["Proyecto"]).ID;
            ViewBag.NombreProyecto = ((Proyecto)Session["Proyecto"]).NombreLista;
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            return View(saldos);
        }

        public ActionResult Excel(int Periodo = 0, int Mes = 0)
        {
            if (Periodo == 0 || Mes == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
            int Ppid;
            Ppid = ((Proyecto)Session["Proyecto"]).ID;
            List<Bodega> saldos = db.Bodega.Where(s => s.Mes == Mes).Where(s => s.Periodo == Periodo).Where(s => s.ProyectoID == Ppid).ToList();

            ViewBag.periodo = Periodo;
            ViewBag.mes = Mes;
            ViewBag.proyectoID = ((Proyecto)Session["Proyecto"]).ID;
            ViewBag.NombreProyecto = ((Proyecto)Session["Proyecto"]).NombreLista;
            ViewBag.CodSename = ((Proyecto)Session["Proyecto"]).CodSename;
            return View(saldos);
        }
        public string darsaldo(int articuloID, int periodo, int mes)
        {
           
          /*  int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];*/
           
            int proyectoID = ((Proyecto)Session["Proyecto"]).ID;

            var dar_saldo = (from b in db.Bodega
                             where b.Periodo == periodo && b.Mes == mes && b.ProyectoID == proyectoID && b.ArticuloID == articuloID
                           orderby b.ArticuloID
                           select new
                           {
                               id = b.ArticuloID,
                               saldos = b.Saldo
                           }).ToList();

         /*   Bodega bodega = db.Bodega.Where(b => b.Periodo == periodo).Where(b => b.Mes == mes).Where(b => b.ProyectoID == proyectoID).Where(b => b.ArticuloID == articuloID).Single();
            
            int dar_saldo = 0;
            if (bodega.Saldo != null)
            {
                dar_saldo = (int)bodega.Saldo;
            }*/

            //return View();
            return new JavaScriptSerializer().Serialize(dar_saldo);
        }
        public ActionResult TrasladoBodega()
        {
            int Ppid;
            int Mes = 0;
            int periodo = 0;
            Ppid = ((Proyecto)Session["Proyecto"]).ID;
  
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            if (periodo == 0 || Mes == 0)
            {
                periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
     
            List<Bodega> saldos = db.Bodega.Where(s => s.Mes == Mes).Where(s => s.Periodo == periodo).Where(s => s.ProyectoID == Ppid).ToList();

            ViewBag.periodo = periodo;
            ViewBag.mes = Mes;
       
            ViewBag.proyectoID = Ppid;
            return View(saldos);
            
        } 
        
        [HttpPost]
        public ActionResult TrasladoBodega(FormCollection  origen)
        {
            int Mes = 0;
            int periodo = 0;
            int Ppid;

            Ppid = ((Proyecto)Session["Proyecto"]).ID;

            
            if (periodo == 0 || Mes == 0)
            {
                periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
            int ProyectoDestino = int.Parse(Request.Form["destino"]);
            if (ProyectoDestino != -1)
            {
                // Rescatar Periodo y mes vigente del Proyecto de Destino
                int periodo_actual, mes_actual;
                Proyecto PrDestino = db.Proyecto.Where(d => d.ID == ProyectoDestino).FirstOrDefault();  
                try
                {
                    Periodo PeriodoVigente = (from p in db.Periodo
                                              where p.ProyectoID == ProyectoDestino
                                              orderby p.Ano descending, p.Mes descending
                                              select p).Take(1).Single();                 

                    if (PeriodoVigente.Mes == 12)
                    {
                        mes_actual = 1;
                        periodo_actual = PeriodoVigente.Ano + 1;
                    }
                    else
                    {
                        mes_actual = PeriodoVigente.Mes + 1;
                        periodo_actual = PeriodoVigente.Ano;
                    }
                }
                catch (Exception e)
                {
                    utils.Log(1, "No hay periodos cerramos en la base de datos, por lo tanto se selecciona fecha actual. " + e.Message);
                    if (PrDestino.MesInicio == null)
                    {
                        periodo_actual = DateTime.Now.Year;
                        mes_actual = DateTime.Now.Month;                        
                    }
                    else
                    {
                        periodo_actual = int.Parse (PrDestino.PeriodoInicio.ToString()) ;
                        mes_actual = int.Parse(PrDestino.MesInicio.ToString()) ;
                                             
                    }
                } 

                // Recorrer Saldo de bodega Movimiento bodega, actualizar saldo
                List<Bodega> saldosTraspasar = db.Bodega.Where(s => s.Mes == Mes).Where(s => s.Periodo == periodo).Where(s => s.ProyectoID == Ppid).Where( s => s.Saldo >0).ToList();
                foreach (var Datos in saldosTraspasar) {
                    Bodega BodegaDestino = db.Bodega.Where(d => d.ProyectoID == ProyectoDestino && d.Mes == mes_actual && d.Periodo == periodo_actual && d.ArticuloID == Datos.ArticuloID).FirstOrDefault();
                    int saldoArt = 0;
                    if (BodegaDestino == null)
                    {
                        Bodega Bodegaentrada = new Bodega();
                        Bodegaentrada.ArticuloID = Datos.ArticuloID;
                        Bodegaentrada.Entrada = Datos.Saldo;
                        Bodegaentrada.Mes = mes_actual;
                        Bodegaentrada.Periodo = periodo_actual;
                        Bodegaentrada.ProyectoID = ProyectoDestino;
                        Bodegaentrada.SaldoInicial = 0;
                        Bodegaentrada.Saldo = Datos.Saldo ;
                        Bodegaentrada.Salida = 0;
                        saldoArt = Datos.Saldo;
                        db.Bodega.Add(Bodegaentrada);

                    }
                    else {

                        BodegaDestino.Entrada = BodegaDestino.Entrada + Datos.Saldo;
                        BodegaDestino.Saldo = BodegaDestino.Saldo + Datos.Saldo;
                        saldoArt = BodegaDestino.Saldo;
                    }
                    db.SaveChanges(); 
                    MovimientosBodega mvTraspasar = new MovimientosBodega();
                    mvTraspasar.ProyectoID = ProyectoDestino;
                    mvTraspasar.ProyectoIDTraspaso = Ppid;
                    mvTraspasar.ArticuloID = Datos.ArticuloID ;
                    mvTraspasar.Entrada = Datos.Saldo;
                    mvTraspasar.DetalleEgresoID = 0;
                    mvTraspasar.Observaciones = "Traspaso";
                    mvTraspasar.Tdoc = 3;
                    mvTraspasar.NroDocumento = 0;
                    mvTraspasar.Periodo = periodo_actual;
                    mvTraspasar.Mes = mes_actual;
                    mvTraspasar.Saldo = saldoArt;
                    mvTraspasar.Fecha = DateTime.Now; 
                    db.MovimientoBodega.Add(mvTraspasar);

                    Bodega BodegaOrigen = db.Bodega.Where(d => d.ProyectoID == Ppid && d.Mes == Mes && d.Periodo == periodo && d.ArticuloID == Datos.ArticuloID).FirstOrDefault();

                    BodegaOrigen.Salida = BodegaOrigen.Salida + BodegaOrigen.Saldo;
                    BodegaOrigen.Saldo = 0;
                    db.SaveChanges(); 
  
                
                } 

            }

            List<Bodega> saldos = db.Bodega.Where(s => s.Mes == Mes).Where(s => s.Periodo == periodo).Where(s => s.ProyectoID == Ppid).ToList();

            ViewBag.periodo = periodo;
            ViewBag.mes = Mes;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.proyectoID = Ppid;
            return View(saldos);
            
        }

    }
}
