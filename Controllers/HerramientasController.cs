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
    public class HerramientasController : Controller
    {
        private Util utils = new Util();
        private SAG2DB db = new SAG2DB();
        //
        // GET: /Herramientas/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Propiedades(FormCollection form)
        {
            var Meses = new string[12]
            {
	            "Enero",
	            "Febrero",
	            "Marzo",
	            "Abril",
	            "Mayo",
	            "Junio",
	            "Julio",
	            "Agosto",
	            "Septiembre",
	            "Octubre",
	            "Noviembre",
	            "Diciembre"
	        };
            if (form["Proyecto"] == null || form["Proyecto"].ToString().Equals(""))
            {
                return Propiedades();
            }

            int proyectoID = Int32.Parse(form["Proyecto"]);
            //int CuentaCorrienteID = Int32.Parse(collection["CuentaCorriente"]);
            Persona Persona = (Persona)Session["Persona"];
            Usuario usuario = (Usuario)Session["Usuario"];
            Rol Rol = new Rol();
            if (!usuario.esAdministrador)
            {
                Rol = (Rol)db.Rol.Where(r => r.PersonaID == Persona.ID).Where(r => r.ProyectoID == proyectoID).Single();
            }
            //Rol Rol = (Rol)db.Rol.Where(r => r.PersonaID == Persona.ID).Where(r => r.ProyectoID == EstablecimientoID).Single();
            Session.Add("Rol", Rol);
            Proyecto Proyecto = db.Proyecto.Find(proyectoID);
            Session.Add("Proyecto", Proyecto);

            Response.Cookies["ProyectoUsado"]["ID"] = proyectoID.ToString();
            Response.Cookies["ProyectoUsado"].Expires = DateTime.Now.AddDays(30);

            // Verificamos periodo y mes abierto
            try
            {
                Periodo Periodo = (from p in db.Periodo
                                   where p.ProyectoID == Proyecto.ID
                                   orderby p.Ano descending, p.Mes descending
                                   select p).Take(1).Single();

                int periodo_actual, mes_actual;

                if (Periodo.Mes == 12)
                {
                    mes_actual = 1;
                    periodo_actual = Periodo.Ano + 1;
                }
                else
                {
                    mes_actual = Periodo.Mes + 1;
                    periodo_actual = Periodo.Ano;
                }

                Session.Add("Mes", mes_actual);
                Session.Add("Periodo", periodo_actual);
                Session.Add("Fecha", Meses[mes_actual-1] + " " + periodo_actual);
            }
            catch (Exception e)
            {
                utils.Log(1, "No hay periodos cerramos en la base de datos, por lo tanto se selecciona fecha actual. " + e.Message);
                if (Proyecto.MesInicio == null)
                {
                    Session.Add("Mes", DateTime.Now.Month);
                    Session.Add("Periodo", DateTime.Now.Year);
                    Session.Add("Fecha", Meses[DateTime.Now.Month-1] + " " + DateTime.Now.Year);
                }
                else
                {
                    Session.Add("Mes", Proyecto.MesInicio);
                    Session.Add("Periodo", Proyecto.PeriodoInicio);
                    Session.Add("Fecha", Meses[Proyecto.MesInicio.Value-1] + " " + Proyecto.PeriodoInicio);
                }
            }

            //Session.Add("InformacionPie", Proyecto.NombreLista + " (" + CuentaCorriente.NumeroLista + ") | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
            //ViewBag.Mensaje = utils.mensajeOK("Propiedades cambiadas satisfactoriamente.");

            try
            {
                CuentaCorriente CuentaCorriente = db.CuentaCorriente.Where(c => c.ProyectoID == proyectoID).Single();
                Session.Add("CuentaCorriente", CuentaCorriente);
                if (usuario.esAdministrador)
                {
                    Session.Add("InformacionPie", Proyecto.NombreLista + " | " + CuentaCorriente.Banco.Nombre + " " + CuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString() + " | ProyectoID: " + Proyecto.ID + " CtaCteID: " + CuentaCorriente.ID + " PersonaID: " + Persona.ID);
                }
                else
                {
                    Session.Add("InformacionPie", Proyecto.NombreLista + " | " + CuentaCorriente.Banco.Nombre + " " + CuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
                }
                //Session.Add("InformacionPie", Proyecto.NombreLista + " (" + CuentaCorriente.NumeroLista + ") | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
                return RedirectToAction("../Home");
            }
            catch (Exception)
            {
                utils.Log(2, "LOGIN NOK | El proyecto no tiene asignada ninguna cuenta corriente asignada.");
                return RedirectToAction("IngresoCuentaCorriente");
            }
            
            //return Propiedades();
        }
        // GET: /Herramientas/Propiedades/
        public ActionResult Propiedades()
        {
            Persona Persona = (Persona)Session["Persona"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            Usuario usuario = (Usuario)Session["Usuario"];
            
            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
            }
            else if (usuario.esSupervisor)
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }
            else
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            }
            
            @ViewBag.Proyecto = Proyecto.NombreLista;
            int periodo = (int) Session["Periodo"];
            @ViewBag.NroIngresos = "0";
            @ViewBag.NroEgresos = "0";
            @ViewBag.NroReintegros = "0";
            @ViewBag.NroDeudas = "0";
            if (db.Movimiento.Where(a => a.TipoComprobanteID == 1).Where(a => a.Periodo == periodo).Where(a => a.ProyectoID == Proyecto.ID).Count() > 0)
            {
                @ViewBag.NroIngresos = db.Movimiento.Where(a => a.TipoComprobanteID == 1).Where(a => a.Periodo == periodo).Where(a => a.ProyectoID == Proyecto.ID).Max(a => a.NumeroComprobante);
            }
            if (db.Movimiento.Where(a => a.TipoComprobanteID == 2).Where(a => a.Periodo == periodo).Where(a => a.ProyectoID == Proyecto.ID).Count() > 0)
            {
                @ViewBag.NroEgresos = db.Movimiento.Where(a => a.TipoComprobanteID == 2).Where(a => a.Periodo == periodo).Where(a => a.ProyectoID == Proyecto.ID).Max(a => a.NumeroComprobante);
            }
            if (db.Movimiento.Where(a => a.TipoComprobanteID == 3).Where(a => a.Periodo == periodo).Where(a => a.ProyectoID == Proyecto.ID).Count() > 0)
            {
                @ViewBag.NroReintegros = db.Movimiento.Where(a => a.TipoComprobanteID == 3).Where(a => a.Periodo == periodo).Where(a => a.ProyectoID == Proyecto.ID).Max(a => a.NumeroComprobante);
            }
            if (db.DeudaPendiente.Where(a => a.Periodo == periodo).Where(a => a.ProyectoID == Proyecto.ID).Count() > 0)
            {
                @ViewBag.NroDeudas = db.DeudaPendiente.Where(a => a.Periodo == periodo).Where(a => a.ProyectoID == Proyecto.ID).Max(a => a.NumeroComprobante);
            }

            return View();
        }

        public string CuentaCorriente(int id)
        {
            var CuentasCorrientes = (from c in db.CuentaCorriente
                                     where c.ProyectoID == id
                                     orderby c.Numero descending
                                     select new
                                     {
                                         Value = c.ID,
                                         Text = c.Banco.Nombre + " - " + c.Numero
                                     }).ToList();

            return new JavaScriptSerializer().Serialize(CuentasCorrientes);
        }

        [HttpPost]
        public ActionResult CambioDeContrasena(FormCollection collection)
        {
            string contraseña = utils.md5(collection["Password"].ToString().ToLower());
            string contraseñaNueva = utils.md5(collection["NewPassword"].ToString().ToLower());
            string contraseñaNueva2 = utils.md5(collection["NewPassword2"].ToString().ToLower());
            Usuario usuario = (Usuario)Session["Usuario"];

            if (!contraseña.Equals(usuario.Password))
            {
                 ViewBag.Mensaje = utils.mensajeError("La contraseña actual no es correcta.");
                 return View();
            }

            if (!contraseñaNueva.Equals(contraseñaNueva2))
            {
                ViewBag.Mensaje = utils.mensajeError("Las nuevas contraseñas ingresadas no coinciden.");
                return View();
            }

            Usuario neo = db.Usuario.Find(usuario.ID);
            neo.Password = contraseñaNueva;

            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(neo).State = EntityState.Modified;
                    db.SaveChanges();
                    Session.Remove("Usuario");
                    Session.Add("Usuario", neo);
                    ViewBag.Mensaje = utils.mensajeOK("Contraseña cambiada satisfactoriamente.");
                    return View();
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = utils.mensajeOK("Ocurrió un error al intentar cambiar la contraseña, intente nuevamente. " + e.Message);
                    return View();
                }
            } 
            else
            {
                ViewBag.Mensaje = utils.mensajeError("Ocurrió un error al intentar cambiar la contraseña, intente nuevamente.");
                return View();
            }
        }

        [HttpGet]
        public ActionResult CambioDeContrasena()
        {
            return View();
        }

        [HttpPost]
        public void IngresoCuentaCorriente(FormCollection collection)
        {
            if (Request.Form["Numero"] != null && Request.Form["BancoID"] != null && !Request.Form["Numero"].ToString().Equals("") && !Request.Form["BancoID"].ToString().Equals(""))
            {
                Proyecto proyecto = (Proyecto)Session["Proyecto"];
                Persona Persona = (Persona)Session["Persona"];

                Direccion direccionCtaCte = new Direccion();
                direccionCtaCte.Mostrar = 1;
                direccionCtaCte.ComunaID = 1;
                db.Direccion.Add(direccionCtaCte);
                db.SaveChanges();

                CuentaCorriente cuentaCorriente = new CuentaCorriente();
                cuentaCorriente.ProyectoID = proyecto.ID;
                cuentaCorriente.BancoID = Int32.Parse(Request.Form["BancoID"].ToString());
                cuentaCorriente.Numero = Request.Form["Numero"].ToString();
                cuentaCorriente.Desactiva = 0;
                cuentaCorriente.LineaCredito = 0;
                cuentaCorriente.DireccionID = direccionCtaCte.ID;

                db.CuentaCorriente.Add(cuentaCorriente);
                db.SaveChanges();

                Saldo saldo = new Saldo();
                saldo.CuentaCorrienteID = cuentaCorriente.ID;
                saldo.Mes = Int32.Parse(Session["Mes"].ToString());
                saldo.Periodo = Int32.Parse(Session["Periodo"].ToString());

                if (Request.Form["SaldoInicial"] != null && !Request.Form["SaldoInicial"].ToString().Equals("") && !Request.Form["SaldoInicial"].ToString().Equals("0"))
                {
                    saldo.SaldoInicialCartola = Int32.Parse(Request.Form["SaldoInicial"].ToString());
                    saldo.SaldoFinal = saldo.SaldoInicialCartola;
                    //saldo.SaldoFinalCartola = saldo.SaldoInicialCartola;
                }
                else
                {
                    saldo.SaldoInicialCartola = 0;
                    saldo.SaldoFinal = 0;
                    //saldo.SaldoFinalCartola = 0;
                }

                db.Saldo.Add(saldo);
                db.SaveChanges();
                Session.Add("CuentaCorriente", cuentaCorriente);
                Session.Add("InformacionPie", (proyecto).NombreLista + " | " + cuentaCorriente.Banco.Nombre + " " + cuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
                Response.Redirect("../Home", false);
            }
            else
            {
                Response.Redirect("./IngresoCuentaCorriente", false);
            }
        }

        public ActionResult IngresoCuentaCorriente()
        {
            if (Session["Logueado"] == null || (bool)Session["Logueado"] != true)
            {
                Response.Redirect("../Login", true);
            }
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Proyecto = Proyecto.NombreLista;
            ViewBag.BancoID = new SelectList(db.Banco, "ID", "Nombre");
            return View();
        }
    }
}
