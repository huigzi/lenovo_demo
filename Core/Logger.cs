using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Logger
    {
        private readonly NLog.Logger logger;

        public Logger() => logger = NLog.LogManager.GetCurrentClassLogger();
        
        public void WriteToLog(string msg) => logger.Debug(msg);
        
        public void WriteToConsole(string msg) => logger.Info(msg);
        
        public void ShutDown() => NLog.LogManager.Shutdown();
    }
}
