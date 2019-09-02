using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace GitReleaseTool_WPF
{
    /// <summary>
    /// Interaktionslogik für LogBox.xaml
    /// </summary>
    public partial class LogBox : UserControl
    {
        /// <summary>
        /// List with all log events
        /// </summary>
        public ObservableCollection<LogEvent> LogEvents { get; private set; }

        /// <summary>
        /// List with all log events that are shown (not filtered)
        /// </summary>
        public ObservableCollection<LogEvent> ShownLogEvents { get; private set; }

        private bool _showInfos;
        /// <summary>
        /// Show all INFO events
        /// </summary>
        public bool ShowInfos
        {
            get { return _showInfos; }
            set
            {
                _showInfos = value;
                RefreshShownLogEvents();
            }
        }

        private bool _showWarnings;
        /// <summary>
        /// Show all WARNING events
        /// </summary>
        public bool ShowWarnings
        {
            get { return _showWarnings; }
            set
            {
                _showWarnings = value;
                RefreshShownLogEvents();
            }
        }

        private bool _showErrors;
        /// <summary>
        /// Show all ERROR events
        /// </summary>
        public bool ShowErrors
        {
            get { return _showErrors; }
            set
            {
                _showErrors = value;
                RefreshShownLogEvents();
            }
        }

        //***********************************************************************************************************************************************************************************************************

        public LogBox()
        {
            InitializeComponent();
            DataContext = this;
            LogEvents = new ObservableCollection<LogEvent>();
            ShownLogEvents = new ObservableCollection<LogEvent>();
            listView_Log.ItemsSource = ShownLogEvents;
            ShowInfos = true;
            ShowWarnings = true;
            ShowErrors = true;
        }

        //***********************************************************************************************************************************************************************************************************

        private void btn_clearLog_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create a new log entry with type, time and message
        /// </summary>
        /// <param name="logType">type of the log entry</param>
        /// <param name="logMessage">log message</param>
        public void LogEvent(LogTypes logType, string logMessage)
        {
            LogEvent logEvent = new LogEvent(logType, DateTime.Now, logMessage);
            LogEvent(logEvent);
        }

        /// <summary>
        /// Create a new log entry with type, time and message
        /// </summary>
        /// <param name="logEvent">log event</param>
        public void LogEvent(LogEvent logEvent)
        {
            LogEvents.Add(logEvent);
            RefreshShownLogEvents();
            listView_Log.ScrollIntoView(logEvent);
        }

        /// <summary>
        /// Clear all log entries
        /// </summary>
        public void ClearLog()
        {
            LogEvents.Clear();
            ShownLogEvents.Clear();
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Refresh the list of shown log entries
        /// </summary>
        private void RefreshShownLogEvents()
        {
            ShownLogEvents = new ObservableCollection<LogEvent>(LogEvents.Where(log => (ShowInfos == true && log.LogType == LogTypes.INFO) || (ShowWarnings == true && log.LogType == LogTypes.WARNING) || (ShowErrors == true && log.LogType == LogTypes.ERROR)));
            listView_Log.ItemsSource = ShownLogEvents;
            Tools.ResizeListViewColumns(listView_Log);
        }

    }
}
