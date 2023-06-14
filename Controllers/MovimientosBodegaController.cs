using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Data;
using System.Web.Script.Serialization;
using System.Data.Entity;

namespace SAG2.Controllers
{
    public class MovimientosBodegaController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        
        //
        // GET: /MovimientosBodega/

        public ActionResult Index()
        {
            ViewBag.DocumentoID = new SelectList(db.Documento.OrderBy(a => a.ID), "ID", "NombreLista");
            ViewBag.ArticuloID = new SelectList(db.Articulo.OrderBy(a => a.Nombre), "ID", "NombreLista");
            ViewBag.periodo = Session["Periodo"].ToString();
            ViewBag.mes = Session["Mes"].ToString();

            return View();
        }

        public ActionResult MovimientosArticulos(int articuloID)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Bodega bodega = db.Bodega.Where(bb => bb.ProyectoID == Proyecto.ID).Where(bb => bb.ArticuloID == articuloID).Take(1).Single();
            Articulo articulo = db.Articulo.Where(a => a.ID == articuloID).Take(1).Single();
            
            ViewBag.saldo = bodega.Saldo;
            ViewBag.nombre = articulo.NombreLista;

            return View(db.MovimientoBodega.Where(b => b.ArticuloID == articuloID).Where(b => b.ProyectoID == Proyecto.ID).OrderByDescending(b => b.ID).ToList());
        }

        public ActionResult ListarMovimientosPeriodo(int mes = 0, int periodo = 0, int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            ViewBag.ListadoProyecto = db.Proyecto.ToList();
            if (periodo == 0)
            {
                periodo = (int)Session["Periodo"];
                mes = (int)Session["Mes"];
            }
            int xPeriodoinicio = anioInicio;
            ViewBag.Periodo = periodo;
            ViewBag.PeriodoInicio = anioInicio;
            ViewBag.Mes = mes;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.nombreProyecto = Proyecto.NombreLista;
            ViewBag.CodSename = Proyecto.CodSename;
            int PrId = Proyecto.ID;
            List<MovimientosBodega> movBodega = new List<MovimientosBodega>();
            if (anioInicio == aniohasta){
                movBodega = db.MovimientoBodega.Where(d => d.Mes >= mesInicio && d.Mes <= mesHasta && d.Periodo == anioInicio && d.ProyectoID == PrId).ToList();
            }
            else
            {
                while (anioInicio < aniohasta)
                {

                    if (xPeriodoinicio == anioInicio)
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio && d.ProyectoID == PrId).ToList())
                        {
                            movBodega.Add(item);
                        }

                    }
                    else
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => d.Periodo == anioInicio && d.ProyectoID == PrId).ToList())
                        {
                            movBodega.Add(item);
                        }
                    }
                    anioInicio++;
                }
                foreach (var item in db.MovimientoBodega.Where(d => d.Mes <= mesHasta && d.Periodo == aniohasta && d.ProyectoID == PrId).ToList())
                {
                    movBodega.Add(item);
                }
              
            }

            
            ViewBag.MesInicio = mesInicio;
            ViewBag.PeriodoHasta = aniohasta;
            ViewBag.MesHasta = mesHasta;
            return View(movBodega.OrderBy(b => b.Fecha).ToList());


        }
        public ActionResult ListarMovimientosMes(int mes = 0 , int periodo = 0, int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            ViewBag.ListadoProyecto = db.Proyecto.ToList();
            if (periodo == 0)
            {
                periodo = (int)Session["Periodo"];
                mes = (int)Session["Mes"];
            }
            ViewBag.Periodo = periodo;
            ViewBag.Mes = mes;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.nombreProyecto = Proyecto.NombreLista;
            ViewBag.CodSename = Proyecto.CodSename;
            var movimientosBodega = db.MovimientoBodega.Where(d => d.Mes == 0).ToList();
            int aux = 0;

            if (anioInicio != aniohasta)
            {
                while (anioInicio < aniohasta)
                {
                    if (aux == 0)
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            movimientosBodega.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => (d.Mes >= 1 && d.Mes <= 12) && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            movimientosBodega.Add(item);
                        }
                    }
                    aux++;
                    anioInicio++;
                }
                foreach (var item in db.MovimientoBodega.Where(d => (d.Mes >= 1 && d.Mes <= mesHasta) && d.Periodo == aniohasta && d.ProyectoID == Proyecto.ID).ToList())
                {
                    movimientosBodega.Add(item);
                }
                return View(movimientosBodega.ToList());
            }

                return View(db.MovimientoBodega.Where(b => (b.Periodo == periodo) && (b.Mes == mes) && (b.ProyectoID == Proyecto.ID)).OrderByDescending(b => b.ArticuloID).ToList());
            

        }
        public ActionResult ListarMovimientosCategoria(int mes = 0, int periodo = 0)
        {
            ViewBag.ListadoProyecto = db.Proyecto.ToList();
            if (periodo == 0)
            {
                periodo = (int)Session["Periodo"];
                mes = (int)Session["Mes"];
            }
            List<Bodega> dp = new List<Bodega>();

            ViewBag.Periodo = periodo;
            ViewBag.Mes = mes;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.nombreproyecto = Proyecto.NombreLista;
            dp = db.Bodega.Where(d => d.ProyectoID == Proyecto.ID).ToList();
            ViewBag.CodSename = Proyecto.CodSename;

            ViewBag.bartic = db.Bodega.Where(d => d.ProyectoID == Proyecto.ID).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).ToList();

            return View(db.MovimientoBodega.Where(b => (b.Periodo == periodo) && (b.Mes == mes) && (b.ProyectoID == Proyecto.ID)).OrderByDescending(b => b.ArticuloID).ToList());
        }
        public ActionResult ListaMovimientoCategoriaEX(int mes = 0, int periodo = 0)
        {
            ViewBag.ListadoProyecto = db.Proyecto.ToList();
            if (periodo == 0)
            {
                periodo = (int)Session["Periodo"];
                mes = (int)Session["Mes"];
            }
            List<Bodega> dp = new List<Bodega>();

            ViewBag.Periodo = periodo;
            ViewBag.Mes = mes;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.nombreproyecto = Proyecto.NombreLista;
            ViewBag.CodSename = Proyecto.CodSename;
            dp = db.Bodega.Where(d => d.ProyectoID == Proyecto.ID).ToList();

            ViewBag.bartic = db.Bodega.Where(d => d.ProyectoID == Proyecto.ID).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).ToList();

            return View(db.MovimientoBodega.Where(b => (b.Periodo == periodo) && (b.Mes == mes) && (b.ProyectoID == Proyecto.ID)).OrderByDescending(b => b.ArticuloID).ToList());
        }


        public ActionResult ListarMovimientos(int tdoc)
        {

            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (tdoc == 1)
            {
                return View(db.MovimientoBodega.Where(b => b.Periodo == periodo).Where(b => b.Mes == mes).Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.Tdoc == 1).OrderByDescending(b => b.ID).ToList());
            }
            else
            {
                if (tdoc == 2)
                {
                    return View(db.MovimientoBodega.Where(b => b.Periodo == periodo).Where(b => b.Mes == mes).Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.Salida > 0).OrderByDescending(b => b.ID).ToList());

                }
                else
                {
                    return View(db.MovimientoBodega.Where(b => b.Periodo == periodo).Where(b => b.Mes == mes).Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.Tdoc == 3).OrderByDescending(b => b.ID).ToList());
                }
            }
        }
        public ActionResult ListarEntradas(int? periodo, int? mes)
        {
            if (periodo == null)
            {
                periodo = (int)Session["Periodo"];
                mes = (int)Session["Mes"];
            }
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            return View(db.MovimientoBodega.Where(b => b.Periodo == periodo).Where(b => b.Mes == mes).Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.Tdoc  == 1).OrderByDescending(b => b.ID).ToList());       
        }
        public ActionResult ListarSalidas()
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            return View(db.MovimientoBodega.Where(b => b.Periodo == periodo).Where(b => b.Mes == mes).Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.Tdoc ==  2).OrderByDescending(b => b.ID).ToList());
            
        }
        public ActionResult ListarDonaciones()
        {
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            return View(db.MovimientoBodega.Where(b => b.Periodo == periodo).Where(b => b.Mes == mes).Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.Tdoc == 3).OrderByDescending(b => b.ID).ToList());

        }

        public ActionResult Imprimir(int Periodo = 0, int Mes = 0, int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            if (Periodo == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.nombreProyecto = Proyecto.NombreLista;
            ViewBag.CodSename = Proyecto.CodSename;
            return View(db.MovimientoBodega.Where(b => (b.Periodo == Periodo) && (b.Mes == Mes) && (b.ProyectoID == Proyecto.ID)).OrderByDescending(b => b.ArticuloID).ToList());
        }

        public ActionResult Excel(int Periodo = 0, int Mes = 0, int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            if (Periodo == 0 || Mes == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
            ViewBag.ListadoProyecto = db.Proyecto.ToList();   
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.NombreProyecto = Proyecto.NombreLista;
            ViewBag.CodSename = Proyecto.CodSename;
            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;
            var movimientosBodega = db.MovimientoBodega.Where(d => d.Mes == 0).ToList();
            int aux = 0;
            if (anioInicio != aniohasta)
            {
                while (anioInicio < aniohasta)
                {
                    if (aux == 0)
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            movimientosBodega.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => (d.Mes >= 1 && d.Mes <= 12) && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            movimientosBodega.Add(item);
                        }
                    }
                    aux++;
                    anioInicio++;
                }
                foreach (var item in db.MovimientoBodega.Where(d => (d.Mes >= 1 && d.Mes <= mesHasta) && d.Periodo == aniohasta && d.ProyectoID == Proyecto.ID).ToList())
                {
                    movimientosBodega.Add(item);
                }
                return View(movimientosBodega.ToList());
            }

            return View(db.MovimientoBodega.Where(b => b.Periodo == Periodo).Where(b => b.Mes == Mes).Where(b => b.ProyectoID == Proyecto.ID).OrderByDescending(b => b.ID).ToList());
        }

        public ActionResult ExcelPeriodo(int Periodo = 0, int Mes = 0, int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            if (Periodo == 0)
            {
                Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
            ViewBag.Periodo = Periodo;
            ViewBag.PeriodoInicio = anioInicio;
            int xPeriodoinicio  = anioInicio;
            ViewBag.Mes = Mes;
            ViewBag.ListadoProyecto = db.Proyecto.ToList();   
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.nombreProyecto = Proyecto.NombreLista;
            ViewBag.CodSename = Proyecto.CodSename;
            var movimientosBodega = db.MovimientoBodega.Where(d => d.Mes == 0).ToList();
            if (anioInicio == aniohasta)
            {
                movimientosBodega = db.MovimientoBodega.Where(d => d.Mes >= mesInicio && d.Mes <= mesHasta && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList();
            }
            else
            {
                while (anioInicio < aniohasta)
                {
                    if (xPeriodoinicio == anioInicio)
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            movimientosBodega.Add(item);
                        }

                    }
                    else {
                        foreach (var item in db.MovimientoBodega.Where(d =>  d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            movimientosBodega.Add(item);
                        }
                    
                    }

                    anioInicio++;
                }
                foreach (var item in db.MovimientoBodega.Where(d =>  d.Mes <= mesHasta && d.Periodo == aniohasta && d.ProyectoID == Proyecto.ID).ToList())
                {
                    movimientosBodega.Add(item);
                }

            }


            ViewBag.MesInicio = mesInicio;
            ViewBag.PeriodoHasta = aniohasta;
            ViewBag.MesHasta = mesHasta;
            return View(movimientosBodega.OrderBy(b => b.Fecha).ToList());
        }
        
        public ActionResult MovimientoDonacion()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            ViewBag.ArticuloID = new SelectList(db.Articulo.OrderBy(a => a.Nombre), "ID", "NombreLista");
            ViewBag.CategoriaID = new SelectList(db.Categoria, "ID", "nombre");

            return View();
        }
        [HttpPost]
        public ActionResult MovimientoDonacion(MovimientosBodega movimientosbodega)
        {
            // int MovimientosBodegaID = ;
            if (movimientosbodega.ID == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                ViewBag.DocumentoID = new SelectList(db.Documento.OrderBy(a => a.ID), "ID", "NombreLista");
                //  ViewBag.ArticuloID = new SelectList(db.Articulo.OrderBy(a => a.Nombre), "ID", "NombreLista");
                movimientosbodega.ProyectoID = Proyecto.ID;
                movimientosbodega.Periodo = (int)Session["Periodo"];
                movimientosbodega.Mes = (int)Session["Mes"];
                movimientosbodega.Fecha = DateTime.Now;
                movimientosbodega.Tdoc = 3;

                int ArticuloID = movimientosbodega.ArticuloID;
                int Cantidad = (int)movimientosbodega.Entrada;
                movimientosbodega.Salida = 0;
                //movimientosbodega.DetalleEgresoID = 7;
                db.MovimientoBodega.Add(movimientosbodega);
                db.SaveChanges();
                saldoBodega((int)Session["Periodo"], (int)Session["Mes"], Proyecto.ID, ArticuloID, Cantidad, 0);
                TempData["Message"] = "Grabado con exito ";//+ cargo.Nombre;
      
            }
            else
            {
                TempData["Message"] = "codigo para modificar entrada ";//+ cargo.Nombre;
            }
            return RedirectToAction("MovimientoDonacion");
        }


        public ActionResult MovimientoSalida()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Periodo = (int)Session["Periodo"];
            ViewBag.Mes = (int)Session["Mes"];

           ViewBag.ArticuloID = new SelectList(db.Articulo.OrderBy(a => a.Nombre), "ID", "NombreLista");
           ViewBag.CategoriaID = new SelectList(db.Categoria, "ID", "nombre");
            //if (ViewBag.Fecha == "")
            //{
                ViewBag.Fecha = DateTime.Now.ToString("yyyy/MM/dd");
            //}
            
   
            return View();
        }
        [HttpPost]
        public ActionResult MovimientoSalida(MovimientosBodega movimientosbodega)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int ArticuloID = movimientosbodega.ArticuloID;
            try
            {
                ViewBag.ArticuloID = new SelectList(db.Articulo.OrderBy(a => a.Nombre), "ID", "NombreLista");
            }
            catch (Exception)
            {
                throw;
            }
            int Cantidad = movimientosbodega.Salida;
            int Periodo = movimientosbodega.Periodo;
            int mes = movimientosbodega.Mes;
            DateTime fecha = movimientosbodega.Fecha;

            Bodega bodega = db.Bodega.Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.ArticuloID == ArticuloID).Where(b => b.Periodo == Periodo).Where(b => b.Mes == mes).Take(1).Single();

            if (bodega.Saldo >= Cantidad)
            {
                int PeriodoActual = (int)Session["Periodo"];
                int MesActual = (int)Session["Mes"];
                if (MesActual != mes)
                {
                    Bodega bodegaActual = db.Bodega.Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.ArticuloID == ArticuloID).Where(b => b.Periodo == PeriodoActual).Where(b => b.Mes == MesActual).Take(1).Single();

                    if (bodegaActual.Saldo >= Cantidad)
                    {
                       // movimientosbodega.Tdoc = 2;
                        movimientosbodega.ProyectoID = Proyecto.ID;
                        movimientosbodega.Fecha = fecha;
                        movimientosbodega.Entrada = 0;
                        db.MovimientoBodega.Add(movimientosbodega);
                        db.SaveChanges();
                        saldoBodega(Periodo, mes, Proyecto.ID, ArticuloID, 0, Cantidad);
                        TempData["Message"] = "Grabado con exito ";//+ cargo.Nombre;
                        return RedirectToAction("MovimientoSalida");
                    }
                    else
                    {
                        TempData["Message"] = "No hay saldo disponible en Bodega ";//+ cargo.Nombre;
                        return RedirectToAction("MovimientoSalida");
                    }
                }
                else
                {
                  //  movimientosbodega.Tdoc = 2;
                    movimientosbodega.ProyectoID = Proyecto.ID;
                    movimientosbodega.Fecha = fecha;//DateTime.Now;
                    movimientosbodega.Entrada = 0;
                    db.MovimientoBodega.Add(movimientosbodega);
                    db.SaveChanges();
                    saldoBodega(Periodo, mes, Proyecto.ID, ArticuloID, 0, Cantidad);
                    TempData["Message"] = "Grabado con exito ";//+ cargo.Nombre;
                    return RedirectToAction("MovimientoSalida");
                }
         }
          else
            {
                TempData["Message"] = "No hay saldo disponible en Bodega ";//+ cargo.Nombre;
                return RedirectToAction("MovimientoSalida");
            }

        }
        
        public ActionResult MovimientoEntrada()
            {
             //   ViewBag.DocumentoID = new SelectList(db.Documento.OrderBy(a => a.ID), "ID", "NombreLista");
                ViewBag.Periodo = (int)Session["Periodo"];
                ViewBag.Mes = (int)Session["Mes"];
                ViewBag.ArticuloID = new SelectList(db.Articulo.OrderBy(a => a.Nombre), "ID", "NombreLista");
                return View();
        }
        [HttpPost]
        public ActionResult MovimientoEntrada(MovimientosBodega movimientosbodega)
        {


           // int MovimientosBodegaID = ;
            if (movimientosbodega.ID ==  0)
            {
                int saldo = 0;
              /*  movimientosbodega.Periodo = (int)Session["Periodo"];
                movimientosbodega.Mes = (int)Session["Mes"];*/
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                try
                {
                    Bodega bodega = db.Bodega.Where(b => b.ProyectoID == Proyecto.ID).Where(b => b.ArticuloID == movimientosbodega.ArticuloID).Where(b => b.Periodo == movimientosbodega.Periodo).Where(b => b.Mes == movimientosbodega.Mes).Take(1).Single();
                    if (bodega.Periodo == movimientosbodega.Periodo && bodega.Mes == movimientosbodega.Mes)
                    {

                        saldo = bodega.Saldo;

                    }
                }
                catch (Exception)
                {
                    saldo = 0;
                }

            


                ViewBag.DocumentoID = new SelectList(db.Documento.OrderBy(a => a.ID), "ID", "NombreLista");
                
                movimientosbodega.ProyectoID = Proyecto.ID;
               
          
                int ArticuloID = movimientosbodega.ArticuloID;
                int Cantidad = (int)movimientosbodega.Entrada;
                
                  
                int detallegreso = (int)movimientosbodega.DetalleEgresoID;

                MovimientosBodega Registro = new MovimientosBodega();

                Registro.ArticuloID = ArticuloID;
                Registro.DetalleEgresoID = detallegreso;
                Registro.Entrada =  movimientosbodega.Entrada;
                Registro.Fecha = DateTime.Now;
                Registro.Tdoc = 1;
                Registro.Salida = 0;
                Registro.Saldo = saldo + Cantidad;
                Registro.ProyectoID = Proyecto.ID;               
                Registro.Observaciones = movimientosbodega.Observaciones;
                Registro.Periodo = movimientosbodega.Periodo;
                Registro.Mes = movimientosbodega.Mes;


                db.MovimientoBodega.Add(Registro);
                db.SaveChanges();
                //saldoBodega((int)Session["Periodo"], (int)Session["Mes"], Proyecto.ID, ArticuloID, Cantidad, 0);
                saldoBodega(movimientosbodega.Periodo, movimientosbodega.Mes , Proyecto.ID, ArticuloID, Cantidad, 0);
                TempData["Message"] = "Grabado con exito ";//+ cargo.Nombre;
                ViewBag.detallegreso = detallegreso;
                ViewBag.NroDocumento = Request.Form["NroDocumento"].ToString();
          
               

            }
            else
            {
                TempData["Message"] = "codigo para modificar entrada ";//+ cargo.Nombre;
            }
            return RedirectToAction("MovimientoEntrada");
        }

        public ActionResult MovientoEntradaEdit(int id)
        {
            MovimientosBodega Entrada = db.MovimientoBodega.Find(id);
            ViewBag.Articulos = db.Articulo.Where(a => a.CategoriaID == Entrada.Articulo.CategoriaID).ToList();   
            return View(Entrada);
        }

        [HttpPost]
        public ActionResult MovientoEntradaEdit(MovimientosBodega movimientosbodega)
        {
            if (ModelState.IsValid)
            {
                int paso = 0;
                MovimientosBodega Entrada = db.MovimientoBodega.Find(movimientosbodega.ID);
                int saldorevisar = 0;
                Bodega bodega = db.Bodega.Where(b => b.ProyectoID == Entrada.ProyectoID).Where(b => b.ArticuloID == Entrada.ArticuloID).Where(b => b.Periodo == Entrada.Periodo).Where(b => b.Mes == Entrada.Mes).Take(1).Single();
                if (bodega.Periodo == Entrada.Periodo && bodega.Mes == Entrada.Mes)
                {
                    saldorevisar = bodega.Saldo;
                }
                if (Entrada.ArticuloID == movimientosbodega.ArticuloID)
                {
                    saldorevisar = saldorevisar - Entrada.Entrada;
                    saldorevisar = saldorevisar + movimientosbodega.Entrada;
                }
                else
                {
                    saldorevisar = saldorevisar - Entrada.Entrada;
                }

                if (saldorevisar >= 0)
                {
                    paso = 1;
                }
                else
                {
                    paso = 2;
                }
                if (paso == 1)
                {
                    movimientosbodega.Tdoc = 1;
                    movimientosbodega.ProyectoID = Entrada.ProyectoID ;
                    movimientosbodega.Mes = Entrada.Mes;
                    movimientosbodega.Periodo = Entrada.Periodo;   
                    movimientosbodega.Fecha = DateTime.Now;
                    movimientosbodega.Salida = 0;
                    movimientosbodega.Saldo = saldorevisar;
                    if (Entrada.ArticuloID == movimientosbodega.ArticuloID)
                    {
                        bodega.Entrada = bodega.Entrada - Entrada.Entrada;
                        bodega.Entrada = bodega.Entrada + movimientosbodega.Entrada;
                        bodega.Saldo = saldorevisar;
                        db.Entry(bodega).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        bodega.Entrada = bodega.Entrada - Entrada.Entrada;
                        bodega.Saldo = saldorevisar;
                        db.Entry(bodega).State = EntityState.Modified;
                        db.SaveChanges();
                        saldoBodega((int)Session["Periodo"], (int)Session["Mes"], Entrada.ProyectoID, movimientosbodega.ArticuloID ,  movimientosbodega.Entrada , 0);


                    }

                    var attachedEntry = db.Entry(Entrada);
                    attachedEntry.CurrentValues.SetValues(movimientosbodega);
                    db.SaveChanges();
                    
                    TempData["Message"] = "Creado con exito ";
                }
                else
                {
                    TempData["Message"] = "Problemas con Saldo Bodega, no se puede modificar registro ";
                }

                return RedirectToAction("MovientoEntradaEdit");
            }
           // ViewBag.UnidadMedidaID = new SelectList(db.UnidadMedida, "ID", "Descripcion", articulo.UnidadMedidaID);
            return RedirectToAction("MovimientoEntrada");
        }
        public ActionResult MovientoSalidaEdit(int id)
        {
            MovimientosBodega Entrada = db.MovimientoBodega.Find(id);
            ViewBag.Articulos = db.Articulo.Where(a => a.CategoriaID == Entrada.Articulo.CategoriaID).ToList();
            return View(Entrada);
        }
        [HttpPost]
        public ActionResult MovientoSalidaEdit(MovimientosBodega movimientosbodega)
        {

            int paso = 0;
            //revisar pro
            MovimientosBodega Entrada = db.MovimientoBodega.Find(movimientosbodega.ID);
            int saldorevisar = 0;
            int saldorevisar2 = 0;
            Bodega bodega = db.Bodega.Where(b => b.ProyectoID == Entrada.ProyectoID).Where(b => b.ArticuloID == Entrada.ArticuloID).Where(b => b.Periodo == Entrada.Periodo).Where(b => b.Mes == Entrada.Mes).Take(1).Single();
            if (bodega.Periodo == Entrada.Periodo && bodega.Mes == Entrada.Mes)
            {
                saldorevisar = bodega.Saldo;
            }
            if (Entrada.ArticuloID == movimientosbodega.ArticuloID)
            {
                saldorevisar = saldorevisar + Entrada.Salida;
                saldorevisar = saldorevisar - movimientosbodega.Salida;
                saldorevisar2 = saldorevisar;
            }
            else
            {
                saldorevisar = saldorevisar + Entrada.Salida;
                try
                {
                    Bodega bodega2 = db.Bodega.Where(b => b.ProyectoID == Entrada.ProyectoID).Where(b => b.ArticuloID == movimientosbodega.ArticuloID).Where(b => b.Periodo == movimientosbodega.Periodo).Where(b => b.Mes == Entrada.Mes).Take(1).Single();
                    if (bodega.Periodo == movimientosbodega.Periodo && bodega.Mes == movimientosbodega.Mes)
                    {

                        saldorevisar2 = bodega.Saldo - movimientosbodega.Salida;

                    }
                }
                catch (Exception)
                {
                    saldorevisar2 = 0 - movimientosbodega.Salida;
                }
            }

            if (saldorevisar >= 0)
            {
                paso = 1;
            }
            else
            {
                paso = 2;
            }

            if (saldorevisar2 >= 0)
            {
                paso = 1;
            }
            else
            {
                paso = 2;
            }

            if (paso == 1)
            {
                movimientosbodega.Tdoc = 2;
                movimientosbodega.ProyectoID = Entrada.ProyectoID;
                movimientosbodega.Mes = Entrada.Mes;
                movimientosbodega.Periodo = Entrada.Periodo;
                movimientosbodega.Fecha = DateTime.Now;
                movimientosbodega.Entrada = 0;
                movimientosbodega.Saldo = saldorevisar;
                if (Entrada.ArticuloID == movimientosbodega.ArticuloID)
                {
                    bodega.Salida = bodega.Salida - Entrada.Salida;
                    bodega.Salida = bodega.Salida + movimientosbodega.Salida;
                    bodega.Saldo = saldorevisar;
                    db.Entry(bodega).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    bodega.Salida = bodega.Salida - Entrada.Salida;
                    bodega.Saldo = saldorevisar;
                    db.Entry(bodega).State = EntityState.Modified;
                    db.SaveChanges();
                    saldoBodega((int)Session["Periodo"], (int)Session["Mes"], Entrada.ProyectoID, movimientosbodega.ArticuloID, movimientosbodega.Entrada, 0);


                }

                var attachedEntry = db.Entry(Entrada);
                attachedEntry.CurrentValues.SetValues(movimientosbodega);
                db.SaveChanges();

                TempData["Message"] = "Creado con exito ";
            }
            else
            {
                TempData["Message"] = "Problemas con Saldo Bodega, no se puede modificar registro ";
            }



            // ViewBag.UnidadMedidaID = new SelectList(db.UnidadMedida, "ID", "Descripcion", articulo.UnidadMedidaID);
            return RedirectToAction("MovientoSalidaEdit");
            // ViewBag.UnidadMedidaID = new SelectList(db.UnidadMedida, "ID", "Descripcion", articulo.UnidadMedidaID);
           
           
        }
        public ActionResult MovientoDonacionEdit(int id)
        {
            MovimientosBodega Entrada = db.MovimientoBodega.Find(id);
            ViewBag.Articulos = db.Articulo.Where(a => a.CategoriaID == Entrada.Articulo.CategoriaID).ToList();
            return View(Entrada);
        }
        [HttpPost]
        public ActionResult MovientoDonacionEdit(MovimientosBodega movimientosbodega)
        {
            if (ModelState.IsValid)
            {
                int paso = 0;
                MovimientosBodega Entrada = db.MovimientoBodega.Find(movimientosbodega.ID);
                int saldorevisar = 0;
                Bodega bodega = db.Bodega.Where(b => b.ProyectoID == Entrada.ProyectoID).Where(b => b.ArticuloID == Entrada.ArticuloID).Where(b => b.Periodo == Entrada.Periodo).Where(b => b.Mes == Entrada.Mes).Take(1).Single();
                if (bodega.Periodo == Entrada.Periodo && bodega.Mes == Entrada.Mes)
                {
                    saldorevisar = bodega.Saldo;
                }
                if (Entrada.ArticuloID == movimientosbodega.ArticuloID)
                {
                    saldorevisar = saldorevisar - Entrada.Entrada;
                    saldorevisar = saldorevisar + movimientosbodega.Entrada;
                }
                else
                {
                    saldorevisar = saldorevisar - Entrada.Entrada;
                }

                if (saldorevisar >= 0)
                {
                    paso = 1;
                }
                else
                {
                    paso = 2;
                }
                if (paso == 1)
                {
                    movimientosbodega.Tdoc = 3;
                    movimientosbodega.ProyectoID = Entrada.ProyectoID;
                    movimientosbodega.Mes = Entrada.Mes;
                    movimientosbodega.Periodo = Entrada.Periodo;
                    movimientosbodega.Fecha = Entrada.Fecha;
                    movimientosbodega.Salida = 0;
                    movimientosbodega.Saldo = saldorevisar;
                    if (Entrada.ArticuloID == movimientosbodega.ArticuloID)
                    {
                        bodega.Entrada = bodega.Entrada - Entrada.Entrada;
                        bodega.Entrada = bodega.Entrada + movimientosbodega.Entrada;
                        bodega.Saldo = saldorevisar;
                        db.Entry(bodega).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        bodega.Entrada = bodega.Entrada - Entrada.Entrada;
                        bodega.Saldo = saldorevisar;
                        db.Entry(bodega).State = EntityState.Modified;
                        db.SaveChanges();
                        saldoBodega((int)Session["Periodo"], (int)Session["Mes"], Entrada.ProyectoID, movimientosbodega.ArticuloID, movimientosbodega.Entrada, 0);


                    }

                    var attachedEntry = db.Entry(Entrada);
                    attachedEntry.CurrentValues.SetValues(movimientosbodega);
                    db.SaveChanges();

                    TempData["Message"] = "Creado con exito ";
                }
                else
                {
                    TempData["Message"] = "Problemas con Saldo Bodega, no se puede modificar registro ";
                }

                return RedirectToAction("MovientoDonacionEdit");
            }
            // ViewBag.UnidadMedidaID = new SelectList(db.UnidadMedida, "ID", "Descripcion", articulo.UnidadMedidaID);
            return RedirectToAction("MovientoDonacionEdit");
        }
        [HttpGet, ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            try
            {
                MovimientosBodega mb = db.MovimientoBodega.Find(id);
                Bodega bodega = db.Bodega.Where(b => b.ProyectoID == mb.ProyectoID).Where(b => b.ArticuloID == mb.ArticuloID).Where(b => b.Periodo == mb.Periodo).Where(b => b.Mes == mb.Mes).Take(1).Single();
                
        
                if (mb.Tdoc == 2)
                {

                    bodega.Salida = bodega.Salida - mb.Salida;
                    bodega.Saldo = bodega.SaldoInicial + bodega.Entrada - bodega.Salida;
                    db.Entry(bodega).State = EntityState.Modified;
                    db.SaveChanges();
                    db.MovimientoBodega.Remove(mb);
                    db.SaveChanges();
                    return RedirectToAction("MovimientoSalida");
                }
                else
                {
                    int saldoR = 0;
                    saldoR = bodega.Saldo - mb.Entrada;
                    if (saldoR >= 0)
                    {
                        bodega.Entrada = bodega.Entrada - mb.Entrada;
                        bodega.Saldo = bodega.SaldoInicial + bodega.Entrada - bodega.Salida;
                        db.Entry(bodega).State = EntityState.Modified;
                        db.SaveChanges();
                        if (mb.Tdoc == 1)
                        {
                            db.MovimientoBodega.Remove(mb);
                            db.SaveChanges();
                            TempData["Message"] = "Eliminado con exito";
                            return RedirectToAction("MovimientoEntrada");
                        }
                        else
                        {
                            db.MovimientoBodega.Remove(mb);
                            db.SaveChanges();
                            TempData["Message"] = "Eliminado con exito";
                            return RedirectToAction("MovimientoDonacion");
                        }
                    }
                    else
                    {
                        if (mb.Tdoc == 1)
                        {
                            TempData["Message"] = "No se puede eliminar el articulo tiene movimiento en el Sistema de Bodega";
                            return RedirectToAction("MovimientoEntrada");
                        }
                        else
                        {
                            TempData["Message"] = "No se puede eliminar el articulo tiene movimiento en el Sistema de Bodega";
                            return RedirectToAction("MovimientoDonacion");
                        }
                    }
                }

               
               
            }
            catch (Exception)
            {
                TempData["Message"] = "No se puede eliminar el articulo tiene movimiento en el Sistema de Bodega";
                return RedirectToAction("Create");
            }
        }

        public void saldoBodega(int periodo,  int mes, int proyectoID, int articuloID, int entrada = 0, int salida = 0)
        {
            //try
            //{

                int bodegaC = db.Bodega.Where(b => b.ProyectoID == proyectoID).Where(b => b.ArticuloID == articuloID).Where(b => b.Mes == mes).Where(b => b.Periodo == periodo).Count();
                if (bodegaC > 0)
                {
                    int SaldoMensual = 0;
                    Bodega bodega = db.Bodega.Where(b => b.ProyectoID == proyectoID).Where(b => b.ArticuloID == articuloID).Where(b => b.Mes == mes).Where(b => b.Periodo == periodo).Take(1).Single();
                    if (bodega.Periodo == periodo && bodega.Mes == mes)
                    {

                        bodega.Entrada += entrada;
                        bodega.Salida += salida;
                        bodega.Saldo = bodega.SaldoInicial + bodega.Entrada - bodega.Salida;
                        db.Entry(bodega).State = EntityState.Modified;
                        db.SaveChanges();
                        SaldoMensual = bodega.SaldoInicial + bodega.Entrada - bodega.Salida;
                        // crear if para actualizar saldo de mes actual cuando se graba mes anterior
                        if ((int)Session["Mes"] != mes)
                        {
                            int PeriodoActual = (int)Session["Periodo"];
                            int MesActual = (int)Session["Mes"];

                            int bodegaActualCount = db.Bodega.Where(b => b.ProyectoID == proyectoID).Where(b => b.ArticuloID == articuloID).Where(b => b.Mes == MesActual).Where(b => b.Periodo == PeriodoActual).Count();

                            if (bodegaActualCount > 0)
                            {
                                Bodega bodegaActual = db.Bodega.Where(b => b.ProyectoID == proyectoID).Where(b => b.ArticuloID == articuloID).Where(b => b.Mes == MesActual).Where(b => b.Periodo == PeriodoActual).Take(1).Single();

                                bodegaActual.SaldoInicial = bodega.Saldo;
                                bodegaActual.Saldo = bodegaActual.SaldoInicial + bodegaActual.Entrada - bodegaActual.Salida;
                                db.Entry(bodegaActual).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {

                                Bodega BodegaActual = new Bodega();
                                BodegaActual.ArticuloID = articuloID;
                                BodegaActual.Entrada = 0;
                                BodegaActual.Salida = 0;
                                BodegaActual.SaldoInicial = SaldoMensual;
                                BodegaActual.Saldo = SaldoMensual;
                                BodegaActual.Periodo = PeriodoActual;
                                BodegaActual.Mes = MesActual;
                                BodegaActual.ProyectoID = proyectoID;
                                db.Bodega.Add(BodegaActual);
                                db.SaveChanges();

                            }

                        }
                    }
                }
                else {
                    Bodega Bodega = new Bodega();
                    Bodega.ArticuloID = articuloID;
                    Bodega.Entrada = entrada;
                    Bodega.Salida = 0;
                    Bodega.SaldoInicial = 0;
                    Bodega.Saldo = entrada;
                    Bodega.Periodo = periodo;
                    Bodega.Mes = mes;
                    Bodega.ProyectoID = proyectoID;
                    db.Bodega.Add(Bodega);
                    db.SaveChanges();
                    if ((int)Session["Mes"] != mes)
                    {
                        int PeriodoActual = (int)Session["Periodo"];
                        int MesActual = (int)Session["Mes"];

                        int bodegaActualCount = db.Bodega.Where(b => b.ProyectoID == proyectoID).Where(b => b.ArticuloID == articuloID).Where(b => b.Mes == MesActual).Where(b => b.Periodo == PeriodoActual).Count();

                        if (bodegaActualCount > 0)
                        {
                            Bodega bodegaActual = db.Bodega.Where(b => b.ProyectoID == proyectoID).Where(b => b.ArticuloID == articuloID).Where(b => b.Mes == MesActual).Where(b => b.Periodo == PeriodoActual).Take(1).Single();

                            bodegaActual.SaldoInicial = Bodega.Saldo;
                            bodegaActual.Saldo = bodegaActual.SaldoInicial + bodegaActual.Entrada - bodegaActual.Salida;
                            db.Entry(bodegaActual).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {

                            Bodega BodegaActual = new Bodega();
                            BodegaActual.ArticuloID = articuloID;
                            BodegaActual.Entrada = 0;
                            BodegaActual.Salida = 0;
                            BodegaActual.SaldoInicial = Bodega.Saldo;
                            BodegaActual.Saldo = Bodega.Saldo;
                            BodegaActual.Periodo = PeriodoActual;
                            BodegaActual.Mes = MesActual;
                            BodegaActual.ProyectoID = proyectoID;
                            db.Bodega.Add(BodegaActual);
                            db.SaveChanges();

                        }

                    }
                
                
                }
           // }
             //catch (Exception)
               
             //   {
             //       //int saldo = 0;

             //   }
            
         
        }

        public void modificarSaldoBodega(int periodo, int mes, int proyectoID, int articuloID, int original, int entrada = 0, int salida = 0)
        {
            try
            {
                Bodega bodega = db.Bodega.Where(b => b.Periodo == periodo).Where(b => b.Mes == mes).Where(b => b.ProyectoID == proyectoID).Where(b => b.ArticuloID == articuloID).Single();
               
                // Registro encontrado de periodo actual
                if (entrada != 0)
                {
                    bodega.Entrada = Convert.ToInt32(bodega.Entrada) - Convert.ToInt32(original) + Convert.ToInt32(entrada);

                }
                else
                {
                    bodega.Salida = Convert.ToInt32(bodega.Salida) - Convert.ToInt32(original) + Convert.ToInt32(salida);
                }

                bodega.Saldo = bodega.SaldoInicial + bodega.Entrada - bodega.Salida;
                db.Entry(bodega).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
               
            }
        }

        public ActionResult ListadoDetalles(int periodo = 0, int mes = 0)
        {
            BodegaCuenta bc = new BodegaCuenta();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (periodo == 0 ) {
             periodo = (int)Session["Periodo"];
             mes = (int)Session["Mes"];
            }
            var egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == Proyecto.ID && m.Egreso.Periodo == periodo && m.Egreso.Mes == mes ).Where(a => a.Temporal == null && a.Nulo == null && a.Cuenta.b == 1).OrderByDescending(d => d.ID).ToList();
           // var egresos = db.DetalleEgreso.Where( m => m.CuentaID in ( bc.CuentaID));
                //( from x in db.DetalleEgreso join y in db.BodegaCuenta on x.CuentaID equals y.CuentaID where x.Egreso.Periodo ==  periodo select x.ID);
                //db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == Proyecto.ID && m.Egreso.Periodo == periodo).Where(a => a.Temporal == null && a.Nulo == null ).Join(bc  //  OrderByDescending(d => d.ID).ToList();
            return View(egresos);
        }


        public string Articulos(int id)
        {
            var Articulos = ( from a in db.Articulo join c in db.BodegaCuenta  on a.Categoria.tipo  equals c.Tipo                          
                               where c.CuentaID == id
                               orderby a.Nombre
                              select new
                              {
                                  Value = a.ID,
                                  Text = a.Nombre
                              }).ToList();
            
                                        
            return new JavaScriptSerializer().Serialize(Articulos);
        }
        public ActionResult ImprimirArti(int periodo = 0, int mes = 0, int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            
                //  int periodo = 0;
                // int mes = 0;
                if (periodo == 0)
                {
                    periodo = (int)Session["Periodo"];
                    mes = (int)Session["Mes"];
                }
                List<Bodega> dp = new List<Bodega>();

                ViewBag.Periodo = periodo;
                ViewBag.Mes = mes;
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                ViewBag.nombreproyecto = Proyecto.NombreLista;
            ViewBag.CodSename = Proyecto.CodSename;
            dp = db.Bodega.Where(d => d.ProyectoID == Proyecto.ID).ToList();

                ViewBag.bartic = db.Bodega.Where(d => d.ProyectoID == Proyecto.ID).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).ToList();

                return View(db.MovimientoBodega.Where(b => (b.Periodo == periodo) && (b.Mes == mes) && (b.ProyectoID == Proyecto.ID)).OrderByDescending(b => b.ArticuloID).ToList());
            
        }
        public ActionResult ImprimirArtiFecha(int periodo = 0, int mes = 0, int mesInicio = 0, int anioInicio = 0, int mesHasta = 0, int aniohasta = 0)
        {
            //  int periodo = 0;
            // int mes = 0;
            if (periodo == 0)
            {
                periodo = (int)Session["Periodo"];
                mes = (int)Session["Mes"];
            }
            List<Bodega> dp = new List<Bodega>();

            ViewBag.Periodo = periodo;
            ViewBag.Mes = mes;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.nombreproyecto = Proyecto.NombreLista;
            ViewBag.CodSename = Proyecto.CodSename;
            var movimientosBodega = db.MovimientoBodega.Where(d => d.Mes == 0).ToList();
            int aux = 0;

            if (anioInicio != aniohasta)
            {
                while (anioInicio < aniohasta)
                {
                    if (aux == 0)
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => d.Mes >= mesInicio && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            movimientosBodega.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var item in db.MovimientoBodega.Where(d => (d.Mes >= 1 && d.Mes <= 12) && d.Periodo == anioInicio && d.ProyectoID == Proyecto.ID).ToList())
                        {
                            movimientosBodega.Add(item);
                        }
                    }
                    aux++;
                    anioInicio++;
                }
                foreach (var item in db.MovimientoBodega.Where(d => (d.Mes >= 1 && d.Mes <= mesHasta) && d.Periodo == aniohasta && d.ProyectoID == Proyecto.ID).ToList())
                {
                    movimientosBodega.Add(item);
                }
                return View(movimientosBodega.ToList());
            }

            return View(db.MovimientoBodega.Where(b => (b.Periodo == periodo) && (b.Mes == mes) && (b.ProyectoID == Proyecto.ID)).OrderByDescending(b => b.ArticuloID).ToList());
        }
    }
}
