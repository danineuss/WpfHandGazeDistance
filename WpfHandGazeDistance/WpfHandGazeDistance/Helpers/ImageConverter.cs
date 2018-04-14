using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WpfHandGazeDistance.Helpers
{
    /// <summary>
    /// Pulled straight from: https://stackoverflow.com/questions/20586/image-urisource-and-data-binding
    /// </summary>
    public class ImageConverter : IValueConverter
    {
        public object Convert(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? new BitmapImage(new Uri(value.ToString())) : null;
        }

        public object ConvertBack(
            object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}