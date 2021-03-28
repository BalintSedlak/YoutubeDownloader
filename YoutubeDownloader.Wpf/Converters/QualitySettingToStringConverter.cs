using System;
using System.Globalization;
using System.Windows.Data;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Wpf.Converters
{
    [ValueConversion(typeof(FormatSetting), typeof(object))]
    public class FormatSettingToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((FormatSetting)value).GetDescription<FormatSetting>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return FormatSetting.VideoAndAudio.GetValueFromDescription<FormatSetting>((string)value); 
        }
    }
}
