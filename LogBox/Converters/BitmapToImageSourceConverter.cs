using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;

namespace LogBox
{
    /// <summary>
    /// Convert bitmap to image source
    /// </summary>
    [ValueConversion(typeof(Bitmap), typeof(BitmapImage))]
    public class BitmapToImageSourceConverter : IValueConverter
    {
        // see: https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) { return null; }
            Bitmap bitmap = (Bitmap)((Bitmap)value).Clone();
            bitmap.MakeTransparent(Color.White);
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
