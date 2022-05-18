using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using SAG2.Models;
using SAG2.Classes;

namespace SAG2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private Logger log = new Logger();
        private SAG2DB db = new SAG2DB();

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                if (Session["InformacionPie"] == null || Session["Logueado"] == null || Session["Usuario"] == null)
                {
                    Session.Remove("Logueado");
                    Session.Add("InformacionPie", "Usuario no identificado.");
                    string pathLogin = System.Configuration.ConfigurationManager.AppSettings["pathLogin"];
                    //Response.Write(Request.Path.ToString());
                    //Response.End();
                    if (!Request.Path.Equals(pathLogin) && !Request.Path.Contains("/Scripts/") && !Request.Path.Contains("/Content/"))
                    {
                        Response.Redirect(pathLogin, true);
                    }
                } else {
                    string path = Request.Path.Replace(System.Configuration.ConfigurationManager.AppSettings["prePath"], "");
                    if (!path.Equals("/Content/none"))
                    {/*
                        //log.info("Verficacion de permisos para " + path);
                        Usuario usuario = (Usuario)Session["Usuario"];
                        if (!usuario.esAdministrador)
                        {
                            try
                            {
                                string tipoUsuario;
                                if (usuario.esSupervisor)
                                {
                                    tipoUsuario = "S";
                                }
                                else
                                {
                                    tipoUsuario = "U";
                                }

                                //Permiso Permiso = db.Permiso.Where(p => p.TipoUsuario == tipoUsuario).Where(p => path.Contains(p.Seccion.URL)).Single();
                                //log.debug("El usuario esta autorizado para ver " + Permiso.Seccion.Nombre);
                            }
                            catch
                            {
                                //log.debug("El usuario no esta autorizado para ver " + path);
                            }
                        }
                        else
                        { 
                            // El usuario es administrador y puede entrar a todas las secciones.
                        }*/
                    }
                }
            }
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start()
        {
            log.debug("Session start " + Session.SessionID.ToString());
        }

        protected void Session_End()
        {
            log.debug("Session end " + Session.SessionID.ToString());
        }
    }
}