using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Permissions;
using System.Configuration;


namespace SAG2.Classes
{
    public class LogReg
    {
       
        string path = ConfigurationManager.AppSettings["RutaHivo"];
        string pathReg = "";
        StreamWriter writer;

        public LogReg()
        {
            DateTime Hoy = DateTime.Today;
            string fecha_actual = Hoy.ToString("yyyyMMdd");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileIOPermission f = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }
            else
            {
                FileIOPermission f = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }

            path = path + fecha_actual + "\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }
            else
            {
                FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
            }
        }

        public void abrirArchivo(string archivo)
        {
            try
            {
                pathReg = archivo + ".csv";

                if (File.Exists(path + pathReg))
                {
                    FileStream newPath = File.Open(Path.Combine(path, pathReg), FileMode.Append);
                    writer = new StreamWriter(newPath);
                }
                else
                {
                    FileStream newPath = File.Create(Path.Combine(path, pathReg), 1, FileOptions.Asynchronous);
                    writer = new StreamWriter(newPath);
                }

            }
            catch(Exception) {
     
            }
        }

        public void encabezado(string logMessage)
        {
            try
            {
                writer.WriteLine("Log Entry: " + logMessage);
            }
            catch (Exception)
            {

            }
        }
        public void registrar(string logMessage)
        {
            try { 
           // writer.Write("{0}", DateTime.Now.ToLongTimeString());
                writer.WriteLine( logMessage);
            }
            catch (Exception)
            {

            }
        }

        public void cerrar()
        {
            try
            {
                writer.Close();
            }
            catch (Exception)
            {

            }
        }
    }
}