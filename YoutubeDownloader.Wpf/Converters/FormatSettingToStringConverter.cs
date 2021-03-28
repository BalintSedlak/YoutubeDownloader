using System;
using System.Globalization;
using System.Windows.Data;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Wpf.Converters
{
    [ValueConversion(typeof(QualitySetting), typeof(object))]
    public class QualitySettingToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((QualitySetting)value).GetDescription<QualitySetting>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return QualitySetting.Best.GetValueFromDescription<QualitySetting>((string)value); 
        }
    }
}
