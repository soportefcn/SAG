using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using SAG2.Classes;

namespace SAG2.Controllers
{ 
    public class CajaChicaController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();

        //
        // GET: /FondoFijo/

        public ViewResult Index(int Grupo)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            var fondofijo = db.FondoFijo.Include(f => f.Cuenta).Include(f => f.Proyecto).Where(f => f.ProyectoID == Proyecto.ID).Where(f => f.EgresoID == null && f.FondoFijoGrupoID == Grupo).OrderBy(f => f.Cuenta.Orden);
            return View(fondofijo.ToList());
        }

        [HttpGet]
        public ActionResult NuevoGrupo()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int Periodo = (int)Session["Periodo"];
            int NumeroFF = 0;

            try
            {
                NumeroFF = db.FondoFijoGrupo.Where(f => f.ProyectoID == Proyecto.ID && f.Periodo == Periodo).Max(f => f.Numero);
            }
            catch (Exception)
            {
                NumeroFF = 1;
            }

            FondoFijoGrupo ffg = new FondoFijoGrupo();
            ffg.Activo = "S";
            ffg.Creacion = DateTime.Now;
            ffg.Modificacion = DateTime.Now;
            ffg.Monto = 0;
            ffg.Numero = NumeroFF + 1;
            ffg.Periodo = Periodo;
            ffg.ProyectoID = Proyecto.ID;
            ffg.EgresoID = null;
            ffg.Descripcion = "Caja Chica " + ffg.Numero;
            ffg.ID = 0;

            db.FondoFijoGrupo.Add(ffg);
            db.SaveChanges();

            return RedirectToAction("Create", new { Grupo = ffg.ID });
        }

        public ViewResult Grupos()
        {
            int proyectoID = ((Proyecto)Session["Proyecto"]).ID;
            var ffg = db.FondoFijoGrupo.Where(f => f.ProyectoID == proyectoID).OrderByDescending(f => f.ID);
            return View(ffg.ToList());
        }

        //
        // GET: /FondoFijo/Details/5

        public ViewResult Details(int id)
        {
            FondoFijo fondofijo = db.FondoFijo.Find(id);
            return View(fondofijo);
        }

        //
        // GET: /FondoFijo/Create

        public ActionResult Create(int Grupo = 0)
        {
            // Se revisa si existe fondo fijo activo, si no se crea uno
            FondoFijoGrupo ffg = new FondoFijoGrupo();
            int Periodo = (int)Session["Periodo"];
            int ProyectoID = ((Proyecto)Session["Proyecto"]).ID;
            int NumeroFF = 0;

            try
            {
                NumeroFF = db.FondoFijoGrupo.Where(f => f.ProyectoID == ProyectoID && f.Periodo == Periodo).Max(f => f.Numero);
            }
            catch (Exception)
            {
                NumeroFF = 1;
            }
           
            if (Grupo > 0)
            {
                // Si el grupo viene definido se busca
                ffg = db.FondoFijoGrupo.Find(Grupo);
            }
            else
            {
                try
                {
                    // Si el grupo no viene definido se intenta obtener una caja chica activa
                    ffg = db.FondoFijoGrupo.Where(f => f.ProyectoID == ProyectoID && f.Periodo == Periodo && f.Activo.Equals("S")).OrderByDescending(f => f.ID).Take(1).Single();
                }
                catch (Exception)
                {
                    // Si no hay caja chica activa se crea una

                    ffg.Activo = "S";
                    ffg.Creacion = DateTime.Now;
                    ffg.Modificacion = DateTime.Now;
                    ffg.Monto = 0;
                    ffg.Numero = NumeroFF + 1;
                    ffg.Periodo = Periodo;
                    ffg.ProyectoID = ProyectoID;
                    ffg.EgresoID = null;
                    ffg.Descripcion = "Caja Chica " + ffg.Numero;
                    ffg.ID = 0;

                    db.FondoFijoGrupo.Add(ffg);
                    db.SaveChanges();
                }
            }

            ViewBag.Estado = ffg.Activo;
            ViewBag.NombreFondoFijo = ffg.Descripcion;
            ViewBag.FondoFijoGrupoID = ffg.ID;
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(30));
            ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista");
            ViewBag.ProyId = ProyectoID;

            // Detalle del FF

            ViewBag.Listado = db.FondoFijo.Include(f => f.Cuenta).Include(f => f.Proyecto).Where(f => f.ProyectoID == ProyectoID && f.FondoFijoGrupoID == ffg.ID).OrderByDescending(f => f.ID);

            return View();
        } 

        //
        // POST: /FondoFijo/Create

        [HttpPost]
        public ActionResult Create(FondoFijo fondofijo)
        {
            fondofijo.Periodo = (int)Session["Periodo"];
            fondofijo.Mes = (int)Session["Mes"];
            fondofijo.ProyectoID = ((Proyecto)Session["Proyecto"]).ID;
            fondofijo.CuentaID = Int32.Parse(Request.Form["CuentaID"].ToString());
            if (fondofijo.Glosa != null)
            {
                fondofijo.Glosa = fondofijo.Glosa.ToUpper();
            }
            else {
                fondofijo.Glosa = " ";
            }
            if (fondofijo.NumeroDocumento == null || fondofijo.NumeroDocumento <= 0)
            {
                fondofijo.NumeroDocumento = 1;
            }
            //fondofijo.Egreso = null;

            if (ModelState.IsValid)
            {
                db.FondoFijo.Add(fondofijo);
                db.SaveChanges();
                
                // Actualizacion del Fondo Fijo Grupo
                FondoFijoGrupo ffg = db.FondoFijoGrupo.Find(fondofijo.FondoFijoGrupoID);
                ffg.Modificacion = DateTime.Now;
                ffg.Monto = db.FondoFijo.Where(f => f.FondoFijoGrupoID == fondofijo.FondoFijoGrupoID).Sum(f => f.Monto);
                db.Entry(ffg).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Create", new { Grupo = ffg.ID });
            }

            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(30));
            ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista", fondofijo.ProyectoID);
            return View(fondofijo);
        }
        
        //
        // GET: /FondoFijo/Edit/5
 
        public ActionResult Edit(int id)
        {
            FondoFijo fondofijo = db.FondoFijo.Find(id);
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(30), fondofijo.CuentaID);
            ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista", fondofijo.ProyectoID);

            FondoFijoGrupo ffg = db.FondoFijoGrupo.Find(fondofijo.FondoFijoGrupoID);
            ViewBag.NombreFondoFijo = ffg.Descripcion;
            ViewBag.FondoFijoGrupoID = ffg.ID;

            return View(fondofijo);
        }

        //
        // POST: /FondoFijo/Edit/5

        [HttpPost]
        public ActionResult Edit(FondoFijo fondofijo)
        {
            fondofijo.CuentaID = Int32.Parse(Request.Form["CuentaID"].ToString());
            //fondofijo.Glosa = fondofijo.Glosa.ToUpper();

            if (fondofijo.NumeroDocumento == null || fondofijo.NumeroDocumento <= 0)
            {
                fondofijo.NumeroDocumento = 1;
            }

            //fondofijo.Egreso = null;
            if (ModelState.IsValid)
            {
                db.Entry(fondofijo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Create", new { Grupo = fondofijo.FondoFijoGrupoID });
            }
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(30), fondofijo.CuentaID);
            ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista", fondofijo.ProyectoID);
            return View(fondofijo);
        }

        //
        // GET: /FondoFijo/Delete/5
 
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            FondoFijo fondofijo = db.FondoFijo.Find(id);
            int ffg = fondofijo.FondoFijoGrupoID;
            db.FondoFijo.Remove(fondofijo);
            db.SaveChanges();
            return RedirectToAction("Create", new { Grupo = ffg });
        }


        [HttpGet]
        public string GenerarEgreso(int fondoFijoGrupoID)
        {
            Session.Remove("DetalleEgreso");
            int ProyectoID = ((Proyecto)Session["Proyecto"]).ID;
            int monto = 0;
            List<DetalleEgreso> lista_tmp = new List<DetalleEgreso>();
            List<DetalleEgreso> lista = new List<DetalleEgreso>();

            try 
            {
                List<FondoFijo> fondos = db.FondoFijo.Where(f => f.ProyectoID == ProyectoID && f.FondoFijoGrupoID == fondoFijoGrupoID).OrderByDescending(f => f.ID).ToList();

                foreach (FondoFijo fondo in fondos)
                {
                    DetalleEgreso detalle = new DetalleEgreso();
                    detalle.FechaEmision = fondo.Fecha;
                    detalle.FechaVencimiento = fondo.Fecha;
                    //detalle.Cuenta = fondo.Cuenta;
                    detalle.CuentaID = fondo.CuentaID;
                    detalle.Glosa = fondo.Glosa;
                    detalle.Monto = fondo.Monto;
                    detalle.FondoFijoID = fondo.ID;
                    //detalle.FondoFijo = fondo;
                    detalle.NDocumento = fondo.NumeroDocumento;
                    detalle.DocumentoID = null;// DocumentoID;
                    //detalle.Documento = db.Documento.Find(DocumentoID);
                    lista_tmp.Add(detalle);
                    monto = monto + fondo.Monto;
                }
            }
            catch (Exception)
            {
                return "-1";
            }

            if (Session["DetalleEgreso"] != null)
            {
                lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                lista.AddRange(lista_tmp);
                Session.Remove("DetalleEgreso");
            }
            else
            {
                lista = lista_tmp;
            }

            Session.Add("DetalleEgreso", lista);
            return monto.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}