using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Logger
    {
        private static NLog.Logger _logger;

        public Logger() => _logger = NLog.LogManager.GetCurrentClassLogger();

        public void WriteToLog(string msg) => _logger.Debug(msg);
        
        public void WriteToConsole(string msg) => _logger.Info(msg);
        
        public void ShutDown() => NLog.LogManager.Shutdown();
    }
}
