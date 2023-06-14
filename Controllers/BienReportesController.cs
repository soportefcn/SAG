using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Comun;
using SAG2.Classes;

namespace SAG2.Controllers
{
    public class BienReportesController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();

        public ActionResult Index(int? comboproyecto, DateTime? desde, DateTime? hasta)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());

            if (comboproyecto == null)
            {
                Proyecto pr = (Proyecto)Session["Proyecto"];
                comboproyecto = pr.ID;
            } 
            BienModInventarioVM model = new BienModInventarioVM();

            
            ViewBag.CantAltas = 0;
            ViewBag.CantBajas = 0;
            ViewBag.CantTraslados = 0;
            ViewBag.General = 0;

            ViewBag.comboproyecto = utils.ProyectoFiltro(filtro, int.Parse(comboproyecto.ToString()) );

            try
            {
                List<BienModInventario> listamodel = new List<BienModInventario>();
                if (desde != null)
                {
                    List<BienMovimiento> listaMov = db.BienMovimiento.Where(d => d.Bien.ProyectoID == comboproyecto).ToList();  


                    listamodel = new List<BienModInventario>();

                    foreach (var item in listaMov)
                    {

                        if (item.Bien.ProyectoID == comboproyecto && item.AutorizacionAuditor == 1 && item.FechaMovimiento.Date <= hasta && item.FechaMovimiento.Date >= desde)
                        {
                            if (item.EstadoID == 1)
                            {
                                ViewBag.CantAltas++;
                                ViewBag.General++;
                            }
                            if (item.EstadoID == 2)
                            {
                                ViewBag.CantBajas++;
                                ViewBag.General++;
                            }
                            if(item.EstadoID == 3)
                            {
                                ViewBag.CantTraslados++;
                                ViewBag.General++;
                            }

                            listamodel.Add(item.Bien);
                        }

                        if (item.Bien.ProyectoAnteriorID == comboproyecto && item.EstadoID == 3 && item.AutorizacionAuditor == 1 && item.FechaMovimiento.Date <= hasta && item.FechaMovimiento.Date >= desde)
                        {
                            ViewBag.CantTraslados++;                            
                            listamodel.Add(item.Bien);
                        }

                    }
                }

                model.lista = new List<BienModInventarioVM>();

                foreach(var item in listamodel)
                {
                    BienModInventarioVM obj = new BienModInventarioVM();
                    obj.ID = item.ID;

                    model.lista.Add(obj);
                }


                model.ID = (int)comboproyecto;
            }
            catch
            {
                if (comboproyecto == null)
                {

                }
                else
                {
                    TempData["Message"] = "Error en la operación.";
                }
            }

            model.Egreso = new DetalleEgreso();
            try
            {
                model.Egreso.FechaEmision = (DateTime)desde;
                model.Egreso.FechaVencimiento = (DateTime)hasta;
            }
            catch
            {

            }

            return View(model);

        }

        public ActionResult Details(int id)
        {
            BienModInventario model = db.BienModInventario.Find(id);

            var q = db.BienFamilia.ToList();
            List<SelectListItem> listfamilia = new List<SelectListItem>();

            foreach (var x in q)
            {
                listfamilia.Add(new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.ID.ToString()
                });
            }
            ViewBag.listadofamilia = listfamilia;

            var q2 = db.BienSubFamilia.ToList();
            List<SelectListItem> listsubfamilia = new List<SelectListItem>();
            foreach (var x in q2)
            {
                listsubfamilia.Add(new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.ID.ToString()
                });
            }
            ViewBag.listadosubfamilia = listsubfamilia;

            var q3 = db.Proyecto.ToList();
            List<SelectListItem> listproyecto = new List<SelectListItem>();
            foreach (var y in q3)
            {
                listproyecto.Add(new SelectListItem
                {
                    Text = y.Nombre,
                    Value = y.ID.ToString()
                });
            }
            ViewBag.listadoproyecto = listproyecto;

            var q4 = db.BienEstadoInventario.ToList();
            List<SelectListItem> listestado = new List<SelectListItem>();
            foreach (var z in q4)
            {
                listestado.Add(new SelectListItem
                {
                    Text = z.Nombre,
                    Value = z.ID.ToString()
                });
            }
            ViewBag.listadoestado = listestado;

            var q5 = db.BienCondicion.ToList();
            List<SelectListItem> listcondicion = new List<SelectListItem>();
            foreach (var b in q5)
            {
                listcondicion.Add(new SelectListItem
                {
                    Text = b.Nombre,
                    Value = b.ID.ToString()
                });
            }
            ViewBag.listacondicion = listcondicion;

            var q6 = db.BienProcedencia.ToList();
            List<SelectListItem> listorigen = new List<SelectListItem>();
            foreach (var c in q6)
            {
                listorigen.Add(new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.ID.ToString()
                });
            }
            ViewBag.listaorigen = listorigen;
            return View(model);
        }

        public ActionResult InformeGeneral(int id, DateTime  desde , DateTime  hasta)
        {
            BienModInventarioVM model = new BienModInventarioVM();
            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();

                model.ProyectoID = id;
                model.Proyecto = proy.Nombre;
                model.Desde = desde;
                model.Hasta = hasta;

                @ViewBag.Institucion = proy.Institucion;
                @ViewBag.CodigoSename = proy.CodSename;
                @ViewBag.CodigoCodeni = proy.NombreEstado;
                @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);

                List<BienMovimiento> listaTodos = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1).ToList();

                hasta.AddDays(1);

                foreach (var item in listaTodos)
                {
                    if (item.FechaMovimiento.Date >= desde.Date && item.FechaMovimiento.Date <= hasta.Date)
                    {
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);

                        int cantidadBaja = 0;
                        try
                        {
                            List<BienMovimiento> listBajas = db.BienMovimiento.Where(a => a.BienID == bien.ID && a.EstadoID == 2 && a.AutorizacionAuditor == 1).ToList();
                            foreach (var item2 in listBajas)
                            {
                                if (item2.FechaMovimiento.Date >= desde.Date && item2.FechaMovimiento.Date <= hasta.Date)
                                {
                                    cantidadBaja += item2.Cantidad;
                                }
                            }
                        }
                        catch { }

                        int cantidadTras = 0;
                        try
                        {
                            List<BienMovimiento> listTras = db.BienMovimiento.Where(a => a.BienID == bien.ID && a.EstadoID == 3).ToList();
                            foreach (var item3 in listTras)
                            {
                                if (item3.FechaMovimiento.Date >= desde.Date && item3.FechaMovimiento.Date <= hasta.Date)
                                {
                                    cantidadTras += item3.Cantidad;
                                }
                            }
                        }
                        catch
                        {

                        }

                        //if ((item.Cantidad - cantidadBaja - cantidadTras) > 0)
                        {
                            BienModInventarioVM obj = new BienModInventarioVM();
                            obj.Familia = bien.SubFamilia.Nombre;
                            obj.Estado = item.Estado.Nombre;
                            obj.Cantidad = item.Cantidad - cantidadBaja - cantidadTras;
                            obj.Detalle = item.Detalle;
                            obj.DescripcionBien = bien.DescripcionBien;
                            obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                            obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                            obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                            obj.CondicionText = item.Condicion.Nombre;
                            obj.Fecha = item.FechaMovimiento;
                            obj.ID = bien.ID;
                            obj.CondicionText = item.Condicion.Nombre;
                            var ubicacionID = int.Parse(bien.Ubicacion);
                            var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                            Dependencia model2 = w;
                            obj.Ubicacion = model2.Nombre;
                            obj.Usuario = item.Usuario.Persona.NombreCompleto;
                            obj.Proyecto = bien.Proyecto.Nombre;
                            obj.Procedencia = bien.Procedencia.Nombre;
                            obj.MontoInt = bien.Monto;
                            model.lista.Add(obj);
                            //}
                            if (item.FechaMovimiento > ViewBag.FechaActualizacion)
                            {
                                @ViewBag.FechaActualizacion = item.FechaMovimiento;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return View("InformeGeneral", model);
        }
        public ActionResult InformeGeneralInventario(int id )
        {
            BienModInventarioVM model = new BienModInventarioVM();
            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();

                model.ProyectoID = id;
                model.Proyecto = proy.Nombre;

                @ViewBag.Institucion = proy.Institucion;
                @ViewBag.CodigoSename = proy.CodSename;
                @ViewBag.CodigoCodeni = proy.NombreEstado;
                @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);

                List<BienMovimiento> listaTodos = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1 && a.Bien.ProyectoID == id ).ToList();

                var listaGroup = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1).GroupBy(z => new { z.Bien }).Select(c => c.Key).ToList();

                foreach (var item in listaTodos)
                {

                    BienModInventario bien = db.BienModInventario.Find(item.BienID);

                    //int cantidadBaja = 0;
                    //try
                    //{
                    //    List<BienMovimiento> listBajas = db.BienMovimiento.Where(a => a.BienID == bien.ID && a.EstadoID == 2 && a.AutorizacionAuditor == 1).ToList();
                    //    foreach (var item2 in listBajas)
                    //    {
                    //            cantidadBaja += item2.Cantidad;
                    //    }
                    //}
                    //catch { }

                    //int cantidadTras = 0;
                    //try
                    //{
                    //    List<BienMovimiento> listTras = db.BienMovimiento.Where(a => ((a.BienID == bien.ID /*|| a.bienAnteriorID != null*/) && a.EstadoID == 3) ).ToList();
                    //    foreach (var item3 in listTras)
                    //    {
                    //            cantidadTras += item3.Cantidad;
                    //    }
                    //}
                    //catch
                    //{

                    //}

                    //if ((item.Cantidad - cantidadBaja - cantidadTras) > 0)
                    //{
                    BienModInventarioVM obj = new BienModInventarioVM();
                    obj.Familia = bien.SubFamilia.Nombre;
                    obj.Estado = item.Estado.Nombre;
                    //if (item.EstadoID == 1)
                    //{
                    //    obj.Cantidad = item.Cantidad - cantidadBaja - cantidadTras;
                    //}
                    //if(item.EstadoID == 3)
                    //{
                    //    obj.Cantidad = item.Cantidad ;
                    //}

                    obj.Detalle = item.Detalle;
                    obj.Cantidad = item.Cantidad;
                    obj.DescripcionBien = bien.DescripcionBien;
                    obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                    obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                    obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                    obj.CondicionText = item.Condicion.Nombre;
                    obj.Fecha = item.FechaMovimiento;
                    obj.ID = bien.ID;
                    obj.CondicionText = item.Condicion.Nombre;
                    var ubicacionID = int.Parse(bien.Ubicacion);
                    var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                    Dependencia model2 = w;
                    obj.Ubicacion = model2.Nombre;
                    obj.ProyectoID = bien.ProyectoID;
                    obj.Usuario = item.Usuario.Persona.NombreCompleto;
                    obj.Proyecto = bien.Proyecto.Nombre;
                    obj.Procedencia = bien.Procedencia.Nombre;
                    obj.MontoInt = bien.Monto;
                    if (item.bienAnteriorID.HasValue)
                    {
                        obj.MovimientoBienID = item.bienAnteriorID.Value;
                    }

                    if (bien.ProyectoAnteriorID.HasValue)
                    {
                        obj.ProyectoAnteriorID = bien.ProyectoAnteriorID.Value;
                    }

                    obj.RutaArchivo = item.RutaArchivo;
                    model.lista.Add(obj);
                    //}
                    if (item.FechaMovimiento > ViewBag.FechaActualizacion)
                    {
                        @ViewBag.FechaActualizacion = item.FechaMovimiento;
                    }
                }
            }
            catch
            {

            }
            //return View("InformeGeneralPrin", model);
            return View("InformeGeneralInventario", model);
        }

        //public ActionResult InformeHojaInventarioEX(int id, DateTime desde, DateTime hasta, string dependencia)
        //{
        //    BienModInventarioVM model = new BienModInventarioVM();
        //    try
        //    {
        //        Proyecto proy = db.Proyecto.Find(id);
        //        model.ClaseProyecto = proy;
        //        model.lista = new List<BienModInventarioVM>();

        //        model.ProyectoID = id;
        //        model.Proyecto = proy.Nombre;
        //        model.Desde = desde;
        //        model.Hasta = hasta;
        //        model.Ubicacion = dependencia;
        //        @ViewBag.Institucion = proy.Institucion;
        //        @ViewBag.CodigoSename = proy.CodSename;
        //        @ViewBag.CodigoCodeni = proy.NombreEstado;
        //        @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);

        //        List<BienMovimiento> listaTodos = db.BienMovimiento.Where(a => a.EstadoID == 1 && a.AutorizacionAuditor == 1).ToList();

        //        hasta.AddDays(1);

        //        foreach (var item in listaTodos)
        //        {
        //            if (item.FechaMovimiento.Date >= desde.Date && item.FechaMovimiento.Date <= hasta.Date)
        //            {
        //                BienModInventario bien = db.BienModInventario.Find(item.BienID);

        //                int cantidadBaja = 0;
        //                try
        //                {
        //                    List<BienMovimiento> listBajas = db.BienMovimiento.Where(a => a.BienID == bien.ID && a.EstadoID == 2 && a.AutorizacionAuditor == 1).ToList();
        //                    foreach (var item2 in listBajas)
        //                    {
        //                        if (item2.FechaMovimiento.Date >= desde.Date && item2.FechaMovimiento.Date <= hasta.Date)
        //                        {
        //                            cantidadBaja += item2.Cantidad;
        //                        }
        //                    }
        //                }
        //                catch { }

        //                int cantidadTras = 0;

        //                try
        //                {
        //                    List<BienMovimiento> listTras = db.BienMovimiento.Where(a => a.BienID == bien.ID && a.EstadoID == 3).Where(p => p.NuevaUbicacion == dependencia).ToList();
        //                    foreach (var item3 in listTras)
        //                    {
        //                        if (item3.FechaMovimiento.Date >= desde.Date && item3.FechaMovimiento.Date <= hasta.Date)
        //                        {
        //                            cantidadTras += item3.Cantidad;
        //                        }
        //                    }
        //                }
        //                catch
        //                {

        //                }

        //                if ((item.Cantidad - cantidadBaja - cantidadTras) > 0)
        //                {
        //                    Dependencia dep = new Dependencia();
        //                    BienModInventarioVM obj = new BienModInventarioVM();
        //                    obj.Familia = bien.SubFamilia.Nombre;
        //                    obj.Estado = item.Estado.Nombre;
        //                    obj.Cantidad = item.Cantidad - cantidadBaja - cantidadTras;
        //                    obj.Detalle = item.Detalle;
        //                    obj.DescripcionBien = bien.DescripcionBien;
        //                    obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
        //                    obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
        //                    obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
        //                    obj.CondicionText = item.Condicion.Nombre;
        //                    obj.Fecha = item.FechaMovimiento;
        //                    obj.ID = bien.ID;
        //                    obj.CondicionText = item.Condicion.Nombre;
        //                    var ubicacionID = int.Parse(bien.Ubicacion);
        //                    var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
        //                    Dependencia model2 = w;
        //                    obj.Ubicacion = model2.Nombre;
        //                    //obj.Ubicacion = bien.Ubicacion;
        //                    obj.Usuario = item.Usuario.Persona.NombreCompleto;
        //                    obj.Proyecto = bien.Proyecto.Nombre;
        //                    obj.Procedencia = bien.Procedencia.Nombre;
        //                    obj.MontoInt = bien.Monto;
        //                    if (bien.Ubicacion == dependencia)
        //                    {
        //                        model.lista.Add(obj);
        //                    }
        //                }
        //                if (item.FechaMovimiento > ViewBag.FechaActualizacion)
        //                {
        //                    @ViewBag.FechaActualizacion = item.FechaMovimiento;
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return View("InformeHojaInventarioEX", model);
        //}

        public ActionResult InformeAltaInventario(int id, DateTime desde, DateTime hasta)
        {
            BienModInventarioVM model = new BienModInventarioVM();
            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                List<BienModInventario> lista = new List<BienModInventario>();
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();
                model.Proyecto = proy.Nombre;
                ViewBag.CodSename = proy.CodSename;
                ViewBag.CCosto = proy.NombreEstado;
            

                List<BienMovimiento> listAltas = db.BienMovimiento.Where(a => a.EstadoID == 1 && a.AutorizacionAuditor == 1 && a.Bien.ProyectoID == id ).ToList();
                foreach (var item in listAltas)
                {
                    if (item.FechaMovimiento.Date <= hasta.Date && item.FechaMovimiento.Date >= desde.Date)
                    {
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);

                        if (bien != null && bien.ProyectoID == id)
                        {
                            BienModInventarioVM obj = new BienModInventarioVM();
                            obj.Familia = bien.SubFamilia.Nombre;
                            obj.Detalle = item.Detalle;
                            obj.DescripcionBien = bien.DescripcionBien;
                            obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                            obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                            obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                            obj.Cantidad = item.Cantidad;
                            obj.CondicionText = item.Condicion.Nombre;
                            obj.Fecha = item.FechaMovimiento;
                            obj.ID = bien.ID;
                            obj.CondicionText = item.Condicion.Nombre;
                            var ubicacionID = int.Parse(bien.Ubicacion);
                            var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                            Dependencia model2 = w;
                            obj.Ubicacion = model2.Nombre;
                            obj.Usuario = item.Usuario.Persona.NombreCompleto;
                            obj.Proyecto = bien.Proyecto.Nombre;
                            obj.Procedencia = bien.Procedencia.Nombre;
                            obj.MontoInt = bien.Monto;
                            if (obj.Cantidad != 0)
                            {
                                model.lista.Add(obj);
                            }
                        }
                    }

                }
            }
            catch( Exception )
            {

            }
            return View(model);
        }


        public ActionResult InformeAlta(int id, DateTime desde, DateTime hasta)
        {
            BienModInventarioVM model = new BienModInventarioVM();
            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                List<BienModInventario> lista = new List<BienModInventario>();
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();
                model.Proyecto = proy.Nombre;
                ViewBag.CodSename = proy.CodSename;
                ViewBag.CCosto = proy.NombreEstado;
                model.ProyectoID = id;
                model.Desde = desde;
                model.Hasta = hasta;

                List<BienMovimiento> listAltas = db.BienMovimiento.Where(a => (a.EstadoID == 1 ) && a.AutorizacionAuditor == 1  && a.Bien.ProyectoID == id ).ToList();
                foreach (var item in listAltas)
                {
                    if (item.FechaMovimiento.Date <= hasta.Date && item.FechaMovimiento.Date >= desde.Date)
                    {
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);
                        //if (item.EstadoID == 3)
                        //{
                        //    bien = db.BienModInventario.Find(item.bienAnteriorID);
                        //}
                        if (bien != null && (bien.ProyectoID == id/* || bien.ProyectoAnteriorID == id*/))
                        {
                            BienModInventarioVM obj = new BienModInventarioVM();
                            obj.Familia = bien.SubFamilia.Nombre;
                            obj.Detalle = item.Detalle;
                            obj.DescripcionBien = bien.DescripcionBien;
                            obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                            obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                            obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                            obj.Cantidad = item.Cantidad;
                            obj.CondicionText = item.Condicion.Nombre;
                            obj.Fecha = item.FechaMovimiento;
                            obj.ID = bien.ID;
                            obj.CondicionText = item.Condicion.Nombre;
                            var ubicacionID = int.Parse(item.NuevaUbicacion);
                            var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                            Dependencia model2 = w;
                            obj.Ubicacion = model2.Nombre;
                            obj.Usuario = item.Usuario.Persona.NombreCompleto;
                            obj.Proyecto = bien.Proyecto.Nombre;
                            obj.Procedencia = bien.Procedencia.Nombre;
                            obj.MontoInt = bien.Monto;
                            if (obj.Cantidad != 0)
                            {
                                model.lista.Add(obj);
                            }
                        }
                    }

                }
            }
            catch (Exception )
            {

            }
            return View(model);
        }
        public ActionResult InformeBajaInventario(int id, DateTime desde, DateTime hasta)
        {
            BienModInventarioVM model = new BienModInventarioVM();
            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                List<BienModInventario> lista = new List<BienModInventario>();
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();
                model.Proyecto = proy.Nombre;
                List<BienMovimiento> listBajas = db.BienMovimiento.Where(a => a.EstadoID == 2 && a.AutorizacionAuditor == 1).ToList();


                foreach (var item in listBajas)
                {
                    if (item.FechaMovimiento.Date >= desde.Date && item.FechaMovimiento.Date <= hasta.Date)
                    {
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);
                        if (bien != null)
                        {
                            BienModInventarioVM obj = new BienModInventarioVM();
                            obj.Familia = bien.SubFamilia.Nombre;
                            obj.Cantidad = item.Cantidad;
                            obj.Detalle = item.Detalle;
                            obj.RutaArchivo = item.RutaArchivo;
                            obj.DescripcionBien = bien.DescripcionBien;
                            obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                            obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                            obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                            obj.Cantidad = item.Cantidad;
                            obj.CondicionText = item.Condicion.Nombre;
                            obj.Fecha = item.FechaMovimiento;
                            obj.ID = bien.ID;
                            obj.CondicionText = item.Condicion.Nombre;
                            var ubicacionID = int.Parse(bien.Ubicacion);
                            var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                            Dependencia model2 = w;
                            obj.Ubicacion = model2.Nombre;
                            obj.Usuario = item.Usuario.Persona.NombreCompleto;
                            obj.Proyecto = bien.Proyecto.Nombre;
                            obj.Procedencia = bien.Procedencia.Nombre;
                            model.lista.Add(obj);
                        }
                    }
                }
            }
            catch(Exception )
            {

            }
            return View(model);
        }

        public ActionResult InformeBaja(int id, DateTime desde, DateTime hasta)
        {
            BienModInventarioVM model = new BienModInventarioVM();
            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                List<BienModInventario> lista = new List<BienModInventario>();
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();
                model.Proyecto = proy.Nombre;
                model.ProyectoID = id;
                model.Desde = desde;
                model.Hasta = hasta;
                List<BienMovimiento> listBajas = db.BienMovimiento.Where(a => a.EstadoID == 2 && a.AutorizacionAuditor == 1).ToList();


                foreach (var item in listBajas)
                {
                    if (item.FechaMovimiento.Date >= desde.Date && item.FechaMovimiento.Date <= hasta.Date)
                    {
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);
                        if (bien != null)
                        {
                            BienModInventarioVM obj = new BienModInventarioVM();
                            obj.Familia = bien.SubFamilia.Nombre;
                            obj.Cantidad = item.Cantidad;
                            obj.Detalle = item.Detalle;
                            obj.DescripcionBien = bien.DescripcionBien;
                            obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                            obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                            obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                            obj.Cantidad = item.Cantidad;
                            obj.CondicionText = item.Condicion.Nombre;
                            obj.Fecha = item.FechaMovimiento;
                            obj.RutaArchivo = item.RutaArchivo;
                            obj.ID = bien.ID;
                            obj.CondicionText = item.Condicion.Nombre;
                            var ubicacionID = int.Parse(bien.Ubicacion);
                            var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                            Dependencia model2 = w;
                            obj.Ubicacion = model2.Nombre;
                            obj.Usuario = item.Usuario.Persona.NombreCompleto;
                            obj.Proyecto = bien.Proyecto.Nombre;
                            obj.Procedencia = bien.Procedencia.Nombre;
                            model.lista.Add(obj);
                        }
                    }
                }
            }
            catch (Exception )
            {

            }
            return View(model);
        }

        public ActionResult InformeTrasladoInventario(int id, DateTime desde, DateTime hasta)
        {
            BienModInventarioVM model = new BienModInventarioVM();
            
            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                @ViewBag.Institucion = proy.Institucion;
                @ViewBag.CodigoSename = proy.CodSename;
                @ViewBag.CodigoCodeni = proy.CodCodeni;
                @ViewBag.CCosto = proy.NombreEstado;
                @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();
                model.Proyecto = proy.Nombre;
                List<BienModInventario> lista = new List<BienModInventario>();
                List<BienMovimiento> listTraslado = db.BienMovimiento.Where(a => a.EstadoID == 3 && a.AutorizacionAuditor == 1 ).ToList();
                foreach (var item in listTraslado)
                {
                    if (item.FechaMovimiento.Date >= desde.Date && item.FechaMovimiento.Date <= hasta.Date)
                    {
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);

                        if (bien != null)
                        {
                            if(bien.ProyectoID == proy.ID)
                            {
                                BienModInventarioVM obj = new BienModInventarioVM();
                                obj.Familia = bien.SubFamilia.Nombre;
                                obj.Cantidad = item.Cantidad;
                                obj.Detalle = item.Detalle;
                                obj.DescripcionBien = bien.DescripcionBien;
                                obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                                obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                                obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                                obj.Cantidad = item.Cantidad;
                                obj.CondicionText = item.Condicion.Nombre;
                                obj.Fecha = item.FechaMovimiento;
                                obj.ID = bien.ID;
                                obj.CondicionText = item.Condicion.Nombre;
                                obj.Ubicacion = /*item.NuevaUbicacion + "-" + */bien.Proyecto.Nombre;
                                obj.Ubicacion2 = /*bien.Ubicacion + "-" +*/ bien.ProyectoAnterior.NombreEstado;
                                obj.Usuario = item.Usuario.Persona.NombreCompleto;
                                obj.Proyecto = bien.ProyectoAnterior.Nombre;
                                obj.Procedencia = bien.Procedencia.Nombre;
                                obj.RutaArchivo = item.RutaArchivo;

                                model.lista.Add(obj);

                            }

                            if (bien.ProyectoAnteriorID == proy.ID)
                            {
                                BienModInventarioVM obj = new BienModInventarioVM();
                                obj.Familia = bien.SubFamilia.Nombre;
                                obj.Cantidad = item.Cantidad;
                                obj.Detalle = item.Detalle;
                                obj.DescripcionBien = bien.DescripcionBien;
                                obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                                obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                                obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                                obj.Cantidad = item.Cantidad;
                                obj.CondicionText = item.Condicion.Nombre;
                                obj.Fecha = item.FechaMovimiento;
                                obj.ID = bien.ID;
                                obj.CondicionText = item.Condicion.Nombre;
                                obj.Ubicacion = item.NuevaUbicacion;
                                obj.Ubicacion2 = bien.Ubicacion + "-" + bien.Proyecto.NombreEstado;
                                obj.Usuario = item.Usuario.Persona.NombreCompleto;
                                obj.Proyecto = bien.ProyectoAnterior.Nombre;
                                obj.Procedencia = bien.Procedencia.Nombre;
                                obj.RutaArchivo = item.RutaArchivo;

                                model.lista.Add(obj);
                            }                            
                        }

                        if (item.FechaMovimiento > ViewBag.FechaActualizacion)
                        {
                            @ViewBag.FechaActualizacion = item.FechaMovimiento;
                        }
                    }
                }

            }
            catch
            {

            }
   
            return View(model);
        }

        [HttpPost]
        public JsonResult GetProyectoBien(int id)
        {
            //var q = db.BienModInventario.Where(x => x.ProyectoID == id).OrderBy(a => a.ID);
            var q = db.Dependencia.Where(z => z.ProyectoID == id);
            List<SelectListItem> listDependenciasProyecto = new List<SelectListItem>();
            listDependenciasProyecto.Add(new SelectListItem
            {
                Text = "Seleccione una dependencia",
                Value = "0"
            });

            foreach (var x in q)
            {
                listDependenciasProyecto.Add(new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.ID.ToString()
                });
            }
            return Json(new SelectList(listDependenciasProyecto, "Value", "Text"));
        }

        public ActionResult InformeTraslado(int id, DateTime desde, DateTime hasta)
        {
            BienModInventarioVM model = new BienModInventarioVM();

            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                @ViewBag.Institucion = proy.Institucion;
                @ViewBag.CodigoSename = proy.CodSename;
                @ViewBag.CodigoCodeni = proy.CodCodeni;
                @ViewBag.CCosto = proy.NombreEstado;
                @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();
                model.Proyecto = proy.Nombre;
                model.ProyectoID = id;
                model.Desde = desde;
                model.Hasta = hasta;
                List<BienModInventario> lista = new List<BienModInventario>();
                List<BienMovimiento> listTraslado = db.BienMovimiento.Where(a => a.EstadoID == 3 && a.AutorizacionAuditor == 1).ToList();
                List<Dependencia> ubicacion = db.Dependencia.ToList();
                foreach (var item in listTraslado)
                {
                    if (item.FechaMovimiento.Date >= desde.Date && item.FechaMovimiento.Date <= hasta.Date)
                    {
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);

                        if (bien != null)
                        {
                            if (bien.ProyectoID == proy.ID)
                            {
                                BienModInventarioVM obj = new BienModInventarioVM();
                                obj.Familia = bien.SubFamilia.Nombre;
                                obj.Cantidad = item.Cantidad;
                                obj.Detalle = item.Detalle;
                                obj.DescripcionBien = bien.DescripcionBien;
                                obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                                obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                                obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                                obj.Cantidad = item.Cantidad;
                                obj.CondicionText = item.Condicion.Nombre;
                                obj.Fecha = item.FechaMovimiento;
                                obj.ID = bien.ID;
                                obj.CondicionText = item.Condicion.Nombre;
                                obj.Ubicacion = /*ubicacion.Find(z => z.ID == int.Parse(item.NuevaUbicacion)).Nombre.ToString() + "-" +*/ bien.ProyectoAnterior.NombreEstado;
                                obj.Ubicacion2 = /*bien.Ubicacion + "-" + */bien.Proyecto.NombreEstado;
                                obj.Usuario = item.Usuario.Persona.NombreCompleto;
                                obj.Proyecto = bien.ProyectoAnterior.Nombre;
                                obj.Procedencia = bien.Procedencia.Nombre;
                                obj.RutaArchivo = item.RutaArchivo;

                                model.lista.Add(obj);

                            }

                            if (bien.ProyectoAnteriorID == proy.ID)
                            {
                                BienModInventarioVM obj = new BienModInventarioVM();
                                obj.Familia = bien.SubFamilia.Nombre;
                                obj.Cantidad = item.Cantidad;
                                obj.Detalle = item.Detalle;
                                obj.DescripcionBien = bien.DescripcionBien;
                                obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                                obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                                obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                                obj.Cantidad = item.Cantidad;
                                obj.CondicionText = item.Condicion.Nombre;
                                obj.Fecha = item.FechaMovimiento;
                                obj.ID = bien.ID;
                                obj.CondicionText = item.Condicion.Nombre;
                                obj.Ubicacion = /*ubicacion.Find(z => z.ID == int.Parse(item.NuevaUbicacion)).Nombre.ToString();*/bien.ProyectoAnterior.NombreEstado;
                                obj.Ubicacion2 = /*ubicacion.Find(z => z.ID == int.Parse(item.NuevaUbicacion)).Nombre.ToString() + "-" +*/ bien.Proyecto.NombreEstado;
                                obj.Usuario = item.Usuario.Persona.NombreCompleto;
                                obj.Proyecto = bien.ProyectoAnterior.Nombre;
                                obj.Procedencia = bien.Procedencia.Nombre;
                                obj.RutaArchivo = item.RutaArchivo;

                                model.lista.Add(obj);
                            }
                        }

                        if (item.FechaMovimiento > ViewBag.FechaActualizacion)
                        {
                            @ViewBag.FechaActualizacion = item.FechaMovimiento;
                        }
                    }
                }

            }
            catch
            {

            }

            return View(model);
        }


        public ActionResult HojaInventario(int? comboproyecto, string dependencia)
        {
            ViewBag.Informe = 1; 
            int filtro = int.Parse(Session["Filtro"].ToString());
            if (comboproyecto == null)
            {
                Proyecto pr = (Proyecto)Session["Proyecto"];
                comboproyecto = pr.ID;
            } 
            if (filtro == 1)
            {
                ViewBag.comboproyecto = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null), "ID", "NombreLista", comboproyecto);
            }
            else
            {
                ViewBag.comboproyecto = new SelectList(db.Proyecto.Where(p => p.Eliminado == null), "ID", "NombreLista", comboproyecto);
            }
            if (dependencia == null)
            {
                ViewBag.Informe = 0; 
            }
            BienModInventarioVM model = new BienModInventarioVM();


            ViewBag.CantAltas = 0;
            ViewBag.CantBajas = 0;
            ViewBag.CantTraslados = 0;
            ViewBag.General = 0;



          try
          {
                List<BienModInventario> listamodel = new List<BienModInventario>();
                if (comboproyecto != null)
                {
                    List<BienModInventario> listaMov = db.BienModInventario.Where(x => x.ProyectoID == comboproyecto).ToList();       
                    listamodel = new List<BienModInventario>();

                    foreach (var itemX in listaMov)
                    {
                        var item = db.BienMovimiento.Where(p => p.BienID.Equals(itemX.ID)).FirstOrDefault();


                        if (item.Bien.ProyectoID == comboproyecto && item.AutorizacionAuditor == 1) //&& item.FechaMovimiento.Date <= hasta && item.FechaMovimiento.Date >= desde && item.NuevaUbicacion == dependencia)
                        {
                            if (item.EstadoID == 1)
                            {
                                ViewBag.CantAltas++;
                                ViewBag.General++;
                            }
                            if (item.EstadoID == 2)
                            {
                                ViewBag.CantBajas++;
                                ViewBag.General++;
                            }
                            if (item.EstadoID == 3)
                            {
                                ViewBag.CantTraslados++;
                                ViewBag.General++;
                            }

                            listamodel.Add(item.Bien);
                        }

                        if (item.Bien.ProyectoAnteriorID == comboproyecto && item.EstadoID == 3 && item.AutorizacionAuditor == 1)//&& item.FechaMovimiento.Date <= hasta && item.FechaMovimiento.Date >= desde)
                        {
                            ViewBag.CantTraslados++;
                            listamodel.Add(item.Bien);
                        }

                    }
                }

                model.lista = new List<BienModInventarioVM>();

                foreach (var item in listamodel)
                {
                    BienModInventarioVM obj = new BienModInventarioVM();
                    obj.ID = item.ID;

                    model.lista.Add(obj);
                }



                model.ID = (int)comboproyecto;
                    }
                      catch
                      {
                          if (comboproyecto == null)
                          {

                          }
                          else
                          {
                              TempData["Message"] = "Error en la operación.";
                          }
                      }
               

            model.Ubicacion = dependencia;
            return View(model);

        }

        public ActionResult InformeHojaInventario(int comboproyecto, string dependencia)
        {
            BienModInventarioVM model = new BienModInventarioVM();
            try
            {
                if (dependencia != null)
                {
                    Proyecto proy = db.Proyecto.Find(comboproyecto);
                    model.ClaseProyecto = proy;
                    model.lista = new List<BienModInventarioVM>();

                    model.ProyectoID = comboproyecto;
                    model.Proyecto = proy.Nombre;

                    model.Ubicacion = dependencia;
                    @ViewBag.Institucion = proy.Institucion;
                    @ViewBag.CodigoSename = proy.CodSename;
                    @ViewBag.CodigoCodeni = proy.NombreEstado;
                    @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);

                    List<BienMovimiento> listaTodos = db.BienMovimiento.Where(a => a.EstadoID == 1 && a.AutorizacionAuditor == 1 && a.NuevaUbicacion == dependencia).ToList();

                    // hasta.AddDays(1);

                    foreach (var item in listaTodos)
                    {
                        int cantidadBaja = 0;
                        if (item.BienID == 1349) {
                            cantidadBaja = 0;
                        }  
                        // if (item.FechaMovimiento.Date >= desde.Date && item.FechaMovimiento.Date <= hasta.Date)
                        //  {
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);

                        
                        try
                        {
                            List<BienMovimiento> listBajas = db.BienMovimiento.Where(a => (a.BienID == bien.ID || a.bienAnteriorID == bien.ID) && a.EstadoID == 2 && a.AutorizacionAuditor == 1).ToList();
                            foreach (var item2 in listBajas)
                            {
                                //      if (item2.FechaMovimiento.Date >= desde.Date && item2.FechaMovimiento.Date <= hasta.Date)
                                //  {
                                cantidadBaja += item2.Cantidad;
                                // }
                            }
                        }
                        catch { }

                        int cantidadTras = 0;
                        try
                        {
                            List<BienMovimiento> listTras = db.BienMovimiento.Where(a => a.EstadoID == 3).ToList();
                            foreach (var item3 in listTras.Where(x => x.bienAnteriorID == item.BienID || x.BienID == bien.ID))
                            {
                                //        if (item3.FechaMovimiento.Date >= desde.Date && item3.FechaMovimiento.Date <= hasta.Date)
                                //      {
                                cantidadTras += item3.Cantidad;
                                //    }
                            }
                        }
                        catch
                        {

                        }

                        //if ((item.Cantidad - cantidadBaja - cantidadTras) > 0)
                        //{
                        BienModInventarioVM obj = new BienModInventarioVM();
                        obj.Familia = bien.SubFamilia.Nombre;
                        obj.Estado = item.Estado.Nombre;
                        if (bien.ProyectoAnteriorID.HasValue == true)
                        {
                            obj.ProyectoAnteriorID = bien.ProyectoAnteriorID.Value;
                        }
                        if (bien.ProyectoAnteriorID.HasValue == false)
                        {
                            obj.ProyectoAnteriorID = 0;
                        }
                        else if (bien.ProyectoAnteriorID == comboproyecto)
                        {
                            obj.Cantidad = item.Cantidad - cantidadBaja - cantidadTras;
                        }
                        else if (bien.ProyectoID == comboproyecto)
                        {
                            obj.Cantidad = item.Cantidad;// - cantidadBaja - cantidadTras;
                        }


                        obj.Detalle = item.Detalle;
                        obj.DescripcionBien = bien.DescripcionBien;
                        obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                        obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                        obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                        obj.CondicionText = item.Condicion.Nombre;
                        obj.Fecha = item.FechaMovimiento;
                        obj.ID = bien.ID;
                        obj.CondicionText = item.Condicion.Nombre;
                        var ubicacionID = int.Parse(bien.Ubicacion);
                        var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                        Dependencia model2 = w;
                        obj.Ubicacion = model2.Nombre;
                        obj.Ubicacion = bien.Ubicacion;
                        obj.Usuario = item.Usuario.Persona.NombreCompleto;
                        obj.Proyecto = bien.Proyecto.Nombre;
                        obj.Procedencia = bien.Procedencia.Nombre;
                        obj.MontoInt = bien.Monto;
                        if (item.bienAnteriorID.HasValue == false)
                        {
                            obj.MovimientoBienID = 0;
                        }
                        else
                        {
                            obj.MovimientoBienID = item.bienAnteriorID.Value;

                        }
                        obj.ProyectoID = comboproyecto;

                        if (bien.Ubicacion == dependencia)
                        {
                            model.lista.Add(obj);
                        }

                        //}
                        if (item.FechaMovimiento > ViewBag.FechaActualizacion)
                        {
                            @ViewBag.FechaActualizacion = item.FechaMovimiento;
                        }
                    }
                    //    }
                }
                else { 
                
                }
            }
            catch
            {

            }
            return View("InformeHojaInventario", model);
        }

        public ActionResult InformeHojaInventarioEX(int id, string dependencia)
        {
            BienModInventarioVM model = new BienModInventarioVM();
            try
            {
                Proyecto proy = db.Proyecto.Find(id);
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();

                model.ProyectoID = id;
                model.Proyecto = proy.Nombre;
                //   model.Desde = desde;
                //     model.Hasta = hasta;
                model.Ubicacion = dependencia;
                @ViewBag.Institucion = proy.Institucion;
                @ViewBag.CodigoSename = proy.CodSename;
                @ViewBag.CodigoCodeni = proy.NombreEstado;
                @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);

                List<BienMovimiento> listaTodos = db.BienMovimiento.Where(a => a.EstadoID == 1 && a.AutorizacionAuditor == 1 && a.NuevaUbicacion == dependencia).ToList();

                // hasta.AddDays(1);

                foreach (var item in listaTodos)
                {
                    // if (item.FechaMovimiento.Date >= desde.Date && item.FechaMovimiento.Date <= hasta.Date)
                    //  {
                    BienModInventario bien = db.BienModInventario.Find(item.BienID);

                    int cantidadBaja = 0;
                    try
                    {
                        List<BienMovimiento> listBajas = db.BienMovimiento.Where(a => a.BienID == bien.ID && a.EstadoID == 2 && a.AutorizacionAuditor == 1).ToList();
                        foreach (var item2 in listBajas)
                        {
                            //      if (item2.FechaMovimiento.Date >= desde.Date && item2.FechaMovimiento.Date <= hasta.Date)
                            //  {
                            cantidadBaja += item2.Cantidad;
                            // }
                        }
                    }
                    catch { }

                    int cantidadTras = 0;
                    try
                    {
                        List<BienMovimiento> listTras = db.BienMovimiento.Where(a => a.BienID == bien.ID && a.EstadoID == 3).ToList();
                        foreach (var item3 in listTras)
                        {
                            //        if (item3.FechaMovimiento.Date >= desde.Date && item3.FechaMovimiento.Date <= hasta.Date)
                            //      {
                            cantidadTras += item3.Cantidad;
                            //    }
                        }
                    }
                    catch
                    {

                    }

                    //if ((item.Cantidad - cantidadBaja - cantidadTras) > 0)
                    //{
                    BienModInventarioVM obj = new BienModInventarioVM();
                    obj.Familia = bien.SubFamilia.Nombre;
                    obj.Estado = item.Estado.Nombre;
                    if (item.EstadoID == 1)
                    {
                        obj.Cantidad = item.Cantidad - cantidadBaja - cantidadTras;
                    }
                    else
                    {
                        obj.Cantidad = item.Cantidad;// - cantidadBaja - cantidadTras;
                    }
                    obj.Detalle = item.Detalle;
                    obj.DescripcionBien = bien.DescripcionBien;
                    obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                    obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                    obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                    obj.CondicionText = item.Condicion.Nombre;
                    obj.Fecha = item.FechaMovimiento;
                    obj.ID = bien.ID;
                    obj.CondicionText = item.Condicion.Nombre;
                    var ubicacionID = int.Parse(bien.Ubicacion);
                    var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                    Dependencia model2 = w;
                    obj.Ubicacion = model2.Nombre;
                    obj.Ubicacion = bien.Ubicacion;
                    obj.Usuario = item.Usuario.Persona.NombreCompleto;
                    obj.Proyecto = bien.Proyecto.Nombre;
                    obj.Procedencia = bien.Procedencia.Nombre;
                    obj.MontoInt = bien.Monto;
                    if (bien.Ubicacion == dependencia)
                    {
                        model.lista.Add(obj);
                    }

                    //}
                    if (item.FechaMovimiento > ViewBag.FechaActualizacion)
                    {
                        @ViewBag.FechaActualizacion = item.FechaMovimiento;
                    }
                }
                //    }
            }
            catch
            {

            }
            return View("InformeHojaInventarioEX", model);
        }
        public ActionResult InventarioGeneralPrin(int? proyecto)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            if (proyecto == null) {
                Proyecto ProyectoActual = (Proyecto)Session["Proyecto"];
                proyecto = ProyectoActual.ID;  
            }
            BienModInventarioVM model = new BienModInventarioVM();

            ViewBag.proyecto = utils.ProyectoFiltro(filtro, int.Parse(proyecto.ToString()));
            ViewBag.CantAltas = 0;
            ViewBag.CantBajas = 0;
            ViewBag.CantTraslados = 0;
            ViewBag.General = 0;
            @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);

            try
            {
                List<BienModInventario> listamodel = new List<BienModInventario>();
                if (proyecto != null)
                {
                    List<BienMovimiento> listaMov = db.BienMovimiento.Where(d => d.Bien.ProyectoID == proyecto).ToList();  
                    listamodel = new List<BienModInventario>();

                    foreach (var item in listaMov)
                    {

                        {
                            if (item.EstadoID == 1)
                            {
                                ViewBag.CantAltas++;
                                ViewBag.General++;
                            }
                            if (item.EstadoID == 2)
                            {
                                ViewBag.CantBajas++;
                                ViewBag.General++;
                            }
                            if (item.EstadoID == 3)
                            {
                                ViewBag.CantTraslados++;
                                ViewBag.General++;
                            }

                            listamodel.Add(item.Bien);
                        }


                            ViewBag.CantTraslados++;
                            listamodel.Add(item.Bien);
                        

                    }
                }

                model.lista = new List<BienModInventarioVM>();

                foreach (var item in listamodel)
                {
                    BienModInventarioVM obj = new BienModInventarioVM();
                    obj.ID = item.ID;

                    model.lista.Add(obj);
                }

                //var q3 = db.Proyecto.Where(a => a.Cerrado == null && a.Eliminado == null).OrderBy(a => a.CodCodeni).ToList();
                //List<SelectListItem> listproyecto = new List<SelectListItem>();
                //listproyecto.Add(new SelectListItem
                //{
                //    Text = "Seleccione",
                //    Value = "0"
                //});
                //foreach (var y in q3)
                //{
                //    listproyecto.Add(new SelectListItem
                //    {
                //        Text = y.NombreEstado,
                //        Value = y.ID.ToString()
                //    });
                //}
                //ViewBag.comboproyecto = listproyecto;
               
                model.ID = (int)proyecto;
            }
            catch
            {
                if (proyecto == null)
                {

                }
                else
                {
                    TempData["Message"] = "Error en la operación.";
                }
            }

            model.Egreso = new DetalleEgreso();

            return View(model);

        }

        public ActionResult InformeGeneralPrin(int id)
        {
            BienModInventarioVM model = new BienModInventarioVM();
        //    try
          //  {
                Proyecto proy = db.Proyecto.Find(id);
                model.ClaseProyecto = proy;
                model.lista = new List<BienModInventarioVM>();

                model.ProyectoID = id;
                model.Proyecto = proy.Nombre;

                @ViewBag.Institucion = proy.Institucion;
                @ViewBag.CodigoSename = proy.CodSename;
                @ViewBag.CodigoCodeni = proy.NombreEstado;
                @ViewBag.FechaActualizacion = new DateTime(1990, 01, 01);

                var listaTodos = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1 && a.Bien.ProyectoID == id ).ToList();

               // var listaGroup = db.BienMovimiento.Where(a => a.AutorizacionAuditor == 1).GroupBy(z => new {z.Bien} ).Select(c => c.Key).ToList();

                foreach (var item in listaTodos)
                {
                    int isi = 1;
                    if (item.BienID == 617) 
                    {
                        
                        isi = 0;
                    }

                    isi = isi + 1;
                       
                        BienModInventario bien = db.BienModInventario.Find(item.BienID);

      
                            BienModInventarioVM obj = new BienModInventarioVM();

                            obj.Familia = bien.SubFamilia.Nombre;
                            obj.Estado = item.Estado.Nombre;


                            obj.Detalle = item.Detalle;
                            obj.Cantidad = item.Cantidad;
                            obj.DescripcionBien = bien.DescripcionBien;
                            obj.Movimiento = db.Movimiento.Find(bien.MovimientoID);
                            obj.Egreso = db.DetalleEgreso.Find(bien.EgresoID);
                            obj.Reintegro = db.DetalleReintegro.Find(bien.ReintegroID);
                            obj.CondicionText = item.Condicion.Nombre;
                            obj.Fecha = item.FechaMovimiento;
                            obj.ID = bien.ID;
                            obj.CondicionText = item.Condicion.Nombre;
                            var ubicacionID = int.Parse(bien.Ubicacion);
                            var w = db.Dependencia.Where(x => x.ID == ubicacionID).First();
                            Dependencia model2 = w;
                            obj.Ubicacion = model2.Nombre;
                            obj.ProyectoID = bien.ProyectoID;
                            obj.Usuario = "xx"; //item.Usuario.Persona.NombreCompleto;revisar !!!
                            obj.Proyecto = bien.Proyecto.Nombre;
                            obj.Procedencia = bien.Procedencia.Nombre;
                            obj.MontoInt = bien.Monto;
                    if (item.bienAnteriorID.HasValue)
                    {
                        obj.MovimientoBienID = item.bienAnteriorID.Value;
                    }
                        
                    if (bien.ProyectoAnteriorID.HasValue)
                    {
                        obj.ProyectoAnteriorID = bien.ProyectoAnteriorID.Value;
                    }
                            
                    obj.RutaArchivo = item.RutaArchivo;
                    model.lista.Add(obj);
                        //}
                        if (item.FechaMovimiento > ViewBag.FechaActualizacion)
                        {
                            @ViewBag.FechaActualizacion = item.FechaMovimiento;
                        }
                    }

            return View("InformeGeneralPrin",model);
        }


    }
}