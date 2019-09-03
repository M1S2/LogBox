using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogBox
{
    /// <summary>
    /// Log event info
    /// </summary>
    public class LogEventInfo : LogEvent
    {
        /// <summary>
        /// Constructor of LogEventInfo
        /// </summary>
        /// <param name="logMessage">Message of log entry</param>
        public LogEventInfo(string logMessage) : base(LogTypes.INFO, logMessage)
        {
        }
    }
}
