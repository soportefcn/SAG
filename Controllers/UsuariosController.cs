using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;
using System.Web.Script.Serialization;

namespace SAG2.Controllers
{ 
    public class UsuariosController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();

        //
        // GET: /Usuarios/

        public ViewResult Index(string q = "")
        {
            var usuario = db.Usuario.Include(u => u.Persona).OrderBy(u => u.Nombre);
            return View(usuario.ToList());
        }
        [HttpPost]
        public ViewResult Index(FormCollection form)
        {
            string q = "";
            try
            {
                q = form["busqueda"].ToString();
            }
            catch (Exception)
            {
                q = "";
            }
            List<Usuario> usuario = new List<Usuario>();

            // Busqueda por app. Paterno
            var BusNombre = db.Persona.Where(d => d.ApellidoParterno.Contains(q)).ToList();
            foreach( var dato in BusNombre){
                var xdato = db.Usuario.Where(d => d.PersonaID == dato.ID).FirstOrDefault();
                if (xdato != null)
                {
                    usuario.Add(xdato);
                }
            }
            // Busqueda por App Materno
           BusNombre = db.Persona.Where(d => d.ApellidoMaterno.Contains(q)).ToList();
            foreach (var dato in BusNombre)
            {
                var xdato = db.Usuario.Where(d => d.PersonaID == dato.ID).FirstOrDefault();
                if (xdato != null)
                {
                    usuario.Add(xdato);
                }
            }
            // busqueda por nombre
            BusNombre = db.Persona.Where(d => d.Nombres.Contains(q)).ToList();
            foreach (var dato in BusNombre)
            {
                var xdato = db.Usuario.Where(d => d.PersonaID == dato.ID).FirstOrDefault();
                if (xdato != null)
                {
                    usuario.Add(xdato);
                }
            }
            // busqueda por tipo 

            bool strAdm = q.Contains("ADMI");
            if (strAdm) {
                var BusAdm = db.Usuario.Where(d => d.Administrador.Equals("S")).ToList();
                foreach (var dato in BusAdm)
                {           
                     usuario.Add(dato);                   
                }
            }

            bool strSup = q.Contains("SUPERVISOR");
            if (strSup)
            {
                var BusAdm = db.Usuario.Where(d => d.Supervisor.Equals("S")).ToList();
                foreach (var dato in BusAdm)
                {
                    usuario.Add(dato);
                }
            }

            bool strNorma = q.Contains("NORMAL");
            if (strNorma)
            {
                var BusAdm = db.Usuario.Where(d => d.Supervisor.Equals("N") && d.Administrador.Equals("N")).ToList();
                foreach (var dato in BusAdm)
                {
                    usuario.Add(dato);
                }
            }

            // busqueda por Usuario
            var BusUsuario = db.Usuario.Where(d => d.Nombre.Contains(q)).ToList();
            foreach (var dato in BusUsuario)
            {
                usuario.Add(dato);
            }

            //var usuario = db.Usuario.Include(u => u.Persona).Where(a => a.Nombre.Contains(q)).OrderBy(u => u.Nombre);
            return View(usuario.ToList());
        }
        //
        // GET: /Usuarios/Details/5

        public ViewResult Details(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            return View(usuario);
        }

        //
        // GET: /Usuarios/Create

        public ActionResult Create()
        {
            ViewBag.PersonaID = new SelectList(db.Persona.OrderBy(u => u.Nombres).ThenBy(u => u.ApellidoParterno).ThenBy(u => u.ApellidoMaterno), "ID", "NombreLista");
            ViewBag.Nombre = "";
            return View();
        } 

        //
        // POST: /Usuarios/Create

        [HttpPost]
        public ActionResult Create(FormCollection form)
        {
            utils.Log(2, "Inicio registro de usuario");
            Usuario usuario = new Usuario();
            usuario.Estado = "I";
            usuario.Administrador = "N";
            usuario.Supervisor = "N";
            usuario.Password = utils.md5(Request.Form["Password"].ToString().ToLower());
            usuario.Nombre = Request.Form["Nombre"].ToString().ToLower();
            usuario.PersonaID = Int32.Parse(Request.Form["PersonaID"].ToString());
            usuario.Comentarios = null;
            utils.Log(2, "Variables inciales asignadas");

            if (Request.Form["Comentarios"] != null && !"".Equals(Request.Form["Comentarios"].ToString()))
            {
                usuario.Comentarios = Request.Form["Comentarios"].ToString();
            }

            if (Request.Form["Estado"] != null && "A".Equals(Request.Form["Estado"].ToString()))
            {
                usuario.Estado = "A";
            }
 
            utils.Log(2, "Estado de usuario definido");

            if (Request.Form["tipoUsuario"] != null && !"".Equals(Request.Form["tipoUsuario"].ToString()))
            {
                string tipoUsuario = Request.Form["tipoUsuario"].ToString();
                if ("A".Equals(tipoUsuario))
                {
                    // usuario supervisor
                    usuario.Administrador = "S";
                }
                else if ("S".Equals(tipoUsuario))
                {
                    // usuario administrador
                    usuario.Supervisor = "S";
                }
            }
            utils.Log(2, "Nivel de usuario definido");
            int RegUsuario = 0;
            if (Request.Form["Nombre"] != null)
            {
                var revUsuario = db.Usuario.Where(d => d.Nombre.Equals(usuario.Nombre)).ToList();
                if (revUsuario.Count > 0)
                {
                    RegUsuario = 1;
                    utils.Log(2, "Usuario ya existe");
                }
                usuario.Estado = "A";
            }
            if (RegUsuario == 0)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Usuario.Add(usuario);
                        db.SaveChanges();
                        utils.Log(2, "Usuario registrado en la base de datos");
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        utils.Log(2, "Error al registrar usuario en la base de datos");
                        throw new Exception("Ocurrión un error al crear el usuario");
                    }
                }
                catch (Exception e)
                {
                    utils.Log(2, "Exception1: " + e.StackTrace);
                    string Mensaje = "";

                    if (e.InnerException.InnerException != null)
                    {
                        utils.Log(2, "Exception3: " + e.InnerException.InnerException.Message);
                        Mensaje = utils.mensajeError(e.InnerException.InnerException.Message);
                    }
                    else if (e.InnerException != null)
                    {
                        utils.Log(2, "Exception2: " + e.InnerException.Message);
                        Mensaje = utils.mensajeError(e.InnerException.Message);
                    }
                    else
                    {
                        utils.Log(2, "Exception4: " + e.Message);
                        Mensaje = utils.mensajeError(e.Message);
                    }

                    if (Mensaje.Contains("IX_Usuario"))
                    {
                        ViewBag.Mensaje = utils.mensajeError("El usuario <strong>" + usuario.Nombre + "</strong> ya se encuentra registrado en la base de datos, presione Buscar en el menu y modifique el Rol ya existente.");
                    }
                }

                ViewBag.PersonaID = new SelectList(db.Persona.OrderBy(u => u.Nombres).ThenBy(u => u.ApellidoParterno).ThenBy(u => u.ApellidoMaterno), "ID", "NombreLista", usuario.PersonaID);
                return View();
            }
            else {
                ViewBag.PersonaID = new SelectList(db.Persona.OrderBy(u => u.Nombres).ThenBy(u => u.ApellidoParterno).ThenBy(u => u.ApellidoMaterno), "ID", "NombreLista", usuario.PersonaID);
                ViewBag.Nombre = Request.Form["Nombre"].ToString().ToLower();
                ViewBag.Mensaje = utils.mensajeError("El Nombre de Usuario <strong>" + usuario.Nombre + "</strong> ya se encuentra registrado en la base de datos.");
                return View();
            }
        }
        
        //
        // GET: /Usuarios/Edit/5
 
        public ActionResult Edit(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            ViewBag.PersonaID = new SelectList(db.Persona.OrderBy(u => u.Rut), "ID", "NombreLista", usuario.PersonaID);
            return View(usuario);
        }

        //
        // POST: /Usuarios/Edit/5

        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            int usuarioID = Int32.Parse(Request.Form["ID"].ToString());
            Usuario usuario = db.Usuario.Find(usuarioID);

            try
            {
                if (Request.Form["NewPassword"] != null && !Request.Form["NewPassword"].ToString().Equals(""))
                {
                    usuario.Password = utils.md5(Request.Form["NewPassword"].ToString().ToLower());
                }
            }
            catch (Exception)
            { }

            try
            {
                utils.Log(2, "Editar usuario");
                usuario.Estado = "I";
                usuario.Administrador = "N";
                usuario.Supervisor = "N";
                usuario.Comentarios = null;
                utils.Log(2, "Variables inciales asignadas");

                if (Request.Form["Comentarios"] != null && !"".Equals(Request.Form["Comentarios"].ToString()))
                {
                    usuario.Comentarios = Request.Form["Comentarios"].ToString();
                }

                if (Request.Form["Estado"] != null && "A".Equals(Request.Form["Estado"].ToString()))
                {
                    usuario.Estado = "A";
                }
                utils.Log(2, "Estado de usuario definido");

                if (Request.Form["tipoUsuario"] != null && !"".Equals(Request.Form["tipoUsuario"].ToString()))
                {
                    string tipoUsuario = Request.Form["tipoUsuario"].ToString();
                    if ("A".Equals(tipoUsuario))
                    {
                        // usuario supervisor
                        usuario.Administrador = "S";
                    }
                    else if ("S".Equals(tipoUsuario))
                    {
                        // usuario administrador
                        usuario.Supervisor = "S";
                    }
                }
                utils.Log(2, "Nivel de usuario definido");
            

                if (ModelState.IsValid)
                {
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }
            }
            catch (Exception e)
            {
                utils.Log(2, "Exception1: " + e.StackTrace);
                if (e.InnerException.InnerException != null)
                {
                    utils.Log(2, "Exception3: " + e.InnerException.InnerException.Message);
                    ViewBag.Mensaje = utils.mensajeError(e.InnerException.InnerException.Message);
                }
                else if (e.InnerException != null)
                {
                    utils.Log(2, "Exception2: " + e.InnerException.Message);
                    ViewBag.Mensaje = utils.mensajeError(e.InnerException.Message);
                }
                else
                {
                    utils.Log(2, "Exception4: " + e.Message);
                    ViewBag.Mensaje = utils.mensajeError(e.Message);
                }
            }
            ViewBag.PersonaID = new SelectList(db.Persona.OrderBy(u => u.Rut), "ID", "NombreLista", usuario.PersonaID);
            return View(usuario);
        }
        //
        // POST: /Usuarios/Delete/5

        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Usuario usuario = db.Usuario.Find(id);
            db.Usuario.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        public string darNombre(string nombre,string ApPaterno, int x) {
            string xNombre = nombre.Substring(0, x) + ApPaterno;
            xNombre = xNombre.ToLower();

                  var revUsuario = db.Usuario.Where(d => d.Nombre.Equals(xNombre)).ToList();
                  if (revUsuario.Count != 0)
                  {
                      x++;
                      xNombre = darNombre(nombre, ApPaterno, x);
                  }
                      return xNombre;
               
           
        }


        public string NombreUsuario(int PersonaID)
        {
            string resultado = "";

            var revUsuario = db.Usuario.Where(d => d.PersonaID == PersonaID ).ToList();
            if (revUsuario.Count > 0)
            {
                resultado = "N";

            }
            else {

                var DatoPersona = db.Persona.Find(PersonaID);

                resultado = darNombre(DatoPersona.NombreCompleto,DatoPersona.ApellidoParterno, 1);
            
            
            }

                     

            return resultado;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}