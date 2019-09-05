using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogBox.LogEvents
{
    /// <summary>
    /// Abstract base class for log events
    /// </summary>
    public abstract class LogEvent
    {
        /// <summary>
        /// Type of log entry
        /// </summary>
        public LogTypes LogType { get; set; }

        /// <summary>
        /// Time of log entry
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// Message of log entry
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// Constructor of LogEvent
        /// </summary>
        /// <param name="logType">Type of log entry</param>
        /// <param name="logMessage">Message of log entry</param>
        public LogEvent(LogTypes logType, string logMessage)
        {
            LogType = logType;
            LogTime = DateTime.Now;
            LogMessage = logMessage;
        }
    }
}
