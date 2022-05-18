using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.IO;

namespace SAG2.Controllers
{
    public class DataController : Controller
    {
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
    }
}
