using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Classes;
using SAG2.Models;

namespace SAG2.Controllers
{
    public class ControlController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Constantes ctes = new Constantes();
        private Util utils = new Util();
        //
        // GET: /Control/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult IndicadoresSag(int Periodo = 0, int PrIndicadores = 0)
        {
            int Mes = 1;
            int pr_id = 0;
            int filtro = int.Parse(Session["Filtro"].ToString());  
            if (Periodo == 0)
            {
                Periodo = (int)Session["Periodo"];
             // Mes =
            }
            if (PrIndicadores == 0)
            {
                
                pr_id = ((Proyecto)Session["Proyecto"]).ID;
                // Mes =
            }
            else
            {
               // Proyecto proyecto = db.Proyecto.Find(PrIndicadores);
                pr_id = PrIndicadores;
            }
            Proyecto proyecto = db.Proyecto.Find(pr_id);
            // Datos Proyecto

                ViewBag.Periodo = Periodo;
                ViewBag.Pidin = pr_id;
                //ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
                ViewBag.Proyectos = utils.FiltroProyecto(filtro);
                try
                {
                    ViewBag.CostosReales = db.IndicadoresCuenta.Where(pp => pp.FACTOR == 1).OrderBy(pp => pp.ID).ToList();
                }
                catch
                {
                    ViewBag.CostosReales = 0;
                }
                try
                {
                    ViewBag.Financiamiento = db.IndicadoresCuenta.Where(pp => pp.FACTOR == 2).OrderBy(pp => pp.ID).ToList();
                }
                catch { }
                try
                {
                    ViewBag.SaldoBanco = db.Saldo.Where(ps => ps.CuentaCorriente.ProyectoID == proyecto.ID).Where(ps => ps.Periodo == Periodo).ToList();
                }
                catch { }
                try{
                    ViewBag.NombreProyecto = proyecto.Nombre;
                }catch{}
              try{
                ViewBag.CodigoSename = proyecto.CodSename;
              }
              catch { }
                ViewBag.CodigoCodeni = proyecto.CodCodeni;
                ViewBag.Ubicacion = proyecto.Direccion.Comuna.Nombre + ", " + proyecto.Direccion.Comuna.Region.Nombre;
            try{
                ViewBag.Auditor = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == proyecto.ID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto;
            }
            catch { }
                ViewBag.Ciudad = proyecto.Direccion.Comuna.Nombre;
                ViewBag.Region = proyecto.Direccion.Comuna.Region.Nombre;
             try{
                ViewBag.NroPlaza = db.Convenio.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).Where(c => c.Mes == Mes).Single().NroPlazas;
             }
             catch { }
             try{
                 ViewBag.Plazas = db.Convenio.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
             }
             catch { }
             try
             {
                 ViewBag.Inter = db.Intervencion.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
             }
             catch { }
             try
             {
                 ViewBag.Indcal = db.IndicadorCalidad.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
             }
             catch { }
                try{
                ViewBag.BolHon = db.BoletaHonorario.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
                 }catch{}
                try{
                ViewBag.HHExtras = db.HorasExtras.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
                }catch{}
                try{
                ViewBag.Indca = db.IndicadoresGestion.Where(i => i.ProyectoID == proyecto.ID).ToList();
                }catch{}

                if (proyecto.ValorSubvencion != null)
                {
                    ViewBag.ValorSubvencion = proyecto.ValorSubvencion;
                }
                else
                {
                    ViewBag.ValorSubvencion = 0;
                }
                try{
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
                }catch{}
                try{
                    ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
                }catch{}
                    try{
                    ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            }
            catch (Exception)
            {

            }
                try
                {
                    List<DetallePresupuesto> dp = new List<DetallePresupuesto>();
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.ProyectoID == pr_id).Where(d=> d.Periodo == Periodo).ToList();

                    List<DeudaPendiente> dpen = new List<DeudaPendiente>();

                    dpen = db.DeudaPendiente.Where(m => m.EgresoID == null).Where(m=> m.Periodo == Periodo).Where(m => m.ProyectoID == pr_id && m.CuentaID != 1).ToList();
                    
                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (Mes > 12)
                        {
                            Mes = 1;
                            Periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == Periodo).Where(m => m.Mes == Mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes == Mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == Periodo).Where(m => m.Mes == Mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }


                        Mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;
                    ViewBag.Detalle = dp;
                    ViewBag.Deudapen = dpen; 

                }
                catch (Exception)
                {
                    
                }

            return View(proyecto);

        }
        public ActionResult IndicadoresExcel(int Periodo = 0, int PrIndicadores = 0)
        {
            int Mes = 1;
            int pr_id = PrIndicadores;
 
            Proyecto proyecto = db.Proyecto.Find(pr_id);
            // Datos Proyecto

            ViewBag.Periodo = Periodo;
            ViewBag.Pidin = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).Where(c => c.Cerrado != "S").OrderBy(p => p.CodCodeni).ToList();
            try
            {
                ViewBag.CostosReales = db.IndicadoresCuenta.Where(pp => pp.FACTOR == 1).OrderBy(pp => pp.ID).ToList();
            }
            catch
            {
                ViewBag.CostosReales = 0;
            }
            try
            {
                ViewBag.Financiamiento = db.IndicadoresCuenta.Where(pp => pp.FACTOR == 2).OrderBy(pp => pp.ID).ToList();
            }
            catch { }
            try
            {
                ViewBag.SaldoBanco = db.Saldo.Where(ps => ps.CuentaCorriente.ProyectoID == proyecto.ID).Where(ps => ps.Periodo == Periodo).ToList();
            }
            catch { }
            try
            {
                ViewBag.NombreProyecto = proyecto.Nombre;
            }
            catch { }
            try
            {
                ViewBag.CodigoSename = proyecto.CodSename;
            }
            catch { }
              try
            {
            ViewBag.CodigoCodeni = proyecto.CodCodeni;
            }
              catch { }
            ViewBag.Ubicacion = proyecto.Direccion.Comuna.Nombre + ", " + proyecto.Direccion.Comuna.Region.Nombre;
            try
            {
                ViewBag.Auditor = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == proyecto.ID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto;
            }
            catch { }
            ViewBag.Ciudad = proyecto.Direccion.Comuna.Nombre;
            ViewBag.Region = proyecto.Direccion.Comuna.Region.Nombre;
            try
            {
                ViewBag.NroPlaza = db.Convenio.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).Where(c => c.Mes == Mes).Single().NroPlazas;
            }
            catch { }
            try
            {
                ViewBag.Plazas = db.Convenio.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
            }
            catch { }
            try
            {
                ViewBag.Inter = db.Intervencion.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
            }
            catch { }
            try
            {
                ViewBag.Indcal = db.IndicadorCalidad.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
            }
            catch { }
            try
            {
                ViewBag.BolHon = db.BoletaHonorario.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
            }
            catch { }
            try
            {
                ViewBag.HHExtras = db.HorasExtras.Where(c => c.ProyectoID == proyecto.ID).Where(c => c.Periodo == Periodo).ToList();
            }
            catch { }
            try
            {
                ViewBag.Indca = db.IndicadoresGestion.Where(i => i.ProyectoID == proyecto.ID).ToList();
            }
            catch { }

            if (proyecto.ValorSubvencion != null)
            {
                ViewBag.ValorSubvencion = proyecto.ValorSubvencion;
            }
            else
            {
                ViewBag.ValorSubvencion = 0;
            }
            try
            {
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            }
            catch { }
            try
            {
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            }
            catch { }
            try
            {
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).OrderBy(m => m.Cuenta.Orden).ToList();
            }
            catch (Exception)
            {

            }
            try
            {
                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();
                dp = db.DetallePresupuesto.Where(d => d.Presupuesto.ProyectoID == pr_id).Where(d => d.Periodo == Periodo).ToList();

                List<DeudaPendiente> dpen = new List<DeudaPendiente>();

                dpen = db.DeudaPendiente.Where(m => m.EgresoID == null).Where(m => m.Periodo == Periodo).Where(m => m.ProyectoID == pr_id && m.CuentaID != 1).ToList();

                List<int> Ingresos = new List<int>();
                List<int> Egresos = new List<int>();
                List<int> Reintegros = new List<int>();

                List<int> IngresosPre = new List<int>();
                List<int> EgresosPre = new List<int>();

                for (int i = 0; i < 12; i++)
                {
                    if (Mes > 12)
                    {
                        Mes = 1;
                        Periodo++;
                    }

                    try
                    {
                        Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == Periodo).Where(m => m.Mes == Mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                    }
                    catch (Exception)
                    {
                        Ingresos.Add(0);
                    }

                    try
                    {
                        Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes == Mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                    }
                    catch (Exception)
                    {
                        Egresos.Add(0);
                    }

                    try
                    {
                        Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == Periodo).Where(m => m.Mes == Mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                    }
                    catch (Exception)
                    {
                        Reintegros.Add(0);
                    }


                    Mes++;
                }

                ViewBag.MovIngresos = Ingresos;
                ViewBag.MovEgresos = Egresos;
                ViewBag.MovReintegros = Reintegros;
                ViewBag.Detalle = dp;
                ViewBag.Deudapen = dpen;

            }
            catch (Exception)
            {

            }

            return View(proyecto);


        }
        public ActionResult Intervenciones() 
        {
            Proyecto proyecto = (Proyecto)Session["Proyecto"];
        
             int   Periodo = (int)Session["Periodo"];
            
            int filtro = int.Parse(Session["Filtro"].ToString());
            List<Proyecto> proyectos = new List<Proyecto>();

            proyectos = db.Proyecto.Where(d => d.ID == proyecto.ID).ToList() ;  

            //if (filtro == 1)
            //{               
            //    ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null), "ID", "NombreLista", proyecto.ID);
            //}
            //else
            //{     
            //    ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null ), "ID", "NombreLista", proyecto.ID);
            //}
            ViewBag.ProyectoID = utils.ProyectoFiltro(filtro, proyecto.ID);
            ViewBag.Exportar = "";
            ViewBag.TipoProgramaID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.regionID = new SelectList(db.Region.ToList(), "ID", "Nombre");
        
            ViewBag.Periodo = Periodo;
            return View(proyectos.ToList());
        }
        [HttpPost] 
        public ActionResult Intervenciones(FormCollection form)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            int pr_id = 1;
            int RegionId = 0;
            int tipoProyectoID = 0;
            int Periodo = int.Parse(form["Periodo"].ToString()); ;
            if (form["ProyectoID"].ToString() != "")
            {
                pr_id = int.Parse(form["ProyectoID"].ToString());
            }
            if (form["regionID"].ToString() != "")
            {
                RegionId = int.Parse(form["regionID"].ToString());
            }
            if (form["TipoProgramaID"].ToString() != "")
            {
                tipoProyectoID = int.Parse(form["TipoProgramaID"].ToString());
            }
            ViewBag.Exportar = form["exportar"].ToString();
            List<Proyecto> proyectos = new List<Proyecto>();

           
            ViewBag.ProyectoID = utils.ProyectoFiltro(filtro, pr_id);
            proyectos = utils.FiltroProyecto(filtro); 

            if (pr_id != 1)
            {
                proyectos = proyectos.Where(d => d.ID == pr_id).ToList();

            }
            else {

                if (RegionId != 0) {
                    proyectos = proyectos.Where(d => d.Direccion.Comuna.RegionID == RegionId).ToList();   
                }

                if (tipoProyectoID != 0)
                {
                    proyectos = proyectos.Where(d => d.TipoProyectoID == tipoProyectoID).ToList();
                }
            
            }
            ViewBag.TipoProgramaID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla",tipoProyectoID );
            ViewBag.regionID = new SelectList(db.Region.ToList(), "ID", "Nombre",RegionId );

            ViewBag.Periodo = Periodo;
            return View(proyectos.ToList());
        }
        public ActionResult ResumenIntervenciones(int Periodo = 0, string export = "")
        {
            if (Periodo == 0)
            {
                Periodo = (int)Session["Periodo"];
            }

            ViewBag.Exportar = "";

            if (export.Equals("xls"))
            {
                ViewBag.Exportar = "xls";
            }

            ViewBag.Periodo = Periodo;
            var proyectos = db.Proyecto.OrderBy(p => p.Nombre);
            return View(proyectos.ToList());
        }

        public ActionResult Indicadores(int Periodo = 0)
        {
            if (Periodo == 0)
            {
                Periodo = (int)Session["Periodo"];
            }

            Proyecto proyecto = db.Proyecto.Find(((Proyecto)Session["Proyecto"]).ID);
            ViewBag.Periodo = Periodo;

            try
            {
                ViewBag.NombreProyecto = proyecto.Nombre;
                ViewBag.CodigoSename = proyecto.CodSename;
                ViewBag.CodigoCodeni = proyecto.CodCodeni;
                ViewBag.Ubicacion = proyecto.Direccion.Comuna.Nombre + ", " + proyecto.Direccion.Comuna.Region.Nombre;
                ViewBag.Auditor = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == proyecto.ID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto;
            }
            catch (Exception)
            { }

            try
            {
                ViewBag.Indicadores = db.IndicadoresGestion.Where(i => i.ProyectoID == proyecto.ID && i.Periodo == Periodo).Single();
            }
            catch (Exception)
            {
                //ViewBag.Indicadores = new IndicadoresGestion();
                //http://localhost/SAG2/Control/Estandares/?Periodo=2011
                return RedirectToAction("Estandares", new { @Periodo = Periodo});
            }

            // Cobertura
            try
            {
                ViewBag.Cobertura_1 = db.Convenio.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes >= 1 && m.Mes <= 3).Sum(m => m.NroPlazas);
            }
            catch (Exception)
            {
                ViewBag.Cobertura_1 = 0;
            }

            try
            {
                ViewBag.Cobertura_2 = db.Convenio.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes >= 4 && m.Mes <= 6).Sum(m => m.NroPlazas);
            }
            catch (Exception)
            {
                ViewBag.Cobertura_2 = 0;
            }

            try
            {
                ViewBag.Cobertura_3 = db.Convenio.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes >= 7 && m.Mes <= 9).Sum(m => m.NroPlazas);
            }
            catch (Exception)
            {
                ViewBag.Cobertura_3 = 0;
            }

            try
            {
                ViewBag.Cobertura_4 = db.Convenio.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes >= 10 && m.Mes <= 12).Sum(m => m.NroPlazas);
            }
            catch (Exception)
            {
                ViewBag.Cobertura_4 = 0;
            }

            // Intervenciones

            try
            {
                ViewBag.Intervenciones_1 = db.Intervencion.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes >= 1 && m.Mes <= 3).Sum(m => m.Atenciones);
            }
            catch (Exception)
            {
                ViewBag.Intervenciones_1 = 0;
            }

            try
            {
                ViewBag.Intervenciones_2 = db.Intervencion.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes >= 4 && m.Mes <= 6).Sum(m => m.Atenciones);
            }
            catch (Exception)
            {
                ViewBag.Intervenciones_2 = 0;
            }

            try
            {
                ViewBag.Intervenciones_3 = db.Intervencion.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes >= 7 && m.Mes <= 9).Sum(m => m.Atenciones);
            }
            catch (Exception)
            {
                ViewBag.Intervenciones_3 = 0;
            }

            try
            {
                ViewBag.Intervenciones_4 = db.Intervencion.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes >= 10 && m.Mes <= 12).Sum(m => m.Atenciones);
            }
            catch (Exception)
            {
                ViewBag.Intervenciones_4 = 0;
            }


            // Ingresos
            try
            {
                ViewBag.Ingresos_Subvencion_1 = db.Movimiento.Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.CuentaID != 5).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Subvencion_1 = 0;
            }

            try
            {
                ViewBag.Ingresos_Subvencion_2 = db.Movimiento.Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.CuentaID != 5).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Subvencion_2 = 0;
            }
            
            try
            {
                ViewBag.Ingresos_Subvencion_3 = db.Movimiento.Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.CuentaID != 5).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Subvencion_3 = 0;
            }

            try
            {
                ViewBag.Ingresos_Subvencion_4 = db.Movimiento.Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.CuentaID != 5).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Subvencion_4 = 0;
            }

            try
            {
                ViewBag.Ingresos_Otros_1 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("3.")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Otros_1 = 0;
            }

            try
            {
                ViewBag.Ingresos_Otros_2 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("3.")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Otros_2 = 0;
            }

            try
            {
                ViewBag.Ingresos_Otros_3 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("3.")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Otros_3 = 0;
            }

            try
            {
                ViewBag.Ingresos_Otros_4 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("3.")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Otros_4 = 0;
            }

            try
            {
                ViewBag.Ingresos_Costos_1 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => !m.Cuenta.Codigo.StartsWith("5")).Where(m => !m.Cuenta.Codigo.StartsWith("4.2")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Costos_1 = 1;
            }

            try
            {
                ViewBag.Costos_Personal_1 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("6")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Personal_1 = 0;
            }

            try
            {
                ViewBag.Ingresos_Costos_2 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => !m.Cuenta.Codigo.StartsWith("5")).Where(m => !m.Cuenta.Codigo.StartsWith("4.2")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Costos_2 = 1;
            }

            try
            {
                ViewBag.Costos_Personal_2 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("6")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Personal_2 = 0;
            }

            try
            {
                ViewBag.Ingresos_Costos_3 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => !m.Cuenta.Codigo.StartsWith("5")).Where(m => !m.Cuenta.Codigo.StartsWith("4.2")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Costos_3 = 1;
            }

            try
            {
                ViewBag.Costos_Personal_3 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("6")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Personal_3 = 0;
            }

            try
            {
                ViewBag.Ingresos_Costos_4 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => !m.Cuenta.Codigo.StartsWith("5")).Where(m => !m.Cuenta.Codigo.StartsWith("4.2")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Ingresos_Costos_4 = 1;
            }

            try
            {
                ViewBag.Costos_Personal_4 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("6")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Personal_4 = 0;
            }

            // ***********************************
            // Funcionamiento
            // ***********************************

            try
            {
                ViewBag.Costos_Funcionamiento_1 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.1")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Funcionamiento_1 = 0;
            }

            try
            {
                ViewBag.Costos_Funcionamiento_2 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.1")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Funcionamiento_2 = 0;
            }

            try
            {
                ViewBag.Costos_Funcionamiento_3 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.1")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Funcionamiento_3 = 0;
            }

            try
            {
                ViewBag.Costos_Funcionamiento_4 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.1")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Funcionamiento_4 = 0;
            }

            // ***********************************
            // Apoyo Técnico
            // ***********************************

            try
            {
                ViewBag.Costos_ApoyoTecnico_1 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.2")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_ApoyoTecnico_1 = 0;
            }

            try
            {
                ViewBag.Costos_ApoyoTecnico_2 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.2")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_ApoyoTecnico_2 = 0;
            }

            try
            {
                ViewBag.Costos_ApoyoTecnico_3 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.2")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_ApoyoTecnico_3 = 0;
            }

            try
            {
                ViewBag.Costos_ApoyoTecnico_4 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.2")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_ApoyoTecnico_4 = 0;
            }

            // ***********************************
            // Apoyo a Beneficiarios
            // ***********************************

            try
            {
                ViewBag.Costos_ApoyoBeneficiarios_1 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.3")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_ApoyoBeneficiarios_1 = 0;
            }

            try
            {
                ViewBag.Costos_ApoyoBeneficiarios_2 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.3")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_ApoyoBeneficiarios_2 = 0;
            }

            try
            {
                ViewBag.Costos_ApoyoBeneficiarios_3 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.3")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_ApoyoBeneficiarios_3 = 0;
            }

            try
            {
                ViewBag.Costos_ApoyoBeneficiarios_4 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("7.3")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_ApoyoBeneficiarios_4 = 0;
            }

            // ***********************************
            // Inversion
            // ***********************************

            try
            {
                ViewBag.Costos_Inversion_1 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("8")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Inversion_1 = 0;
            }

            try
            {
                ViewBag.Costos_Inversion_2 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("8")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Inversion_2 = 0;
            }

            try
            {
                ViewBag.Costos_Inversion_3 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("8")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Inversion_3 = 0;
            }

            try
            {
                ViewBag.Costos_Inversion_4 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("8")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Inversion_4 = 0;
            }

            // ***********************************
            // Indemnizacion
            // ***********************************

            try
            {
                ViewBag.Costos_Indemnizacion_1 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("9") || m.Cuenta.Codigo.StartsWith("10")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Indemnizacion_1 = 0;
            }

            try
            {
                ViewBag.Costos_Indemnizacion_2 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("9") || m.Cuenta.Codigo.StartsWith("10")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Indemnizacion_2 = 0;
            }

            try
            {
                ViewBag.Costos_Indemnizacion_3 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("9") || m.Cuenta.Codigo.StartsWith("10")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Indemnizacion_3 = 0;
            }

            try
            {
                ViewBag.Costos_Indemnizacion_4 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("E")).Where(m => m.Cuenta.Codigo.StartsWith("9") || m.Cuenta.Codigo.StartsWith("10")).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Costos_Indemnizacion_4 = 0;
            }

            // ***********************************
            // Préstamos 
            // ***********************************

            try
            {
                ViewBag.Finan_Prestamos_1 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("5.1")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Finan_Prestamos_1 = 0;
            }

            try
            {
                ViewBag.Finan_Prestamos_2 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("5.1")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Finan_Prestamos_2 = 0;
            }

            try
            {
                ViewBag.Finan_Prestamos_3 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("5.1")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Finan_Prestamos_3 = 0;
            }

            try
            {
                ViewBag.Finan_Prestamos_4 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("5.1")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Finan_Prestamos_4 = 0;
            }

            // ***********************************
            // Aportes de Terceros 
            // ***********************************

            try
            {
                ViewBag.Finan_Aportes_1 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("3.1") || m.Cuenta.Codigo.StartsWith("5.2")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Finan_Aportes_1 = 0;
            }

            try
            {
                ViewBag.Finan_Aportes_2 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("3.1") || m.Cuenta.Codigo.StartsWith("5.2")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Finan_Aportes_2 = 0;
            }

            try
            {
                ViewBag.Finan_Aportes_3 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("3.1") || m.Cuenta.Codigo.StartsWith("5.2")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Finan_Aportes_3 = 0;
            }

            try
            {
                ViewBag.Finan_Aportes_4 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Cuenta.Tipo.Equals("I")).Where(m => m.Cuenta.Codigo.StartsWith("3.1") || m.Cuenta.Codigo.StartsWith("5.2")).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            {
                ViewBag.Finan_Aportes_4 = 0;
            }

            // ***********************************
            // Saldo Banco
            // ***********************************

            try
            {
                int Ingresos = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto_Ingresos);
                int Egresos = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
                int saldoInicial = 0;
                try
                {
                    CuentaCorriente CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == proyecto.ID).Single();
                    saldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes == 1).Single().SaldoInicialCartola;
                }
                catch (Exception)
                { }
                ViewBag.Prod_SaldoBanco_1 = Ingresos - Egresos + saldoInicial;
            }
            catch (Exception)
            {
                ViewBag.Prod_SaldoBanco_1 = 0;
            }

            try
            {
                int Ingresos = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto_Ingresos);
                int Egresos = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
                int saldoInicial = 0;
                try
                {
                    CuentaCorriente CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == proyecto.ID).Single();
                    saldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes == 4).Single().SaldoInicialCartola;
                }
                catch (Exception)
                { } 
                ViewBag.Prod_SaldoBanco_2 = Ingresos - Egresos + saldoInicial;
            }
            catch (Exception)
            {
                ViewBag.Prod_SaldoBanco_2 = 0;
            }

            try
            {
                int Ingresos = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto_Ingresos);
                int Egresos = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
                int saldoInicial = 0;
                try
                {
                    CuentaCorriente CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == proyecto.ID).Single();
                    saldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes == 7).Single().SaldoInicialCartola;
                }
                catch (Exception)
                { } 
                ViewBag.Prod_SaldoBanco_3 = Ingresos - Egresos + saldoInicial;
            }
            catch (Exception)
            {
                ViewBag.Prod_SaldoBanco_3 = 0;
            }

            try
            {
                int Ingresos = db.Movimiento.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto_Ingresos);
                int Egresos = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
                int saldoInicial = 0;
                try
                {
                    CuentaCorriente CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == proyecto.ID).Single();
                    saldoInicial = db.Saldo.Where(m => m.CuentaCorrienteID == CuentaCorriente.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes == 10).Single().SaldoInicialCartola;
                }
                catch (Exception)
                { } 
                ViewBag.Prod_SaldoBanco_4 = Ingresos - Egresos + saldoInicial;
            }
            catch (Exception)
            {
                ViewBag.Prod_SaldoBanco_4 = 0;
            }

            // ***********************************
            // Resultado
            // ***********************************

            try
            {
                int Ingresos = db.Movimiento.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto_Ingresos);
                ViewBag.Prod_Resultado_Ing_1 = Ingresos;
            }
            catch (Exception)
            {
                ViewBag.Prod_Resultado_Ing_1 = 1;
            }

            try
            {
                int Egresos = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => (m.Cuenta.Codigo.StartsWith("6.") || m.Cuenta.Codigo.StartsWith("7.")) && !m.Cuenta.Codigo.StartsWith("7.1.12")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
                ViewBag.Prod_Resultado_Egr_1 = Egresos;
            }
            catch (Exception)
            {
                ViewBag.Prod_Resultado_Egr_1 = 0;
            }

            try
            {
                int Ingresos = db.Movimiento.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto_Ingresos);
                ViewBag.Prod_Resultado_Ing_2 = Ingresos;
            }
            catch (Exception)
            {
                ViewBag.Prod_Resultado_Ing_2 = 1;
            }

            try
            {
                int Egresos = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => (m.Cuenta.Codigo.StartsWith("6.") || m.Cuenta.Codigo.StartsWith("7.")) && !m.Cuenta.Codigo.StartsWith("7.1.12")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
                ViewBag.Prod_Resultado_Egr_2 = Egresos;
            }
            catch (Exception)
            {
                ViewBag.Prod_Resultado_Egr_2 = 0;
            }

            try
            {
                int Ingresos = db.Movimiento.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto_Ingresos);
                ViewBag.Prod_Resultado_Ing_3 = Ingresos;
            }
            catch (Exception)
            {
                ViewBag.Prod_Resultado_Ing_3 = 1;
            }

            try
            {
                int Egresos = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => (m.Cuenta.Codigo.StartsWith("6.") || m.Cuenta.Codigo.StartsWith("7.")) && !m.Cuenta.Codigo.StartsWith("7.1.12")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
                ViewBag.Prod_Resultado_Egr_3 = Egresos;
            }
            catch (Exception)
            {
                ViewBag.Prod_Resultado_Egr_3 = 0;
            }

            try
            {
                int Ingresos = db.Movimiento.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto_Ingresos);
                ViewBag.Prod_Resultado_Ing_4 = Ingresos;
            }
            catch (Exception)
            {
                ViewBag.Prod_Resultado_Ing_4 = 1;
            }

            try
            {
                int Egresos = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => (m.Cuenta.Codigo.StartsWith("6.") || m.Cuenta.Codigo.StartsWith("7.")) && !m.Cuenta.Codigo.StartsWith("7.1.12")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
                ViewBag.Prod_Resultado_Egr_4 = Egresos;
            }
            catch (Exception)
            {
                ViewBag.Prod_Resultado_Egr_4 = 0;
            }

            // ***********************************
            // Proveedores
            // ***********************************

            try
            {
                ViewBag.Finan_Proveedores_1 = db.DeudaPendiente.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Finan_Proveedores_1 = 0;
            }

            try
            {
                ViewBag.Finan_Proveedores_2 = db.DeudaPendiente.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Finan_Proveedores_2 = 0;
            }

            try
            {
                ViewBag.Finan_Proveedores_3 = db.DeudaPendiente.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Finan_Proveedores_3 = 0;
            }

            try
            {
                ViewBag.Finan_Proveedores_4 = db.DeudaPendiente.Include(d => d.Cuenta).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.Finan_Proveedores_4 = 0;
            }

            // ***********************************
            // Aporte Subvención
            // ***********************************

            ViewBag.AporteSubvencion_Ing_1 = 1;
            ViewBag.AporteSubvencion_Ing_2 = 1;
            ViewBag.AporteSubvencion_Ing_3 = 1;
            ViewBag.AporteSubvencion_Ing_4 = 1;

            try
            {
                ViewBag.AporteSubvencion_1 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("7.1.13")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.AporteSubvencion_1 = 0;
            }

            try
            {
                ViewBag.AporteSubvencion_Ing_1 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 1).Where(m => m.Mes <= 3).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            { }

            try
            {
                ViewBag.AporteSubvencion_2 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("7.1.13")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.AporteSubvencion_2 = 0;
            }

            try
            {
                ViewBag.AporteSubvencion_Ing_2 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 4).Where(m => m.Mes <= 6).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            { }

            try
            {
                ViewBag.AporteSubvencion_3 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("7.1.13")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.AporteSubvencion_3 = 0;
            }

            try
            {
                ViewBag.AporteSubvencion_Ing_3 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 7).Where(m => m.Mes <= 9).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            { }

            try
            {
                ViewBag.AporteSubvencion_4 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("7.1.13")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
            }
            catch (Exception)
            {
                ViewBag.AporteSubvencion_4 = 0;
            }

            try
            {
                ViewBag.AporteSubvencion_Ing_4 = db.Movimiento.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("1.")).Where(m => m.ProyectoID == proyecto.ID).Where(m => m.Periodo == Periodo).Where(m => m.Mes >= 10).Where(m => m.Mes <= 12).Sum(m => m.Monto_Ingresos);
            }
            catch (Exception)
            { }
            	
            // ***********************************
            // Costo Niño/Mes
            // ***********************************
            /*
             * Se mide trimestralmente dividiendo los Gastos Totales (cuentas Nº 6, 7, 8, 9, y 10 (la 10 solo del último mes)) por los Ingresos Totales (cuentas Nº 1.1, 1.2 y 1.3)
             */

            ViewBag.CostoNiño_1 = 0;
            ViewBag.CostoNiño_2 = 0;
            ViewBag.CostoNiño_3 = 0;
            ViewBag.CostoNiño_4 = 0;

            try
            {
                int GastosTotales = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("6") || m.Cuenta.Codigo.StartsWith("7") || m.Cuenta.Codigo.StartsWith("8") || m.Cuenta.Codigo.StartsWith("9")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 1).Where(m => m.Egreso.Mes <= 3).Sum(m => m.Monto);
                ViewBag.CostoNiño_1 = ViewBag.CostoNiño_1 + GastosTotales;
            }
            catch (Exception)
            { }

            try
            {
                int GastosCta10 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("10")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes == 3).Sum(m => m.Monto);
                ViewBag.CostoNiño_1 = ViewBag.CostoNiño_1 + GastosCta10;
            }
            catch (Exception)
            { }

            try
            {
                int GastosTotales = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("6") || m.Cuenta.Codigo.StartsWith("7") || m.Cuenta.Codigo.StartsWith("8") || m.Cuenta.Codigo.StartsWith("9")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 4).Where(m => m.Egreso.Mes <= 6).Sum(m => m.Monto);
                ViewBag.CostoNiño_2 = GastosTotales + ViewBag.CostoNiño_2;
            }
            catch (Exception)
            { }

            try
            {
                int GastosCta10 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("10")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes == 6).Sum(m => m.Monto);
                ViewBag.CostoNiño_2 = ViewBag.CostoNiño_2 + GastosCta10;
            }
            catch (Exception)
            { }

            try
            {
                int GastosTotales = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("6") || m.Cuenta.Codigo.StartsWith("7") || m.Cuenta.Codigo.StartsWith("8") || m.Cuenta.Codigo.StartsWith("9")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 7).Where(m => m.Egreso.Mes <= 9).Sum(m => m.Monto);
                ViewBag.CostoNiño_3 = GastosTotales + ViewBag.CostoNiño_3;
            }
            catch (Exception)
            { }

            try
            {
                int GastosCta10 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("10")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes == 9).Sum(m => m.Monto);
                ViewBag.CostoNiño_3 = ViewBag.CostoNiño_3 + GastosCta10;
            }
            catch (Exception)
            { }

            try
            {
                int GastosTotales = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("6") || m.Cuenta.Codigo.StartsWith("7") || m.Cuenta.Codigo.StartsWith("8") || m.Cuenta.Codigo.StartsWith("9")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes >= 10).Where(m => m.Egreso.Mes <= 12).Sum(m => m.Monto);
                ViewBag.CostoNiño_4 = ViewBag.CostoNiño_4 + GastosTotales;
            }
            catch (Exception)
            { }

            try
            {
                int GastosCta10 = db.DetalleEgreso.Include(d => d.Cuenta).Where(m => m.Cuenta.Codigo.StartsWith("10")).Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.Egreso.Periodo == Periodo).Where(m => m.Egreso.Mes == 12).Sum(m => m.Monto);
                ViewBag.CostoNiño_4 = ViewBag.CostoNiño_4 + GastosCta10;
            }
            catch (Exception)
            { }

            // ***********************************
            // Desviación del Presupuesto
            // ***********************************
            /*
             * Midiendo la desviación (+ ó -) de los Ingresos Presupuestados respecto de los Ingresos Reales del semestre (Total Ingresos Reales en las cuentas Nº 1.1, 1.2 y 1.3 (Menos) el Total de Ingresos Presupuestados de las cuentas Nº 1.1, 1.2 y 1.3 )  
             */

            ViewBag.Presupuesto_Ingresos_1 = 1;
            ViewBag.Presupuesto_Ingresos_2 = 1;

            ViewBag.Presupuesto_Egresos_1 = 1;
            ViewBag.Presupuesto_Egresos_2 = 1;

            List<DetallePresupuesto> dp = new List<DetallePresupuesto>();
            try
            {
                dp = db.DetallePresupuesto.Where(d => d.Presupuesto.ProyectoID == proyecto.ID && d.Periodo == Periodo).ToList();
            }
            catch(Exception)
            {}

            try
            {
                int IngresosPresupuesto = dp.Where(m => m.Cuenta.Codigo.StartsWith("1.") && m.Mes >= 1 && m.Mes <= 6).Sum(m => m.Monto);
                ViewBag.Presupuesto_Ingresos_1 = IngresosPresupuesto;
            }
            catch (Exception)
            { }

            try
            {
                int EgresosPresupuesto = dp.Where(m => m.Cuenta.Codigo.StartsWith("6") || m.Cuenta.Codigo.StartsWith("7") || m.Cuenta.Codigo.StartsWith("8") || m.Cuenta.Codigo.StartsWith("9") && m.Mes >= 1 && m.Mes <= 6).Sum(m => m.Monto);
                ViewBag.Presupuesto_Egresos_1 = ViewBag.Presupuesto_Egresos_1 + EgresosPresupuesto;
            }
            catch (Exception)
            { }

            try
            {
                int EgresosPresupuesto = dp.Where(m => m.Cuenta.Codigo.StartsWith("10") && m.Mes == 6).Sum(m => m.Monto);
                ViewBag.Presupuesto_Egresos_1 = ViewBag.Presupuesto_Egresos_1 + EgresosPresupuesto;
            }
            catch (Exception)
            { }

            try
            {
                int IngresosPresupuesto = dp.Where(m => m.Cuenta.Codigo.StartsWith("1.") && m.Mes >= 7 && m.Mes <= 12).Sum(m => m.Monto);
                ViewBag.Presupuesto_Ingresos_2 = IngresosPresupuesto;
            }
            catch (Exception)
            { }

            try
            {
                int EgresosPresupuesto = dp.Where(m => m.Cuenta.Codigo.StartsWith("6") || m.Cuenta.Codigo.StartsWith("7") || m.Cuenta.Codigo.StartsWith("8") || m.Cuenta.Codigo.StartsWith("9") && m.Mes >= 7 && m.Mes <= 12).Sum(m => m.Monto);
                ViewBag.Presupuesto_Egresos_2 = ViewBag.Presupuesto_Egresos_2 + EgresosPresupuesto;
            }
            catch (Exception)
            { }

            try
            {
                int EgresosPresupuesto = dp.Where(m => m.Cuenta.Codigo.StartsWith("10") && m.Mes == 12).Sum(m => m.Monto);
                ViewBag.Presupuesto_Egresos_2 = ViewBag.Presupuesto_Egresos_2 + EgresosPresupuesto;
            }
            catch (Exception)
            { }

            return View();
        }

        public ActionResult Estandares(int Periodo = 0)
        {
            int Mes = (int)Session["Mes"];
            if (Periodo == 0)
            {
                Periodo = (int)Session["Periodo"];
            }
            int PresupuestoID = 0;

            Proyecto proyecto = db.Proyecto.Find(((Proyecto)Session["Proyecto"]).ID);
            IndicadoresGestion Indicadores = new IndicadoresGestion();
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == proyecto.ID && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                PresupuestoID = Presupuesto.ID;
            }

            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el Indicador de Gestion Estandares debe existir un Presupuesto formulado.");
                //prep = 1;
            }



            List<Estandarvalores> EValores = new List<Estandarvalores>();

            try
            {
                ViewBag.NombreProyecto = proyecto.Nombre;
                ViewBag.CodigoSename = proyecto.CodSename;
                ViewBag.CodigoCodeni = proyecto.CodCodeni;
                ViewBag.Periodo = Periodo;
                ViewBag.Ubicacion = proyecto.Direccion.Comuna.Nombre + ", " + proyecto.Direccion.Comuna.Region.Nombre;
                ViewBag.Auditor = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == proyecto.ID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto;
            }
            catch (Exception)
            { }

            try
            {
                Indicadores = db.IndicadoresGestion.Where(i => i.ProyectoID == proyecto.ID && i.Periodo == Periodo).Single();
            }
            catch(Exception)
            {
                Indicadores.Periodo = Periodo;
                Indicadores.ProyectoID = proyecto.ID;
            }

            try
            {
                Indicadores.IngSubvencion = Int32.Parse(proyecto.ValorSubvencion.ToString()) * proyecto.Convenio.NroPlazas;
            }
            catch (Exception)
            {
                Indicadores.IngSubvencion = 0;
            }
            // Capacidad
            Indicadores.CapCobertura = db.Convenio.Where(m => m.ProyectoID == proyecto.ID && m.Periodo == Periodo && m.Mes == Mes).Single().NroPlazas;

            int capIntervenciones = db.Estandarvalores.Where(d => d.EstandarID == 2).Single().predeterminado;

            Indicadores.CapIntervenciones = capIntervenciones;




            // Ingresos
            long? subvencionSename = 0;
            long? Ingresos_sename = 0;
            long? subvencionSenamemes = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 3).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    long? Ingresos_senameC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Ingresos_sename = Ingresos_sename + Ingresos_senameC;
                }
                catch (Exception)
                {
                    Ingresos_sename = Ingresos_sename + 0;
                }

            }
            if (Ingresos_sename != 0)
            {
                Ingresos_sename = Ingresos_sename / 12;
            }

            if (proyecto.ValorSubvencion != null)
            {
                subvencionSename = (Indicadores.CapCobertura * proyecto.ValorSubvencion.Value);
                subvencionSenamemes = (Indicadores.CapCobertura * proyecto.ValorSubvencion.Value);
            }

            subvencionSename = subvencionSename + Ingresos_sename;

            Indicadores.IngSubvencion = int.Parse(Ingresos_sename.ToString());


            decimal? Estandar_4 = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 4).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal? Estandar_PersonalC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_4 = Estandar_4 + Estandar_PersonalC;
                }
                catch (Exception)
                {
                    Estandar_4 = Estandar_4 + 0;
                }

            }
            Estandar_4 = (Estandar_4 / 3);
            Indicadores.IngOtros = int.Parse(Estandar_4.ToString());


            // Costos Reales



            decimal Estandar_Personal = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 5 && d.tipo == 1).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_PersonalC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_Personal = Estandar_Personal + Estandar_PersonalC;
                }
                catch (Exception)
                {
                    Estandar_Personal = Estandar_Personal + 0;
                }

            }
            Estandar_Personal = Estandar_Personal / 4;

            decimal Estandar_Personal2 = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 5 && d.tipo == 3).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_PersonalC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_Personal2 = Estandar_Personal2 + Estandar_PersonalC;
                }
                catch (Exception)
                {
                    Estandar_Personal2 = Estandar_Personal2 + 0;
                }

            }
            Estandar_Personal2 = Estandar_Personal2 / 4;

            decimal CosPersonal = (Estandar_Personal2 / Estandar_Personal) * 100;

            var CosPersonal2 = CosPersonal.ToString("##.#0");

            Indicadores.CosPersonal = decimal.Parse(CosPersonal2);


            decimal Estandar_funcionamiento = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 6 && d.tipo == 1).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_funcionamientoC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_funcionamiento = Estandar_funcionamiento + Estandar_funcionamientoC;
                }
                catch (Exception)
                {
                    Estandar_funcionamiento = Estandar_funcionamiento + 0;
                }

            }
            Estandar_funcionamiento = (Estandar_funcionamiento / 4);
            decimal Cosfuncionamiento = (Estandar_funcionamiento / Estandar_Personal) * 100;
            var CosFuncionamiento2 = Cosfuncionamiento.ToString("##.#0");
            Indicadores.CosFuncionamiento = decimal.Parse(CosFuncionamiento2);

            decimal Estandar_apoyotecnico = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 7).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_apoyotecnicoC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_apoyotecnico = Estandar_apoyotecnico + Estandar_apoyotecnicoC;
                }
                catch (Exception)
                {
                    Estandar_apoyotecnico = Estandar_apoyotecnico + 0;
                }

            }
            Estandar_apoyotecnico = (Estandar_apoyotecnico / 4);
            decimal CosTecnico = (Estandar_apoyotecnico / Estandar_Personal) * 100;
            var Costecnico2 = CosTecnico.ToString("##.#0");

            Indicadores.CosTecnico = decimal.Parse(Costecnico2);

            decimal Estandar_apoyoBeneficiario = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 8).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_apoyoBeneficiarioC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_apoyoBeneficiario = Estandar_apoyoBeneficiario + Estandar_apoyoBeneficiarioC;
                }
                catch (Exception)
                {
                    Estandar_apoyoBeneficiario = Estandar_apoyoBeneficiario + 0;
                }

            }
            try
            {
                Estandar_apoyoBeneficiario = (Estandar_apoyoBeneficiario / 4);
            }
            catch (Exception)
            {
                Indicadores.CosBeneficiario = 0;
            }
            decimal CosBeneficiario = (Estandar_apoyoBeneficiario / Estandar_Personal) * 100;
            var CosBeneficiario2 = CosBeneficiario.ToString("##.#0");

            Indicadores.CosBeneficiario = decimal.Parse(CosBeneficiario2);


            decimal Estandar_Invesion = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 9).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_InvesionC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_Invesion = Estandar_Invesion + Estandar_InvesionC;
                }
                catch (Exception)
                {
                    Estandar_Invesion = Estandar_Invesion + 0;
                }

            }
            try
            {
                decimal CosInversion = ((Estandar_Invesion / 4) / Estandar_Personal) * 100;
                var CosInversion2 = CosInversion.ToString("##.#0");
                Indicadores.CosInversion = decimal.Parse(CosInversion2);
            }
            catch (Exception)
            {
                Indicadores.CosInversion = 0;
            }

            decimal Estandar_fondo_indemnizacion = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 10).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_fondo_indemnizacionC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_fondo_indemnizacion = Estandar_fondo_indemnizacion + Estandar_fondo_indemnizacionC;
                }
                catch (Exception)
                {
                    Estandar_fondo_indemnizacion = Estandar_fondo_indemnizacion + 0;
                }

            }
            try
            {
                Estandar_fondo_indemnizacion = (Estandar_fondo_indemnizacion / 4);
            }
            catch (Exception)
            {
                Estandar_fondo_indemnizacion = 0;
            }
            decimal CosFondo = (Estandar_fondo_indemnizacion / Estandar_Personal) * 100;
            var CosFondo2 = CosFondo.ToString("##.#0");
            Indicadores.CosFondo = decimal.Parse(CosFondo2);

            // Funcionamiento 
            int estandar11 = db.Estandarvalores.Where(d => d.EstandarID == 11).Single().predeterminado;
            Indicadores.FinPrestamos = estandar11;



            int estandar_12 = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 12).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    int Estandar_InvesionC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    estandar_12 = estandar_12 + Estandar_InvesionC;
                }
                catch (Exception)
                {
                    estandar_12 = estandar_12 + 0;
                }

            }
            try
            {
                Indicadores.FinPrestamosOtros = (estandar_12 / 4);
            }
            catch (Exception)
            {
                Indicadores.FinPrestamosOtros = 0;
            }

            //

            int estandar_13 = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 13).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    int Estandar_InvesionC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    estandar_13 = estandar_13 + Estandar_InvesionC;
                }
                catch (Exception)
                {
                    estandar_13 = estandar_13 + 0;
                }

            }
            try
            {
                Indicadores.FinAportes = (estandar_13 / 4);
            }
            catch (Exception)
            {
                Indicadores.FinAportes = 0;
            }


            ///
            int estandar_14 = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 14).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    int Estandar_InvesionC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    estandar_14 = estandar_14 + Estandar_InvesionC;
                }
                catch (Exception)
                {
                    estandar_14 = estandar_14 + 0;
                }

            }
            try
            {
                Indicadores.FinAportesExcendentes = (estandar_14 / 4);
            }
            catch (Exception)
            {
                Indicadores.FinAportesExcendentes = 0;
            }


            int estandar_15 = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 15).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    int Estandar_InvesionC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    estandar_15 = estandar_15 + Estandar_InvesionC;
                }
                catch (Exception)
                {
                    estandar_15 = estandar_15 + 0;
                }

            }
            try
            {
                Indicadores.FinAportesTerceros = (estandar_15 / 4);
            }
            catch (Exception)
            {
                Indicadores.FinAportesTerceros = 0;
            }

            // cambiar
            int estandar16 = db.Estandarvalores.Where(d => d.EstandarID == 16).Single().predeterminado;
            double esttandar_16 = (estandar16 * int.Parse(subvencionSenamemes.ToString())) / 100;
            Indicadores.FinProveedores = int.Parse(esttandar_16.ToString());

            int estandar17 = db.Estandarvalores.Where(d => d.EstandarID == 17).Single().predeterminado;
            Indicadores.CalObjSename = estandar17;

            int estandar18 = db.Estandarvalores.Where(d => d.EstandarID == 18).Single().predeterminado;
            Indicadores.CalRecSename = estandar18;

            int estandar19 = db.Estandarvalores.Where(d => d.EstandarID == 19).Single().predeterminado;
            Indicadores.CalObsSename = estandar19;

            int estandar20 = db.Estandarvalores.Where(d => d.EstandarID == 20).Single().predeterminado;
            Indicadores.CalObjCodeni = estandar20;

            int estandar21 = db.Estandarvalores.Where(d => d.EstandarID == 21).Single().predeterminado;
            Indicadores.CalRecCodeni = estandar21;

            int estandar22 = db.Estandarvalores.Where(d => d.EstandarID == 22).Single().predeterminado;
            Indicadores.CalObsCodeni = estandar22;

            int estandar23 = db.Estandarvalores.Where(d => d.EstandarID == 23).Single().predeterminado;
            Indicadores.CalImpuestos = estandar23;

            int estandar24 = db.Estandarvalores.Where(d => d.EstandarID == 24).Single().predeterminado;
            Indicadores.CalInventario = estandar24;

            decimal Estandar_AporteSubencionPro = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 25 && d.tipo == 1).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_AporteSubencionProC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_AporteSubencionPro = Estandar_AporteSubencionProC + Estandar_AporteSubencionPro;
                }
                catch (Exception)
                {
                    Estandar_AporteSubencionPro = Estandar_AporteSubencionPro + 0;
                }

            }

            decimal Estandar_AporteSubencionPro2 = 0;

            EValores = db.Estandarvalores.Where(d => d.EstandarID == 25 && d.tipo == 2).ToList();
            foreach (var Evalor in EValores)
            {
                try
                {
                    decimal Estandar_AporteSubencionProC = db.DetallePresupuesto.Where(d => d.PresupuestoID == PresupuestoID).Where(d => d.CuentaID == Evalor.CuentaID).Sum(d => d.Monto);
                    Estandar_AporteSubencionPro2 = Estandar_AporteSubencionProC + Estandar_AporteSubencionPro2;
                }
                catch (Exception)
                {
                    Estandar_AporteSubencionPro2 = Estandar_AporteSubencionPro2 + 0;
                }

            }

            decimal ProAporte = (Estandar_AporteSubencionPro / Estandar_AporteSubencionPro2) * 100;
            var ProAporte2 = ProAporte.ToString("##.0");
            Indicadores.ProAporte = decimal.Parse(ProAporte2);

            int estandar26 = db.Estandarvalores.Where(d => d.EstandarID == 26).Single().predeterminado;
            Indicadores.ProHoras = estandar26;
            // cambiar
            int estandar27 = db.Estandarvalores.Where(d => d.EstandarID == 27).Single().predeterminado;
            double estandar_27 = (estandar27 * int.Parse(subvencionSenamemes.ToString())) / 100;
            Indicadores.ProSaldo = int.Parse(estandar_27.ToString());

            int estandar28 = db.Estandarvalores.Where(d => d.EstandarID == 28).Single().predeterminado;
            Indicadores.ProResultado = estandar28;

            int estandar29 = db.Estandarvalores.Where(d => d.EstandarID == 29).Single().predeterminado;
            Indicadores.ProDesviacion = estandar29;

            int estandar30 = db.Estandarvalores.Where(d => d.EstandarID == 30).Single().predeterminado;
            Indicadores.ProDesviacionEgr = estandar30;

            int estandar31 = db.Estandarvalores.Where(d => d.EstandarID == 31).Single().predeterminado;
            Indicadores.ProCostoNino = estandar31;
            return View(Indicadores);
        }

        [HttpPost]
        public ActionResult Estandares(IndicadoresGestion Indicadores)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(Indicadores).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                if (ModelState.IsValid)
                {
                    db.IndicadoresGestion.Add(Indicadores);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Estandares");
        }
    }
}
