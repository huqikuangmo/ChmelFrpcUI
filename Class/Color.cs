using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FrpcUI.Class
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == null)
            {
                return Brushes.Gray; // 默认颜色
            }

            switch (status)
            {
                case "online":
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                case null:
                    return Brushes.Red;
                case "Pending":
                    return Brushes.Yellow;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要反向转换，这里可以实现
            throw new NotImplementedException();
        }
    }

    public class StatusToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == null)
            {
                return Brushes.Black; // 默认颜色
            }

            switch (status)
            {
                case "Active":
                    return Brushes.White;
                case "Inactive":
                    return Brushes.White;
                case "Pending":
                    return Brushes.Black;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要反向转换，这里可以实现
            throw new NotImplementedException();
        }
    }

    public class WebToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string webState)
            {
                switch (webState)
                {
                    case "yes":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                    case "no":
                        return Brushes.White;
                    case "vip":
                        return Brushes.Gold;
                    case "user":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2080f0"));
                    case null:
                        return Brushes.Gray;
                    default:
                        return webState;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要反向转换，这里可以实现
            throw new NotImplementedException();
        }
    }

    public class WebToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string webState)
            {
                switch (webState)
                {
                    case "yes":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#98FB98"));
                    case "no":
                        return Brushes.Gray;
                    case "vip":
                        return Brushes.LemonChiffon;
                    case "user":
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cfe2f3"));
                    case null:
                        return Brushes.Gray;
                    default:
                        return webState;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 如果需要反向转换，这里可以实现
            throw new NotImplementedException();
        }
    }
}
