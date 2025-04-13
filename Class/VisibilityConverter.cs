using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FrpcUI.Class
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return bool.TryParse(stringValue, out bool result) ? (result ? Visibility.Visible : Visibility.Collapsed) : Visibility.Collapsed;
            }
            else if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
