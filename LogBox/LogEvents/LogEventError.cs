using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogBox.LogEvents
{
    /// <summary>
    /// Log event error
    /// </summary>
    public class LogEventError : LogEvent
    {
        /// <summary>
        /// Constructor of LogEventError
        /// </summary>
        /// <param name="logMessage">Message of log entry</param>
        public LogEventError(string logMessage) : base(LogTypes.ERROR, logMessage)
        {
        }
    }
}
