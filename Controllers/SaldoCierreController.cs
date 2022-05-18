using LumenWorks.Framework.IO.Csv;
using SAG2.Classes;
using SAG2.Models;
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

namespace SAG2.Controllers
{
    public class SaldoCierreController : Controller
    {
        //
        // GET: /SaldoCierre/
        private SAG2DB db = new SAG2DB();
        private Constantes ctes = new Constantes();
        private Util utils = new Util();

        [HttpGet, ActionName("DeleteParametro")]
        public ActionResult DeleteParametroConfirmed(int id)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (!Usuario.esUsuario)
            {
                try
                {
                    ParametroInforme  dato = db.ParametroInforme.Find(id);
                    db.ParametroInforme.Remove(dato);
                    db.SaveChanges();
                    return RedirectToAction("Parametro");
                }
                catch (Exception)
                {
                    TempData["Message"] = "No se puede eliminar ";
                    return RedirectToAction("Parametro");
                }
            }
            else
            {
                TempData["Message"] = "No se puede eliminar ";
                return RedirectToAction("Parametro");
            }
        }

        [HttpPost]
        public ActionResult EditParametros(ParametroInforme  dato)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (ModelState.IsValid)
            {
                if (!Usuario.esUsuario)
                {
                    db.Entry(dato).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + dato.ID;
                    return RedirectToAction("Parametro");
                }
                else
                {
                    TempData["Message"] = "Sin permiso para esta accion";
                }
            }
            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", dato.ProyectoID);
            ViewBag.CInformeID = new SelectList(db.CinformeCierre, "ID", "Nombrecuenta", dato.CInformeID); 
            return View(dato);
        }
        public ActionResult EditParametros(int id)
        {
            ParametroInforme  datos = db.ParametroInforme.Find(id);
           
            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", datos.ProyectoID );
            ViewBag.CInformeID = new SelectList(db.CinformeCierre, "ID", "Nombrecuenta",datos.CInformeID ); 
            return View(datos);
        }

        public ActionResult ListaParametros()
        {

            var datos = db.ParametroInforme.ToList();  
            return View(datos);

        }


        [HttpPost]
        public ActionResult Parametro(ParametroInforme datos )
        {
            var xdatos = db.ParametroInforme.Where(d => d.ProyectoID  == datos.ProyectoID  && d.CInformeID  == datos.CInformeID ).Count();
            if (xdatos == 0)
            {
                db.ParametroInforme.Add(datos);
                db.SaveChanges();
            }
            int ProyectoID = datos.ProyectoID;

            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", ProyectoID);
            ViewBag.CInformeID = new SelectList(db.CinformeCierre, "ID", "Nombrecuenta" );
            return View();
        }

        public ActionResult Parametro()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.ProyectoID = new SelectList(db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni), "ID", "NombreLista", Proyecto.ID);
            ViewBag.CInformeID = new SelectList(db.CinformeCierre, "ID", "Nombrecuenta"); 
            return View(); 
        }

        public ActionResult ReporteCierreExcel(int periodo, int mes)
        {
            // ViewBag.cinforme = db.CinformeCierre.ToList();
            ViewBag.DesdeMes = mes;
            ViewBag.DesdePeriodo = periodo;


            ViewBag.saldo = db.Saldo.Where(d => d.Mes == mes && d.Periodo == periodo).ToList();
            ViewBag.Cuentas = db.Cuenta.ToList();
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.SaldosCorpo = db.SaldosCorporativos.Where(p => p.Mes == mes).ToList();
            ViewBag.InformeCuenta = db.CinformeCierre.OrderBy(p => p.GinformeID).ThenBy(p => p.CuentaID).ToList();
            ViewBag.Cta = db.CuentaCorriente.ToList();
            ViewBag.rol = db.Rol.Where(d => d.TipoRolID == 4 || d.TipoRolID == 7).ToList();
            ViewBag.per = db.Persona.ToList();
            ViewBag.ingresos = db.Movimiento.Where(m => m.Mes == mes && m.Proyecto.Eliminado == null && m.Proyecto.Cerrado == null && m.Periodo == periodo && m.Temporal == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Mes == mes && e.Egreso.Proyecto.Eliminado == null && e.Egreso.Proyecto.Cerrado == null && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Mes == mes).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.Provision = db.Periodo.Where(d => d.Ano == periodo && d.Mes == mes).ToList();
            ViewBag.Nocobrados = db.DetalleEgreso.Where(m => m.Egreso.Mes <= mes && m.Egreso.Periodo == periodo && m.Conciliado == null && m.Egreso.TipoComprobanteID == 2 && m.Egreso.Monto_Egresos > 1 && m.Egreso.Conciliado == null && m.Egreso.Eliminado == null).ToList();
            ViewBag.Conciliacion = db.Conciliacion.Where(m => m.Mes == mes && m.Periodo == periodo).ToList();
            ViewBag.NombreGrupo = db.GinformeCierre.ToList();
            ViewBag.Parametro = db.ParametroInforme.ToList();

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

            return View();

        }

         [HttpPost]
        public ActionResult ReporteCierre(FormCollection form)
        {
            // ViewBag.cinforme = db.CinformeCierre.ToList();
            ViewBag.DesdeMes = int.Parse(form["DMes"].ToString());
            ViewBag.DesdePeriodo = int.Parse(form["DPeriodo"].ToString());
            int mes = int.Parse(form["DMes"].ToString());
            int periodo = int.Parse(form["DPeriodo"].ToString());

            ViewBag.saldo = db.Saldo.Where(d => d.Mes == mes && d.Periodo == periodo).ToList();
            ViewBag.Cuentas = db.Cuenta.ToList();
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null ).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.SaldosCorpo = db.SaldosCorporativos.Where(p => p.Mes == mes).ToList();
            ViewBag.InformeCuenta = db.CinformeCierre.OrderBy(p => p.GinformeID).ThenBy(p => p.CuentaID).ToList();
            ViewBag.Cta = db.CuentaCorriente.ToList();
            ViewBag.rol = db.Rol.Where(d => d.TipoRolID == 4 || d.TipoRolID == 7).ToList();
            ViewBag.per = db.Persona.ToList();
            ViewBag.ingresos = db.Movimiento.Where(m => m.Mes == mes && m.Proyecto.Eliminado == null && m.Proyecto.Cerrado == null && m.Periodo == periodo && m.Temporal == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Mes == mes && e.Egreso.Proyecto.Eliminado == null && e.Egreso.Proyecto.Cerrado == null && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null).ToList();
            ViewBag.reintegros = db.Movimiento.Where(m => m.Mes == mes).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Mes == mes).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.Provision = db.Periodo.Where(d => d.Ano == periodo && d.Mes == mes).ToList();
            ViewBag.Nocobrados = db.DetalleEgreso.Where(m => m.Egreso.Mes <= mes && m.Egreso.Periodo == periodo && m.Conciliado == null && m.Egreso.TipoComprobanteID == 2 && m.Egreso.Monto_Egresos > 1 && m.Egreso.Conciliado == null && m.Egreso.Eliminado == null ).ToList();
            ViewBag.Conciliacion = db.Conciliacion.Where(m => m.Mes == mes && m.Periodo == periodo).ToList();
            ViewBag.NombreGrupo = db.GinformeCierre.ToList();
            ViewBag.Parametro = db.ParametroInforme.ToList();

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

            return View();

        }

        public ActionResult ReporteCierre()
        {
           // ViewBag.cinforme = db.CinformeCierre.ToList();
            ViewBag.DesdeMes = DateTime.Now.Month - 1;
            ViewBag.DesdePeriodo = DateTime.Now.Year;
            int mes = DateTime.Now.Month - 1;
            int periodo = DateTime.Now.Year;
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.saldo = db.Saldo.Where(d => d.Mes == mes && d.Periodo == periodo).ToList();     
            ViewBag.Cuentas = db.Cuenta.ToList();
            ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null ).OrderBy(p => p.CodCodeni).ToList();
            ViewBag.SaldosCorpo = db.SaldosCorporativos.Where(p => p.Mes == mes).ToList();
            ViewBag.InformeCuenta = db.CinformeCierre.OrderBy(p => p.GinformeID).ThenBy(p => p.CuentaID).ToList();  
            ViewBag.Cta = db.CuentaCorriente.ToList();
            ViewBag.rol = db.Rol.Where(d => d.TipoRolID == 4 || d.TipoRolID == 7).ToList();
            ViewBag.per = db.Persona.ToList();
            ViewBag.ingresos = db.Movimiento.Where(m => m.Mes == mes && m.Proyecto.Eliminado == null && m.Proyecto.Cerrado == null && m.Periodo == periodo && m.Temporal == null).ToList();
            ViewBag.egresos = db.DetalleEgreso.Where(e => e.Egreso.Mes == mes && e.Egreso.Proyecto.Eliminado == null && e.Egreso.Proyecto.Cerrado == null && e.Egreso.Periodo == periodo && e.Egreso.Temporal == null).ToList();               
            ViewBag.reintegros = db.Movimiento.Where(m => m.Mes  == mes).Where(m => m.TipoComprobanteID == 3).Where(m => m.CuentaID != null && m.CuentaID != 1 && m.Nulo == null && m.Eliminado == null && m.Temporal == null && m.Periodo == periodo).OrderBy(m => m.Cuenta.Orden).ToList();
            ViewBag.reintegrosGastos = db.DetalleReintegro.Where(m => m.Reintegro.Mes == mes).Where(m => m.CuentaIDD != null && m.Reintegro.Periodo == periodo).OrderBy(m => m.CuentaIDD).ToList();
            ViewBag.Provision = db.Periodo.Where(d => d.Ano == periodo && d.Mes == mes).ToList();
            ViewBag.Nocobrados = db.DetalleEgreso.Where(m => m.Egreso.Mes <= mes && m.Egreso.Periodo == periodo && m.Conciliado == null && m.Egreso.TipoComprobanteID == 2 && m.Egreso.Monto_Egresos > 1 && m.Egreso.Conciliado == null && m.Egreso.Eliminado == null).ToList();
            ViewBag.Conciliacion = db.Conciliacion.Where(m => m.Mes == mes && m.Periodo == periodo).ToList();
            ViewBag.NombreGrupo = db.GinformeCierre.ToList();
            ViewBag.Parametro = db.ParametroInforme.ToList();

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


            return View();

        }


        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (!Usuario.esUsuario)
            {
                try
                {
                    CInformeCierre dato = db.CinformeCierre.Find(id);
                    db.CinformeCierre.Remove(dato );
                    db.SaveChanges();
                    return RedirectToAction("AsociarCuenta");
                }
                catch (Exception)
                {
                    TempData["Message"] = "No se puede eliminar el articulo tiene movimiento en el Sistema de Bodega";
                    return RedirectToAction("Create");
                }
            }
            else
            {
                TempData["Message"] = "No se puede eliminar el articulo tiene movimiento en el Sistema de Bodega";
                return RedirectToAction("Create");
            }
        }

        public ActionResult Edit(int id)
        {
            CInformeCierre  datos = db.CinformeCierre.Find(id);
            ViewBag.GinformeID = new SelectList(db.GinformeCierre , "ID", "Descripcion",datos.GinformeID  );
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "NombreLista", datos.CuentaID);
            return View(datos);
        }
        [HttpPost]
        public ActionResult Edit(CInformeCierre  dato)
        {
            SAG2.Models.Usuario Usuario = (SAG2.Models.Usuario)Session["Usuario"];
            if (ModelState.IsValid)
            {
                if (!Usuario.esUsuario)
                {
                    db.Entry(dato).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Creado con exito " + dato.ID ;
                    return RedirectToAction("AsociarCuenta");
                }
                else
                {
                    TempData["Message"] = "Sin permiso para esta accion";
                }
            }
            ViewBag.GinformeID = new SelectList(db.GinformeCierre, "ID", "Descripcion", dato.GinformeID);
            ViewBag.CuentaID = new SelectList(db.Cuenta, "ID", "NombreLista", dato.CuentaID);
            return View(dato);
        }
        public ActionResult Lista()
        {
            ViewBag.Ginforme = db.GinformeCierre.ToList();
            ViewBag.Cuentas = db.Cuenta.ToList();   
            var datos = db.CinformeCierre.ToList().OrderBy(d => d.GinformeID);  
            return View(datos);

        }
         
        [HttpPost]
        public ActionResult AsociarCuenta(CInformeCierre datos )
       {
           var xdatos = db.CinformeCierre.Where(d => d.CuentaID == datos.CuentaID && d.GinformeID == datos.GinformeID).Count();
            if (xdatos == 0) {
               db.CinformeCierre.Add(datos);
               db.SaveChanges();
            }
           ViewBag.GinformeID = new SelectList(db.GinformeCierre, "ID", "Descripcion");
           ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
           return View();

       }
        public ActionResult AsociarCuenta()
        {
            // ViewBag.Arbol = 
            ViewBag.GinformeID = new SelectList(db.GinformeCierre, "ID", "Descripcion");
            ViewBag.Arbol = utils.generarSelectHijos2(db.Cuenta.Find(ctes.raizCuentaIngresos));
            return View();
        }

        public ActionResult InformeSaldo()
        {
            ViewBag.DesdeMes = DateTime.Now.Month;
            ViewBag.DesdePeriodo = DateTime.Now.Year;
            int mes = DateTime.Now.Month;
            int periodo = DateTime.Now.Year; 
            var Datos = db.SaldosCorporativos.Where(d => d.Mes == mes && d.Periodo == periodo).ToList();
            return View(Datos);
        }
        public ActionResult InformeSaldoExcel(int Periodo, int Mes)
        {
            ViewBag.DesdeMes = Periodo;
            ViewBag.DesdePeriodo = Mes;
            int mes = Mes;
            int periodo = Periodo;
            var Datos = db.SaldosCorporativos.Where(d => d.Mes == mes && d.Periodo == periodo).ToList();
            return View(Datos);
        }
         [HttpPost]
        public ActionResult InformeSaldo(FormCollection Form)
        {
            int Mes = int.Parse(Form["mes"].ToString());
            int Periodo = int.Parse(Form["periodo"].ToString());
            ViewBag.DesdeMes = Mes;
            ViewBag.DesdePeriodo = Periodo;
            var Datos = db.SaldosCorporativos.Where(d => d.Mes == Mes && d.Periodo == Periodo).ToList();

            return View(Datos);
        }

        public ActionResult DetalleImportacion(int Periodo, int Mes )
        {
            var Datos = db.SaldosCorporativos.Where(d => d.Mes == Mes && d.Periodo == Periodo).ToList();

            return View(Datos);
        }

        public ActionResult Index()
        {
             ViewBag.DesdeMes =  DateTime.Now.Month;
             ViewBag.DesdePeriodo = DateTime.Now.Year ;
            
            return View();
        }
         
        [HttpPost]
        public ActionResult Index(FormCollection Form, HttpPostedFileBase upload)
        {

            try
            {
                List<SaldosCorporativos> lista = new List<SaldosCorporativos>();
                int Mes = int.Parse(Form["DMes"].ToString());
                int Periodo = int.Parse(Form["DPeriodo"].ToString());
                Usuario Usuario = (Usuario)Session["Usuario"];
                string pathX = "";

                int xx = db.SaldosCorporativos.Where(d => d.Mes.Equals( Mes) && d.Periodo.Equals( Periodo)).Count();
                if (xx.Equals(0))
                {
                if (upload != null && upload.ContentLength > 0)
                    {
                        if (upload.FileName.EndsWith(".csv"))
                        {
                            try
                            {
                                if (upload.ContentLength > 0)
                                {
                                    string name = String.Format("Informe de atención Mensual {0}.csv", DateTime.Now.ToString().Replace(":", "."));
                                    string _path = Path.Combine(Server.MapPath("~/Archivos"), name);
                                    upload.SaveAs(_path);
                                    System.Threading.Thread.Sleep(1000);

                                    pathX = _path;
                                }
                            }
                            catch (Exception ex)
                            {
                                TempData["Message"] = "Error, FALLO GUARDADO." + ex.Message;
                            }
                            StreamReader reader = new StreamReader(pathX, Encoding.GetEncoding("iso-8859-1"));
    

                            //string vara1, vara2, vara3, vara4;
                            int linea = 0;
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                if (!String.IsNullOrWhiteSpace(line))
                                {
                                    if (linea > 0)
                                    {
                                        SaldosCorporativos sn = new SaldosCorporativos();

                                        string[] values = line.Split(';');
               
                                        {
                                           // sn.ID = 1;
                                            sn.RutEmpresa = values[0];
                                            sn.NombreEmpresa = values[1];
                                            sn.Numerocuenta =  values[2];
                                            sn.Moneda = values[3];                                           
                                            sn.SaldoContable = double.Parse(values[4]);
                                            sn.Retencion = double.Parse(values[5]);
                                            sn.Retencion1 = double.Parse(values[6]);
                                            sn.Saldo_Contable = double.Parse(values[7]);
                                            sn.Saldo_Disponible = double.Parse(values[8]);
                                            sn.Periodo = Periodo;
                                            sn.Fecha = DateTime.Now;
                                            sn.UsuarioID = Usuario.ID;
                                            sn.Mes = Mes; 
                                        }
                                        db.SaldosCorporativos.Add(sn);
                                        db.SaveChanges(); 
                                    }
                                    linea++;
                                }
                            }







                            return RedirectToAction("DetalleImportacion", new {  Periodo = Periodo  , Mes = Mes});
                        }
                        else
                        {
                            TempData["Message"] += "Formato de archivo no soportado.";
                            return RedirectToAction("index");
                        }
                    }
                    else
                    {
                        TempData["Message"] += "Seleccione su archivo.";
                    }
                        }
                    else
                    {
                        TempData["Message"] += "Ya existe mes ";
                    }
                }
            
            catch
            {
                TempData["Message"] += "Error, verifique los datos del archivo.";
            }
            return RedirectToAction("index"); ;
        }
    }


}
