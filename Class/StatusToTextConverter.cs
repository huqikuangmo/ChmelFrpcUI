using System;
using System.Globalization;
using System.Windows.Data;

namespace FrpcUI.Class
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string nodeState)
            {
                switch (nodeState)
                {
                    case "online":
                        return "在线";
                    case "offline":
                        return "离线";
                    case null:
                        return "离线";

                    default:
                        return nodeState;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WebToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string webState)
            {
                switch (webState)
                {
                    case "yes":
                        return "可建站";
                    case "no":
                        return "不可建站";
                    case null:
                        return "未知";
                    case "vip":
                        return "会员";
                    case "user":
                        return "普通";
                    default:
                        return webState;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

