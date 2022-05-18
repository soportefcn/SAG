using SAG2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAG2.Controllers
{
    public class InventarioEspecieController : Controller
    {
         private SAG2DB db = new SAG2DB();
        //
        // GET: /InventarioEspecie/

         public PartialViewResult _GetForInventario(Int32 inventarioID)
         {
             ViewBag.inventarioID = inventarioID;
             ViewBag.especi = db.InventarioEspecie.Where(c => c.InventarioID == inventarioID).ToList();
           List<InventarioEspecie> inventarioespecies = db.InventarioEspecie.Where(c => c.InventarioID == inventarioID).ToList();
             return PartialView("_GetForInventario", inventarioespecies);
         }

         [ChildActionOnly()]
         public PartialViewResult _InventarioEspecieForm(Int32 inventarioID)
         {
             ViewBag.EspecieID = new SelectList(db.Especie, "ID", "nombre");
            
             
             //ViewBag.especi = db.InventarioEspecie.Where(c => c.InventarioID == inventarioID).ToList();
             InventarioEspecie inventarioEspecie = new InventarioEspecie() { InventarioID = inventarioID };
            // List<InventarioEspecie> InventarioEspecie = db.InventarioEspecie.Where(c => c.InventarioID == inventarioID).ToList();
             return PartialView("_InventarioEspecieForm", inventarioEspecie);
         }

         public PartialViewResult _Submit(InventarioEspecie inventarioespecies)
         {
             
             db.InventarioEspecie.Add(inventarioespecies);
             db.SaveChanges();
             ViewBag.especi = db.InventarioEspecie.Where(c => c.InventarioID == inventarioespecies.InventarioID).ToList();
             List<InventarioEspecie> InventarioEspecie = db.InventarioEspecie.Where(c => c.InventarioID == inventarioespecies.InventarioID).ToList();
             ViewBag.InventarioID = inventarioespecies.InventarioID;

             return PartialView("_GetForInventario", InventarioEspecie);
         }




    }
}
