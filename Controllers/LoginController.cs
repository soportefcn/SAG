using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Web.Security;
using System.Web.Script.Serialization;

namespace SAG2.Controllers
{
    public class LoginController : Controller
    {
        private Util utils = new Util();
        private SAG2DB db = new SAG2DB();
        //
        // GET: /Login/

        public ActionResult Index(string mensaje = "", string logout = "")
        {
            if (Session["InformacionPie"] == null)
            {
                Session.Add("InformacionPie", "Usuario no identificado.");
            }
            ViewBag.Mensaje = mensaje;
            return View();
        }

        public void Logout()
        {
            Session.Clear();
            Session.Add("InformacionPie", "Usuario no identificado.");
            Response.Redirect(FormsAuthentication.LoginUrl, false);
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection)
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

            string mensaje = "";
            Usuario usuario = new Usuario();
            string contraseña = utils.md5(collection["Password"].ToString().ToLower());
            string nombreUsuario = collection["NombreUsuario"].ToString().ToLower();
            try
            {
                usuario = (from u in db.Usuario
                           where u.Nombre == nombreUsuario && u.Password == contraseña
                           select u).Single();

                if (!usuario.estaHabilitado)
                {
                    mensaje = utils.mensajeError("Su cuenta se encuentra deshabilitado para ingresar a SAG2.");
                    return Index(mensaje);
                }

                Persona Persona = db.Persona.Find(usuario.PersonaID);
                Session.Add("Persona", Persona);
                Session.Add("Usuario", usuario);
                Session.Add("Logueado", true);

                Rol Rol = new Rol();
                if (usuario.esAdministrador)
                {
                    Session.Add("CambioTipo", true);
                    Session.Add("Rol", Rol);
                    Response.Redirect("./Proyecto", true);
                }
                Proyecto Proyecto = new Proyecto();
                CuentaCorriente CuentaCorriente = new CuentaCorriente();

                // Se obtiene lista de roles del usuario
                List<Rol> Roles = db.Rol.Where(r => r.PersonaID == Persona.ID).ToList();

                if (Roles.Count == 1)
                {
                    // Carga de perfil y establecimiento para cuando tiene un establecimiento asignado
                    Session.Add("Rol", Roles[0]);
                    Proyecto = db.Proyecto.Find(Roles[0].ProyectoID);
                    Session.Add("Proyecto", Proyecto);
                    List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(r => r.ProyectoID == Proyecto.ID).Distinct().ToList();

                    // Si el proyecto está eliminado, no es posible acceder a él.
                    if (Proyecto.estaEliminado)
                    {
                        throw new Exception("El Proyecto asignado se eliminó y no es posible acceder.");
                    }

                    // Si el usuario es normal, no es ni supervisor ni administrador
                    if (!usuario.esSupervisor && !usuario.esAdministrador && Proyecto.estaCerrado)
                    {
                        throw new Exception("El Proyecto asignado está cerrado y no es posible acceder.");
                    }

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
                        Session.Add("Fecha", Meses[mes_actual - 1] + " " + periodo_actual);
                    }
                    catch (Exception e)
                    {
                        utils.Log(1, "No hay periodos cerramos en la base de datos, por lo tanto se selecciona fecha actual. " + e.Message);
                        if (Proyecto.MesInicio == null)
                        {
                            Session.Add("Mes", DateTime.Now.Month);
                            Session.Add("Periodo", DateTime.Now.Year);
                            Session.Add("Fecha", Meses[DateTime.Now.Month - 1] + " " + DateTime.Now.Year);
                        }
                        else
                        {
                            Session.Add("Mes", Proyecto.MesInicio);
                            Session.Add("Periodo", Proyecto.PeriodoInicio);
                            Session.Add("Fecha", Meses[Proyecto.MesInicio.Value - 1] + " " + Proyecto.PeriodoInicio);
                        }
                    }

                    if (CuentasCorrientes.Count == 1)
                    {
                        CuentaCorriente = (CuentaCorriente)db.CuentaCorriente.Where(r => r.ProyectoID == Proyecto.ID).Single();
                        Session.Add("CuentaCorriente", CuentaCorriente);

                        if (usuario.esAdministrador)
                        {
                            Session.Add("InformacionPie", Proyecto.NombreLista + " | " + CuentaCorriente.Banco.Nombre + " " + CuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString() + " | ProyectoID: " + Proyecto.ID + " CtaCteID: " + CuentaCorriente.ID + " PersonaID: " + Persona.ID);
                        }
                        else
                        {
                            Session.Add("InformacionPie", Proyecto.NombreLista + " | " + CuentaCorriente.Banco.Nombre + " " + CuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
                        }
                    }
                    else
                    {
                        utils.Log(2, "LOGIN NOK | El proyecto no tiene asignada ninguna cuenta corriente asignada.");
                        return RedirectToAction("IngresoCuentaCorriente");
                    }
                }
                else if (Roles.Count > 1)
                {
                    Response.Redirect("./Proyecto", true);
                }
                else
                {
                    mensaje = utils.mensajeError("Usted no tiene permisos sobre ningún proyecto.");
                    return Index(mensaje);
                }
            }
            catch (Exception e)
            {
                mensaje = utils.mensajeError("Nombre de usuario o contraseña inválida. " + e.Message);
                utils.Log(2, "LOGIN NOK | " + e.Message + e.StackTrace);
                if (e.InnerException != null)
                {
                    utils.Log(2, "LOGIN NOK | " + e.InnerException.Message + e.StackTrace);
                    if (e.InnerException.Message.Contains("The server was not found"))
                    {
                        mensaje = utils.mensajeError("El servidor de Base de Datos no se encuentra disponible. " + e.InnerException.Message + e.InnerException.StackTrace);
                    }
                }
                return Index(mensaje);
            }

            utils.Log(1, "LOGIN OK | " + usuario.Nombre + " | " + Request.UserAgent + " | " + Request.UserHostAddress);
            Response.Redirect(FormsAuthentication.DefaultUrl, true);
            return Index();
        }


        public ActionResult Proyecto()
        {
            if (Session["Logueado"] == null || (bool)Session["Logueado"] != true)
            {
                Response.Redirect(FormsAuthentication.LoginUrl, true);
            }

            if (Request.UrlReferrer == null || Request.UrlReferrer.ToString().Equals(""))
            {
                Response.Redirect(FormsAuthentication.LoginUrl, true);
            }

            Usuario usuario = (Usuario)Session["Usuario"];
            Persona Persona = (Persona) Session["Persona"];
            ViewBag.Administrador = usuario.esAdministrador;

            if (Request.Cookies["ProyectoUsado"] != null && Request.Cookies["ProyectoUsado"]["ID"] != "")
            {
                ViewBag.ProyectoUsado = Request.Cookies["ProyectoUsado"]["ID"];
            }

            if (usuario.esAdministrador)
            {
                ViewBag.Proyectos = db.Proyecto.Where(p => p.Eliminado == null).OrderBy(p => p.CodCodeni).ToList();
                return View();
            }

            if (usuario.esSupervisor)
            {
                ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
                return View();
            }

            ViewBag.Proyectos = db.Rol.Where(r => r.PersonaID == Persona.ID).Select(r => r.Proyecto).Where(r => r.Eliminado == null && r.Cerrado == null).OrderBy(p => p.CodCodeni).Distinct().ToList();
            return View();
        }

        [HttpPost]
        public void IngresoCuentaCorriente(FormCollection collection)
        {
            if (Request.Form["Numero"] != null && Request.Form["BancoID"] != null && !Request.Form["Numero"].ToString().Equals("") && !Request.Form["BancoID"].ToString().Equals(""))
            {
                Proyecto proyecto = (Proyecto)Session["Proyecto"];
                Persona Persona = (Persona)Session["Persona"];
                Usuario Usuario = (Usuario)Session["Usuario"];

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
                //Session.Add("InformacionPie", (proyecto).NombreLista + " (" + cuentaCorriente.NumeroLista + ") | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());

                if (Usuario.esAdministrador)
                {
                    Session.Add("InformacionPie", proyecto.NombreLista + " | " + cuentaCorriente.Banco.Nombre + " " + cuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString() + " | ProyectoID: " + proyecto.ID + " CtaCteID: " + cuentaCorriente.ID + " PersonaID: " + Persona.ID);
                }
                else
                {
                    Session.Add("InformacionPie", proyecto.NombreLista + " | " + cuentaCorriente.Banco.Nombre + " " + cuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
                }

                Response.Redirect(FormsAuthentication.DefaultUrl, false);
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
                Response.Redirect(FormsAuthentication.LoginUrl, true);
            }
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Proyecto = Proyecto.NombreLista;
            ViewBag.BancoID = new SelectList(db.Banco, "ID", "Nombre");
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
            //return View();
        }

        [HttpPost]
        public void Proyecto(FormCollection collection)
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

            // Carga de perfil y establecimiento para cuando tiene un establecimiento asignado
            if (collection["Proyecto"] == null || collection["Proyecto"].ToString().Equals(""))
            {
                Response.Redirect("./Proyecto", true);
            }

            // El establecimiento debe tener solo una cuenta corriente asociada.
            int proyectoID = Int32.Parse(collection["Proyecto"]);
            Persona Persona = (Persona) Session["Persona"];
            Usuario usuario = (Usuario)Session["Usuario"];
            Rol Rol = new Rol();
            if (!usuario.esAdministrador)
            {
                Rol = (Rol)db.Rol.Where(r => r.PersonaID == Persona.ID).Where(r => r.ProyectoID == proyectoID).Single();
            }

            Proyecto Proyecto = db.Proyecto.Find(proyectoID);
            Session.Add("Proyecto", Proyecto);
            Session.Add("Rol", Rol);

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
                Session.Add("Fecha", Meses[mes_actual - 1] + " " + periodo_actual);
            }
            catch (Exception e)
            {
                utils.Log(1, "No hay periodos cerramos en la base de datos, por lo tanto se selecciona fecha actual. " + e.Message);
                if (Proyecto.MesInicio == null)
                {
                    Session.Add("Mes", DateTime.Now.Month);
                    Session.Add("Periodo", DateTime.Now.Year);
                    Session.Add("Fecha", Meses[DateTime.Now.Month - 1] + " " + DateTime.Now.Year);
                }
                else
                {
                    Session.Add("Mes", Proyecto.MesInicio);
                    Session.Add("Periodo", Proyecto.PeriodoInicio);
                    Session.Add("Fecha", Meses[Proyecto.MesInicio.Value - 1] + " " + Proyecto.PeriodoInicio);
                }
            }

            // Verificacion de Cuenta Corriente
            List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(r => r.ProyectoID == Proyecto.ID).Distinct().ToList();

            if (CuentasCorrientes.Count == 1)
            {
                CuentaCorriente CuentaCorriente = (CuentaCorriente)db.CuentaCorriente.Where(r => r.ProyectoID == Proyecto.ID).Single();
                Session.Add("CuentaCorriente", CuentaCorriente);
                //Session.Add("InformacionPie", Proyecto.NombreLista + " (" + CuentaCorriente.NumeroLista + ") | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
                if (usuario.esAdministrador)
                {
                    Session.Add("InformacionPie", Proyecto.NombreLista + " | " + CuentaCorriente.Banco.Nombre + " " + CuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString() + " | ProyectoID: " + Proyecto.ID + " CtaCteID: " + CuentaCorriente.ID + " PersonaID: " + Persona.ID);
                }
                else
                {
                    Session.Add("InformacionPie", Proyecto.NombreLista + " | " + CuentaCorriente.Banco.Nombre + " " + CuentaCorriente.NumeroLista + " | " + Persona.NombreCompleto + " | " + Session["Fecha"].ToString());
                }
                Response.Redirect(FormsAuthentication.DefaultUrl, false);
            }
            else
            {
                utils.Log(2, "LOGIN NOK | El proyecto no tiene asignada ninguna cuenta corriente asignada.");
                Response.Redirect("./IngresoCuentaCorriente", true);
            }
        }
    }
}
