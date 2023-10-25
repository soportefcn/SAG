using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SAG2.Classes;
using SAG2.Models;
using SAG2.Comun;

namespace SAG2.Controllers
{
    public class HallazgoCierreController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Constantes ctes = new Constantes();
        private Util utils = new Util();
        //
        // GET: /HallazgoCierre/

        public ActionResult Index(int? ProyectoID , int? mes , int? periodo)
        {
            if (ProyectoID == null)
            {
                Proyecto Proyecto = (Proyecto)Session["Proyecto"];
                periodo = (int)Session["Periodo"];
                mes = (int)Session["Mes"];
                ProyectoID = Proyecto.ID;
            }

          
            ViewBag.DesdeMes = mes;
            ViewBag.DesdePeriodo = periodo;           
            ViewBag.ProyectoID = ProyectoID;
            ViewBag.HallazgoCierre = db.HallazgoCierre.Where(d => d.ProyectoID == ProyectoID && d.Periodo == periodo && d.Mes == mes).ToList();
            ViewBag.saldo = db.Saldo.Where(d => d.Mes == mes && d.Periodo == periodo).ToList();
            ViewBag.Cuentas = db.Cuenta.ToList();
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.Cta = db.CuentaCorriente.Where( d => d.ProyectoID == ProyectoID ).ToList();
            ViewBag.SaldosCorpo = db.SaldosCorporativos.Where(p => p.Mes == mes && p.Periodo == periodo).ToList();
            ViewBag.InformeCuenta = db.CinformeCierre.OrderBy(p => p.GinformeID).ThenBy(p => p.CuentaID).ToList();
            
            ViewBag.rol = db.Rol.Where(d => d.TipoRolID == 4 || d.TipoRolID == 7 && d.ProyectoID == ProyectoID).ToList();
            ViewBag.per = db.Persona.ToList();
            ViewBag.ingresos = db.Movimiento.Where(m => m.Mes == mes && m.Proyecto.Eliminado == null && m.Proyecto.Cerrado == null && m.Periodo == periodo && m.Temporal == null && m.TipoComprobanteID == 1 && m.ProyectoID == ProyectoID).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Mes == mes && e.Egreso.Proyecto.Eliminado == null && e.Egreso.Proyecto.Cerrado == null && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.ProyectoID == ProyectoID).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo && m.ProyectoID == ProyectoID).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Mes == mes).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.ProyectoID == ProyectoID).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.Provision = db.Periodo.Where(d => d.Ano == periodo && d.Mes == mes && d.ProyectoID == ProyectoID).ToList();
            ViewBag.Nocobrados = db.DetalleEgreso.Where(m => m.Egreso.Mes <= mes && m.Egreso.Periodo == periodo && m.Conciliado == null && m.Egreso.TipoComprobanteID == 2 && m.Egreso.Monto_Egresos > 1 && m.Egreso.Conciliado == null && m.Egreso.Eliminado == null && m.Egreso.ProyectoID == ProyectoID).ToList();
            ViewBag.Conciliacion = db.Conciliacion.Where(m => m.Mes == mes && m.Periodo == periodo && m.ProyectoID == ProyectoID).ToList();
            ViewBag.NombreGrupo = db.GinformeCierre.ToList();
            ViewBag.Parametro = db.ParametroInforme.ToList();
            string Auditor = " ";
            try
            {
                Auditor  = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == ProyectoID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto;
            }
            catch (Exception){
                Auditor = " ";
            }
            ViewBag.Auditor = Auditor;
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            try
            {


                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SAG2DB"].ConnectionString);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "execute ChequesNoconciliados " + mes.ToString() + ",  " + periodo.ToString();
                cmd.Connection.Open();
                cmd.ExecuteNonQuery(); sda.Fill(ds);
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excepcion Capturada:{0}", ex.Message.ToString());
                ds = null;
            }
            List<SAG2.Models.chequesNo> datosCheques = new List<SAG2.Models.chequesNo>();
            foreach (DataRow spf in ds.Tables[0].Rows)
            {
                chequesNo data = new chequesNo();
                data.ID = int.Parse(spf["ID"].ToString());
                data.ProyectoID = int.Parse(spf["ProyectoID"].ToString());
                data.Monto = int.Parse(spf["Monto"].ToString());
                datosCheques.Add(data);
            }
            ViewBag.Nocobrados2 = datosCheques ; 




            ViewBag.Conceptosclasificacion = db.ConceptosClasificacion.Where(d => d.Estado == 1).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection form) {

         
            int periodo = Int32.Parse(form["PeriodoCierre"]);
            int mes = Int32.Parse(form["MesCierre"]);
            int ProyectoID = Int32.Parse(form["ProyectoCierreID"]);

            Proyecto Proyecto = db.Proyecto.Find(ProyectoID);

           

            ViewBag.DesdeMes = mes;
            ViewBag.DesdePeriodo = periodo;
            ViewBag.HallazgoCierre = db.HallazgoCierre.Where(d => d.ProyectoID == ProyectoID && d.Periodo == periodo && d.Mes == mes).ToList();
            ViewBag.ProyectoID = ProyectoID;
            ViewBag.saldo = db.Saldo.Where(d => d.Mes == mes && d.Periodo == periodo).ToList();
            ViewBag.Cuentas = db.Cuenta.ToList();
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.Cta = db.CuentaCorriente.Where(d => d.ProyectoID == ProyectoID).ToList();
            ViewBag.SaldosCorpo = db.SaldosCorporativos.Where(p => p.Mes == mes && p.Periodo == periodo).ToList();
            ViewBag.InformeCuenta = db.CinformeCierre.OrderBy(p => p.GinformeID).ThenBy(p => p.CuentaID).ToList();

            ViewBag.rol = db.Rol.Where(d => d.TipoRolID == 4 || d.TipoRolID == 7 && d.ProyectoID == ProyectoID).ToList();
            ViewBag.per = db.Persona.ToList();
            ViewBag.ingresos = db.Movimiento.Where(m => m.Mes == mes && m.Proyecto.Eliminado == null && m.Proyecto.Cerrado == null && m.Periodo == periodo && m.Temporal == null && m.TipoComprobanteID == 1 && m.ProyectoID == ProyectoID).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Mes == mes && e.Egreso.Proyecto.Eliminado == null && e.Egreso.Proyecto.Cerrado == null && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null && e.Egreso.ProyectoID == ProyectoID).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo && m.ProyectoID == ProyectoID).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Mes == mes).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo && m.Reintegro.ProyectoID == ProyectoID).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.Provision = db.Periodo.Where(d => d.Ano == periodo && d.Mes == mes && d.ProyectoID == ProyectoID).ToList();
            ViewBag.Nocobrados = db.DetalleEgreso.Where(m => m.Egreso.Mes <= mes && m.Egreso.Periodo == periodo && m.Conciliado == null && m.Egreso.TipoComprobanteID == 2 && m.Egreso.Monto_Egresos > 1 && m.Egreso.Conciliado == null && m.Egreso.Eliminado == null && m.Egreso.ProyectoID == ProyectoID).ToList();
            ViewBag.Conciliacion = db.Conciliacion.Where(m => m.Mes == mes && m.Periodo == periodo && m.ProyectoID == ProyectoID).ToList();
            ViewBag.NombreGrupo = db.GinformeCierre.ToList();
            ViewBag.Parametro = db.ParametroInforme.ToList();
            string Auditor = " ";
            try
            {
                Auditor = db.Rol.Include(r => r.TipoRol).Include(r => r.Persona).Where(r => r.ProyectoID == ProyectoID).Where(r => r.TipoRolID == 4).Single().Persona.NombreCompleto;
            }
            catch (Exception)
            {
                Auditor = " ";
            }
            ViewBag.Auditor = Auditor;
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            try
            {


                cmd.Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SAG2DB"].ConnectionString);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "execute ChequesNoconciliados " + mes.ToString() + ",  " + periodo.ToString();
                cmd.Connection.Open();
                cmd.ExecuteNonQuery(); sda.Fill(ds);
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Excepcion Capturada:{0}", ex.Message.ToString());
                ds = null;
            }
            List<SAG2.Models.chequesNo> datosCheques = new List<SAG2.Models.chequesNo>();
            foreach (DataRow spf in ds.Tables[0].Rows)
            {
                chequesNo data = new chequesNo();
                data.ID = int.Parse(spf["ID"].ToString());
                data.ProyectoID = int.Parse(spf["ProyectoID"].ToString());
                data.Monto = int.Parse(spf["Monto"].ToString());
                datosCheques.Add(data);
            }
            ViewBag.Nocobrados2 = datosCheques;




            ViewBag.Conceptosclasificacion = db.ConceptosClasificacion.Where(d => d.Estado == 1).ToList();
            return View();
        
        
        }

        public ActionResult Ingreso(FormCollection form) {
            Usuario Usuario = (Usuario)Session["Usuario"];
            int i = 0;
            var valor = "";
            int periodo = Int32.Parse(form["Periodo"]);
            int mes = Int32.Parse(form["Mes"]);
            DateTime FechaPeriodo = new DateTime(periodo, mes, 1);
            int ProyectoID = Int32.Parse(form["ProyectoID"]);
            foreach (string dato in form)
            {
                if (dato.Contains("OBS"))
                {
                    string[] words = dato.Split('-');
                    string conceptoID = words[1];
                    string Indice = words[0];
                    Indice = Indice.Substring(3,( Indice.Length - 3));
                    valor = form[i];
                    if (valor != "") {
                        int xConceptoID =  int.Parse(conceptoID);
                        int xIndiceID = int.Parse(Indice);
                        HallazgoCierre VerRegistro = db.HallazgoCierre.Where(d => d.Periodo == periodo && d.Mes == mes && d.ProyectoID == ProyectoID && d.ConceptosClasificacionID == xConceptoID && d.Indice == xIndiceID).FirstOrDefault();
                        if (VerRegistro == null)
                        {
                            HallazgoCierre Registro = new HallazgoCierre();
                            Registro.ConceptosClasificacionID = int.Parse(conceptoID);
                            Registro.Indice = int.Parse(Indice);
                            Registro.ProyectoID = ProyectoID;
                            Registro.Periodo = periodo;
                            Registro.Mes = mes;
                            Registro.Fecha = DateTime.Now;
                            Registro.EtapaID = 2;
                            Registro.Observaciones = valor;
                            Registro.Estado = 1;
                            Registro.UsuarioID = Usuario.ID;
                            Registro.FechaPeriodo = FechaPeriodo;
                            db.HallazgoCierre.Add(Registro);
                            db.SaveChanges();
                        }
                        else {
                            VerRegistro.Fecha = DateTime.Now;
                            VerRegistro.Observaciones = valor;                          
                            db.Entry(VerRegistro).State = EntityState.Modified;
                            db.SaveChanges();
                        }               
                    }
                }
                i++;
            }
            return RedirectToAction("Index", new { ProyectoID = ProyectoID, mes = mes , periodo = periodo });
        }
        
        public ActionResult ConsolidadoMensual(int? periodo, int? mes) {
            if (periodo == null)
            {
               
                periodo = (int)Session["Periodo"];
                mes = (int)Session["Mes"];              
            }
            int xperiodo = int.Parse(periodo.ToString());
            int xmes = int.Parse(mes.ToString());
            int numberOfDays = DateTime.DaysInMonth(xperiodo, xmes);
            DateTime Inicio = new DateTime(xperiodo, xmes, 1);
            DateTime Fin = new DateTime(xperiodo, xmes, numberOfDays);
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();

            List<HallazgoCierre> Hallazgo = new List<HallazgoCierre>();
            Hallazgo = db.HallazgoCierre.Where(d => d.Mes == mes && d.Periodo == periodo).ToList();
            ViewBag.periodo = periodo;
            ViewBag.mes = mes;
            return View(Hallazgo);       
        
        }

        public ActionResult ConsolidadoMensualExcel(string Desde = "", string Hasta = "")
        {
            DateTime Inicio = DateTime.Parse(Desde);
            DateTime Fin = DateTime.Parse(Hasta);
            List<HallazgoCierre> Hallazgo = new List<HallazgoCierre>();
        
            Hallazgo = db.HallazgoCierre.Where(d => d.FechaPeriodo >= Inicio && d.FechaPeriodo <= Fin).OrderBy(d => d.FechaPeriodo).ToList();
           
            return View(Hallazgo);
        }
        
        [HttpPost]
        public ActionResult ConsolidadoMensual(FormCollection data)
        {
            DateTime Inicio = DateTime.Parse(data["Desde"]);
            DateTime Fin = DateTime.Parse(data["Hasta"]);
            ViewBag.Desde = data["Desde"];
            ViewBag.Hasta = data["Hasta"];
            //   IniLog = db.InicioLog.Where(d => d.Fecha >= Inicio).Where( d =>  d.Fecha <= Fin).OrderByDescending(d => d.Fecha).ToList();
            List<HallazgoCierre> Hallazgo = new List<HallazgoCierre>();
            Hallazgo = db.HallazgoCierre.Where(d => d.FechaPeriodo >= Inicio && d.FechaPeriodo <= Fin).OrderBy(d => d.FechaPeriodo).ToList();

            return View(Hallazgo);

        }

        public ActionResult HallazgoAvance(int ID) {
            var Hallazgo = db.HallazgoCierre.Find(ID);
            ViewBag.Hallazgo = Hallazgo;
            ViewBag.EtapaID = new SelectList(db.Etapa.Where(d => d.Estado == 1 ).ToList(), "ID", "Descripcion",Hallazgo.EtapaID);   
            return View();
        }

        [HttpPost]
        public ActionResult HallazgoAvance(HallazgoAvance Model) {
            Usuario Usuario = (Usuario)Session["Usuario"];
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"]; 
            Model.UsuarioID = Usuario.ID;
            Model.Fecha = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.HallazgoAvance.Add(Model);

                int HallazgocierreID = Model.HallazgoCierreID;
                int EtapaID = Model.EtapaID;

                HallazgoCierre Hallazgo = db.HallazgoCierre.Find(HallazgocierreID);
                Hallazgo.EtapaID = EtapaID;
                db.Entry(Hallazgo).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Message"] = "Creado con exito " ;
                 periodo = Hallazgo.Periodo;
                 mes = Hallazgo.Mes;

            }

            return RedirectToAction("ConsolidadoMensual", new { mes = mes, periodo = periodo });
        
        }

        public ActionResult HallazgoGrafico() {
           
             int   periodo = (int)Session["Periodo"];             
            
            int xperiodo = int.Parse(periodo.ToString());
            int mes = (int)Session["Mes"]; 
            int xmes = int.Parse(mes.ToString());
            int numberOfDays = DateTime.DaysInMonth(xperiodo, xmes);
            DateTime Inicio = new DateTime(xperiodo, 1, 1);
            DateTime Fin = new DateTime(xperiodo, xmes, numberOfDays);
            ViewBag.Desde = Inicio.ToShortDateString();
            ViewBag.Hasta = Fin.ToShortDateString();
            // Linea 1
            List<DatoGrafico> Linea = new List<DatoGrafico>();
            var LineaDato = (from Hallazgocierre in db.HallazgoCierre
                            join p in db.Proyecto on Hallazgocierre.ProyectoID equals p.ID
                            join tp in db.TipoProyecto on p.TipoProyectoID equals tp.ID
                            join la in db.LineasAtencion on tp.LineasAtencionID equals la.ID
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                            group la by la.Sigla into g
                            select new
                            {
                                Sigla = g.Key,
                                Total = g.Count()
                            }).ToList();
            
            
            foreach (var dato in LineaDato) {
                DatoGrafico xDato = new DatoGrafico();
                xDato.Sigla = dato.Sigla;
                xDato.Total = dato.Total;
                Linea.Add(xDato);
            }


            // Tipo
            List<DatoGrafico> Tipo = new List<DatoGrafico>();
            var TipoDato = (from Hallazgocierre in db.HallazgoCierre
                            join p in db.Proyecto on Hallazgocierre.ProyectoID equals p.ID
                            join tp in db.TipoProyecto on p.TipoProyectoID equals tp.ID
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                            group tp by tp.Sigla into g
                            select new
                            {
                                Sigla = g.Key,
                                Total = g.Count()
                            }).ToList();

            foreach (var dato in TipoDato)
            {
                DatoGrafico xDato = new DatoGrafico();
                xDato.Sigla = dato.Sigla;
                xDato.Total = dato.Total;
                Tipo.Add(xDato);
            }

            // Concepto
            List<DatoGrafico> Concepto = new List<DatoGrafico>();
            var ConceptoDato = (from Hallazgocierre in db.HallazgoCierre
                            join CC in db.ConceptosClasificacion on Hallazgocierre.ConceptosClasificacionID equals CC.ID
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                            group CC by CC.Descripcion into g
                            select new
                            {
                                Sigla = g.Key,
                                Total = g.Count()
                            }).ToList();

            foreach (var dato in ConceptoDato)
            {
                DatoGrafico xDato = new DatoGrafico();
                xDato.Sigla = dato.Sigla;
                xDato.Total = dato.Total;
                Concepto.Add(xDato);
            }

            // Estado
            List<DatoGrafico> Estado = new List<DatoGrafico>();
            var EstadoDato = (from Hallazgocierre in db.HallazgoCierre
                            join E in db.Etapa on Hallazgocierre.EtapaID equals E.ID
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                            group E by E.Descripcion into g
                            select new
                            {
                                Sigla = g.Key,
                                Total = g.Count()
                            }).ToList();
            foreach (var dato in EstadoDato)
            {
                DatoGrafico xDato = new DatoGrafico();
                xDato.Sigla = dato.Sigla;
                xDato.Total = dato.Total;
                Estado.Add(xDato);
            }
            // Grupo Clasificacion
            int Acumulado = 0;
            double Total = db.HallazgoCierre.Where(d => d.FechaPeriodo >= Inicio && d.FechaPeriodo <= Fin).Count();
            List<GraficoPareto> GrupoClasi = new List<GraficoPareto>();
            var GrupoDato = (from Hallazgocierre in db.HallazgoCierre
                             join CC in db.ConceptosClasificacion on Hallazgocierre.ConceptosClasificacionID equals CC.ID
                             join C in db.Clasificacion on CC.ClasificacionID equals C.ID
                             join Gc in db.Grupo on C.GrupoID equals Gc.ID
                             where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                             group Gc by Gc.ID into g
                             select new
                             {
                                 Sigla = g.Key,
                                 Total = g.Count()
                             }).ToList();

            GrupoDato = GrupoDato.OrderByDescending(d => d.Total).ToList();

            foreach (var dato in GrupoDato)
            {
                GraficoPareto xDato = new GraficoPareto();

                Acumulado += dato.Total;

                double TotalFrecuencia = double.Parse(dato.Total.ToString());
                double TAcumulado = double.Parse(Acumulado.ToString());

                double PorcentajeFrecuencia = (TotalFrecuencia / Total) * 100;
                double PorcentajeAcumulado = (TAcumulado / Total) * 100;

                xDato.ClasificacionID = dato.Sigla;
                xDato.Clasificacion = db.Grupo.Find(dato.Sigla).Nombre;
                xDato.Frecuencia = dato.Total;              
                xDato.PorcentajeFrecuencia = PorcentajeFrecuencia;
                xDato.Acumulado = Acumulado;              
                xDato.PorcentajeAcumulado = PorcentajeAcumulado;
                GrupoClasi.Add(xDato);
            }


            ViewBag.LineaGrafico = Linea.ToList();
            ViewBag.TipoGrafico = Tipo.ToList();
            ViewBag.ConceptoGrafico = Concepto.ToList();
            ViewBag.EstadoGrafico = Estado.ToList();
            ViewBag.Grupo = GrupoClasi.ToList();

            return View();
        }


        [HttpPost]
        public ActionResult HallazgoGrafico(FormCollection data)
        {

            DateTime Inicio = DateTime.Parse(data["Desde"]);
            DateTime Fin = DateTime.Parse(data["Hasta"]);
            ViewBag.Desde = data["Desde"];
            ViewBag.Hasta = data["Hasta"];

            // Linea 1
            List<DatoGrafico> Linea = new List<DatoGrafico>();
            var LineaDato = (from Hallazgocierre in db.HallazgoCierre
                             join p in db.Proyecto on Hallazgocierre.ProyectoID equals p.ID
                             join tp in db.TipoProyecto on p.TipoProyectoID equals tp.ID
                             join la in db.LineasAtencion on tp.LineasAtencionID equals la.ID
                             where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                             group la by la.Sigla into g
                             select new
                             {
                                 Sigla = g.Key,
                                 Total = g.Count()
                             }).ToList();


            foreach (var dato in LineaDato)
            {
                DatoGrafico xDato = new DatoGrafico();
                xDato.Sigla = dato.Sigla;
                xDato.Total = dato.Total;
                Linea.Add(xDato);
            }


            // Tipo
            List<DatoGrafico> Tipo = new List<DatoGrafico>();
            var TipoDato = (from Hallazgocierre in db.HallazgoCierre
                            join p in db.Proyecto on Hallazgocierre.ProyectoID equals p.ID
                            join tp in db.TipoProyecto on p.TipoProyectoID equals tp.ID
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                            group tp by tp.Sigla into g
                            select new
                            {
                                Sigla = g.Key,
                                Total = g.Count()
                            }).ToList();

            foreach (var dato in TipoDato)
            {
                DatoGrafico xDato = new DatoGrafico();
                xDato.Sigla = dato.Sigla;
                xDato.Total = dato.Total;
                Tipo.Add(xDato);
            }

            // Concepto
            List<DatoGrafico> Concepto = new List<DatoGrafico>();
            var ConceptoDato = (from Hallazgocierre in db.HallazgoCierre
                                join CC in db.ConceptosClasificacion on Hallazgocierre.ConceptosClasificacionID equals CC.ID
                                where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                                group CC by CC.Descripcion into g
                                select new
                                {
                                    Sigla = g.Key,
                                    Total = g.Count()
                                }).ToList();

            foreach (var dato in ConceptoDato)
            {
                DatoGrafico xDato = new DatoGrafico();
                xDato.Sigla = dato.Sigla;
                xDato.Total = dato.Total;
                Concepto.Add(xDato);
            }

            // Estado
            List<DatoGrafico> Estado = new List<DatoGrafico>();
            var EstadoDato = (from Hallazgocierre in db.HallazgoCierre
                              join E in db.Etapa on Hallazgocierre.EtapaID equals E.ID
                              where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                              group E by E.Descripcion into g
                              select new
                              {
                                  Sigla = g.Key,
                                  Total = g.Count()
                              }).ToList();
            foreach (var dato in EstadoDato)
            {
                DatoGrafico xDato = new DatoGrafico();
                xDato.Sigla = dato.Sigla;
                xDato.Total = dato.Total;
                Estado.Add(xDato);
            }

            // Grupo Clasificacion
            int Acumulado = 0;
            double Total = db.HallazgoCierre.Where(d => d.FechaPeriodo >= Inicio && d.FechaPeriodo <= Fin).Count();
            List<GraficoPareto> GrupoClasi = new List<GraficoPareto>();
            var GrupoDato = (from Hallazgocierre in db.HallazgoCierre
                             join CC in db.ConceptosClasificacion on Hallazgocierre.ConceptosClasificacionID equals CC.ID
                             join C in db.Clasificacion on CC.ClasificacionID equals C.ID
                             join Gc in db.Grupo on C.GrupoID equals Gc.ID
                             where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                             group Gc by Gc.ID into g
                             select new
                             {
                                 Sigla = g.Key,
                                 Total = g.Count()
                             }).ToList();

            foreach (var dato in GrupoDato)
            {
                GraficoPareto xDato = new GraficoPareto();

                Acumulado += dato.Total;

                double TotalFrecuencia = double.Parse(dato.Total.ToString());
                double TAcumulado = double.Parse(Acumulado.ToString());

                double PorcentajeFrecuencia = (TotalFrecuencia / Total) * 100;
                double PorcentajeAcumulado = (TAcumulado / Total) * 100;

                xDato.ClasificacionID = dato.Sigla;
                xDato.Clasificacion = db.Grupo.Find(dato.Sigla).Nombre;
                xDato.Frecuencia = dato.Total;
                xDato.PorcentajeFrecuencia = PorcentajeFrecuencia;
                xDato.Acumulado = Acumulado;
                xDato.PorcentajeAcumulado = PorcentajeAcumulado;
                GrupoClasi.Add(xDato);
            }


            ViewBag.LineaGrafico = Linea.ToList();
            ViewBag.TipoGrafico = Tipo.ToList();
            ViewBag.ConceptoGrafico = Concepto.ToList();
            ViewBag.EstadoGrafico = Estado.ToList();
            ViewBag.Grupo = GrupoClasi.ToList();

            return View();
        }

        [HttpPost]
        public JsonResult GraficoLinea(string Desde = "", string Hasta = "")
        {
            DateTime Inicio = DateTime.Parse(Desde);
            DateTime Fin = DateTime.Parse(Hasta);

            var hallazgo = (from Hallazgocierre in db.HallazgoCierre
                            join p in db.Proyecto on Hallazgocierre.ProyectoID equals p.ID
                               join tp in db.TipoProyecto on  p.TipoProyectoID equals tp.ID 
	                            join la in db.LineasAtencion on tp.LineasAtencionID equals la.ID 
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                                 group la by la.Sigla into g
                                  select new
                                     {   
                                         Sigla = g.Key,
                                         Total = g.Count()
                                     }).ToList();

            return Json(hallazgo, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GraficoTipo(string Desde = "", string Hasta = "")
        {
            DateTime Inicio = DateTime.Parse(Desde);
            DateTime Fin = DateTime.Parse(Hasta);

            var hallazgo = (from Hallazgocierre in db.HallazgoCierre
                            join p in db.Proyecto on Hallazgocierre.ProyectoID equals p.ID
                            join tp in db.TipoProyecto on p.TipoProyectoID equals tp.ID                          
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                            group tp by tp.Sigla into g
                            select new
                            {
                                Sigla = g.Key,
                                Total = g.Count()
                            }).ToList();


            return Json(hallazgo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GraficoConcepto(string Desde = "", string Hasta = "")
        {
            DateTime Inicio = DateTime.Parse(Desde);
            DateTime Fin = DateTime.Parse(Hasta);

            var hallazgo = (from Hallazgocierre in db.HallazgoCierre
                            join CC in db.ConceptosClasificacion on Hallazgocierre.ConceptosClasificacionID equals CC.ID                            
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                            group CC by CC.Descripcion into g
                            select new
                            {
                                Sigla = g.Key,
                                Total = g.Count()
                            }).ToList();


            return Json(hallazgo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GraficoEstado(string Desde = "", string Hasta = "")
        {

            DateTime Inicio = DateTime.Parse(Desde);
            DateTime Fin = DateTime.Parse(Hasta);

            var hallazgo = (from Hallazgocierre in db.HallazgoCierre
                            join E in db.Etapa on Hallazgocierre.EtapaID equals E.ID
                            where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                            group E by E.Descripcion into g
                            select new
                            {
                                Sigla = g.Key,
                                Total = g.Count()
                            }).ToList();


            return Json(hallazgo, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GraficoGrupo(string Desde = "", string Hasta = "")
        {

            DateTime Inicio = DateTime.Parse(Desde);
            DateTime Fin = DateTime.Parse(Hasta);
            List<GraficoPareto> GrupoClasi = new List<GraficoPareto>();
            int Acumulado = 0;
            var hallazgo = (from Hallazgocierre in db.HallazgoCierre
                             join CC in db.ConceptosClasificacion on Hallazgocierre.ConceptosClasificacionID equals CC.ID
                             join C in db.Clasificacion on CC.ClasificacionID equals C.ID
                             join Gc in db.Grupo on C.GrupoID equals Gc.ID
                             where Hallazgocierre.FechaPeriodo >= Inicio && Hallazgocierre.FechaPeriodo <= Fin
                             group Gc by Gc.Nombre into g
                             select new
                             {
                                 Sigla = g.Key,
                                 Total = g.Count()
                             }).ToList();
            hallazgo = hallazgo.OrderByDescending(d => d.Total).ToList();

            foreach (var dato in hallazgo)
            {
                GraficoPareto xDato = new GraficoPareto();
                Acumulado += dato.Total;


                xDato.Clasificacion = dato.Sigla;
                xDato.Frecuencia = dato.Total; 
                xDato.Acumulado = Acumulado;     
                GrupoClasi.Add(xDato);
            }


            return Json(GrupoClasi.ToList(), JsonRequestBehavior.AllowGet);

        }

    }
}
