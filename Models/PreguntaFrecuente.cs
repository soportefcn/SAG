using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SAG2.Models
{
    public class PreguntaFrecuente
    {
        public int ID { get; set; }
        public string Pregunta { get; set; }
        public string Respuesta { get; set; }
        //public int PreguntaID { get; set; }
        //public int RespondeID { get; set; }
        [Display(Name = "Fecha Pregunta")]
        public DateTime PreguntaFecha { get; set; }

        [Display(Name = "Fecha Respuesta")]
        public DateTime? RespondeFecha { get; set; }
        public string Comentario { get; set; }
        public string Activo { get; set; }

        public int PersonaID { get; set; }
        public virtual Persona Persona { get; set; }

        //public int? RespondeID { get; set; }
        //public virtual Persona RespondePersona { get; set; }
    }
}