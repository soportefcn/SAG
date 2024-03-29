﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
namespace SAG2.Comun
{
    public class Correo
    {
        public static string emisor = WebConfigurationManager.AppSettings["emisor"];
        public static string password = WebConfigurationManager.AppSettings["password"];
        public static string Host = WebConfigurationManager.AppSettings["Host"];
        public static string Port = WebConfigurationManager.AppSettings["Port"];
        public static string EnableSsl = WebConfigurationManager.AppSettings["EnableSsl"];
        public static string SitioWeb = WebConfigurationManager.AppSettings["SitioWed"];
        public static string rutaCorreo = WebConfigurationManager.AppSettings["rutaCorreo"];

        public static string enviarCorreo(string destinatario, string mensaje, string asunto)
        {

            MailMessage correos = new MailMessage();
            SmtpClient envios = new SmtpClient();
            try
            {

                correos.To.Clear();
                correos.Body = "";
                correos.Subject = "";
                mensaje.Replace("Ñ", "N"); 
                string textofinal = "";
                textofinal = "<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'><meta name='language' content='es'>";
	            textofinal = textofinal + "<title>" + asunto + "</title></head>";
                textofinal = textofinal + "<style> table, th, td { border: 1px solid black; border-collapse: collapse; border-color: #96D4D4;}</style>";
                textofinal = textofinal + "<body><div style='padding:10px; border-radius:5px; border: 1px solid #d8d8d8;'>";
                textofinal = textofinal + "<div>" + "para" +   mensaje + "</div> ";
                textofinal = textofinal + "<div> <p style='font-weight:bold; text-align:left;'>Saludos<br>Fundacion Ciudad del Ni&ntilde;o </p> <p style='text-align:left;'>Por favor, No responder este correo electr&oacute;nico</p> </div>";
                textofinal = textofinal + "<a href='https://sag.ciudaddelnino.cl/SAG_5/Login/'>Sistema SAG</a>";

        

                correos.Body = textofinal;
                correos.Subject = asunto;
                correos.IsBodyHtml = true;
                correos.To.Add(destinatario.Trim());


                correos.From = new MailAddress(emisor);
                envios.Credentials = new NetworkCredential(emisor, password);
                envios.Host = Host;
                envios.Port = int.Parse(Port);
                envios.EnableSsl = bool.Parse(EnableSsl);

                envios.Send(correos);
                return "OK";
            }
            catch (Exception e )
            {
               string  a = e.Message;
                return "NOK";
            }

        }
    }
}