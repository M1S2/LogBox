using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.IconPacks;

namespace LogBox
{
    /// <summary>
    /// Convert LogTypes enum values to GeometryDrawing representation of the LogType icon
    /// </summary>
    [ValueConversion(typeof(LogTypes), typeof(GeometryDrawing))]
    public class LogTypesToIconGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string iconDataStr = null;
            Brush foregroundBrush = null;
            Brush strokeBrush = null;
            Brush backgroundRectBrush = null;

            Geometry backgroundRectGeometry = null;

            switch ((LogTypes)value)
            {
                case LogTypes.INFO:
                    iconDataStr = (new PackIconModern() { Kind = PackIconModernKind.InformationCircle }).Data;
                    foregroundBrush = new SolidColorBrush(Colors.Blue);
                    strokeBrush = new SolidColorBrush(Colors.LightBlue);
                    backgroundRectBrush = new SolidColorBrush(Colors.White);
                    backgroundRectGeometry = new RectangleGeometry(new System.Windows.Rect(33, 20, 10, 34));
                    break;
                case LogTypes.WARNING:
                    iconDataStr = (new PackIconModern() { Kind = PackIconModernKind.Warning }).Data;
                    foregroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE4E44C"));
                    strokeBrush = new SolidColorBrush(Colors.Orange);
                    backgroundRectBrush = new SolidColorBrush(Colors.Black);
                    backgroundRectGeometry = new RectangleGeometry(new System.Windows.Rect(33, 20, 10, 34));
                    break;
                case LogTypes.ERROR:
                    iconDataStr = (new PackIconEntypo() { Kind = PackIconEntypoKind.CircleWithCross }).Data;
                    foregroundBrush = new SolidColorBrush(Colors.Red);
                    strokeBrush = new SolidColorBrush(Colors.OrangeRed);
                    backgroundRectBrush = new SolidColorBrush(Colors.White);
                    backgroundRectGeometry = new RectangleGeometry(new System.Windows.Rect(5, 5, 10, 10));
                    break;
                case LogTypes.IMAGE:
                    iconDataStr = (new PackIconModern() { Kind = PackIconModernKind.Image }).Data;
                    foregroundBrush = new SolidColorBrush(Colors.Gray);
                    strokeBrush = new SolidColorBrush(Colors.LightGray);
                    backgroundRectBrush = new SolidColorBrush(Colors.White);
                    backgroundRectGeometry = new RectangleGeometry(new System.Windows.Rect(18, 21, 40, 34));
                    break;
                default:
                    return null;
            }

            Geometry iconGeometry = Geometry.Parse(iconDataStr);
            GeometryDrawing iconGeometryDrawing = new GeometryDrawing(foregroundBrush, new Pen(strokeBrush, 1), iconGeometry);
            GeometryDrawing rectGeometryDrawing = new GeometryDrawing(backgroundRectBrush, new Pen(backgroundRectBrush, 1), backgroundRectGeometry);
            DrawingGroup iconCombined = new DrawingGroup();
            iconCombined.Children.Add(rectGeometryDrawing);
            iconCombined.Children.Add(iconGeometryDrawing);
            return iconCombined;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
