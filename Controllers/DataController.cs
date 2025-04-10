using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.IO;
using System.Web.Script.Serialization;


namespace SAG2.Controllers
{
    public class DataController : Controller
    {
        private SAG2DB db = new SAG2DB();
        public string SumaDetalleEgreso()
        {
            if (Session["DetalleEgreso"] != null)
            {
                int monto = 0;
                List<DetalleEgreso> lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                foreach (DetalleEgreso detalle in lista)
                {
                    monto += detalle.Monto;
                }

                return monto.ToString();
            }
            else
            {
                return "0";
            }
        }

        public string NDetalleEgreso(int x) { 
        DetalleEgreso xdata =  new DetalleEgreso();
        List<DetalleEgreso> Lista = new List<DetalleEgreso>();
        if (Session["DetalleEgreso"] != null)
        {
            Lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
            int i = 0;
            foreach (var data in Lista)
            {
                i++;
                if (x == i){
                    xdata.ID = data.ID;
                    xdata.BoletaHonorarioID = data.BoletaHonorarioID;
                    xdata.CuentaID = data.CuentaID;
                    xdata.Monto = data.Monto;
                    xdata.NDocumento = data.NDocumento;
                    xdata.DocumentoID = data.DocumentoID;
                    xdata.Glosa = data.Glosa;
                    xdata.DeudaPendienteID = data.DeudaPendienteID;
                    xdata.FondoFijoID = data.FondoFijoID;
                    xdata.FechaEmision = data.FechaEmision;
                    xdata.FechaVencimiento = data.FechaVencimiento;
                    xdata.Iva = data.Iva;
                }
            }
        }
        return new JavaScriptSerializer().Serialize(xdata);
        }


        public string NDetalleEgresoGrilla(int x)
        {
            DetalleEgreso xdata = new DetalleEgreso();          
            List<DetalleEgreso> Lista = new List<DetalleEgreso>();
            if (Session["DetalleEgreso"] != null)
            {
                Lista = (List<DetalleEgreso>)Session["DetalleEgreso"];

              var   data = Lista.Where(d => d.ID == x).FirstOrDefault();
              if (data != null)
              {
                  xdata.ID = data.ID;
                  xdata.BoletaHonorarioID = data.BoletaHonorarioID;
                  xdata.CuentaID = data.CuentaID;
                  xdata.Monto = data.Monto;
                  xdata.NDocumento = data.NDocumento;
                  xdata.DocumentoID = data.DocumentoID;
                  xdata.Glosa = data.Glosa;
                  xdata.DeudaPendienteID = data.DeudaPendienteID;
                  xdata.FondoFijoID = data.FondoFijoID;
                  xdata.FechaEmision = data.FechaEmision;
                  xdata.FechaVencimiento = data.FechaVencimiento;
                  xdata.Iva = data.Iva;
              }
              else {
                  int i = 0;
                  foreach (var dataN in Lista)
                  {
                      i++;
                      if (x == i)
                      {
                          xdata.ID = dataN.ID;
                          xdata.BoletaHonorarioID = dataN.BoletaHonorarioID;
                          xdata.CuentaID = dataN.CuentaID;
                          xdata.Monto = dataN.Monto;
                          xdata.NDocumento = dataN.NDocumento;
                          xdata.DocumentoID = dataN.DocumentoID;
                          xdata.Glosa = dataN.Glosa;
                          xdata.DeudaPendienteID = dataN.DeudaPendienteID;
                          xdata.FondoFijoID = dataN.FondoFijoID;
                          xdata.FechaEmision = dataN.FechaEmision;
                          xdata.FechaVencimiento = dataN.FechaVencimiento;
                          xdata.Iva = dataN.Iva;
                      }
                  }
              
              
              }
            }
            return new JavaScriptSerializer().Serialize(xdata);
        }

        public string DetalleEgreso()
        {
            List<DetalleEgreso> Lista = new List<DetalleEgreso>();
            List<FrmEgreso> ListaData = new List<FrmEgreso>();
            if (Session["DetalleEgreso"] != null)
            {            
                Lista = (List<DetalleEgreso>)Session["DetalleEgreso"];
                foreach( var data in Lista){
                    FrmEgreso xdata = new FrmEgreso();
                    xdata.NComprobanteDP = data.NComprobanteDP.ToString();
                    if (data.DocumentoID != null)
                    {
                        xdata.Documento = db.Documento.Find(data.DocumentoID).NombreLista;
                    }
                    xdata.NDocumento = long.Parse(data.NDocumento.ToString());
                    xdata.Monto = data.Monto;
                    xdata.FechaEmision = data.FechaEmision.ToShortDateString();
                    xdata.FechaVencimiento = data.FechaVencimiento.ToShortDateString();
                    xdata.NombreCuenta = db.Cuenta.Find(data.CuentaID).NombreLista;
                    xdata.FondoFijoID = data.FondoFijoID;
                    xdata.DeudaPendienteID = data.DeudaPendienteID;
                    xdata.BoletaHonorarioID = data.BoletaHonorarioID;
                    xdata.Iva = data.Iva;
                    xdata.DetalleID = data.ID;
                    if (data.Glosa == null)
                    {
                        xdata.Glosa = " ";
                    }
                    else
                    {
                        int len = data.Glosa.Length;
                        if (len > 20)
                        {
                            xdata.Glosa = data.Glosa.Substring(0, 19);
                        }
                        else
                        {
                            xdata.Glosa = data.Glosa;
                        }
                    }
                    ListaData.Add(xdata);
                  
                }
            }
            return new JavaScriptSerializer().Serialize(ListaData);
           
        }
        
        public string DetalleEgresoEliminar(int x)
        {
            DetalleEgreso xdata = new DetalleEgreso();
            List<DetalleEgreso> Lista = new List<DetalleEgreso>();
            if (Session["DetalleEgreso"] != null)
            {
                Lista = (List<DetalleEgreso>)Session["DetalleEgreso"];          

            }


            return new JavaScriptSerializer().Serialize(xdata);
        }
        
       
        public string SumaDetalleReintegro()
        {
            if (Session["DetalleReintegro"] != null)
            {
                int monto = 0;
                List<DetalleReintegro> lista = (List<DetalleReintegro>)Session["DetalleReintegro"];
                foreach (DetalleReintegro detalle in lista)
                {
                    monto += detalle.Monto;
                }

                return monto.ToString();
            }
            else
            {
                return "0";
            }
        }
       
        public int recibir()
        {
            int datos = 0;
       
                var httpPostedFile = System.Web.HttpContext.Current.Request.Files["file"];
                // var pic = System.Web.HttpContext.Current.Request.Files["file"];
                try
                {
                    HttpPostedFileBase filebase = new HttpPostedFileWrapper(httpPostedFile);
                    var fileName = Path.GetFileName(filebase.FileName);
                    // var path = Path.Combine(Server.MapPath("~/pdf/"), fileName);
                    // StreamReader csvreader = new StreamReader(filebase.FileName);
                    Session.Remove("DetalleIngreso");
                    List<DetalleIngreso> lista = null;

                    lista = new List<DetalleIngreso>();

                    StreamReader csvreader = new StreamReader(filebase.InputStream);

                    while (!csvreader.EndOfStream)
                    {
                        DetalleIngreso detalle = new DetalleIngreso();
                        var line = csvreader.ReadLine();
                        string[] values = line.Split(new Char[] { ';' });
                        detalle.Cc = values[0];
                        detalle.Cta = values[1];
                        detalle.Reg = values[2];
                        detalle.Sename = values[3];
                        detalle.Servicio = values[4];
                        detalle.Subvencion = values[5];
                        detalle.Porcentaje = values[6];
                        detalle.Traspaso = int.Parse(values[7]);
                        detalle.MovimientoID = 0;
                        lista.Add(detalle);
                        datos = datos + detalle.Traspaso;

                    }
                    Session.Add("DetalleIngreso", lista);
                    // filebase.SaveAs(path);
                }
                catch {
                    datos = 0;
                    Session.Remove("DetalleIngreso");
                }
                    return datos;
             

    }
   
    // Obtener glosa
        public string Glosacuenta(int xCuentaID)
        {
            CuentaGlosa Dato = new CuentaGlosa();
            // Datos Referenciados
            var Meses = new string[13]
        {
        " ","Enero","Febrero","Marzo","Abril","Mayo","Junio","Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"
        };
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            int mes_anterior = mes - 1;
            int Periodo_Anterior = periodo;
            if (mes_anterior == 0) {
                mes_anterior = 12;
                Periodo_Anterior = periodo - 1;
            }
          


            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            var xDato = db.CuentaGlosa.Where(d => d.CuentaID == xCuentaID && d.Estado == 1).FirstOrDefault();

            if (xDato != null) {

                string Glosa = xDato.Glosa;

                // Palabras Reservadas

                Glosa = Glosa.Replace("_mes",Meses[mes]);
                Glosa = Glosa.Replace("_periodo", periodo.ToString());
                Glosa = Glosa.Replace("_nProyecto", Proyecto.NombreLista);
                Glosa = Glosa.Replace("_Proyecto", Proyecto.Nombre);
                Glosa = Glosa.Replace("_MesAnterior", mes_anterior.ToString());
                Glosa = Glosa.Replace("_PeriodoAnterior", Periodo_Anterior.ToString());



                Dato.ID = xDato.ID;
                Dato.CuentaID = xDato.CuentaID;
                Dato.Glosa = Glosa;
                Dato.Respaldo = xDato.Respaldo;



            }

             

            return new JavaScriptSerializer().Serialize(Dato);
        }

        public string cuentaGlosa(int xCuentaID)
        {
            CuentaGlosa Dato = new CuentaGlosa();
            // Datos Referenciados
            var Meses = new string[13]
        {
        " ","Enero","Febrero","Marzo","Abril","Mayo","Junio","Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"
        };
            int periodo = (int)Session["Periodo"];
            int mes = (int)Session["Mes"];
            Proyecto Proyecto = (Proyecto)Session["Proyecto"];

            var xDato = db.CuentaGlosa.Where(d => d.CuentaID == xCuentaID && d.Estado == 1).FirstOrDefault();

            if (xDato != null)
            {

                string Glosa = xDato.Glosa;

                // Palabras Reservadas




                Dato.ID = xDato.ID;
                Dato.CuentaID = xDato.CuentaID;
                Dato.Glosa = Glosa;
                Dato.Respaldo = xDato.Respaldo;



            }



            return new JavaScriptSerializer().Serialize(Dato);
        }
    }
}
