using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogBox.LogEvents
{
    /// <summary>
    /// Log event warning
    /// </summary>
    public class LogEventWarning : LogEvent
    {
        /// <summary>
        /// Constructor of LogEventWarning
        /// </summary>
        /// <param name="logMessage">Message of log entry</param>
        public LogEventWarning(string logMessage) : base(LogTypes.WARNING, logMessage)
        {
        }
    }
}
