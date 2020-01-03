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
using System.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LogBox.LogEvents;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace LogBox
{
    /// <summary>
    /// Interaktionslogik für LogBoxControl.xaml
    /// </summary>
    public partial class LogBoxControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This method is called by the Set accessor of each property. The CallerMemberName attribute that is applied to the optional propertyName parameter causes the property name of the caller to be substituted as an argument.
        /// </summary>
        /// <param name="propertyName">Name of the property that is changed</param>
        /// see: https://docs.microsoft.com/de-de/dotnet/framework/winforms/how-to-implement-the-inotifypropertychanged-interface
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// List with all log events
        /// </summary>
        public ObservableCollection<LogEvent> LogEvents { get; private set; }

        private LogEvent _selectedLogEvent;
        /// <summary>
        /// LogEvent that is selected in the listView
        /// </summary>
        public LogEvent SelectedLogEvent
        {
            get { return _selectedLogEvent; }
            set
            {
                _selectedLogEvent = value;
                IsSelectedLogEventTypeImage = (SelectedLogEvent == null ? false : SelectedLogEvent.LogType == LogTypes.IMAGE);
                LogImageProperty = (IsSelectedLogEventTypeImage == false ? null : ((LogEventImage)SelectedLogEvent).LogImage);
            }
        }

        private bool _isSelectedLogEventTypeImage;
        /// <summary>
        /// True if the SelectedLogEvent is an image log event; otherwise False
        /// </summary>
        public bool IsSelectedLogEventTypeImage
        {
            get { return _isSelectedLogEventTypeImage; }
            set { _isSelectedLogEventTypeImage = value; NotifyPropertyChanged(); }
        }

        private Bitmap _logImageProperty;
        /// <summary>
        /// The image of the selected log event if it's an image log event; otherwise null
        /// </summary>
        public Bitmap LogImageProperty
        {
            get { return _logImageProperty; }
            set { _logImageProperty = value; NotifyPropertyChanged(); }
        }

        //***********************************************************************************************************************************************************************************************************

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
                NotifyPropertyChanged();
                CollectionViewSource.GetDefaultView(LogEvents).Refresh();
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
                NotifyPropertyChanged();
                CollectionViewSource.GetDefaultView(LogEvents).Refresh();
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
                NotifyPropertyChanged();
                CollectionViewSource.GetDefaultView(LogEvents).Refresh();
            }
        }

        private bool _showImageLogs;
        /// <summary>
        /// Show all IMAGE events
        /// </summary>
        public bool ShowImageLogs
        {
            get { return _showImageLogs; }
            set
            {
                _showImageLogs = value;
                NotifyPropertyChanged();
                CollectionViewSource.GetDefaultView(LogEvents).Refresh();
            }
        }

        private bool _enableImageLogs;
        /// <summary>
        /// Enable or disable the possibility to log images
        /// </summary>
        public bool EnableImageLogs
        {
            get { return _enableImageLogs; }
            set { _enableImageLogs = value; NotifyPropertyChanged(); ShowImageLogs = _enableImageLogs; }
        }

        //***********************************************************************************************************************************************************************************************************

        private bool _autoScrollToLastLogEntry;
        /// <summary>
        /// Automatically scroll to the last log entry
        /// </summary>
        public bool AutoScrollToLastLogEntry
        {
            get { return _autoScrollToLastLogEntry; }
            set
            {
                _autoScrollToLastLogEntry = value;
                NotifyPropertyChanged();
                if (_autoScrollToLastLogEntry) { ScrollToLastLogEvent(); }
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private bool _isSearchEnabled;
        /// <summary>
        /// Is Search enabled
        /// </summary>
        public bool IsSearchEnabled
        {
            get { return _isSearchEnabled; }
            set
            {
                _isSearchEnabled = value;
                NotifyPropertyChanged();
                CollectionViewSource.GetDefaultView(LogEvents).Refresh();
            }
        }

        private string _searchText;
        /// <summary>
        /// Search text
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyPropertyChanged();
                CollectionViewSource.GetDefaultView(LogEvents).Refresh();
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private string _lastLogDirectory;

        //***********************************************************************************************************************************************************************************************************

        public LogBoxControl()
        {
            InitializeComponent();
            DataContext = this;
            LogEvents = new ObservableCollection<LogEvent>();

            listView_Log.ItemsSource = LogEvents;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(LogEvents);
            view.Filter = filterListViewItems;

            ShowInfos = true;
            ShowWarnings = true;
            ShowErrors = true;
            ShowImageLogs = false;
            EnableImageLogs = false;
            AutoScrollToLastLogEntry = true;

            _lastLogDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
        }

        //***********************************************************************************************************************************************************************************************************

        private ICommand _clearLogCommand;
        /// <summary>
        /// Command to clear all log entries
        /// </summary>
        public ICommand ClearLogCommand
        {
            get
            {
                if (_clearLogCommand == null)
                {
                    _clearLogCommand = new RelayCommand(param => ClearLog());
                }
                return _clearLogCommand;
            }
        }

        private ICommand _saveLogCommand;
        /// <summary>
        /// Command to save all log entries
        /// </summary>
        public ICommand SaveLogCommand
        {
            get
            {
                if (_saveLogCommand == null)
                {
                    _saveLogCommand = new RelayCommand(param => SaveLog());
                }
                return _saveLogCommand;
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create a new error log entry with time and message
        /// </summary>
        /// <param name="errorMessage">error message</param>
        public void LogError(string errorMessage)
        {
            LogEventError logEvent = new LogEventError(errorMessage);
            LogEvent(logEvent);
        }

        /// <summary>
        /// Create a new warning log entry with time and message
        /// </summary>
        /// <param name="warningMessage">warning message</param>
        public void LogWarning(string warningMessage)
        {
            LogEventWarning logEvent = new LogEventWarning(warningMessage);
            LogEvent(logEvent);
        }

        /// <summary>
        /// Create a new info log entry with time and message
        /// </summary>
        /// <param name="infoMessage">info message</param>
        public void LogInfo(string infoMessage)
        {
            LogEventInfo logEvent = new LogEventInfo(infoMessage);
            LogEvent(logEvent);
        }

        /// <summary>
        /// Create a new image log entry with time and message
        /// </summary>
        /// <param name="imageMessage">image message</param>
        /// <param name="image">image of log entry</param>
        public void LogImage(string imageMessage, Bitmap image)
        {
            if (EnableImageLogs)
            {
                LogEventImage logEvent = new LogEventImage(imageMessage, image);
                LogEvent(logEvent);
            }
        }

        /// <summary>
        /// Create a new log entry with type, time and message
        /// </summary>
        /// <param name="logEvent">log event</param>
        public void LogEvent(LogEvent logEvent)
        {
            if (logEvent.LogType != LogTypes.IMAGE || (logEvent.LogType == LogTypes.IMAGE && EnableImageLogs))
            {
                LogEvents.Add(logEvent);
                if (AutoScrollToLastLogEntry) { ScrollToLastLogEvent(); }
            }
        }

        /// <summary>
        /// Clear all log entries
        /// </summary>
        public void ClearLog()
        {
            LogEvents.Clear();
        }

        /// <summary>
        /// Scroll to the given log event
        /// </summary>
        /// <param name="logEvent">LogEvent to scroll to</param>
        public void ScrollToSpecificLogEvent(LogEvent logEvent)
        {
            listView_Log.ScrollIntoView(logEvent);
        }

        /// <summary>
        /// Scroll to the last (newest) log event
        /// </summary>
        public void ScrollToLastLogEvent()
        {
            if (LogEvents.Count == 0) { return; }
            listView_Log.ScrollIntoView(LogEvents.Last());
        }

        /// <summary>
        /// Save all log entries
        /// </summary>
        public async void SaveLog()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Title = "Save log";
            saveFileDialog.DefaultExt = ".log";
            saveFileDialog.Filter = "Log file (*.log)|*.log|Text file (*.txt)|*.txt";
            saveFileDialog.InitialDirectory = _lastLogDirectory;
            
            if(saveFileDialog.ShowDialog().Value)
            {
                string logString = "Date/Time           |  Type   |  Message " + Environment.NewLine +
                                   "------------------- | ------- | ---------------------------------" + Environment.NewLine;

                foreach (LogEvent logEvent in listView_Log.Items)
                {
                    logString += logEvent.LogTime.ToString() + " | " + String.Format("{0,-7}", logEvent.LogType.ToString()) + " | " + logEvent.LogMessage + Environment.NewLine;
                }
                System.IO.File.WriteAllText(saveFileDialog.FileName, logString);
                _lastLogDirectory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);

                MetroWindow parentWindow = FindParent<MetroWindow>(this);
                await parentWindow.ShowMessageAsync("Log saved", "Log was successfully saved to" + Environment.NewLine + "\"" + saveFileDialog.FileName + "\"", MessageDialogStyle.Affirmative);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Method that is used to filter the listView items
        /// </summary>
        /// <param name="item">ListViewItem</param>
        /// <returns>Should the item be shown or not</returns>
        private bool filterListViewItems(object item)
        {
            LogEvent log = (LogEvent)item;
            bool logTypeVisible = (ShowInfos == true && log.LogType == LogTypes.INFO) || (ShowWarnings == true && log.LogType == LogTypes.WARNING) || (ShowErrors == true && log.LogType == LogTypes.ERROR) || (EnableImageLogs == true && ShowImageLogs == true && log.LogType == LogTypes.IMAGE);
            bool logMessageVisible = (!IsSearchEnabled || string.IsNullOrEmpty(SearchText)) ? true : (log.LogMessage.Contains(SearchText) || log.LogTime.ToString().Contains(SearchText));
            return logTypeVisible && logMessageVisible;
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Find the parent to the given child with a specific type
        /// </summary>
        /// <typeparam name="T">Type of the parent to find</typeparam>
        /// <param name="child">Child to search the parent for</param>
        /// <returns>parent with the specific type</returns>
        /// see: https://social.msdn.microsoft.com/Forums/vstudio/en-US/c47754bd-38c7-40b3-b64a-38a48884fde8/how-to-find-a-parent-of-a-specific-type?forum=wpf
        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObj = VisualTreeHelper.GetParent(child);
            T parent = parentObj as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObj);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Autosize the columns of the listView
        /// </summary>
        /// see: https://dotnet-snippets.de/snippet/automatische-anpassung-der-breite-von-gridviewcolumns/1286
        private void resizeListViewColumns(ListView listView)
        {
            GridView gridView = listView.View as GridView;
            if (gridView == null) { return; }

            foreach (GridViewColumn column in gridView.Columns)
            {
                if (column.Header.ToString() == "Message") { continue; }
                column.Width = column.ActualWidth;
                column.Width = double.NaN;
            }
        }
    }
}
