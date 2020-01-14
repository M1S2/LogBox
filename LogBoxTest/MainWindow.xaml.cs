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
using MahApps.Metro.Controls;

namespace LogBoxTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            logBoxCtrl.LogEvent(new LogBox.LogEvents.LogEventInfo("Info log message"));
            logBoxCtrl.LogEvent(new LogBox.LogEvents.LogEventWarning("Warning log message"));
            logBoxCtrl.LogEvent(new LogBox.LogEvents.LogEventError("Error log message"));

            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(100, 100);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.Clear(System.Drawing.Color.Green);
            logBoxCtrl.LogEvent(new LogBox.LogEvents.LogEventImage("Image log message", bmp));

            for (int i = 0; i < 9996; i++)
            {
                logBoxCtrl.LogEvent(new LogBox.LogEvents.LogEventInfo("Info log #" + i.ToString()));
            }
        }
    }
}
