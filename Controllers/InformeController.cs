//using Rotativa;
using SAG2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;


namespace SAG_5.Controllers
{
    public class InformeController : Controller
    {
        private SAG2DB db = new SAG2DB();


        public ActionResult ReporteRegionEstandarCC()
        {

            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado != "S" && p.Cerrado != "S"), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            return View();
        }
        [HttpPost]

        public ActionResult ReporteRegionEstandarCC(int region, int Mes, int? periodo)
        {

            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado != "S" && p.Cerrado != "S"), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Idregion = region;
            ViewBag.Mes = Mes;
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.Mes = Mes;
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {Mes,};
            }

            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };
            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };
            }

            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            return View();
        }
        // GET: Informes1
        public ActionResult Index()
        {
            var informe = db.Informe.Include(i => i.tipo);
            return View(informe.ToList());
        }

        // GET: Informes1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Informe informe = db.Informe.Find(id);
            if (informe == null)
            {
                return HttpNotFound();
            }
            return View(informe);
        }

        // GET: Informes1/Create
        public ActionResult Create()
        {
            ViewBag.tipoInformeID = new SelectList(db.TipoInforme, "ID", "Nombre");
            ViewBag.cuentaID = new SelectList(db.Cuenta, "ID", "Nombre");
            return View();
        }

        // POST: Informes1/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nombreInforme,tipoInformeID,estado,descripcion")] Informe informe)
        {
            informe.fecha = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Informe.Add(informe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.tipoInformeID = new SelectList(db.TipoInforme, "ID", "Nombre", informe.tipoInformeID);
            return View(informe);
        }

        // GET: Informes1/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return HttpNotFound();
            }
            Informe informe = db.Informe.Find(id);
            if (informe == null)
            {
                return HttpNotFound();
            }
            Session.Add("InformeID", informe.ID);
            ViewBag.tipoInformeID = new SelectList(db.TipoInforme, "ID", "Nombre", informe.tipoInformeID);
            return View(informe);
        }

        // POST: Informes1/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,nombreInforme,tipoInformeID,estado,descripcion,CuentaPadreID")] Informe informe)
        {
            informe.fecha = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Entry(informe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.tipoInformeID = new SelectList(db.TipoInforme, "ID", "Nombre", informe.tipoInformeID);
            return View(informe);
        }

        // GET: Informes1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return HttpNotFound(); 
            }
            Informe informe = db.Informe.Find(id);
            if (informe == null)
            {
                return HttpNotFound();
            }
            return View(informe);
        }

        // POST: Informes1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Informe informe = db.Informe.Find(id);
            db.Informe.Remove(informe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult ListarLineas(int? t)
        {
            int id = (int)Session["InformeID"];
            ViewBag.cuentaID = new SelectList(db.Cuenta.OrderBy(c => c.Orden), "ID", "NombreLista");
            ViewBag.CuentaPadre_ID = new SelectList(db.CuentasPadres.OrderBy(c => c.ID), "ID", "NombreGrupo");
            List<DetalleInformes> lista = new List<DetalleInformes>();
            ViewBag.ID = id;
            try
            {
                if (id != 0)
                {
                    lista = db.DetalleInformes.Where(i => i.informeID == id).ToList();
                    Session.Add("DetalleInforme", lista);
                }
                else
                {
                    Session.Add("DetalleInforme", lista);
                }
                ViewBag.ID = t;
                
            }
            catch
            {
                Session.Add("DetalleInforme", lista);
            }
            return View((List<DetalleInformes>)Session["DetalleInforme"]);
        }
        public ActionResult EditarLinea(int id, int CuentaID, int tipoCuenta, byte estado = 0, byte valor = 0)
        {
            DetalleInformes detalle = db.DetalleInformes.Find(id);
            detalle.cuentaID = CuentaID;
            detalle.tipoCuenta = tipoCuenta.ToString();
            detalle.estado = estado;
            detalle.valor = valor;
       

            List<DetalleInformes> lista = new List<DetalleInformes>();
            ViewBag.cuentaID = new SelectList(db.Cuenta, "ID", "NombreLista");
            lista = db.DetalleInformes.Where(d => d.informeID == detalle.informeID).ToList();
            Session.Add("DetalleInformes", lista);
            Session.Add("DetalleInformes", lista);

            //if (detalleMovimiento != db.DetalleIngreso.Find(int.Parse(id)))
            //{
            if (ModelState.IsValid)
            {
                db.Entry(detalle).State = EntityState.Modified;
                db.SaveChanges();
            }

            return View("ListarLineas", (List<DetalleInformes>)Session["DetalleInformes"]);


            //int periodo = (int)Session["Periodo"];

            //DetalleMovimiento.Cc = DetalleMovimiento.Cc;

            //return View(DetalleMovimiento);
            // return View("ListarLineas", movimiento);
        }
        public ActionResult ReportePrograma()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            return View();
        }
        [HttpPost]
        public ActionResult ReportePrograma(int Informe, int Proyectos, int  Mes, int  periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Periodo = periodo;
            ViewBag.IdInforme = Informe;
            ViewBag.Mes = Mes;
            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();

        }
        public ActionResult ReporteProgramaCC()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null), "ID", "NombreLista",Proyecto.ID);
            ViewBag.periodo = (int)Session["Periodo"];
            ViewBag.Mes = (int)Session["Mes"];
            return View();
        }
        [HttpPost]
        public ActionResult ReporteProgramaCC(int Informe, int Proyectos, int Mes, int periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Periodo = periodo;
            ViewBag.IdInforme = Informe;
            ViewBag.Mes = Mes;
            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteProgramaExcel(int Informe, int Proyectos, int Mes, int periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Periodo = periodo;
            ViewBag.IdInforme = Informe;
            ViewBag.Mes = Mes;
            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

       


        public ActionResult ReporteProgramaExcelCC(int Informe, int Proyectos, int Mes, int periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Periodo = periodo;
            ViewBag.IdInforme = Informe;
            ViewBag.Mes = Mes;
            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

       



        public ActionResult ReporteProgramaEstandar()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            return View();
        }
        public ActionResult ReporteProgramaEstandarCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            return View();
        }

        [HttpPost]
        public ActionResult ReporteProgramaEstandar( int Proyectos, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;

            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View(); 
        }

        [HttpPost]
        public ActionResult ReporteProgramaEstandarCC(int Proyectos, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.Mes = Mes;
            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteProgramaEstandarExcel( int Proyectos, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();
            ViewBag.Mes = Mes;

            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

       

        public ActionResult ReporteProgramaEstandarExcelCC(int Proyectos, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;

            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

     

        public ActionResult ReporteProgramaSintesis()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteProgramaSintesis( int Proyectos, int Mes, int periodo)
        {

            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();

            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");

            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;

            //    ViewBag.IdLinea = linea;
            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;


            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }



            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteProgramaSintesisExcel( int Proyectos, int Mes, int? periodo)
        {

            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;
            ViewBag.CuentaFinancimiento = db.cuentaGrupo.Where(d => d.grupo.Equals(1)).ToList();
            ViewBag.CuentaApoyo = db.cuentaGrupo.Where(d => d.grupo.Equals(2)).ToList();

            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");

            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;

            //    ViewBag.IdLinea = linea;
            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;


            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }



            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

       

        public ActionResult ReporteProgramaSintesisCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteProgramaSintesisCC(int Proyectos, int Mes, int periodo)
        {

            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");

            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;
            var presupuesto = db.Presupuesto.Where(m => m.ProyectoID == Proyectos && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).OrderByDescending(p => p.ID).Take(1).Single();
            ViewBag.SaldoInicial = presupuesto.SaldoInicial;

            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;


            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }



            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteProgramaSintesisExcelCC(int Proyectos, int Mes, int? periodo)
        {

            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;


            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.ProyectoID == Proyectos && p.Periodo == periodo);
            }
            return View();
        }

    

        public ActionResult ReporteLineaSintesis()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteLineaSintesis( int linea, int Mes, int? periodo)
        {
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.IdLinea = linea;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }



            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteLineaSintesisExcel(int linea, int Mes, int periodo)
        {
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdLinea = linea;
         //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }

    

        public ActionResult ReporteLineaSintesisCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteLineaSintesisCC(int linea, int Mes, int? periodo)
        {
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.IdLinea = linea;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }



            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteLineaSintesisExcelCC(int linea, int Mes, int periodo)
        {
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdLinea = linea;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }

   


        public ActionResult ReporteLineaEstandar()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteLineaEstandar(int linea, int Mes, int periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.IdLinea = linea;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteLineaEstandarExcel( int linea, int Mes, int periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdLinea = linea;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteLineaEstandarCC()
        {
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.Eliminado == null && p.Cerrado == null), "ID", "NombreLista", Proyecto.ID);
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteLineaEstandarCC(int linea, int Mes, int periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.IdLinea = linea;
            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteLineaEstandarExcelCC(int linea, int Mes, int periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdLinea = linea;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {
                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == 0)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }

       


      

        public ActionResult ReporteTipoSintesis()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteTipoSintesis( int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.IdtipoPrograma = tipoPrograma;
            ViewBag.Mes = Mes;
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == tipoPrograma && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }


            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteTipoSintesisExcel( int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdtipoPrograma = tipoPrograma;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }


            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            return View();
        }

       


        public ActionResult ReporteTipoSintesisCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteTipoSintesisCC(int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.IdtipoPrograma = tipoPrograma;
            ViewBag.Mes = Mes;
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == tipoPrograma && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }


            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteTipoSintesisExcelCC(int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdtipoPrograma = tipoPrograma;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;

            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }


            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            return View();
        }

     


        public ActionResult ReporteTipoEstandar()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteTipoEstandar(int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.IdtipoPrograma = tipoPrograma;
            ViewBag.Mes = Mes;
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == tipoPrograma && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteTipoEstandarExcel(int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdtipoPrograma = tipoPrograma;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;

            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == tipoPrograma && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            return View();
        }
       
        public ActionResult ReporteTipoEstandarCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteTipoEstandarCC(int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.IdtipoPrograma = tipoPrograma;
            ViewBag.Mes = Mes;
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteTipoEstandarExcelCC(int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdtipoPrograma = tipoPrograma;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;

            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            return View();
        }

     


        public ActionResult ReporteRegionSintesis()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteRegionSintesis( int region, int Mes, int? periodo)
        {
            ViewBag.Periodo = periodo;
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Idregion = region;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteRegionSintesisExcel( int region, int Mes, int? periodo)
        {
            ViewBag.IdRegion = region;
            ViewBag.Periodo = periodo;
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }



            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            return View();
        }

   


        public ActionResult ReporteRegionSintesisCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteRegionSintesisCC(int region, int Mes, int? periodo)
        {
            ViewBag.Periodo = periodo;
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Idregion = region;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteRegionSintesisExcelCC(int region, int Mes, int? periodo)
        {
            ViewBag.IdRegion = region;
            ViewBag.Periodo = periodo;
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }



            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            return View();
        }

    

        public ActionResult ReporteRegionEstandar()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteRegionEstandar( int region, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.Periodo = periodo;
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Idregion = region;
            ViewBag.Mes = Mes;
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            return View();
        }

        public ActionResult ReporteRegionEstandarExcel( int region, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdRegion = region;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Mes = Mes;

            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            return View();
        }


   
        public ActionResult ReporteRegionEstandarExcelCC(int region, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.IdRegion = region;
            //   ViewBag.ID = Proyectos;
            ViewBag.Periodo = periodo;
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.Mes = Mes;

            ViewBag.Mes = Mes;

            if (Mes < 13)
            {
                ViewBag.GrupoMeses = new int[1] {
                Mes,
                };
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
                1,
                2,
                3,
                4,
                5,
                6
                };

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
                7,
                8,
                9,
                10,
                11,
                12
                };

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12 };
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            return View();
        }

    



        public ActionResult ReporteLinea()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteLinea(int Informe, int linea, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.IdLinea = linea;
            ViewBag.Periodo = periodo;
            ViewBag.IdInforme = Informe;
            ViewBag.Mes = Mes;
            ViewBag.IdInforme = Informe;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }





            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }
        //[HttpPost]
        public ActionResult ReporteLineaNormalExcel(int Informe , int linea , int Mes, int? periodo =0)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.Mes = Mes;
            ViewBag.IdLinea = linea;
            ViewBag.Periodo = periodo;
            ViewBag.IdInforme = Informe;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }






            return View();
        }

      



        public ActionResult ReporteLineaCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteLineaCC(int Informe, int linea, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.IdLinea = linea;
            ViewBag.Periodo = periodo;
            ViewBag.IdInforme = Informe;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }





            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            return View();
        }
        //[HttpPost]
        public ActionResult ReporteLineaNormalExcelCC(int Informe, int linea, int Mes, int? periodo = 0)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.Mes = Mes;
            ViewBag.IdLinea = linea;
            ViewBag.Periodo = periodo;
            ViewBag.IdInforme = Informe;
            ViewBag.Mes = Mes;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyecto.LineasAtencionID == linea && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.LineaAtencion.ID == linea && p.Periodo == periodo);
            }
            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }






            return View();
        }

  



        public ActionResult ReporteTipoPrograma()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteTipoPrograma(int Informe, int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.IdtipoPrograma = tipoPrograma;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.IdInforme = Informe;
            ViewBag.Periodo = periodo;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID  == tipoPrograma && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }


            return View();
        }
        //[HttpPost]
        public ActionResult ReporteTipoProgramaNormalExcel(int Informe, int tipoPrograma, int Mes, int? periodo = 0)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.Mes = Mes;
            ViewBag.IdtipoPrograma = tipoPrograma;
            ViewBag.IdInforme = Informe;
            ViewBag.Periodo = periodo;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == tipoPrograma && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }

            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }



            return View();
        }

     

        public ActionResult ReporteTipoProgramaCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteTipoProgramaCC(int Informe, int tipoPrograma, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.IdtipoPrograma = tipoPrograma;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.IdInforme = Informe;
            ViewBag.Periodo = periodo;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == tipoPrograma && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }

            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }
            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }


            return View();
        }
        //[HttpPost]
        public ActionResult ReporteTipoProgramaNormalExcelCC(int Informe, int tipoPrograma, int Mes, int? periodo = 0)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.Mes = Mes;
            ViewBag.IdtipoPrograma = tipoPrograma;
            ViewBag.IdInforme = Informe;
            ViewBag.Periodo = periodo;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.TipoProyectoID == tipoPrograma && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.TipoProyecto.ID == tipoPrograma && p.Periodo == periodo);
            }

            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }



            return View();
        }

    

        public ActionResult ReporteRegion()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre"); 
            return View();
        }
        [HttpPost]
        public ActionResult ReporteRegion(int Informe, int region, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.IdRegion = region;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.IdInforme = Informe;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch {
                ViewBag.SaldoInicial = 0;
            }
                
             if (periodo == null)
            {

            }
            else
            {
                //var ingresos = db.Movimiento.Where(e => (e.CuentaID == 3 || e.CuentaID == 4 || e.CuentaID == 5 || e.CuentaID == 8 || e.CuentaID == 12) && e.Temporal == null && e.Periodo == periodo || e.Proyecto.Direccion.Comuna.Region.ID == region).Sum(m => m.Monto_Ingresos);
                //ViewBag.valorIngresos = (System.Numerics.Complex)(ingresos.Where(e => (e.CuentaID == 3 || e.CuentaID == 4 || e.CuentaID == 5 || e.CuentaID == 8 || e.CuentaID == 12) && e.Temporal == null && e.Periodo == periodo).Sum(m => m.Monto_Ingresos));


                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }

            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }

            return View();
        }
        //[HttpPost]
        public ActionResult ReporteRegionNormalExcel(int Informe, int region, int Mes, int? periodo = 0)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.Mes = Mes;
            ViewBag.IdRegion = region;
            ViewBag.IdInforme = Informe;
            ViewBag.Periodo = periodo;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
               
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }



            return View();
        }


      
        public ActionResult ReporteRegionCC()
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Linea = new SelectList(db.LineasAtencion.ToList(), "ID", "Sigla");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            return View();
        }
        [HttpPost]
        public ActionResult ReporteRegionCC(int Informe, int region, int Mes, int? periodo)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.IdRegion = region;
            ViewBag.Periodo = periodo;
            ViewBag.Mes = Mes;
            ViewBag.IdInforme = Informe;
            try
            {
                ViewBag.SaldoInicial = db.Presupuesto.Where(m => m.Proyecto.Direccion.Comuna.RegionID == region && m.Activo != null && m.Activo.Equals("S") && m.Periodo == periodo).Sum(m => m.SaldoInicial);
            }
            catch
            {
                ViewBag.SaldoInicial = 0;
            }
            if (periodo == null)
            {

            }
            else
            {
                //var ingresos = db.Movimiento.Where(e => (e.CuentaID == 3 || e.CuentaID == 4 || e.CuentaID == 5 || e.CuentaID == 8 || e.CuentaID == 12) && e.Temporal == null && e.Periodo == periodo || e.Proyecto.Direccion.Comuna.Region.ID == region).Sum(m => m.Monto_Ingresos);
                //ViewBag.valorIngresos = (System.Numerics.Complex)(ingresos.Where(e => (e.CuentaID == 3 || e.CuentaID == 4 || e.CuentaID == 5 || e.CuentaID == 8 || e.CuentaID == 12) && e.Temporal == null && e.Periodo == periodo).Sum(m => m.Monto_Ingresos));


                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }

            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }

            return View();
        }
        //[HttpPost]
        public ActionResult ReporteRegionNormalExcelCC(int Informe, int region, int Mes, int? periodo = 0)
        {
            ViewBag.Informe = new SelectList(db.Informe, "ID", "nombreInforme");
            ViewBag.Proyectos = new SelectList(db.Proyecto.Where(p => p.estado == 1), "ID", "NombreLista");
            ViewBag.tipoPrograma = new SelectList(db.TipoProyecto.ToList(), "ID", "Sigla");
            ViewBag.region = new SelectList(db.Region.ToList(), "ID", "Nombre");
            ViewBag.listaCuentas = db.DetalleInformes.Where(d => d.informeID == Informe).OrderBy(d => d.cuentaID).ToList();
            ViewBag.Mes = Mes;
            ViewBag.IdRegion = region;
            ViewBag.IdInforme = Informe;
            ViewBag.Periodo = periodo;
            if (periodo == null)
            {

            }
            else
            {
                ViewBag.PresupuestoID = db.Presupuesto.Where(p => p.Proyecto.Direccion.Comuna.Region.ID == region && p.Periodo == periodo);
            }
            if (Mes < 13)
            {

                ViewBag.GrupoMeses = new int[1] {
Mes,
};
            }
            else if (Mes == 13)
            {
                ViewBag.GrupoMeses = new int[6] {
1,
2,
3,
4,
5,
6
};

            }
            else if (Mes == 14)
            {
                ViewBag.GrupoMeses = new int[6] {
7,
8,
9,
10,
11,
12
};

            }
            else if (Mes == 15)
            {

                ViewBag.GrupoMeses = new int[12] {
1,
2,
3,
4,
5,
6,
7,
8,
9,
10,
11,
12 };
            }



            return View();
        }

        


    }
}
