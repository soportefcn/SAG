using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace SAG2.Classes
{
    public class Logger
    {
        public Logger()
        {
            
        }

        public void error(string texto)
        {
            StackTrace stackTrace = new StackTrace(true);
            StackFrame stackframe = stackTrace.GetFrame(1);
            Trace.WriteLine(DateTime.Now.ToString() + " | " + stackframe.GetMethod().Name + " | " + stackframe.GetFileName() + ":" + stackframe.GetFileLineNumber() + " | ERROR | " + texto);
        }

        public void debug(string texto)
        {
            StackTrace stackTrace = new StackTrace(true);
            StackFrame stackframe = stackTrace.GetFrame(1);
            Trace.WriteLine(DateTime.Now.ToString() + " | " +stackframe.GetMethod().Name + " | " + stackframe.GetFileName() + ":" + stackframe.GetFileLineNumber() + " | DEBUG | " + texto);
        }

        public void info(string texto)
        {
            StackTrace stackTrace = new StackTrace(true);
            StackFrame stackframe = stackTrace.GetFrame(1);
            Trace.WriteLine(DateTime.Now.ToString() + " | " +stackframe.GetMethod().Name + " | " + stackframe.GetFileName() + ":" + stackframe.GetFileLineNumber() + " | INFO | " + texto);
        }
    }
}