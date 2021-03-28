using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using YoutubeDownloader.Models;

namespace YoutubeDownloader.Wpf.Views.UserControls
{
    /// <summary>
    /// Interaction logic for DownloadOptionsControl.xaml
    /// </summary>
    public partial class DownloadOptionsControl : UserControl
    {
        public DownloadOptionsControl()
        {
            InitializeComponent();
        }

        public IEnumerable<string> QualityComboboxItemsSource { get => QualitySetting.Best.GetAllDescription<QualitySetting>(); }

        public QualitySetting SelectedQualitySetting
        {
            get { return (QualitySetting)GetValue(SelectedQualitySettingsProperty); }
            set { SetValue(SelectedQualitySettingsProperty, value); }
        }

        public static readonly DependencyProperty SelectedQualitySettingsProperty =
            DependencyProperty.Register(nameof(SelectedQualitySetting), typeof(QualitySetting), typeof(DownloadOptionsControl), new FrameworkPropertyMetadata(QualitySetting.Best, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public IEnumerable<string> FormatComboboxItemsSource { get => FormatSetting.VideoAndAudio.GetAllDescription<FormatSetting>(); }

        public FormatSetting SelectedFormatSetting
        {
            get { return (FormatSetting)GetValue(SelectedFormatSettingProperty); }
            set { SetValue(SelectedFormatSettingProperty, value); }
        }

        public static readonly DependencyProperty SelectedFormatSettingProperty =
            DependencyProperty.Register(nameof(SelectedFormatSetting), typeof(FormatSetting), typeof(DownloadOptionsControl), new FrameworkPropertyMetadata(FormatSetting.VideoAndAudio, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
