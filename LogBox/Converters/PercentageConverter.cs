using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;

namespace LogBox
{
    /// <summary>
    /// Multiply a value with a factor. This can be used for example as converter in xaml to use only half the width (factor 0.5) of a control.
    /// </summary>
    /// see: https://stackoverflow.com/questions/20326744/wpf-binding-width-to-parent-width0-3
    [ValueConversion(typeof(double), typeof(double))]
    public class PercentageConverter : MarkupExtension, IValueConverter
    {
        private static PercentageConverter _instance;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new PercentageConverter());
        }
    }
}
