using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Data;
using System.Data.Entity;
using System.Web.Mvc;
using SAG2.Models;
using System.Diagnostics;
using System.Globalization;

namespace SAG2.Classes
{
    public class Util
    {
        private Constantes ctes = new Constantes();
           
        public string nombreAuditor(int Id){
            SAG2DB db = new SAG2DB();

            Persona Persona = db.Persona.Where(d=> d.ID == Id).Take(1).Single() ;
            string nombrecompleto = Persona.NombreCompleto;
            return nombrecompleto;
        }
        public string Digito(int rut)
        {
            int suma = 0;
            int multiplicador = 1;
            while (rut != 0)
            {
                multiplicador++;
                if (multiplicador == 8)
                    multiplicador = 2;
                suma += (rut % 10) * multiplicador;
                rut = rut / 10;
            }
            suma = 11 - (suma % 11);
            if (suma == 11)
            {
                return "0";
            }
            else if (suma == 10)
            {
                return "K";
            }
            else
            {
                return suma.ToString();
            }
        }

        public int darcorrelativoinventario(int periodo, int tipodoc, int proyectoID)
        {
            SAG2DB db = new SAG2DB();
            try
            {

                InventarioCorrelativo InventarioCorrelativo = db.InventarioCorrelativo.Where(b => b.ProyectoID == proyectoID && b.Periodo == periodo && b.TipoDoc == tipodoc).OrderByDescending(b => b.Periodo).Take(1).Single();
                if (InventarioCorrelativo.Periodo == periodo && InventarioCorrelativo.TipoDoc == tipodoc)
                {
                    // Registro encontrado de periodo actual
                    int valor = InventarioCorrelativo.Valor;
                    return valor;
                }
                else
                {
                    InventarioCorrelativo InventarioCorrelativos = new InventarioCorrelativo();
                    InventarioCorrelativos.ProyectoID = proyectoID;
                    InventarioCorrelativos.TipoDoc = tipodoc;
                    InventarioCorrelativos.Periodo = periodo;
                    InventarioCorrelativos.Valor = 1;
                    db.InventarioCorrelativo.Add(InventarioCorrelativos);
                    db.SaveChanges();
                    int valor = 1;
                    return valor;

                }
            }
            catch (Exception)
            {
                // No hay registro de bodega para el articulo y proyecto
                InventarioCorrelativo InventarioCorrelativos = new InventarioCorrelativo();
                InventarioCorrelativos.ProyectoID = proyectoID;
                InventarioCorrelativos.TipoDoc = tipodoc;
                InventarioCorrelativos.Periodo = periodo;
                InventarioCorrelativos.Valor = 1;
        //        db.InventarioCorrelativo.Add(InventarioCorrelativos);
         //       db.SaveChanges();
                int valor = 1;
                return valor;
                
            }
             }

        public void Aumentarcorrelativoinventario(int periodo, int tipodoc, int proyectoID)
        {
            SAG2DB db = new SAG2DB();
            try
            {

                InventarioCorrelativo InventarioCorrelativo = db.InventarioCorrelativo.Where(b => b.ProyectoID == proyectoID && b.Periodo == periodo && b.TipoDoc == tipodoc).OrderByDescending(b => b.Periodo).Take(1).Single();
                if (InventarioCorrelativo.Periodo == periodo && InventarioCorrelativo.TipoDoc == tipodoc)
                {
                    // Registro encontrado de periodo actual
                    InventarioCorrelativo.Valor = InventarioCorrelativo.Valor + 1;
                    db.Entry(InventarioCorrelativo).State = EntityState.Modified;
                    db.SaveChanges();
                }
               
            }
            catch (Exception)
            {
             

            }
        }
        
        public string mensajeError(string mensaje)
        {
            return "<div class=\"alert alert-danger fade in alert-dismissible\">" + mensaje + "</div>";
        }

        public string mensajeInfo(string mensaje)
        {
            return "<div class=\"alert alert-info fade in alert-dismissible\">" + mensaje + "</div>";
        }

        public string mensajeAdvertencia(string mensaje)
        {
            return "<div class=\"alert alert-warning fade in alert-dismissible\">" + mensaje + "</div>";
        }

        public string mensajeOK(string mensaje)
        {
            return "<div class=\"alert alert-success fade in alert-dismissible\">" + mensaje + "</div>";
        }

        public string Mensaje(Exception e)
        {
            // SAG2.Classes.Util.Mensaje(Exception e) in C:\Users\Daniel\Documents\Visual Studio 2010\Projects\SAG2\SAG2\Classes\Util.cs:41
            if (e.InnerException != null && e.InnerException.InnerException != null && e.InnerException.InnerException.InnerException != null)
            {
                return e.InnerException.InnerException.InnerException.Message + " " + e.StackTrace.ToString();
            }
            else if (e.InnerException != null && e.InnerException.InnerException != null)
            {
                return e.InnerException.InnerException.Message + " " + e.StackTrace.ToString();
            }
            else if (e.InnerException != null)
            {
                return e.InnerException.Message + " " + e.StackTrace.ToString();
            }
            else
            {
                return e.Message + " " + e.StackTrace.ToString();
            }
        }

        public string ToTitle(string text)
        {
            TextInfo myTI = new CultureInfo("es-CL", false).TextInfo;
            return myTI.ToTitleCase(text.ToString().ToLower());
        }

        public string desabilitarFormulario()
        {
            return "$(document).ready(function () { " +
                     "  $(\"input\").each(function () {" +
                     "      $(this).attr(\"readonly\", \"readonly\");" +
                     "      $(this).attr(\"disabled\", \"disabled\");" +
                     "  });" +
                     "  $(\"select\").each(function () {" +
                     "      $(this).attr(\"readonly\", \"readonly\");" +
                     "      $(this).attr(\"disabled\", \"disabled\");" +
                     "  });" +
                     "  $(\"textarea\").each(function () {" +
                     "      $(this).attr(\"readonly\", \"readonly\");" +
                     "      $(this).attr(\"disabled\", \"disabled\");" +
                     "  });" +
                     "});";
        }

        public string md5(string value)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

            byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
            data = provider.ComputeHash(data);

            string md5 = string.Empty;

            for (int i = 0; i < data.Length; i++)
                md5 += data[i].ToString("x2").ToLower();

            return md5;
        }

        public void erroresState(ModelStateDictionary Model)
        {
            foreach (var modelStateValue in Model.Values)
            {
                foreach (var error in modelStateValue.Errors)
                {
                    Log(2, "Exception LINQ | " + error.ErrorMessage + " | " + error.Exception);
                }
            }
        }

        public void Log(int level, string texto)
        {
            Trace.WriteLine(DateTime.Now.ToString() + " | " + level.ToString() + " | " + texto);
        }

        public string generarSelectHijos(Cuenta cuenta, int? ID = 0)
        {
            int egresoID = 6;
            int ingresoID = 1;
            string response = "";
            cuenta.Hijos = cuenta.Hijos.Where(c => c.ID != 80).OrderBy(c => c.Orden).ToList();
            foreach (Cuenta Hijo in cuenta.Hijos)
            {
                if (Hijo.Estado == 1)
                {
                    if (Hijo.ID == egresoID || Hijo.ID == ingresoID)
                        continue;

                    if (Hijo.Hijos.Count.Equals(0))
                    {
                        if (Hijo.ID.Equals(ID))
                        {
                            response += "<option value=\"" + Hijo.ID + "\" selected=\"selected\" title=\"" + Hijo.Descripcion + "\">" + Hijo.NombreLista + "</option>";
                        }
                        else
                        {
                            response += "<option value=\"" + Hijo.ID + "\" title=\"" + Hijo.Descripcion + "\">" + Hijo.NombreLista + "</option>";
                        }
                    }
                    else
                    {
                        response += "<optgroup label=\"" + Hijo.NombreLista + "\" title=\"" + Hijo.Descripcion + "\">";
                        response += generarSelectHijos(Hijo, ID);
                        response += "</optgroup>";
                    }
                }
            }
            return response;
        }

        public string generarSelectHijos2(Cuenta cuenta, int? ID = 0, int? Entrada = 1)
        {
            string response;
                SAG2DB db = new SAG2DB();
                response = generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaIngresos));
                response += generarSelectHijos(db.Cuenta.Find(ctes.raizCuentaEgresos));
                return response;

        }
        public string selectSecciones(Seccion seccion, int? ID = 0, string espacio = "")
        {
            string response = "";
            seccion.Hijos = seccion.Hijos.OrderBy(c => c.Orden).ToList();
            
            foreach (Seccion Hijo in seccion.Hijos)
            {
                if (Hijo.Hijos.Count.Equals(0))
                {
                    if (Hijo.ID.Equals(ID))
                    {
                        response += "<option value=\"" + Hijo.ID + "\" title=\"" + Hijo.Nombre + "\" selected=\"selected\">" + espacio + Hijo.Nombre + "</option>";
                    }
                    else
                    {
                        response += "<option value=\"" + Hijo.ID + "\" title=\"" + Hijo.Nombre + "\">" + espacio + Hijo.Nombre + "</option>";
                    }
                }
                else
                {
                    if (Hijo.ID.Equals(ID))
                    {
                        response += "<option value=\"" + Hijo.ID + "\" title=\"" + Hijo.Nombre + "\" selected=\"selected\">" + espacio + Hijo.Nombre + "</option>";
                    }
                    else
                    {
                        response += "<option value=\"" + Hijo.ID + "\" title=\"" + Hijo.Nombre + "\">" + espacio + Hijo.Nombre + "</option>";
                    }
                    //response += "<optgroup label=\"" + Hijo.Nombre + "\">";
                    response += selectSecciones(Hijo, ID, espacio + "&nbsp;" + "&nbsp;" + "&nbsp;" + "&nbsp;");
                    //response += "</optgroup>";
                }
            }
            return response;
        }

        public bool ingresarSaldoIngreso(Movimiento movimiento, ModelStateDictionary ModelState)
        {
            Saldo final = null;
            SAG2DB db = new SAG2DB();
            int saldoFinal = 0;
            try
            {
                // Si existe registro del saldo actual, se suma monto del ingreso al saldo final
                Saldo Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == movimiento.CuentaCorrienteID).Where(s => s.Periodo == movimiento.Periodo).Where(s => s.Mes == movimiento.Mes).Single();
                Saldo.SaldoFinal = Saldo.SaldoFinal + movimiento.Monto_Ingresos - movimiento.Monto_Egresos;
                if (ModelState.IsValid)
                {
                    db.Entry(Saldo).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    erroresState(ModelState);
                    return false;
                }
                saldoFinal = Saldo.SaldoFinal;
                Log(1, "Existe registro de saldo para periodo actual, se actualiza el saldo final.");
                final = Saldo;
            }
            catch (Exception e)
            {
                Log(1, "No existe registro de saldo para periodo actual. " + e.Message);
                try
                {
                    // Verificamos si existe periodo anterior
                    int periodo_anterior = movimiento.Periodo, mes_anterior = movimiento.Mes;
                    if (movimiento.Mes == 1)
                    {
                        mes_anterior = 12;
                        periodo_anterior--;
                    }
                    else
                    {
                        mes_anterior--;
                    }
                    Saldo SaldoAnterior = db.Saldo.Where(s => s.CuentaCorrienteID == movimiento.CuentaCorrienteID).Where(s => s.Periodo == periodo_anterior).Where(s => s.Mes == mes_anterior).Single();
                    // Ingresamos nuevo registro de saldo y actualizamos saldo final
                    Saldo SaldoActual = new Saldo();
                    SaldoActual.SaldoInicialCartola = SaldoAnterior.SaldoFinal;
                    SaldoActual.SaldoFinal = SaldoAnterior.SaldoFinal + movimiento.Monto_Ingresos - movimiento.Monto_Egresos;
                    SaldoActual.Periodo = movimiento.Periodo;
                    SaldoActual.Mes = movimiento.Mes;
                    SaldoActual.CuentaCorrienteID = movimiento.CuentaCorrienteID;
                    db.Saldo.Add(SaldoActual);
                    db.SaveChanges();
                    saldoFinal = SaldoActual.SaldoFinal;
                    Log(1, "Existe registro de saldo para periodo anterior y se ingresa y actualiza saldo para periodo actual");
                    final = SaldoActual;
                }
                catch (Exception e2)
                {
                    Log(1, "No existe registro de saldo para periodo anterior. " + e2.Message);
                    try
                    {
                        // Si no existe saldo para periodo actual ni anterior, definimos saldo inicial en CERO en el periodo actual para la cuenta corriente.
                        Saldo SaldoActual = new Saldo();
                        SaldoActual.SaldoInicialCartola = 0;
                        SaldoActual.SaldoFinal = movimiento.Monto_Ingresos - movimiento.Monto_Egresos;
                        SaldoActual.Periodo = movimiento.Periodo;
                        SaldoActual.Mes = movimiento.Mes;
                        SaldoActual.CuentaCorrienteID = movimiento.CuentaCorrienteID;
                        db.Saldo.Add(SaldoActual);
                        db.SaveChanges();

                        saldoFinal = 0;// SaldoActual.SaldoFinal;
                        Log(1, "Se crea registro para saldo para periodo actual y se define saldo inicial CERO.");
                        final = SaldoActual;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            actualizarSaldos(final);
            return true;
        }

        public bool actualizarSaldoIngreso(Movimiento movimiento, ModelStateDictionary ModelState, int montoOriginal = 0)
        {
            SAG2DB db = new SAG2DB();
            Saldo Saldo = null;
            try
            {
                Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == movimiento.CuentaCorrienteID).Where(s => s.Periodo == movimiento.Periodo).Where(s => s.Mes == movimiento.Mes).Single();
                Saldo.SaldoFinal = Saldo.SaldoFinal - montoOriginal + movimiento.Monto_Ingresos;
                db.Entry(Saldo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            actualizarSaldos(Saldo);
            return true;
        }

        public bool actualizarSaldoIEgreso(Movimiento movimiento, ModelStateDictionary ModelState, int montoOriginal = 0)
        {
            SAG2DB db = new SAG2DB();
            Saldo Saldo = null;
            try
            {
                Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == movimiento.CuentaCorrienteID).Where(s => s.Periodo == movimiento.Periodo).Where(s => s.Mes == movimiento.Mes).Single();
                Saldo.SaldoFinal = Saldo.SaldoFinal + montoOriginal + movimiento.Monto_Egresos;
                db.Entry(Saldo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            actualizarSaldos(Saldo);
            return true;
        }

        public bool actualizarSaldoEgreso(Movimiento movimiento, ModelStateDictionary ModelState, int montoOriginal = 0)
        {
            SAG2DB db = new SAG2DB();
            Saldo Saldo = null;
            try
            {
                Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == movimiento.CuentaCorrienteID).Where(s => s.Periodo == movimiento.Periodo).Where(s => s.Mes == movimiento.Mes).Single();
                Saldo.SaldoFinal = Saldo.SaldoFinal + montoOriginal - movimiento.Monto_Egresos;
                db.Entry(Saldo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            actualizarSaldos(Saldo);
            return true;
        }

        public bool anularSaldoIngreso(Movimiento movimiento, ModelStateDictionary ModelState, int montoOrginal)
        {
            SAG2DB db = new SAG2DB();
            Saldo Saldo = null;
            try
            {
                Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == movimiento.CuentaCorrienteID).Where(s => s.Periodo == movimiento.Periodo).Where(s => s.Mes == movimiento.Mes).Single();
                Saldo.SaldoFinal = Saldo.SaldoFinal - montoOrginal;
                db.Entry(Saldo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            actualizarSaldos(Saldo);
            return true;
        }

        public bool anularSaldoEgreso(Movimiento movimiento, ModelStateDictionary ModelState, int montoOrginal)
        {
            SAG2DB db = new SAG2DB();
            Saldo Saldo = null;
            try
            {
                Saldo = db.Saldo.Where(s => s.CuentaCorrienteID == movimiento.CuentaCorrienteID).Where(s => s.Periodo == movimiento.Periodo).Where(s => s.Mes == movimiento.Mes).Single();
                Saldo.SaldoFinal = Saldo.SaldoFinal + montoOrginal;
                db.Entry(Saldo).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                return false;
            }

            actualizarSaldos(Saldo);
            return true;
        }

        private void actualizarSaldos(Saldo saldo)
        {
            if (saldo == null)
            {
                return;
            }

            SAG2DB db = new SAG2DB();
            int SaldoInicialPeriodo = saldo.SaldoInicialCartola;
            List<Movimiento> Movimientos = (from m in db.Movimiento
                                            where m.Periodo == saldo.Periodo && m.Mes == saldo.Mes && m.CuentaCorrienteID == saldo.CuentaCorrienteID && 
                                                  m.Temporal == null && m.Nulo == null && m.Eliminado == null
                                            orderby m.ID
                                            select m).ToList();

            foreach (Movimiento Movimiento in Movimientos)
            {
                Movimiento.Saldo = SaldoInicialPeriodo + Movimiento.Monto_Ingresos - Movimiento.Monto_Egresos;
                SaldoInicialPeriodo = Movimiento.Saldo;
                db.Entry(Movimiento).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void RecalcularSaldos(int p_desde, int p_hasta, int m_desde, int m_hasta, Proyecto p, CuentaCorriente c)
        {
            SAG2DB db = new SAG2DB();
            int saldoInicial = 0;

            try
            {
                saldoInicial = db.Saldo.Where(s => s.CuentaCorrienteID == c.ID && s.Mes == m_desde && s.Periodo == p_desde).Single().SaldoInicialCartola;
            }
            catch (Exception)
            {
                saldoInicial = 0;
            }

            int saldo = saldoInicial;

            if (p_desde == p_hasta)
            {
                for (int i = m_desde; i <= m_hasta; i++)
                {
                    int ig = db.Movimiento.Where(m => m.Mes == i && m.ProyectoID == p.ID && m.Periodo == p_desde && m.Nulo == null && m.Temporal == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos);
                    int eg = db.Movimiento.Where(m => m.Mes == i && m.ProyectoID == p.ID && m.Periodo == p_desde && m.Nulo == null && m.Temporal == null && m.Eliminado == null).Sum(m => m.Monto_Egresos);

                    Saldo sa = db.Saldo.Where(s => s.CuentaCorrienteID == c.ID && s.Mes == i && s.Periodo == p_desde).Single();
                    sa.SaldoInicialCartola = saldo;
                    saldo = saldo + ig - eg;
                    sa.SaldoFinal = saldo;
                    db.Entry(sa).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            // Exite cambio de año en las modificaciones
            if (p_desde < p_hasta)
            {
                for (int i = m_desde; i <= 12; i++)
                {
                    int ig = db.Movimiento.Where(m => m.Mes == i && m.ProyectoID == p.ID && m.Periodo == p_desde && m.Nulo == null && m.Temporal == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos);
                    int eg = db.Movimiento.Where(m => m.Mes == i && m.ProyectoID == p.ID && m.Periodo == p_desde && m.Nulo == null && m.Temporal == null && m.Eliminado == null).Sum(m => m.Monto_Egresos);

                    Saldo sa = db.Saldo.Where(s => s.CuentaCorrienteID == c.ID && s.Mes == i && s.Periodo == p_desde).Single();
                    sa.SaldoInicialCartola = saldo;
                    saldo = saldo + ig - eg;
                    sa.SaldoFinal = saldo;
                    db.Entry(sa).State = EntityState.Modified;
                    db.SaveChanges();
                }

                for (int i = 1; i <= m_hasta; i++)
                {
                    int ig = db.Movimiento.Where(m => m.Mes == i && m.ProyectoID == p.ID && m.Periodo == p_hasta && m.Nulo == null && m.Temporal == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos);
                    int eg = db.Movimiento.Where(m => m.Mes == i && m.ProyectoID == p.ID && m.Periodo == p_hasta && m.Nulo == null && m.Temporal == null && m.Eliminado == null).Sum(m => m.Monto_Egresos);

                    Saldo sa = db.Saldo.Where(s => s.CuentaCorrienteID == c.ID && s.Mes == i && s.Periodo == p_hasta).Single();
                    sa.SaldoInicialCartola = saldo;
                    saldo = saldo + ig - eg;
                    sa.SaldoFinal = saldo;
                    db.Entry(sa).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public void calcularSaldos(int p_, int m_, Proyecto p, CuentaCorriente c)
        {
            SAG2DB db = new SAG2DB();
            int saldoInicial = db.Saldo.Where(s => s.CuentaCorrienteID == c.ID && s.Mes == p_ && s.Periodo == p_).Single().SaldoInicialCartola;
            int saldo = saldoInicial;
            int ig = db.Movimiento.Where(m => m.Mes == m_ && m.ProyectoID == p.ID && m.Periodo == p_ && m.Nulo == null && m.Temporal == null && m.Eliminado == null).Sum(m => m.Monto_Ingresos);
            int eg = db.Movimiento.Where(m => m.Mes == m_ && m.ProyectoID == p.ID && m.Periodo == p_ && m.Nulo == null && m.Temporal == null && m.Eliminado == null).Sum(m => m.Monto_Egresos);
            Saldo sa = db.Saldo.Where(s => s.CuentaCorrienteID == c.ID && s.Mes == m_ && s.Periodo == p_).Single();
            sa.SaldoInicialCartola = saldo;
            saldo = saldo + ig - eg;
            sa.SaldoFinal = saldo;
            db.Entry(sa).State = EntityState.Modified;
            db.SaveChanges();
        }

        public bool fondoFijoDisponible()
        {
            SAG2DB db = new SAG2DB();
            try
            {
                int monto = db.FondoFijo.Where(f => f.EgresoID == null).Sum(f => f.Monto);
                if (monto > ctes.porcentajeFondoFijo * ctes.montoFondoFijo)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public string cuentaPadre(Cuenta cuenta, int i = 0)
        {
            if (i == 0)
            {
                if (!cuenta.Codigo.Contains("."))
                {
                    return cuenta.NombreLista;
                }
                else
                {
                    return cuentaPadre(cuenta.Padre);
                }
            }
            else
            {
                if (!cuenta.Codigo.Contains("."))
                {
                    if (cuenta.NombreLista.Length > i)
                        return cuenta.NombreLista.Substring(0, i);
                    else
                        return cuenta.NombreLista;
                }
                else
                {
                    return cuentaPadre(cuenta.Padre, i);
                }
            }
        }
        /*
        public int cuentasHijos(Cuenta padre)
        { 
            
        }*/

        /*
        public int? sumarHijos(Cuenta cuenta, int proyectoID, int periodo, int mes)
        { 
            SAG2DB db = new SAG2DB();
            int subtotal = 0;
            
            foreach (Cuenta hijo in cuenta.Hijos)
            {
                return subtotal + sumarHijos(hijo, proyectoID, periodo, mes);
            }

            if (cuenta.Hijos.Count == 0)
            {
                int ingresos = db.Movimiento.Where(m => m.ProyectoID == proyectoID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Sum(m => m.Monto_Ingresos);
                int egresos = db.Movimiento.Where(m => m.ProyectoID == proyectoID).Where(m => m.Periodo == periodo).Where(m => m.Mes == mes).Sum(m => m.Monto_Egresos);
                int total = ingresos - egresos;
                return total;
            }
        }
         * */



        public string desgloseCuentas(Cuenta cuenta)
        {
            int egresoID = 6;
            int ingresoID = 1;
            string response = "";
            cuenta.Hijos = cuenta.Hijos.OrderBy(c => c.Orden).ToList();

            foreach (Cuenta Hijo in cuenta.Hijos)
            {
                if (Hijo.ID == egresoID || Hijo.ID == ingresoID)
                    continue;

                if (Hijo.Hijos.Count.Equals(0))
                {
                    response += "<tr><td>" + Hijo.NombreLista + "</td></tr>";
                }
                else
                {
                    response += "<tr><td><strong>" + Hijo.NombreLista + "</strong></td>";
                    // Subtotales por cuenta
                    response += "<td></td>";


                    response += "</tr>";
                    response += "<group cuenta=\"" + Hijo.NombreLista + "\">";
                    response += desgloseCuentas(Hijo);
                    response += "</group>";
                }
            }
            return response;
        }
     
    }
}