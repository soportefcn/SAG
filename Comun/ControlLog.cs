using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAG2.Models;
using System.Data;
using SAG2.Classes;
using System.Data.Entity;

namespace SAG2.Comun
{
    public class ControlLog
    {
          

            public int RegistraControl(string Proceso, string Descripcion, int periodo, int mes, int UsuarioID, int ProyectoID)
            {
                using (SAG2DB d = new SAG2DB())
                {
                    ControlFlujo Datos = new ControlFlujo();
                    Datos.Descripcion = Descripcion;
                    Datos.Fecha = DateTime.Now;
                    Datos.Proceso = Proceso;
                    Datos.MesSistema = mes;
                    Datos.PeriodoSistema = periodo;
                    Datos.UsuarioID = UsuarioID;
                    Datos.ProyectoID = ProyectoID;

                    d.ControlFlujo.Add(Datos);
                    d.SaveChanges();
                    return Datos.ID; 
                }
 
            }
       
    }
}