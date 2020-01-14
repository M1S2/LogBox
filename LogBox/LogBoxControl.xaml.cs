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
using System.Text.RegularExpressions;
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
                HighlightAllListViewItems(false);
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
                HighlightAllListViewItems(false);
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
                HighlightAllListViewItems(false);
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
                HighlightAllListViewItems(false);
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
                HighlightAllListViewItems(false);
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
                HighlightAllListViewItems(false);
            }
        }

        //***********************************************************************************************************************************************************************************************************

        private int _numSaveOperations = 0;
        /// <summary>
        /// Number of currently running save operations
        /// </summary>
        public int NumSaveOperations
        {
            get { return _numSaveOperations; }
            set
            {
                _numSaveOperations = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsSaving");
            }
        }
        
        /// <summary>
        /// Is the log currently saving or not
        /// </summary>
        public bool IsSaving
        {
            get { return NumSaveOperations > 0; }
        }

        //***********************************************************************************************************************************************************************************************************

        private string _lastLogPath;

        //***********************************************************************************************************************************************************************************************************

        public LogBoxControl()
        {
            InitializeComponent();
            DataContext = this;
            LogEvents = new ObservableCollection<LogEvent>();

            listView_Log.ItemsSource = LogEvents;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(LogEvents);
            view.Filter = filterListViewItems;

            // see: https://social.msdn.microsoft.com/Forums/vstudio/en-US/62482ae9-ddd0-4c89-b872-a80c7c367385/setting-scroll-scrollchanged-and-valuechanged-listview-event-handlers-programatically?forum=wpf
            listView_Log.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(ListView_ScrollChanged));
            
            ShowInfos = true;
            ShowWarnings = true;
            ShowErrors = true;
            ShowImageLogs = false;
            EnableImageLogs = false;
            AutoScrollToLastLogEntry = true;

            _lastLogPath = System.AppDomain.CurrentDomain.BaseDirectory + "logFile.log";
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
                if (IsSearchEnabled) { HighlightAllListViewItems(true); }
            }
        }

        /// <summary>
        /// Clear all log entries
        /// </summary>
        public async void ClearLog()
        {
            MetroWindow parentWindow = FindParent<MetroWindow>(this);
            MessageDialogResult dialogResult = await parentWindow.ShowMessageAsync("Clear log", "Do you really want to clear all log entries.", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "Clear", NegativeButtonText = "Cancel" });

            if (dialogResult == MessageDialogResult.Affirmative)
            {
                LogEvents.Clear();
            }
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
            //see: https://stackoverflow.com/questions/2337822/wpf-listbox-scroll-to-end-automatically
            ScrollViewer scrollViewer = FindVisualChildren<ScrollViewer>(listView_Log).FirstOrDefault();
            scrollViewer?.ScrollToBottom();
        }

        /// <summary>
        /// Save all log entries
        /// </summary>
        public async void SaveLog()
        {
            NumSaveOperations++;

            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Title = "Save log";
            saveFileDialog.DefaultExt = ".log";
            saveFileDialog.Filter = "Log file (*.log)|*.log|Text file (*.txt)|*.txt";
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(_lastLogPath);
            saveFileDialog.FileName = System.IO.Path.GetFileName(_lastLogPath);

            if (saveFileDialog.ShowDialog().Value)
            {
                StringBuilder logString = new StringBuilder("Date/Time           |  Type   |  Message " + Environment.NewLine +
                                                            "------------------- | ------- | ---------------------------------" + Environment.NewLine);

                LogEvent[] logEvents = new LogEvent[listView_Log.Items.Count];
                listView_Log.Items.CopyTo(logEvents, 0);

                await Task.Run(() =>
               {
                   foreach (LogEvent logEvent in logEvents)
                   {
                       logString.Append(logEvent.LogTime.ToString() + " | " + String.Format("{0,-7}", logEvent.LogType.ToString()) + " | " + logEvent.LogMessage + Environment.NewLine);
                   }
                   System.IO.File.WriteAllText(saveFileDialog.FileName, logString.ToString());
               });
                _lastLogPath = saveFileDialog.FileName;

                MetroWindow parentWindow = FindParent<MetroWindow>(this);
                await parentWindow.ShowMessageAsync("Log saved", "Log was successfully saved to" + Environment.NewLine + "\"" + saveFileDialog.FileName + "\"", MessageDialogStyle.Affirmative);
            }

            NumSaveOperations--;
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
            bool logMessageVisible = (!IsSearchEnabled || string.IsNullOrEmpty(SearchText)) ? true : (log.LogMessage.ToLower().Contains(SearchText.ToLower()) || log.LogTime.ToString().ToLower().Contains(SearchText.ToLower()));
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

        /// <summary>
        /// Find all children with a specific type
        /// </summary>
        /// <typeparam name="T">Type of the children to find</typeparam>
        /// <param name="depObj">Parent to search the children for</param>
        /// <returns>List of children with specific type</returns>
        /// see: https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        //***********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Highlight all items that are scrolled into view (because of virtualization)
        /// </summary>
        void ListView_ScrollChanged(object sender, RoutedEventArgs e)
        {
            HighlightAllListViewItems(true);
        }

        /// <summary>
        /// Find all ListViewItems and call the HighlightText method for each
        /// </summary>
        /// <param name="isScrolling">Was the highlight function called while scrolling or not</param>
        private void HighlightAllListViewItems(bool isScrolling)
        {
            listView_Log.UpdateLayout();
            foreach (ListViewItem lvi in FindVisualChildren<ListViewItem>(listView_Log))
            {
                if (!isScrolling || (isScrolling && (lvi.Tag == null || (bool)lvi.Tag == false)))       // if scrolling, only highlight new items (created by virtualization, Tag == null)
                {
                    HighlightText(lvi, IsSearchEnabled ? SearchText : "");
                    lvi.Tag = true;
                }
            }
        }

        /// <summary>
        /// Find all TextBlocks and highlight the given text
        /// </summary>
        /// <param name="itx">Object that is used to highlight</param>
        /// <param name="highlightText">Text to highlight</param>
        /// see: https://social.msdn.microsoft.com/Forums/sqlserver/en-US/3f157426-6673-4548-89b9-e4681f7c96ea/how-to-highlight-part-of-a-text-in-a-listview-item?forum=wpf
        /// see: https://www.codeproject.com/Articles/103259/Highlight-Searched-Text-in-WPF-ListView
        private void HighlightText(DependencyObject itx, string highlightText)
        {
            if (itx != null)
            {
                if (itx is TextBlock)
                {
                    Regex regex = new Regex("(" + highlightText + ")", RegexOptions.IgnoreCase);
                    TextBlock tb = itx as TextBlock;
                    if (string.IsNullOrEmpty(highlightText))
                    {
                        string str = tb.Text;
                        tb.Inlines.Clear();
                        tb.Inlines.Add(str);
                        return;
                    }
                    string[] substrings = regex.Split(tb.Text);
                    tb.Inlines.Clear();
                    foreach (var item in substrings)
                    {
                        if (regex.Match(item).Success)
                        {
                            Run runx = new Run(item);
                            runx.FontWeight = FontWeights.Bold;
                            runx.FontSize += 2;
                            tb.Inlines.Add(runx);
                        }
                        else
                        {
                            tb.Inlines.Add(item);
                        }
                    }
                    return;
                }
                else
                {
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(itx as DependencyObject); i++)
                    {
                        HighlightText(VisualTreeHelper.GetChild(itx as DependencyObject, i), highlightText);
                    }
                }
            }
        }
    }
}
