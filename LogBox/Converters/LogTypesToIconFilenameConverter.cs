using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LogBox
{
    /// <summary>
    /// Convert LogTypes enum values to icon filenames that can be used to display icons in an image control
    /// </summary>
    [ValueConversion(typeof(LogTypes), typeof(string))]
    public class LogTypesToIconFilenameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((LogTypes)value)
            {
                case LogTypes.INFO:
                    return "/LogBox;component/Resources/Info.png";
                case LogTypes.WARNING:
                    return "/LogBox;component/Resources/Warning.png";
                case LogTypes.ERROR:
                    return "/LogBox;component/Resources/Error.png";
                case LogTypes.IMAGE:
                    return "/LogBox;component/Resources/Image.png";
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
