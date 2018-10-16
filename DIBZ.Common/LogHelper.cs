using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Common
{
    public static class LogHelper
    {
        private static ILog _logger;

        static LogHelper()
        {
            _logger = log4net.LogManager.GetLogger(typeof(LogHelper));
        }

        public static void LogInfo(string message)
        {
            _logger.Info(message);
        }
        public static void LogError(string message, Exception ex = null)
        {
            if (ex != null)
                _logger.Error(message, ex);
            else
                _logger.Error(message);
        }

    }
}
