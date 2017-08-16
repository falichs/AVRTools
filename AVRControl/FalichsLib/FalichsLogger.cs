using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;


namespace FalichsLib
{
    public class FalichsLogger
    {


        private static FalichsLogger _instance;
        public enum Severity
        {
            INFO,
            WARNING,
            ERROR,
            DEBUG
        }
        private static readonly string LOG_FILENAME = "logfile.txt";

        private static Object theLock = new Object();


        public static FalichsLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FalichsLogger();
                }
                return _instance;
            }
        }


        private FalichsLogger()
        {
            File.WriteAllText(LOG_FILENAME, "LOG");
        }

        public void log(Severity severity, string msg)
        {
            msg = string.Format("{0:G}: [{1}] {2}{3}", DateTime.Now, severity ,msg, Environment.NewLine);
            lock (theLock)
            {
                File.AppendAllText(LOG_FILENAME, msg);
            }
        }
    }
}
