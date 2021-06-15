using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Wpf.Views.UserControls
{
    /// <summary>
    /// Interaction logic for OutputFormatControl.xaml
    /// </summary>
    public partial class OutputFormatControl : UserControl
    {
        public OutputFormatControl()
        {
            InitializeComponent();
        }

        public IEnumerable<string> OutputFormatItemsSource
        {
            get { return (IEnumerable<string>)GetValue(OutputFormatItemsSourceProperty); }
            set { SetValue(OutputFormatItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OutputFormatItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutputFormatItemsSourceProperty =
            DependencyProperty.Register(nameof(OutputFormatItemsSource), typeof(IEnumerable<string>), typeof(OutputFormatControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FormatSetting SelectedFormatSetting
        {
            get { return (FormatSetting)GetValue(SelectedFormatSettingProperty); }
            set { SetValue(SelectedFormatSettingProperty, value); }
        }

        public static readonly DependencyProperty SelectedFormatSettingProperty =
            DependencyProperty.Register(nameof(SelectedFormatSetting), typeof(FormatSetting), typeof(OutputFormatControl), new FrameworkPropertyMetadata(FormatSetting.VideoAndAudio, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SelectedOutputFormat
        {
            get { return (string)GetValue(SelectedOutputFormatProperty); }
            set { SetValue(SelectedOutputFormatProperty, value); }
        }

        public static readonly DependencyProperty SelectedOutputFormatProperty =
            DependencyProperty.Register(nameof(SelectedOutputFormat), typeof(string), typeof(OutputFormatControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
