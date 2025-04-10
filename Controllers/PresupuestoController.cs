using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Data;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;

namespace SAG2.Controllers
{
    public class PresupuestoController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Constantes ctes = new Constantes();
        private Util utils = new Util();
        LogReg ArchHivo = new LogReg();
        // public int pr_id;
        //
        // GET: /Presupuesto/Control
        public ActionResult ExportarExcel()
        {
            return Control();
        }

        [HttpPost]
        public ActionResult Control(FormCollection form)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            int pr_id;
            int Periodo = 0;
            if (Periodo == 0)
            {
                Periodo = Int32.Parse(form["periodoControl"].ToString());
            }
            pr_id = Int32.Parse(form["Proyectos2"].ToString());
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
           
            if (!usuario.esUsuario)
            {

                ViewBag.Proyectos = utils.FiltroProyecto(filtro);  

            }
            else
            {
          
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                
            }


            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = pr_id;
            // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();


                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }
       
        public ActionResult Control(int Periodo = 0, int pr_id = 0)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];

            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            if (!usuario.esUsuario)
            {

                ViewBag.Proyectos = utils.FiltroProyecto(filtro);  
            }
            else
            {          
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();              
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = pr_id;
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        // Visualizar Presupuesto
        public ActionResult PresVisualizar(int Periodo = 0, int pr_id = 0)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
           // int Miva = 0;
           
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];

            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
               // Miva = Proyecto.MI;
            }
            if (!usuario.esUsuario)
            {

                ViewBag.Proyectos = utils.FiltroProyecto(filtro);
            }
            else
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = pr_id;
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;



                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }


                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }
           
            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.Presupuesto == 1).OrderBy(c => c.Orden);
           
            return View(cuenta.ToList());
        }

        [HttpPost]
        public ActionResult PresVisualizar(FormCollection form)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());
            int pr_id;
            int Periodo = 0;
            if (Periodo == 0)
            {
                Periodo = Int32.Parse(form["periodoControlPresupuesto"].ToString());
            }
            pr_id = Int32.Parse(form["Proyectos2"].ToString());
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];

            if (!usuario.esUsuario)
            {

                ViewBag.Proyectos = utils.FiltroProyecto(filtro);

            }
            else
            {

                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

            }


            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = pr_id;
            // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();


                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;



                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }




                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }



                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        //
        public ActionResult ControlLinea(int Periodo = 0, int LineaAtencion = 0, int La = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            int Linea = LineaAtencion;
            ViewBag.LineaAtencion = LineaAtencion.ToString();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];


            if (LineaAtencion == 0)
            {

                if (La == 4)
                {
                    Linea = 5;
                    ViewBag.LineaAtencion = "5";
                    LineaAtencion = Linea;
                }
                else if (La == 7)
                {
                    Linea = 0;
                    ViewBag.LineaAtencion = "0";
                    LineaAtencion = Linea;
                }


                else
                {
                    Linea = 9;
                    ViewBag.LineaAtencion = "9";
                    LineaAtencion = Linea;
                }

            }


            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (La == 0)
            {
                La = db.TipoProyecto.Where(d => d.ID == Linea).Take(1).Single().LineasAtencionID;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = LineaAtencion;
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.NombreLinea = db.LineasAtencion.Where(d => d.ID == La).Take(1).Single().Nombre;
            ViewBag.tipopr = db.TipoProyecto.Where(d => d.LineasAtencionID == La).ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);
                    //  ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        [HttpPost]
        public ActionResult ControlLineaRes(FormCollection form)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            int Linea = Int32.Parse(form["lineaProteccionControl"].ToString());
            ViewBag.LineaAtencion = Linea.ToString();
            //    Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            if (usuario.esAdministrador)
            {
                //ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    //ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    //ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }


            int Periodo = Int32.Parse(form["periodoControl"].ToString()); ;

            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = Linea;
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);
                    //  ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ControlLineaRes(int Periodo = 0, int pr_id = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            int Linea = 9;
            ViewBag.LineaAtencion = Linea.ToString();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (pr_id == 0)
            {
                // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = pr_id;
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);
                    //  ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }


        public ActionResult ControlV2()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyecto.ID && m.Activo != null && m.Activo.Equals("S")).OrderByDescending(p => p.ID).Take(1).Single();




                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyecto.ID && m.Activo != null && m.Activo.Equals("S")).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = Presupuesto.Mes_Inicio;
                    int periodo = Presupuesto.Periodo_Inicio;

                    ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                    ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == Proyecto.ID).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();


                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == Proyecto.ID).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult CargaExcel(FormCollection form, HttpPostedFileBase file)
        {
            int periodoPresupuesto = Int32.Parse(form["XperiodoPresupuesto"].ToString());
            int XMesTope = Int32.Parse(form["XMesTope"].ToString());
            List<DetalleArch> ListaData = new List<DetalleArch>();
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Usuario Usuario = (Usuario)Session["Usuario"];
            int proyectoID = Proyecto.ID;
            //
            string pathX = "";
            int ArchivoExcelAct = 0;
            try
            {
                if (file.ContentLength > 0)
                {
                    ArchivoExcelAct = 1;
                    string name = String.Format("PPto" + Proyecto.CodCodeni + " {0}.csv", DateTime.Now.ToString().Replace(":", "."));
                    string _path = Path.Combine(Server.MapPath("~/Archivos"), name);
                    file.SaveAs(_path);
                    System.Threading.Thread.Sleep(1000);
                    pathX = _path;
                    StreamReader reader = new StreamReader(pathX, Encoding.GetEncoding("iso-8859-1"));
                 
                    int linea = 0;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!String.IsNullOrWhiteSpace(line))
                        {
                            string[] values = line.Split(';');
                            string CCuenta = values[0];
                            string CodigoCuenta = "";
                            int largo = CCuenta.Length;
                            int paso = 0;
                            foreach (char c in CCuenta)
                            {
                                if (char.IsNumber(c))
                                {
                                    CodigoCuenta = CodigoCuenta + c;
                                    paso = 1;
                                    if (c == '6') {
                                        paso = 1;
                                    }
                                }
                                else
                                {
                                    if (c.Equals('.'))
                                    {
                                        CodigoCuenta = CodigoCuenta + c;
                                        paso = 1;
                                    }
                                    else
                                    {
                                        if (paso == 1)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            if (CodigoCuenta.Length > 1)
                            {

                                try
                                {
                                    int codigoCuenta = db.Cuenta.Where(d => d.Codigo.Equals(CodigoCuenta)).FirstOrDefault().ID;
                                    for (int i = 0; i < (XMesTope); i++)
                                    {
                                        string signoPeso = "$";
                                        string signopunto = ".";
                                        string Blanco = "";
                                        string mmonto = values[i + 1];
                                        mmonto = mmonto.Replace(signoPeso, Blanco);
                                        mmonto = mmonto.Replace(signopunto, Blanco);

                                        double ValorX = double.Parse(mmonto);
                                        int Valor = (int)Math.Round(ValorX);
                                        if (Valor > 0)
                                        {
                                            DetalleArch data = new DetalleArch();
                                            data.CuentaID = codigoCuenta;
                                            data.Mes = i + 1;
                                            data.Periodo = periodoPresupuesto;
                                            data.Monto = Valor;
                                            ListaData.Add(data);
                                        }

                                    }


                                }
                                catch (Exception)
                                {


                                }

                            }
                        }
                        linea = linea + 1;
                    }
                }
            }
            catch (Exception)
            {
                ArchivoExcelAct = 0;
            }

            if (ArchivoExcelAct != 0) {
                Session.Remove("PptoCsv");
                Session.Add("PptoCsv", ListaData);
            
            
            }


            return RedirectToAction("Formulacion", new { Periodo = periodoPresupuesto });
        }

        //
        // GET: /Presupuesto/Formulacion

        public ActionResult Formulacion(int Periodo = 0)
        {
            int mes = 0;
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
                mes = DateTime.Now.Month;   
            }
            string Confirmado = "N";
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var resol = db.Resolucion.Where(d => d.ProyectoID == Proyecto.ID && d.Estado == 1).FirstOrDefault();
            int PeriodoTermino =0;
            int MesTermino = 0;
            int PeriodoInicio = 0;
            int MesInicio = 0;
            DateTime Fechainicio = DateTime.Parse(Proyecto.Convenio.FechaInicio.ToString());
            PeriodoInicio = Fechainicio.Year;
            MesInicio = Fechainicio.Month;

            if (resol == null)
            {
                try
                {
                    DateTime FechaTermino = DateTime.Parse(Proyecto.Convenio.FechaTermino.ToString());
                    MesTermino = FechaTermino.Month ;
                    PeriodoTermino = FechaTermino.Year;

                }
                catch (Exception)
                {
                    PeriodoTermino = 0;
                    MesTermino = 0;
 

                }
            }
            else {
                DateTime FechaTermino = DateTime.Parse(resol.FechaTermino.ToString());
                MesTermino = FechaTermino.Month ;
                PeriodoTermino = FechaTermino.Year;                     
            }
           
            if (PeriodoTermino > Periodo)
            {
                MesTermino = 13;
            }
            else
            {
                if (PeriodoTermino < Periodo)
                {
                    MesTermino = 0;
                }
          
            }

            if (PeriodoInicio < Periodo)
            {
                MesInicio = 1;
            }
            else
            {
                if (PeriodoInicio > Periodo)
                {
                    MesInicio = 13;
                }

            }
            ViewBag.MesTermino = MesTermino; 
            ViewBag.PresupuestoID = string.Empty;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.InicioMes = MesInicio.ToString();

            List<DetallePresupuesto> dp = new List<DetallePresupuesto>();
            int activo = db.Presupuesto.Where(m => m.ProyectoID == Proyecto.ID && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Count();

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyecto.ID && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                ViewBag.Periodo_Inicio = Periodo.ToString();
                ViewBag.Mes_Inicio = "1";
                ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                // desde Saldo
                if (Presupuesto.Confirmado != null)
                {
                    Confirmado = Presupuesto.Confirmado;
                }
                else
                {
                    Confirmado = "N";
                }
                dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();
            }
            catch (Exception)
            {
                ViewBag.SaldoInicial = 0;
            }
            int PeriodoAnt = Periodo - 1;
            int SaldoInicial = 0;
            try
            {
                SaldoInicial = db.Saldo.Where(d => d.Mes == 12 && d.Periodo == PeriodoAnt && d.CuentaCorriente.ProyectoID == Proyecto.ID).FirstOrDefault().SaldoFinal;

            }
            catch (Exception)
            {
                SaldoInicial = 0;
            }
            string mensajeCSV = ""; 
            ViewBag.comfirmado = Confirmado;
            ViewBag.SaldoInicial = SaldoInicial;
            if (Session["PptoCsv"] != null)
            {
                List<DetallePresupuesto> dppTTO = new List<DetallePresupuesto>();
                List<DetalleArch> lista = new List<DetalleArch>();
                lista = (List<DetalleArch>)Session["PptoCsv"];
                if (lista.Count == 0)
                {
                    ViewBag.Detalle = dp;
                }
                else {
                    mensajeCSV = "Previsualizar CSV";
                    int PresupuestoID = 0;
                    try
                    {
                        PresupuestoID = int.Parse(ViewBag.PresupuestoID);
                    }
                    catch (Exception) {
                        PresupuestoID = 0;
                    }
                    foreach (DetalleArch Data in lista) {
                        DetallePresupuesto Registro = new DetallePresupuesto();
                        Registro.CuentaID = Data.CuentaID;
                        Registro.Mes = Data.Mes;
                        Registro.Monto = Data.Monto;
                        Registro.Periodo = Data.Periodo;
                        Registro.PresupuestoID = PresupuestoID;
                        dppTTO.Add(Registro);
                    
                    
                    }
                    ViewBag.Detalle = dppTTO;
                }
                Session.Remove("PptoCsv");
            }
            else
            {
                ViewBag.Detalle = dp;
            }
            ViewBag.MensajeCSV = mensajeCSV;
            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.Presupuesto == 1).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        [HttpPost]
        public ActionResult Formulacion(FormCollection form)
        {
            int presupuestoID;
            Presupuesto Presupuesto = null;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int mesInicio = Int32.Parse(form["MesInicioPresupuesto"].ToString());
            int periodoPresupuesto = Int32.Parse(form["periodoPresupuesto"].ToString());

            try
            {
                presupuestoID = Int32.Parse(form["PresupuestoID"].ToString());
                Presupuesto = db.Presupuesto.Find(presupuestoID);
                Presupuesto.Periodo = periodoPresupuesto;
                Presupuesto.Mes = mesInicio;
                Presupuesto.SaldoInicial = 0;

                if (form["SaldoInicial"] != null && !form["SaldoInicial"].ToString().Equals(""))
                {
                    Presupuesto.SaldoInicial = Int32.Parse(form["SaldoInicial"].ToString());
                }
                else
                {
                    Presupuesto.SaldoInicial = 0;
                }

                if (ModelState.IsValid)
                {
                    db.Entry(Presupuesto).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                Presupuesto = new Presupuesto();
                Presupuesto.Periodo = periodoPresupuesto;
                Presupuesto.Mes = mesInicio;
                Presupuesto.Activo = "S";
                Presupuesto.ProyectoID = Proyecto.ID;
                Presupuesto.SaldoInicial = Int32.Parse(form["SaldoInicial"].ToString());

                if (ModelState.IsValid)
                {
                    db.Presupuesto.Add(Presupuesto);
                    db.SaveChanges();
                }
            }


            db.Database.ExecuteSqlCommand("DELETE FROM DetallePresupuesto WHERE PresupuestoID = " + Presupuesto.ID);

            foreach (var key in form.AllKeys)
            {
                if (key.Contains("Presupuesto_") && key.Contains("_"))
                {
                    string[] datos = key.Split('_');
                    if ("Presupuesto".Equals(datos[0]) && datos.Count() == 3)
                    {
                        //Presupuesto_7_3
                        int monto = Int32.Parse(form[key].ToString());

                        if (monto == 0)
                        {
                            continue;
                        }

                        DetallePresupuesto dp = new DetallePresupuesto();
                        int mes = Int32.Parse(datos[1]) - 1 + Presupuesto.Mes;
                        int periodo = Presupuesto.Periodo;

                        if (mes > 12)
                        {
                            mes = mes - 12;
                            periodo = periodo + 1;
                        }

                        int cuentaID = Int32.Parse(datos[2]);

                        /*
                        try
                        {
                            dp = db.DetallePresupuesto.Where(m => m.PresupuestoID == Presupuesto.ID).Where(m => m.CuentaID == cuentaID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Single();
                            dp.Monto = monto;

                            if (ModelState.IsValid)
                            {
                                db.Entry(dp).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        catch (Exception)
                        {
                        */
                        dp.PresupuestoID = Presupuesto.ID;
                        dp.CuentaID = cuentaID;
                        dp.Periodo = periodo;
                        dp.Mes = mes;
                        dp.Monto = monto;

                        db.DetallePresupuesto.Add(dp);
                        db.SaveChanges();
                        //}
                    }
                }
            }

            return RedirectToAction("Formulacion", new { Periodo = periodoPresupuesto });
        }

      
        public string ConfirmacionPresupuesto(int PresupuestoID)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Presupuesto Registro = db.Presupuesto.Find(PresupuestoID);
            string Respuesta = "Confirmado";
            if (Registro != null)
            {
                Registro.Confirmado = "S";
                db.Entry(Registro).State = EntityState.Modified;
                db.SaveChanges();

                // Registrar LOG Inicio
                InicioLog log = new InicioLog();
                log.Tipo = "ConfirmacionPresupuesto";
                log.ProyectoId = Registro.ProyectoID;
                log.Fecha = DateTime.Now;
                log.Mes = Registro.Mes;
                log.Periodo = Registro.Periodo;
                log.RegistroID = PresupuestoID;
                log.UsuarioID = usuario.ID;
                log.Descripcion = "Confirmacion Presupuesto  :" + Registro.Proyecto.NombreLista;
                db.InicioLog.Add(log);
                db.SaveChanges();

                Respuesta = "Confirmado";
            }
            else {
                Respuesta = "Sin Presupuesto";
            
            }
            return new JavaScriptSerializer().Serialize(Respuesta);
            
        }

        public string DesConfirmacionPresupuesto(int PresupuestoID)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Presupuesto Registro = db.Presupuesto.Find(PresupuestoID);

            Registro.Confirmado = "N";
            db.Entry(Registro).State = EntityState.Modified;
            db.SaveChanges();

            // Registrar LOG Inicio
            InicioLog log = new InicioLog();
            log.Tipo = "ConfirmacionPresupuesto";
            log.ProyectoId = Registro.ProyectoID;
            log.Fecha = DateTime.Now;
            log.Mes = Registro.Mes;
            log.Periodo = Registro.Periodo;
            log.RegistroID = PresupuestoID;
            log.UsuarioID = usuario.ID;
            log.Descripcion = "Desconfirmado Presupuesto  :" + Registro.Proyecto.NombreLista;
            db.InicioLog.Add(log);
            db.SaveChanges();

            string Respuesta = "Desconfirmado";
            return new JavaScriptSerializer().Serialize(Respuesta);

        }
        
        
        public ActionResult Excel(int Periodo = 0, int pr_id = 0)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();
                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = Presupuesto.Mes_Inicio;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }
 
                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelPresTodos(int Periodo = 0) {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            List<Presupuesto> ProPresupuesto = new List<Presupuesto>();
            List<Presupuesto> Proyectos = new List<Presupuesto>();
            List<DetallePresupuesto> dp = new List<DetallePresupuesto>();
            List<DetallePresupuesto> detallePresupuesto = new List<DetallePresupuesto>();
            

            ProPresupuesto = db.Presupuesto.Where(m => m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).ToList();
            foreach (var Registo in ProPresupuesto) {
                int prID = Registo.ProyectoID;
                var xRevisar = Proyectos.Where(d => d.ProyectoID == prID).FirstOrDefault();
                if (xRevisar == null) {
                    Proyectos.Add(Registo);
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Registo.ID && d.Cuenta.Presupuesto == 1).ToList();
                    detallePresupuesto.AddRange(dp);              
                }                                
            }

            ViewBag.Detalle = detallePresupuesto.OrderBy(d => d.PresupuestoID).ToList(); 
            ViewBag.Presupuesto = Proyectos.ToList();
           
           
            return View();
        }

        public ActionResult ExcelRealTodos(int Periodo = 0)
        {
            Usuario Usuario = (Usuario)Session["Usuario"];
            int UsuarioID = Usuario.ID;
            List<InformeCuenta> Registros = new List<InformeCuenta>();

            db.Database.ExecuteSqlCommand("SP_InformeCuenta " + Periodo + ", " + Usuario.ID);
            Registros = db.informeCuenta.Where(d => d.Periodo == Periodo && d.UsuarioID == UsuarioID).ToList();
            return View(Registros);
        }
        public ActionResult ExcelRealTodosDetalle(int Periodo = 0)
        {
            Usuario Usuario = (Usuario)Session["Usuario"];
            int UsuarioID = Usuario.ID;
            List<InformeCuentaDetalle> Registros = new List<InformeCuentaDetalle>();

            db.Database.ExecuteSqlCommand("SP_InformeCuentaDetalle " + Periodo + ", " + Usuario.ID);
            Registros = db.InformeCuentaDetalle.Where(d => d.Periodo == Periodo && d.UsuarioID == UsuarioID).ToList();
            return View(Registros);
        }
        public ActionResult RealTodosDetalle()
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            int mes = (int)Session["Mes"];
            int año = (int)Session["Periodo"];
            int numberOfDays = DateTime.DaysInMonth(año, mes);
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, numberOfDays);
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int filtro = int.Parse(Session["Filtro"].ToString());

            var pr_id = Proyecto.ID;
            ViewBag.tpID = 0;
            ViewBag.RegID = 0;
            if (!usuario.esUsuario)
            {
                ViewBag.ProyectoID = utils.ProyectoFiltro(filtro, Proyecto.ID);
            }
            else
            {
                ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null && p.ID == Proyecto.ID), "ID", "NombreLista", Proyecto.ID);
            }

            ViewBag.TipoProgramaID = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.regionID = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.PrID = Proyecto.ID.ToString();
            ViewBag.Entrada = 0;
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));


            return View();
        }
        public ActionResult RealTodosDetalleExcel(string Hasta, string Desde, int Cuenta, int ProyectoID, int tipoProyectoID, int RegionID)
        {
            Usuario Usuario = (Usuario)Session["Usuario"];
            int UsuarioID = Usuario.ID;
            List<InformeCuentaDetalle> Registros = new List<InformeCuentaDetalle>();
            //string NombreRegion = "";
            string Sigla = "";
            string Region = "";
            string[] xFecha1 = Hasta.Split('-');
            string[] xFecha2 = Desde.Split('-');
            Hasta = xFecha1[2] + "/" + xFecha1[1] + "/" + xFecha1[0];
            Desde = xFecha2[2] + "/" + xFecha2[1] + "/" + xFecha2[0];
            DateTime Hastax = DateTime.Parse(Hasta);
            DateTime DesdeX = DateTime.Parse(Desde);
            if (tipoProyectoID != 0)
            {
                Sigla = db.TipoProyecto.Find(tipoProyectoID).Sigla;
            }
            if (RegionID != 0)
            {
                Region = db.Region.Find(RegionID).Nombre;

            }
            int xPeriodo = 2024;


            db.Database.ExecuteSqlCommand("SP_InformeCuentaDetalle " + xPeriodo + ", " + Usuario.ID);

            Registros = db.InformeCuentaDetalle.Where(d => d.Periodo == xPeriodo && d.UsuarioID == UsuarioID).ToList();
            Registros = Registros.Where(d => d.FechaDoc >= DesdeX).Where(d => d.FechaDoc <= Hastax).ToList();
            // .Where(m => m.Fecha >= Inicio).Where(m => m.Fecha <= Fin)
            if (ProyectoID != 1)
            {
                Registros = Registros.Where(d => d.ProgramaID == ProyectoID).ToList();
            }
            else
            {
                if (tipoProyectoID != 0)
                {
                    Registros = Registros.Where(d => d.TipoPrograma.Equals(Sigla)).ToList();
                }

            }
            if (RegionID != 0)
            {
                Registros = Registros.Where(d => d.Region.Equals(Region)).ToList();
            }
            if (Cuenta != 0)
            {
                Registros = Registros.Where(d => d.CuentaID == Cuenta).ToList();
            }

            return View(Registros);

        }
        public ActionResult ExcelRealPptoTodos(int Periodo = 0)
        {
            Usuario Usuario = (Usuario)Session["Usuario"];
            int UsuarioID = Usuario.ID;
            List<InformeCuentaPto> Registros = new List<InformeCuentaPto>();


            db.Database.ExecuteSqlCommand("SP_InformeCuentaPto " + Periodo + ", " + Usuario.ID);

            Registros = db.informeCuentaPto.Where(d => d.Periodo == Periodo && d.UsuarioID == UsuarioID).ToList();
            return View(Registros);


        }
        
        public ActionResult ExcelPresupuesto(int Periodo = 0, int pr_id = 0)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                 List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = Presupuesto.Mes_Inicio;
                    int periodo = Presupuesto.Periodo_Inicio;


                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }
                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }
                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }



                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelResumenProyecto(int Periodo = 0, int pr_id = 0)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();
                ViewBag.cuentas = db.Cuenta.Where(m => m.resumen != 0).ToList();
                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    ViewBag.NomProyecto = db.Proyecto.Where(a => a.ID == pr_id).Take(1).Single().NombreLista;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = Presupuesto.Mes_Inicio;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelLinea(int Periodo = 0, int Linea = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);
                    //  ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1 ).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelLineaSemestral(int Periodo = 0, int Linea = 0, int se = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            ViewBag.se = se;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {

                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);



                    //  ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1  ).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelLineaTrimestral(int Periodo = 0, int Linea = 0, int tri = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            ViewBag.tri = tri;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = tri;

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {

                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);



                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelLineaResumen(int Periodo = 0, int Linea = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();

            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);
                    //  ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelLineaResumenSemestral(int Periodo = 0, int Linea = 0, int se = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.se = se;
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);

                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelLineaResumenTrimestral(int Periodo = 0, int Linea = 0, int tri = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.tri = tri;
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null && m.Egreso.Periodo == Periodo).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {

                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelLineaResumenInforme(int Periodo = 0, int Linea = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();

            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            @ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);
                    //  ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelLineaResumenInformeSemestral(int Periodo = 0, int Linea = 0, int se = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.se = se;
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            @ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {

                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);


                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }
        public ActionResult ExcelLineaResumenInformeTrimestral(int Periodo = 0, int Linea = 0, int tri = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.tri = tri;
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            @ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {

                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);



                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0 ).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }


        public ActionResult ExcelLineaResumenInformeSinte(int Periodo = 0, int Linea = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();

            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            @ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);
                    //  ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    // ViewBag.SaldoInicial = 0;
                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }
        public ActionResult ExcelLineaResumenInformeSinteSemestral(int Periodo = 0, int Linea = 0, int se = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.se = se;
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            @ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {

                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);


                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }
        public ActionResult ExcelLineaResumenInformeSinteTrimestral(int Periodo = 0, int Linea = 0, int tri = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            // int Linea = 9;
            // TipoProyecto tipoProyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single();
            ViewBag.LineaAtencion = Linea.ToString();
            ViewBag.tri = tri;
            ViewBag.NombreTipoproyecto = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().NombreListaCompleto;
            @ViewBag.NombreLinea = db.TipoProyecto.Where(a => a.ID == Linea).Take(1).Single().LineaAtencion.NombreLista;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            /*  if (pr_id == 0)
              {
                  // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                  pr_id = Proyecto.ID;
              }**/
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }

            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            // Linea = Proyecto.TipoProyectoID;
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {

                    ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).Sum(m => m.SaldoInicial);


                    dp = db.DetallePresupuesto.Where(d => d.Presupuesto.Proyecto.TipoProyectoID == Linea && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelSemestre2(int Periodo = 0, int pr_id = 0)
        {
            int cc_id;
            cc_id = 0;
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();
            var CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == pr_id);
            foreach (SAG2.Models.CuentaCorriente cuentacorr in CuentaCorriente)
            {
                cc_id = cuentacorr.ID;
            }

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                // Obtenemos saldo inicial para Julio (Inicio segundo semestre)
                int añoPresupuesto = Presupuesto.Periodo;
                try
                {
                    ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 7 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                }
                catch (Exception)
                {
                    try
                    {
                        ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 6 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 5 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 4 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 3 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 2 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                                    }
                                    catch (Exception)
                                    {
                                        try
                                        {
                                            ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 1 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                                        }
                                        catch (Exception)
                                        {
                                            ViewBag.SaldoJulio = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();
                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    //ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.Mes_Inicio = 7;
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    //int mes = Presupuesto.Mes_Inicio;
                    int mes = 7;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();
                    /*
                    for (int i = 0; i < 6; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                            continue;
                        }
                        mes++;
                    }*/

                    if (mes <= 6)
                    {
                        mes = mes + 6;
                    }
                    else if (mes == 7)
                    {
                        mes = 1;
                        periodo = periodo + 1;
                    }
                    else if (mes == 8)
                    {
                        mes = 2;
                        periodo = periodo + 1;
                    }
                    else if (mes == 9)
                    {
                        mes = 3;
                        periodo = periodo + 1;
                    }
                    else if (mes == 10)
                    {
                        mes = 4;
                        periodo = periodo + 1;
                    }
                    else if (mes == 11)
                    {
                        mes = 5;
                        periodo = periodo + 1;
                    }
                    else if (mes == 12)
                    {
                        mes = 6;
                        periodo = periodo + 1;
                    }

                    mes = 6;
                    ViewBag.Mes_Inicio = mes;

                    for (int i = 0; i < 6; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelSemestre1(int Periodo = 0, int pr_id = 0)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();
            // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();
                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    //ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.Mes_Inicio = 1;
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    //int mes = Presupuesto.Mes_Inicio;
                    int mes = 1;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 6; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelResumenSemestre1(int Periodo = 0, int pr_id = 0)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();
            // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null).OrderBy(m => m.CuentaIDD).ToList();
                ViewBag.cuentas = db.Cuenta.Where(m => m.resumen != 0).ToList();
                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    //ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.Mes_Inicio = 1;
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    //int mes = Presupuesto.Mes_Inicio;
                    int mes = 1;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 6; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelResumenSemestre2(int Periodo = 0, int pr_id = 0)
        {
            int cc_id = 0;
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();

            var CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == pr_id);
            foreach (SAG2.Models.CuentaCorriente cuentacorr in CuentaCorriente)
            {
                cc_id = cuentacorr.ID;
            }
            // Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 7 && s.Periodo == Periodo).Single().SaldoInicialCartola;

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();
                ViewBag.cuentas = db.Cuenta.Where(m => m.resumen != 0).ToList();
                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    //ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.Mes_Inicio = 1;
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    //int mes = Presupuesto.Mes_Inicio;
                    int mes = 1;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 6; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelBalance(int Periodo = 0, int pr_id = 0)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();
   
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

         
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = Presupuesto.Mes_Inicio;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelBalanceLineaResponsabilidad(int Periodo = 0, int Linea = 5)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyecto.ID && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == Proyecto.ID).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyecto.ID && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = Presupuesto.Mes_Inicio;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == Proyecto.ID).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == Proyecto.ID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }


        public ActionResult ExcelBalanceSemestre1(int Periodo = 0, int pr_id = 0)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.NProy = db.Proyecto.Where(p => p.ID == pr_id).Take(1).Single().Nombre;
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id && m.Reintegro.Periodo == Periodo).Where(m => m.CuentaIDD != null).OrderBy(m => m.CuentaIDD).ToList();
                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = Presupuesto.Mes_Inicio;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }
            ViewBag.NProy = db.Proyecto.Where(p => p.ID == pr_id).Take(1).Single().Nombre;
            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult ExcelBalanceSemestre2(int Periodo = 0, int pr_id = 0)
        {
            int cc_id;
            cc_id = 0;
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            ViewBag.pr_id = pr_id;
            ViewBag.Proyectos = db.Proyecto.Where(p => p.ID == pr_id).OrderBy(p => p.CodCodeni).ToList();
            var CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == pr_id);
            foreach (SAG2.Models.CuentaCorriente cuentacorr in CuentaCorriente)
            {
                cc_id = cuentacorr.ID;
            }



            int añoPresupuesto = Periodo;
            try
            {
                ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 7 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                try
                {
                    ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 6 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                }
                catch (Exception)
                {
                    try
                    {
                        ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 5 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 4 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 3 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 2 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        ViewBag.SaldoJulio = db.Saldo.Where(s => s.CuentaCorrienteID == cc_id && s.Mes == 1 && s.Periodo == añoPresupuesto).Single().SaldoInicialCartola;
                                    }
                                    catch (Exception)
                                    {
                                        ViewBag.SaldoJulio = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }


            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();


                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();


                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Presupuesto.Periodo_Inicio.ToString();
                    ViewBag.Mes_Inicio = Presupuesto.Mes_Inicio.ToString();
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = Presupuesto.Mes_Inicio;
                    int periodo = Presupuesto.Periodo_Inicio;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Temporal == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception e)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + e.StackTrace);
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception ex)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado. " + ex.StackTrace);
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult Graficos()
        {
            return View();
        }

        public ActionResult Balance(int Periodo = 0, int pr_id = 0)
        {
            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona)Session["Persona"];
            int filtro = int.Parse(Session["Filtro"].ToString()); 

            if (pr_id == 0)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                pr_id = Proyecto.ID;
            }
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (!usuario.esUsuario)
            {

                ViewBag.Proyectos = utils.FiltroProyecto(filtro);   
          

            }
            else
            {
                if (usuario.esSupervisor)
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();

                }
                else
                {
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                }
            }
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = pr_id;
            //pr_id = ((SAG2.Models.Proyecto)Session["Proyecto"]).ID;

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();              
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();

                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).Where(c => c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        [HttpPost]
        public ActionResult Balance(FormCollection form)
        {
            int pr_id;
            int Periodo = 0;
            int filtro = int.Parse(Session["Filtro"].ToString());
            Persona Persona = (Persona)Session["Persona"];
             Usuario usuario = (Usuario)Session["Usuario"];
            if (Periodo == 0)
            {
                Periodo = Int32.Parse(form["periodoBalance"].ToString()); ;
            }

            if (!usuario.esUsuario)
            {
                ViewBag.Proyectos = utils.FiltroProyecto(filtro);  
            }
            else
            {    
                    ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();                
            }
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.Mes_Inicio = "1";
            ViewBag.pr_id = Int32.Parse(form["Proyectos2"].ToString());
            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            pr_id = Int32.Parse(form["Proyectos2"].ToString());
            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();

                /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
                */
                ViewBag.Ingresos = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.Reintegros = db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
                ViewBag.ReintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.ProyectoID == pr_id).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == Periodo).OrderBy(m => m.CuentaIDD).ToList();
                /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
                */
                ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.ProyectoID == pr_id && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();

                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    ViewBag.Detalle = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    int mes = 1;
                    int periodo = Periodo;

                    List<int> Ingresos = new List<int>();
                    List<int> Egresos = new List<int>();
                    List<int> Reintegros = new List<int>();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            Ingresos.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Ingresos.Add(0);
                        }

                        try
                        {
                            Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == pr_id).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            Egresos.Add(0);
                        }

                        try
                        {
                            Reintegros.Add(db.Movimiento.Where(m => m.ProyectoID == pr_id).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                        }
                        catch (Exception)
                        {
                            Reintegros.Add(0);
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.MovIngresos = Ingresos;
                    ViewBag.MovEgresos = Egresos;
                    ViewBag.MovReintegros = Reintegros;

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

               // ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.Estado == 1 && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult LineaProteccion(int Periodo = 0, int Linea = 5)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.LineaAtencion = Linea.ToString();

            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
            */
            ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
            */
            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            List<int> Ingresos = new List<int>();
            List<int> Egresos = new List<int>();
            List<int> Reintegros = new List<int>();

            int mes = 1;
            int periodo = Periodo;

            for (int i = 0; i < 12; i++)
            {
                try
                {
                    Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Ingresos.Add(0);
                }

                try
                {
                    Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                }
                catch (Exception)
                {
                    Egresos.Add(0);
                }

                try
                {
                    Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Reintegros.Add(0);
                }
            }

            ViewBag.MovIngresos = Ingresos;
            ViewBag.MovEgresos = Egresos;
            ViewBag.MovReintegros = Reintegros;

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();


                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Periodo.ToString();
                    ViewBag.Mes_Inicio = "1";
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }

                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult LineaProteccionExcel(int Periodo = 0, int Linea = 5)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.LineaAtencion = Linea.ToString();

            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
            */
            ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
            */
            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            List<int> Ingresos = new List<int>();
            List<int> Egresos = new List<int>();
            List<int> Reintegros = new List<int>();

            int mes = 1;
            int periodo = Periodo;

            for (int i = 0; i < 12; i++)
            {
                try
                {
                    Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Ingresos.Add(0);
                }

                try
                {
                    Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                }
                catch (Exception)
                {
                    Egresos.Add(0);
                }

                try
                {
                    Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Reintegros.Add(0);
                }
            }

            ViewBag.MovIngresos = Ingresos;
            ViewBag.MovEgresos = Egresos;
            ViewBag.MovReintegros = Reintegros;

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();


                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Periodo.ToString();
                    ViewBag.Mes_Inicio = "1";
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }



                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult LineaPrevencion(int Periodo = 0, int Linea = 0, int La = 0)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }
            if (La == 0)
            {
                La = db.TipoProyecto.Where(d => d.ID == Linea).Take(1).Single().LineasAtencionID;
            }
            ViewBag.NombreLinea = db.LineasAtencion.Where(d => d.ID == La).Take(1).Single().Nombre;
            ViewBag.tipopr = db.TipoProyecto.Where(d => d.LineasAtencionID == La).ToList();
            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.LineaAtencion = Linea.ToString();

            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
            */
            ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
            */
            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            List<int> Ingresos = new List<int>();
            List<int> Egresos = new List<int>();
            List<int> Reintegros = new List<int>();

            int mes = 1;
            int periodo = Periodo;

            for (int i = 0; i < 12; i++)
            {
                try
                {
                    Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Ingresos.Add(0);
                }

                try
                {
                    Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                }
                catch (Exception)
                {
                    Egresos.Add(0);
                }

                try
                {
                    Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Reintegros.Add(0);
                }
            }

            ViewBag.MovIngresos = Ingresos;
            ViewBag.MovEgresos = Egresos;
            ViewBag.MovReintegros = Reintegros;

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();


                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Periodo.ToString();
                    ViewBag.Mes_Inicio = "1";
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }



                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult LineaProyecto()
        {
            int periodo = (int)Session["Periodo"];
            int Mes = (int)Session["Mes"];
            int mes = Mes;
            int año = periodo;
            int Linea = 1;
            int numberOfDays = DateTime.DaysInMonth(año, mes);
            DateTime Inicio = new DateTime(año, mes, 1);
            DateTime Fin = new DateTime(año, mes, numberOfDays);
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();

            var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.Proyecto.TipoProyectoID == Linea).Where(a => a.Temporal == null && a.Eliminado == null && a.Nulo == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.NumeroComprobante);

            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Periodo == periodo && m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyectoID == Linea && p.Cerrado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Saldos = db.Saldo.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).ToList();
            ViewBag.Mes = mes;
            ViewBag.periodo = periodo;
            ViewBag.Linea = Linea;
            ViewBag.Conceptos = db.Cuenta.Where(m => m.Codigo != "0").Where(m => m.Estado == 1).OrderBy(p => p.Orden).ToList();
            return View(movimientos.ToList());
        }
        [HttpPost]
        public ActionResult LineaProyecto(FormCollection form)
        {
            DateTime desde = DateTime.Parse(form["desde"].ToString());
            DateTime hasta = DateTime.Parse(form["hasta"].ToString());
            int Linea = Int32.Parse(form["TProyecto"].ToString());
            int mes = desde.Month;
            int periodo = desde.Year;
            int filtro = int.Parse(Session["Filtro"].ToString());   

            var movimientos = db.Movimiento.Where(m => m.Fecha >= desde).Where(m => m.Fecha <= hasta).Where(m => m.Proyecto.TipoProyectoID == Linea).Where(a => a.Temporal == null && a.Eliminado == null && a.Nulo == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.NumeroComprobante);
            if (filtro == 1)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null).ToList();
            }
            else
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).ToList();
            }
            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= desde).Where(m => m.Egreso.Fecha <= hasta).Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyectoID == Linea && p.Cerrado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Saldos = db.Saldo.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).ToList();
            ViewBag.Mes = mes;
            ViewBag.periodo = periodo;
            ViewBag.Desde = desde.ToShortDateString();
            ViewBag.Hasta = hasta.ToShortDateString();
            ViewBag.Linea = Linea;
            ViewBag.Conceptos = db.Cuenta.Where(m => m.Codigo != "0").Where(m => m.Estado == 1).OrderBy(p => p.Orden).ToList();
            return View(movimientos.ToList());
        }
        public ActionResult LineaProyectoAnual()
        {
            int periodo = (int)Session["Periodo"];
            int Mes = (int)Session["Mes"];
            int filtro = int.Parse(Session["Filtro"].ToString());   
            int año = periodo;
            int Linea = 1;


            var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Proyecto.TipoProyectoID == Linea).Where(a => a.Temporal == null && a.Eliminado == null && a.Nulo == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.NumeroComprobante);

            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Periodo == periodo).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Proyectos = utils.FiltroProyecto(filtro);   

           // ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyectoID == Linea && p.Cerrado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Saldos = db.Saldo.Where(m => m.Periodo == periodo).ToList();
            //ViewBag.Mes = mes;
            ViewBag.periodo = periodo;
            ViewBag.Linea = Linea;
            ViewBag.Conceptos = db.Cuenta.Where(m => m.Codigo != "0").Where(m => m.Estado == 1).OrderBy(p => p.Orden).ToList();
            return View(movimientos.ToList());
        }
        [HttpPost]
        public ActionResult LineaProyectoAnual(FormCollection form)
        {
            // DateTime desde = DateTime.Parse(form["desde"].ToString());
            // DateTime hasta = DateTime.Parse(form["hasta"].ToString());
            int Linea = Int32.Parse(form["TProyecto"].ToString());
            int filtro = int.Parse(Session["Filtro"].ToString()); 
            int periodo = Int32.Parse(form["anual"].ToString());

            var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Proyecto.TipoProyectoID == Linea).Where(a => a.Temporal == null && a.Eliminado == null && a.Nulo == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.NumeroComprobante);

            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).OrderBy(m => m.Cuenta.Orden).ToList();


            ViewBag.Proyectos = utils.FiltroProyecto(filtro);   

            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Saldos = db.Saldo.Where(m => m.Periodo == periodo).ToList();
            // ViewBag.Mes = mes;
            ViewBag.periodo = periodo;
            //  ViewBag.Desde = desde.ToShortDateString();
            // ViewBag.Hasta = hasta.ToShortDateString();
            ViewBag.Linea = Linea;
            ViewBag.Conceptos = db.Cuenta.Where(m => m.Codigo != "0").Where(m => m.Estado == 1).OrderBy(p => p.Orden).ToList();
            return View(movimientos.ToList());
        }
        public ActionResult LineaProyectoExcel(string inicio, string fin, int Linea)
        {
            DateTime desde = DateTime.Parse(inicio);
            DateTime hasta = DateTime.Parse(fin);
            //int Linea = Int32.Parse(form["TProyecto"].ToString());
            int mes = desde.Month;
            int periodo = desde.Year;


            var movimientos = db.Movimiento.Where(m => m.Fecha >= desde).Where(m => m.Fecha <= hasta).Where(m => m.Proyecto.TipoProyectoID == Linea).Where(a => a.Temporal == null && a.Eliminado == null && a.Nulo == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.NumeroComprobante);

            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Fecha >= desde).Where(m => m.Egreso.Fecha <= hasta).Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.TipoProyectoID == Linea && p.Cerrado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Saldos = db.Saldo.Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).ToList();
            ViewBag.Mes = mes;
            ViewBag.periodo = periodo;
            ViewBag.Desde = desde.ToShortDateString();
            ViewBag.Hasta = hasta.ToShortDateString();
            ViewBag.Linea = Linea;
            ViewBag.Conceptos = db.Cuenta.Where(m => m.Codigo != "0").Where(m => m.Estado == 1).OrderBy(p => p.Orden).ToList();
            return View(movimientos.ToList());
        }
        public ActionResult LineaProyectoExcelAnual(int anual, int Linea)
        {
            int filtro = int.Parse(Session["Filtro"].ToString());   
            int periodo = anual;


            var movimientos = db.Movimiento.Where(m => m.Periodo == periodo).Where(m => m.Proyecto.TipoProyectoID == Linea).Where(a => a.Temporal == null && a.Eliminado == null && a.Nulo == null && ((a.CuentaID != 1 && a.CuentaID != 6) || a.CuentaID == null)).OrderBy(m => m.ProyectoID).ThenBy(m => m.NumeroComprobante);

            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).OrderBy(m => m.Cuenta.Orden).ToList();
           
           
            if (filtro == 1)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null && p.TipoProyectoID == Linea).OrderBy(p => p.CodCodeni).ToList();
            }
            else
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null && p.TipoProyectoID == Linea).OrderBy(p => p.CodCodeni).ToList();
            }
            ViewBag.TipoProyecto = db.TipoProyecto.ToList();
            ViewBag.Saldos = db.Saldo.Where(m => m.Periodo == periodo).ToList();
            // ViewBag.Mes = mes;
            ViewBag.periodo = periodo;

            ViewBag.Linea = Linea;
            ViewBag.Conceptos = db.Cuenta.Where(m => m.Codigo != "0").Where(m => m.Estado == 1).OrderBy(p => p.Orden).ToList();
            return View(movimientos.ToList());
        }
        public ActionResult LineaResponsabilidad(int Periodo = 0, int Linea = 9)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.LineaAtencion = Linea.ToString();

            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
            */
            ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            /*
                SELECT cuentaid, SUM(monto)
                FROM DetalleEgreso
                where CuentaID is not null and Nulo is null
                and MovimientoID in (select ID from Movimiento where Nulo is null)
                group by cuentaID
            */
            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            List<int> Ingresos = new List<int>();
            List<int> Egresos = new List<int>();
            List<int> Reintegros = new List<int>();

            int mes = 1;
            int periodo = Periodo;

            for (int i = 0; i < 12; i++)
            {
                try
                {
                    Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Ingresos.Add(0);
                }

                try
                {
                    Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                }
                catch (Exception)
                {
                    Egresos.Add(0);
                }

                try
                {
                    Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Reintegros.Add(0);
                }
            }

            ViewBag.MovIngresos = Ingresos;
            ViewBag.MovEgresos = Egresos;
            ViewBag.MovReintegros = Reintegros;

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();


                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Periodo.ToString();
                    ViewBag.Mes_Inicio = "1";
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();

                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }



                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult LineaResponsabilidadExcel(int Periodo = 0, int Linea = 9)
        {
            if (Periodo == 0)
            {
                Periodo = DateTime.Now.Year;
            }

            ViewBag.Periodo_Inicio = Periodo.ToString();
            ViewBag.LineaAtencion = Linea.ToString();

            //Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            /*
                SELECT cuentaid, periodo, mes, SUM(monto_ingresos)
                FROM Movimiento
                where CuentaID is not null and Nulo is null
                group by periodo, mes, cuentaID
            */
            ViewBag.Ingresos = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.Reintegros = db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            ViewBag.Egresos = db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == Periodo).OrderBy(m => m.Cuenta.Orden).ToList();

            List<int> Ingresos = new List<int>();
            List<int> Egresos = new List<int>();
            List<int> Reintegros = new List<int>();

            int mes = 1;
            int periodo = Periodo;

            for (int i = 0; i < 12; i++)
            {
                try
                {
                    Ingresos.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Ingresos.Add(0);
                }

                try
                {
                    Egresos.Add(db.DetalleEgreso.Where(m => m.Egreso.Proyecto.TipoProyectoID == Linea).Where(m => m.Egreso.Periodo == periodo).Where(m => m.Egreso.Mes == mes).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null).Sum(m => m.Monto));
                }
                catch (Exception)
                {
                    Egresos.Add(0);
                }

                try
                {
                    Reintegros.Add(db.Movimiento.Where(m => m.Proyecto.TipoProyectoID == Linea).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null).Sum(m => m.Monto_Ingresos));
                }
                catch (Exception)
                {
                    Reintegros.Add(0);
                }
            }

            ViewBag.MovIngresos = Ingresos;
            ViewBag.MovEgresos = Egresos;
            ViewBag.MovReintegros = Reintegros;

            try
            {
                Presupuesto Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();


                List<DetallePresupuesto> dp = new List<DetallePresupuesto>();

                try
                {
                    Presupuesto = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == Linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == Periodo).OrderByDescending(p => p.ID).Take(1).Single();
                    ViewBag.PresupuestoID = Presupuesto.ID.ToString();
                    ViewBag.Periodo_Inicio = Periodo.ToString();
                    ViewBag.Mes_Inicio = "1";
                    ViewBag.SaldoInicial = Presupuesto.SaldoInicial;
                    dp = db.DetallePresupuesto.Where(d => d.PresupuestoID == Presupuesto.ID && d.Cuenta.Presupuesto == 1).ToList();


                    List<int> IngresosPre = new List<int>();
                    List<int> EgresosPre = new List<int>();

                    for (int i = 0; i < 12; i++)
                    {
                        if (mes > 12)
                        {
                            mes = 1;
                            periodo++;
                        }



                        try
                        {
                            IngresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("I")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            IngresosPre.Add(0);
                        }

                        try
                        {
                            EgresosPre.Add(dp.Where(d => d.Cuenta.Tipo.Equals("E")).Where(d => d.Mes == mes).Where(d => d.Periodo == periodo).Sum(m => m.Monto));
                        }
                        catch (Exception)
                        {
                            EgresosPre.Add(0);
                        }

                        mes++;
                    }

                    ViewBag.PreIngresos = IngresosPre;
                    ViewBag.PreEgresos = EgresosPre;
                }
                catch (Exception)
                {
                    ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
                }

                ViewBag.Detalle = dp;
            }
            catch (Exception)
            {
                ViewBag.NoHayPresupuesto = utils.mensajeError("Para poder ver el control debe existir un Presupuesto formulado.");
            }

            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9") && c.CuentaIva == 0).OrderBy(c => c.Orden);
            return View(cuenta.ToList());
        }

        public ActionResult CSVIngresosGastos(int programa, int mes, bool todos = false)
        {
            string NombreArch = "Hivo-" + programa + "_" + mes;
            string datosRegistrar = "";
            ArchHivo.abrirArchivo(NombreArch);
            int PeriodoRevisar = DateTime.Now.Year;
            string Proyecto = ((SAG2.Models.Proyecto)Session["Proyecto"]).NombreLista;
            string texto = string.Empty;
            var Proyectos = db.Proyecto.Where(p => p.Eliminado == null && p.CodCodeni != null).OrderBy(p => p.CodCodeni).ToList();
            string textoCSV = null;
            var cuenta = db.Cuenta.Where(c => !c.Codigo.Equals("0") && !c.Codigo.Equals("7.3.9")).OrderBy(c => c.Orden).ToList();
            var textoCSVLineaPro = "";

            if (todos == true)
            {
                foreach (var proyecto in Proyectos)
                {
                    var cod = proyecto.CodCodeni;
                    // aqui 
                    var Ingresos = db.Movimiento.Where(m => m.ProyectoID == proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == PeriodoRevisar).OrderBy(m => m.Cuenta.Orden).ToList();
                    var Reintegros = db.Movimiento.Where(m => m.ProyectoID == proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == PeriodoRevisar).OrderBy(m => m.Cuenta.Orden).ToList();
                    var Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == PeriodoRevisar).OrderBy(m => m.Cuenta.Orden).ToList();

                    foreach (var item2 in cuenta)
                    {
                        double valor_ingresos = 0;
                        double valor_egresos = 0;
                        double valor_reintegros = 0;
                        try
                        {
                            valor_ingresos = Ingresos.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.Mes == mes).Sum(d => d.Monto_Ingresos);      // db.Movimiento.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.Mes == mes && d.Periodo == PeriodoRevisar && d.ProyectoID == proyecto.ID).Sum(d => d.Monto_Ingresos);
                        }
                        catch (Exception)
                        {
                            valor_ingresos = 0;
                        }
                        try
                        {
                            valor_reintegros = Reintegros.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.TipoComprobanteID == 3 && d.Mes == mes && d.Periodo == PeriodoRevisar).Sum(d => d.Monto_Ingresos);                   //db.Movimiento.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.TipoComprobanteID == 3).Where(d => d.Mes == mes).Where(d => d.Periodo == PeriodoRevisar).Where(p => p.ProyectoID == proyecto.ID).Sum(d => d.Monto_Ingresos);
                        }
                        catch (Exception)
                        {
                            valor_reintegros = 0;
                        }

                        if ((valor_ingresos != 0 && item2.Tipo == "I") || (valor_reintegros != 0 && item2.Tipo == "I") || (valor_ingresos != 0 && item2.Tipo == null) || (valor_reintegros != 0 && item2.Tipo == null))
                        {
                            textoCSVLineaPro += textoCSV + cod + "|" + item2.Codigo + "|" + (valor_ingresos - valor_reintegros).ToString() + "|Glosa Editable|" + mes + "|" + PeriodoRevisar + "\n";
                        }

                        try
                        {
                            valor_egresos = Egresos.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.Egreso.Mes == mes).Sum(d => d.Monto);         //db.DetalleEgreso.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo)).Where(d => d.Egreso.Mes == mes).Where(d => d.Egreso.Periodo == PeriodoRevisar).Where(p => p.Egreso.ProyectoID == proyecto.ID && p.Temporal != "S").Sum(d => d.Monto);
                        }
                        catch (Exception)
                        {
                            valor_egresos = 0;
                        }

                        try
                        {
                            valor_reintegros = Reintegros.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.Mes == mes).Sum(d => d.Monto_Ingresos);    // db.Movimiento.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.TipoComprobanteID == 3).Where(d => d.Mes == mes).Where(d => d.Periodo == PeriodoRevisar).Where(p => p.ProyectoID == proyecto.ID).Sum(d => d.Monto_Ingresos);
                        }
                        catch (Exception)
                        {
                            valor_reintegros = 0;
                        }
                        if ((valor_egresos != 0 && item2.Tipo == "E") || (item2.Tipo == "E" && valor_reintegros != 0))
                        {
                            textoCSVLineaPro += textoCSV + cod + "|" + item2.Codigo + "|" + (valor_egresos - valor_reintegros).ToString() + "|Glosa Editable|" + mes + "|" + PeriodoRevisar + "\n";
                        }
                    }


             
                }
            }
            else
            {
                try
                {

                    var proyecto = db.Proyecto.Where(p => p.Eliminado == null && p.CodCodeni != null && p.ID == programa).OrderBy(p => p.CodCodeni).First();
                    var cod = proyecto.CodCodeni;

                    var Ingresos = db.Movimiento.Where(m => m.ProyectoID  == proyecto.ID  ).Where(m => m.TipoComprobanteID == ctes.tipoIngreso).Where(m => m.CuentaID != null && m.Nulo == null && m.Eliminado == null && m.CuentaID != 1 && m.Nulo == null && m.Periodo == PeriodoRevisar).OrderBy(m => m.Cuenta.Orden).ToList();
                    var Reintegros = db.Movimiento.Where(m => m.ProyectoID == proyecto.ID).Where(m => m.TipoComprobanteID == ctes.tipoReintegro).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == PeriodoRevisar).OrderBy(m => m.Cuenta.Orden).ToList();
                    var Egresos = db.DetalleEgreso.Where(m => m.Egreso.ProyectoID == proyecto.ID).Where(m => m.CuentaID != null).Where(m => m.Nulo == null && m.Egreso.Eliminado == null && m.Egreso.Periodo == PeriodoRevisar).OrderBy(m => m.Cuenta.Orden).ToList();


                    foreach (var item2 in cuenta)
                    {
                        if (item2.Codigo.Equals("7.1.12"))
                        {
                            PeriodoRevisar = DateTime.Now.Year;
                        } 
                        double valor_ingresos = 0;
                        double valor_egresos = 0;
                        double valor_reintegros = 0;
                        try
                        {
                            valor_ingresos = Ingresos.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.Mes == mes).Sum(d => d.Monto_Ingresos);      // db.Movimiento.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.Mes == mes && d.Periodo == PeriodoRevisar && d.ProyectoID == proyecto.ID).Sum(d => d.Monto_Ingresos);
                        }
                        catch (Exception)
                        {
                            valor_ingresos = 0;
                        }
                        try
                        {
                            valor_reintegros = Reintegros.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.TipoComprobanteID == 3 && d.Mes == mes && d.Periodo == PeriodoRevisar).Sum(d => d.Monto_Ingresos);                   //db.Movimiento.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.TipoComprobanteID == 3).Where(d => d.Mes == mes).Where(d => d.Periodo == PeriodoRevisar).Where(p => p.ProyectoID == proyecto.ID).Sum(d => d.Monto_Ingresos);
                        }
                        catch (Exception)
                        {
                            valor_reintegros = 0;
                        }

                        if ((valor_ingresos != 0 && item2.Tipo == "I") || (valor_reintegros != 0 && item2.Tipo == "I") || (valor_ingresos != 0 && item2.Tipo == null) || (valor_reintegros != 0 && item2.Tipo == null))
                        {
                            textoCSVLineaPro += textoCSV + cod + "|" + item2.Codigo + "|" + (valor_ingresos - valor_reintegros).ToString() + "|Glosa Editable|" + mes + "|" + PeriodoRevisar + "\n";
                            datosRegistrar = cod + "|" + item2.Codigo + "|" + (valor_ingresos - valor_reintegros).ToString() + "|Glosa Editable|" + mes + "|" + PeriodoRevisar;
                            ArchHivo.registrar(datosRegistrar); 
                        }

                        try
                        {
                            valor_egresos = Egresos.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.Egreso.Mes == mes).Sum(d => d.Monto);         //db.DetalleEgreso.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo)).Where(d => d.Egreso.Mes == mes).Where(d => d.Egreso.Periodo == PeriodoRevisar).Where(p => p.Egreso.ProyectoID == proyecto.ID && p.Temporal != "S").Sum(d => d.Monto);
                        }
                        catch (Exception)
                        {
                            valor_egresos = 0;
                        }

                        try
                        {
                            valor_reintegros = Reintegros.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.Mes == mes).Sum(d => d.Monto_Ingresos);    // db.Movimiento.Where(d => d.Cuenta.Codigo.Equals(item2.Codigo) && d.TipoComprobanteID == 3).Where(d => d.Mes == mes).Where(d => d.Periodo == PeriodoRevisar).Where(p => p.ProyectoID == proyecto.ID).Sum(d => d.Monto_Ingresos);
                        }
                        catch (Exception)
                        {
                            valor_reintegros = 0;
                        }
                        if ((valor_egresos != 0 && item2.Tipo == "E") || (item2.Tipo == "E" && valor_reintegros != 0))
                        {
                            textoCSVLineaPro += textoCSV + cod + "|" + item2.Codigo + "|" + (valor_egresos - valor_reintegros).ToString() + "|Glosa Editable|" + mes + "|" + PeriodoRevisar + "\n";
                           datosRegistrar = cod + "|" + item2.Codigo + "|" + (valor_egresos - valor_reintegros).ToString() + "|Glosa Editable|" + mes + "|" + PeriodoRevisar;
                            ArchHivo.registrar(datosRegistrar);  
                        }
                    }
                }

                catch (Exception)
                {

                }



            }


                if (textoCSVLineaPro != "")
                {
                    texto = textoCSVLineaPro.ToString();
                }

                string nombre = "IngresosGastos-" + DateTime.Now;

                ViewBag.NombreArchivo = nombre;
               ArchHivo.cerrar();
                ViewBag.Texto = texto;



                return View("ExportarCSV");
        }
    }
}
    


