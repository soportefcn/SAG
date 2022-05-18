using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;

namespace SAG2.Controllers
{
    public class PermisosController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        
        //
        // GET: /Permisos/

        public ActionResult Index()
        {
           // @ViewBag.Secciones_Mantenedores = db.Seccion.Where(s => s.Menu == "Mantenedores").OrderBy(s => s.Orden).ToList();
            @ViewBag.Secciones_Auditorias = db.Seccion.Where(s => s.Menu == "Auditorías").OrderBy(s => s.Orden).ToList();
            @ViewBag.Secciones_Rendicion = db.Seccion.Where(s => s.Menu == "Rendición").OrderBy(s => s.Orden).ToList();
            @ViewBag.TiposRol = db.TipoRol.OrderBy(t => t.Nombre).ToList();
            return View();
        }

        
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            utils.Log(1, "Inicio de actualizacion de permisos.");
            try
            {
                utils.Log(1, "Intento de borrar datos de tabla de permisos.");
                db.Database.ExecuteSqlCommand("DELETE FROM Permisos");
                utils.Log(1, "Datos borrados.");
                utils.Log(1, "Se recorre coleccion del formulario.");
                foreach (var key in collection.AllKeys)
                {
                    string value = collection[key].ToString();
                    utils.Log(1, "Valor de " + key.ToString() + ": " + value);
                    if (value.Contains("_"))
                    { 
                        int seccionID = Int32.Parse(value.Split('_')[0].ToString());
                        Permiso Permiso = new Permiso();
                        Permiso.SeccionID = seccionID;
                        Permiso.TipoUsuario = value.Split('_')[1].ToString();
                        db.Permiso.Add(Permiso);
                        db.SaveChanges();
                        utils.Log(1, "Permiso guardado.");
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                utils.Log(2, "Error al asignar permisos: " + e.Message);
                if (e.InnerException != null)
                {
                    utils.Log(2, "Error al asignar permisos (Inner): " + e.InnerException.Message);
                }
                return View();
            }
        }

        public bool verificarPermiso(int seccionID, string tipoUsuario)
        {
            try
            {
                Permiso Permiso = db.Permiso.Where(p => p.SeccionID == seccionID).Where(p => p.TipoUsuario == tipoUsuario).Single();
                utils.Log(1, "Permiso encontrado.");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
