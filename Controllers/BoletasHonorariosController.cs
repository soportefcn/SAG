using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Classes;
using SAG2.Comun; 
using SAG2.Models;

namespace SAG2.Controllers
{ 
    public class BoletasHonorariosController : Controller
    {
        private SAG2DB db = new SAG2DB();
        private Util utils = new Util();
        private Constantes ctes = new Constantes();

        //
        // GET: /BoletasHonorarios/

        public ViewResult Index()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
           // int Periodo = (int)Session["Periodo"];
            //int Mes = (int)Session["Mes"];

            var boletahonorario = db.BoletaHonorario.Include(b => b.Persona).Include(b => b.Proyecto).Where(b => b.ProyectoID == Proyecto.ID).OrderByDescending(b => b.Fecha);
            return View(boletahonorario.ToList());
        }
        public ViewResult CorregirHonorario(int? Periodo = 0, int? Mes = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            if (Periodo == 0)
            {
                 Periodo = (int)Session["Periodo"];
                Mes = (int)Session["Mes"];
            }
            
            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;
            var boletahonorario = db.BoletaHonorario.Include(b => b.Persona).Include(b => b.Proyecto).Where(b => b.ProyectoID == Proyecto.ID && b.Periodo == Periodo && b.Mes == Mes).OrderByDescending(b => b.Fecha);
            return View(boletahonorario.ToList());
        }
        [HttpPost]
        public ActionResult CorregirHonorario(int Periodo = 0, int Mes = 0, int flag = 1)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
        
            ViewBag.Periodo = Periodo;
            ViewBag.Mes = Mes;
            var boletahonorario = db.BoletaHonorario.Include(b => b.Persona).Include(b => b.Proyecto).Where(b => b.ProyectoID == Proyecto.ID && b.Periodo == Periodo && b.Mes == Mes).OrderByDescending(b => b.Fecha);
            return View(boletahonorario.ToList());
        }
        //
        // GET: /BoletasHonorarios/Details/5

        public ViewResult Details(int id)
        {
            BoletaHonorario boletahonorario = db.BoletaHonorario.Find(id);
            return View(boletahonorario);
        }

        //
        // GET: /BoletasHonorarios/Create
        public ViewResult EditarPopUp(int ID)
        {
            List<DetalleEgreso> Lista = new List<DetalleEgreso>();
            string mensaje = "Si";
            int periodoActivo = (int)Session["Periodo"];
            int mesActivo = (int)Session["Mes"];
            int CuentaID = 0;
            if (Session["DetalleEgreso"] != null)
            {
                Lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                int i = 0;
                foreach (var data in Lista)
                {
                    i++;
                    if (data.BoletaHonorarioID == ID)
                    {
                        CuentaID = data.CuentaID;
                    }
                }
            }

            BoletaHonorario boletahonorario = db.BoletaHonorario.Find(ID);
            int PeriodoDocumento = boletahonorario.Periodo;
            int MesDocumento = boletahonorario.Mes;

            var Porc = db.Referencia.Where(d => d.GRUPO == "PORCENTAJE" && d.PERIODO == PeriodoDocumento && d.VALOR == boletahonorario.Porcentaje).FirstOrDefault().VALOR;
            ViewBag.PORCENTAJE = new SelectList(db.Referencia.Where(d => d.GRUPO == "PORCENTAJE" && d.PERIODO == PeriodoDocumento).ToList(), "VALOR", "VALOR", Porc);
            ViewBag.Arbol = utils.generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos),CuentaID);
            ViewBag.ValorPorcentaje = boletahonorario.Porcentaje;
         
            if (boletahonorario.PersonaID != null)
            {
                @ViewBag.PersonalID = boletahonorario.PersonaID.ToString();
                @ViewBag.NombreLista = db.Persona.Find(boletahonorario.PersonaID).NombreLista;
            }
            else if (boletahonorario.ProveedorID != 0)
            {
                @ViewBag.ProveedorID = boletahonorario.ProveedorID.ToString();
                @ViewBag.NombreLista = db.Proveedor.Find(boletahonorario.ProveedorID).NombreLista;
            }
            else
            {
                @ViewBag.Rut = boletahonorario.Rut.ToString();
                @ViewBag.DV = boletahonorario.DV.ToString();
                @ViewBag.Beneficiario = boletahonorario.Beneficiario.ToString();
                @ViewBag.NombreLista = boletahonorario.Rut + "-" + boletahonorario.DV + " " + boletahonorario.Beneficiario;
            }

            // mensaje 
            if ((PeriodoDocumento != periodoActivo) || (MesDocumento != mesActivo )){
                mensaje = "La boleta no es del mes en curso ";            
            }
            var RevisarEgreso = db.DetalleEgreso.Where(r => r.BoletaHonorarioID == ID).FirstOrDefault();
            if (RevisarEgreso != null)
            {
                if (RevisarEgreso.Conciliado != null)
                {
                    mensaje = mensaje + "<br/>" + "La boleta fue conciliada";
                }
            }
            ViewBag.Mensaje = mensaje;
            return View(boletahonorario);
        }

        [HttpPost]
        public ActionResult EditarPopUp(BoletaHonorario boletahonorario)
        {
            List<DetalleEgreso> lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
            List<DetalleEgreso> listax = new List<DetalleEgreso>();
            DetalleEgreso detalle = new DetalleEgreso();
            int BoletaID = boletahonorario.ID;
            detalle = lista.Where(d => d.BoletaHonorarioID == BoletaID).Single();
            detalle.DocumentoID = 3;
            detalle.NDocumento = boletahonorario.NroBoleta;
            detalle.Monto = boletahonorario.Neto;
            detalle.CuentaID = int.Parse(Request.Form["CuentaID"].ToString());
            detalle.Glosa = boletahonorario.Concepto;
           
            listax.Add(detalle);

            if (!Request.Form["PersonaID"].ToString().Equals(""))
            {
                boletahonorario.ProveedorID = null;
                boletahonorario.Rut = null;
                boletahonorario.DV = null;
                @ViewBag.PersonalID = boletahonorario.PersonaID.ToString();
                @ViewBag.NombreLista = db.Persona.Find(boletahonorario.PersonaID).NombreLista;
            }
            else if (!Request.Form["ProveedorID"].ToString().Equals(""))
            {
                boletahonorario.PersonaID = null;
                boletahonorario.Rut = null;
                boletahonorario.DV = null;
                @ViewBag.ProveedorID = boletahonorario.ProveedorID.ToString();
                @ViewBag.NombreLista = db.Proveedor.Find(boletahonorario.ProveedorID).NombreLista;
            }
            else if (!Request.Form["Rut"].ToString().Equals(""))
            {
                boletahonorario.ProveedorID = null;
                boletahonorario.PersonaID = null;
                @ViewBag.Rut = boletahonorario.Rut.ToString();
                @ViewBag.DV = boletahonorario.DV.ToString();
                @ViewBag.Beneficiario = boletahonorario.Beneficiario.ToString();
                @ViewBag.NombreLista = boletahonorario.Rut + "-" + boletahonorario.DV + " " + boletahonorario.Beneficiario;
            }
            else
            {
                throw new Exception("El beneficiario seleccionado no es válido.");
            }

            @ViewBag.Title = "Ingresar Boleta de Honorarios";



            if (Request.Form["Electronica"] != null)
                boletahonorario.Electronica = "S";

            if (Request.Form["Nula"] != null)
                boletahonorario.Nula = "S";

            if (ModelState.IsValid)
            {
                Session.Remove("DetalleEgreso");
                Session.Add("DetalleEgreso", listax);

                db.Entry(boletahonorario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CerrarPopUp", new { @id = boletahonorario.ID });
            }

            return RedirectToAction("IngresarPopUp", new { @personalID = boletahonorario.PersonaID });
        }
        
        
        public ActionResult IngresarPopUp(int personalID = 0, int proveedorID = 0, string rut = "", string dv = "", string beneficiario = "")
        {
            int periodoActivo = (int)Session["Periodo"];
            // 24-04-2023 malditos porcentajes
            try
            {
                ViewBag.PORCENTAJE = new SelectList(db.Referencia.Where(d => d.GRUPO == "PORCENTAJE" && d.PERIODO == periodoActivo).ToList(), "VALOR", "VALOR");
                ViewBag.ValorPorcentaje = db.Referencia.Where(d => d.GRUPO == "PORCENTAJE" && d.PERIODO == periodoActivo && d.PREDETERMINADO == 1).FirstOrDefault().VALOR;
            }
            catch {
                ViewBag.PORCENTAJE = 0;
                ViewBag.ValorPorcentaje = 0; 
            }
            if (personalID != 0) 
            {
                @ViewBag.PersonalID = personalID.ToString();
                @ViewBag.NombreLista = db.Persona.Find(personalID).NombreLista;
            }
            else if (proveedorID != 0) 
            {
                @ViewBag.ProveedorID = proveedorID.ToString();
                @ViewBag.NombreLista = db.Proveedor.Find(proveedorID).NombreLista;
            }
            else 
            {
                @ViewBag.Rut = rut.ToString();
                @ViewBag.DV = dv.ToString();
                @ViewBag.Beneficiario = beneficiario.ToString();
                @ViewBag.NombreLista = rut + "-" + dv + " " + beneficiario;
            }

            @ViewBag.Title = "Ingresar Boleta de Honorarios";
            return View();
        }

        [HttpPost]
        public ActionResult IngresarPopUp(BoletaHonorario boletahonorario)
        {
            if (!Request.Form["PersonaID"].ToString().Equals(""))
            {
                boletahonorario.ProveedorID = null;
                boletahonorario.Rut = null;
                boletahonorario.DV = null;
                @ViewBag.PersonalID = boletahonorario.PersonaID.ToString();
                @ViewBag.NombreLista = db.Persona.Find(boletahonorario.PersonaID).NombreLista;
            }
            else if (!Request.Form["ProveedorID"].ToString().Equals(""))
            {
                boletahonorario.PersonaID = null;
                boletahonorario.Rut = null;
                boletahonorario.DV = null;
                @ViewBag.ProveedorID = boletahonorario.ProveedorID.ToString();
                @ViewBag.NombreLista = db.Proveedor.Find(boletahonorario.ProveedorID).NombreLista;
            }
            else if (!Request.Form["Rut"].ToString().Equals(""))
            {
                boletahonorario.ProveedorID = null;
                boletahonorario.PersonaID = null;
                @ViewBag.Rut = boletahonorario.Rut.ToString();
                @ViewBag.DV = boletahonorario.DV.ToString();
                @ViewBag.Beneficiario = boletahonorario.Beneficiario.ToString();
                @ViewBag.NombreLista = boletahonorario.Rut + "-" + boletahonorario.DV + " " + boletahonorario.Beneficiario;
            }
            else
            {
                throw new Exception("El beneficiario seleccionado no es válido.");
            }

            @ViewBag.Title = "Ingresar Boleta de Honorarios";
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            boletahonorario.Periodo = (int)Session["Periodo"];
            boletahonorario.Mes = (int)Session["Mes"];
            boletahonorario.ProyectoID = Proyecto.ID;
            boletahonorario.EgresoID = null;
            boletahonorario.Electronica = null;
            boletahonorario.Nula = null;

            if (Request.Form["Electronica"] != null)
                boletahonorario.Electronica = "S";

            if (Request.Form["Nula"] != null)
                boletahonorario.Nula = "S";
            
            if (ModelState.IsValid)
            {
                db.BoletaHonorario.Add(boletahonorario);
                db.SaveChanges();
                return RedirectToAction("CerrarPopUp", new { @id = boletahonorario.ID });
            }

            return RedirectToAction("IngresarPopUp", new { @personalID = boletahonorario.PersonaID });
        }

        public ActionResult CerrarPopUp(int id)
        {
            BoletaHonorario boletahonorario = db.BoletaHonorario.Find(id);
            return View(boletahonorario);
        }

        public ActionResult Create()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            int periodoActivo = (int)Session["Periodo"];
            var rol = db.Rol.Include(r => r.Persona).Include(r => r.TipoRol).Where(r => r.ProyectoID == Proyecto.ID);
            var persona = from r in rol
                          select r.Persona;
            ViewBag.PersonaID = new SelectList(persona, "ID", "NombreLista");
            ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista");
            ViewBag.PORCENTAJE = new SelectList(db.Referencia.Where(d => d.GRUPO == "PORCENTAJE" && d.PERIODO == periodoActivo).ToList(), "VALOR", "VALOR");
            ViewBag.ValorPorcentaje = db.Referencia.Where(d => d.GRUPO == "PORCENTAJE" && d.PERIODO == periodoActivo && d.PREDETERMINADO == 1).FirstOrDefault().VALOR;   
            return View();
        } 

        //
        // POST: /BoletasHonorarios/Create

        [HttpPost]
        public ActionResult Create(BoletaHonorario boletahonorario)
        {
            int periodoActivo = (int)Session["Periodo"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            boletahonorario.Periodo = (int)Session["Periodo"];
            boletahonorario.Mes = (int)Session["Mes"];
            boletahonorario.ProyectoID = Proyecto.ID;
            boletahonorario.EgresoID = null;
            boletahonorario.Electronica = null;
            boletahonorario.Nula = null;

            ViewBag.PORCENTAJE = new SelectList(db.Referencia.Where(d => d.GRUPO == "PORCENTAJE" && d.PERIODO == periodoActivo).ToList(), "VALOR", "VALOR");
            ViewBag.ValorPorcentaje = db.Referencia.Where(d => d.GRUPO == "PORCENTAJE" && d.PERIODO == periodoActivo && d.PREDETERMINADO == 1).FirstOrDefault().VALOR;  

            if (Request.Form["Electronica"] != null)
                boletahonorario.Electronica = "S";

            if (Request.Form["Nula"] != null)
                boletahonorario.Nula = "S";

            if (ModelState.IsValid)
            {
                db.BoletaHonorario.Add(boletahonorario);
                db.SaveChanges();
                return RedirectToAction("Create");  
            }

            var rol = db.Rol.Include(r => r.Persona).Include(r => r.TipoRol).Where(r => r.ProyectoID == Proyecto.ID);
            var persona = from r in rol
                          select r.Persona;
            ViewBag.PersonaID = new SelectList(persona, "ID", "NombreLista", boletahonorario.PersonaID);
            ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista", boletahonorario.ProyectoID);
            return View(boletahonorario);
        }
        
        //
        // GET: /BoletasHonorarios/Edit/5

        public ActionResult Edit(int id)
        {
            int MesCurso = DateTime.Now.Month;
            string RegistroModificar = "Si";
            BoletaHonorario boletahonorario = db.BoletaHonorario.Find(id);
            var RevisarEgreso = db.DetalleEgreso.Where(r => r.BoletaHonorarioID == id).FirstOrDefault();

            if (MesCurso > boletahonorario.Mes)
            {
                RegistroModificar = "La boleta no es del mes en curso";
            }
            if (RevisarEgreso != null)
            {
                if (RevisarEgreso.Conciliado != null)
                {
                    RegistroModificar = "La boleta fue conciliada";
                }
            }
            var rol = db.Rol.Include(r => r.Persona).Include(r => r.TipoRol).Where(r => r.ProyectoID == boletahonorario.ProyectoID);
            var persona = from r in rol
                          select r.Persona;
           
            ViewBag.ProyectoID = new SelectList(db.Proyecto, "ID", "NombreLista", boletahonorario.ProyectoID);
            int periodoActivo = (int)Session["Periodo"];
            //try
            //{
            //    var XValor = db.Referencia.Where(d => d.VALOR == boletahonorario.Porcentaje).FirstOrDefault().VALOR;
            //}
            //catch { }
     
        
            ViewBag.RegistroModificar = RegistroModificar;

            if (boletahonorario.PersonaID != null)
            {
                @ViewBag.PersonalID = boletahonorario.PersonaID.ToString();
                @ViewBag.NombreLista = db.Persona.Find(boletahonorario.PersonaID).NombreLista;
            }
            else if (boletahonorario.ProveedorID != null)
            {
                @ViewBag.ProveedorID = boletahonorario.ProveedorID.ToString();
                @ViewBag.NombreLista = db.Proveedor.Find(boletahonorario.ProveedorID).NombreLista;
            }
            else
            {
                @ViewBag.Rut = boletahonorario.Rut.ToString();
                @ViewBag.DV = boletahonorario.DV.ToString();
                @ViewBag.Beneficiario = boletahonorario.Beneficiario.ToString();
                @ViewBag.NombreLista = boletahonorario.Rut.ToString() + "-" + boletahonorario.DV.ToString() + " " + boletahonorario.Beneficiario.ToString();
            }

            return View(boletahonorario);

        }
        //
        // POST: /BoletasHonorarios/Edit/5

        [HttpPost]
        public ActionResult Edit(BoletaHonorario bhonorario)
        {
            int periodoSel = bhonorario.Periodo;
            int mesSel = bhonorario.Mes;
            BoletaHonorario Boleta = new BoletaHonorario();
            Boleta = db.BoletaHonorario.Find(bhonorario.ID);
            Boleta.NroBoleta = bhonorario.NroBoleta;
            Boleta.Concepto = bhonorario.Concepto;
            if (Request.Form["Electronica"] != null)
                Boleta.Electronica = "S";

            if (Request.Form["Nula"] != null)
                Boleta.Nula = "S";

            if (ModelState.IsValid)
            {
                db.Entry(Boleta).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CorregirHonorario", new { periodo = periodoSel, mes = mesSel });
            }

            return RedirectToAction("CorregirHonorario", new { periodo = periodoSel, mes = mesSel });
        }

        public ActionResult ListadoEgreso(int personalID = 0)
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            if (personalID != 0)
            {
                return View(db.BoletaHonorario.Where(d => d.PersonaID == personalID && d.EgresoID == null && d.ProyectoID == Proyecto.ID).Where(d => d.Nula == null).OrderByDescending(b => b.Fecha).ToList());
            }
            else
            {
                return View(db.BoletaHonorario.Where(d => d.EgresoID == null && d.Nula == null && d.ProyectoID == Proyecto.ID).OrderByDescending(b => b.Fecha).ToList());
            }
        }

        //
        // GET: /BoletasHonorarios/Delete/5
 
        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            BoletaHonorario boletahonorario = db.BoletaHonorario.Find(id);
            int periodoSel = boletahonorario.Periodo;
            int mesSel = boletahonorario.Mes;
            db.BoletaHonorario.Remove(boletahonorario);
            db.SaveChanges();
            return RedirectToAction("CorregirHonorario", new { periodo = periodoSel,mes = mesSel });
        }

        public ActionResult Reporte(int Periodo = 0, int Mes = 0)
        {
            if (Periodo == 0)
            {
                ViewBag.Periodo = (int)Session["Periodo"];
                ViewBag.Mes = (int)Session["Mes"];
            }
            else
            {
                ViewBag.Periodo = Periodo;
                ViewBag.Mes = Mes;
            }

            var proyecto = db.Proyecto.OrderBy(p => p.CodCodeni).Where(r => r.Eliminado == null).Where(r => r.Cerrado == null).ToList();
            return View(proyecto);
        }

    
        public ActionResult ReporteExcel(int Periodo = 0, int Mes = 0)
        {
            if (Periodo == 0)
            {
                ViewBag.Periodo = (int)Session["Periodo"];
                ViewBag.Mes = (int)Session["Mes"];
            }
            else
            {
                ViewBag.Periodo = Periodo;
                ViewBag.Mes = Mes;
            }

            var proyecto = db.Proyecto.OrderBy(p => p.CodCodeni).Where(r => r.Eliminado == null).Where(r => r.Cerrado == null).ToList();
            Proyecto Proy = (Proyecto)Session["Proyecto"];
            ViewBag.Proyecto = Proy.NombreLista;
            ViewBag.CodSename = Proy.CodSename;  

            return View(proyecto);
        }

        public ActionResult ReporteImprimir(int Periodo = 0, int Mes = 0)
        {
            if (Periodo == 0)
            {
                ViewBag.Periodo = (int)Session["Periodo"];
                ViewBag.Mes = (int)Session["Mes"];
            }
            else
            {
                ViewBag.Periodo = Periodo;
                ViewBag.Mes = Mes;
            }

            var proyecto = db.Proyecto.OrderBy(p => p.CodCodeni).Where(r => r.Eliminado == null).Where(r => r.Cerrado == null).ToList();
            Proyecto Proy = (Proyecto)Session["Proyecto"];
            ViewBag.Proyecto = Proy.NombreLista;
            ViewBag.CodSename = Proy.CodSename;  
            return View(proyecto);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}