using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAG2.Controllers
{
    public class ListaController : Controller
    {
        SAG2.Models.Lista Lista = new SAG2.Models.Lista();
        
        public ActionResult Años()
        {
            return View(Lista);
        }

        public ActionResult Meses()
        {
            return View(Lista);
        }
    }
}
