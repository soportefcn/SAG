using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAG2.Models
{
    public class Lista
    {
        public Lista ()
        {

        }

        public IEnumerable<SelectListItem> Años
        {
            get
            {
                return new SelectList(
                    Enumerable.Range(DateTime.Now.Year - 5, 6)
                    .OrderByDescending(year => year)
                    .Select(year => new SelectListItem
                    {
                        Value = year.ToString(),
                        Text = year.ToString()
                    }
                ), "Value", "Text", DateTime.Now.Year);
            }
        }

        public IEnumerable<SelectListItem> Meses
        {
            get
            {
                return new SelectList(new[] {"Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" }
                  .Select(x => new { value = x, text = x }),
                  "Value", "Text");
            }
        }

    }
}