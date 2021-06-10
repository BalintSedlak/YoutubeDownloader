using System;
using System.Threading.Tasks;
using YoutubeDownloader.Models;
using YoutubeDownloader.Service;
using YoutubeDownloader.Wpf.Views.Dialogs;
using WpfFramework.Core;

namespace YoutubeDownloader.Wpf.Views.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly YoutubeDownloaderService _youtubeDownloaderService;
        private readonly ApplicationService _applicationService;
        private readonly FolderDialogViewModel _folderDialogViewModel;

        private string _consoleOutput;
        private string _url;
        private string _saveLocation;
        private bool _isDownloadCommandRunning;
        private bool _isUpdateYoutubeDlCommandRunning;
        private QualitySetting selectedQualitySetting;
        private FormatSetting selectedFormatSetting;

        public string ConsoleOutput
        {
            get => _consoleOutput;
            set => SetField(ref _consoleOutput, value, nameof(ConsoleOutput));
        }

        public string Url
        {
            get => _url;
            set => SetField(ref _url, value, nameof(Url));
        }

        public string SaveLocation
        {
            get => _saveLocation;
            set => SetField(ref _saveLocation, value, nameof(SaveLocation));
        }

        public QualitySetting SelectedQualitySetting
        {
            get => selectedQualitySetting;
            set => SetField(ref selectedQualitySetting, value, nameof(SelectedQualitySetting));
        }

        public FormatSetting SelectedFormatSetting
        {
            get => selectedFormatSetting;
            set => SetField(ref selectedFormatSetting, value, nameof(SelectedFormatSetting));
        }

        public RelayCommand DownloadCommand { get; set; }
        public RelayCommand AbortCommand { get; set; }
        public RelayCommand UpdateYoutubeDlCommand { get; set; }
        public RelayCommand ShowFolderDialogCommand { get; set; }

        public MainWindowViewModel(YoutubeDownloaderService youtubeDownloaderService, ApplicationService applicationService, FolderDialogViewModel folderDialogViewModel)
        {
            _youtubeDownloaderService = youtubeDownloaderService;
            _applicationService = applicationService;
            _folderDialogViewModel = folderDialogViewModel;

            _youtubeDownloaderService.ConsoleOutputReceived += ConsoleOutputReceived;

            DownloadCommand = new RelayCommand(ExecuteDownloadCommand, CanExecuteDownloadCommand);
            AbortCommand = new RelayCommand(ExecuteAbortCommand, CanExecuteAbortCommand);
            UpdateYoutubeDlCommand = new RelayCommand(ExecuteUpdateYoutubeDlCommand, CanExecuteUpdateYoutubeDlCommand);
            ShowFolderDialogCommand = new RelayCommand(ExecuteShowFolderDialogCommand, CanExecuteShowFolderDialogCommand);

            selectedQualitySetting = QualitySetting.Best;
            selectedFormatSetting = FormatSetting.VideoAndAudio;
        }

        private void ConsoleOutputReceived(object sender, string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return;
            }

            ConsoleOutput += $"{arg}{Environment.NewLine}";
        }

        private bool IsFieldsFilled()
        {
            if (string.IsNullOrEmpty(_saveLocation))
            {
                return false;
            }

            if (string.IsNullOrEmpty(_url))
            {
                return false;
            }

            return true;
        }

        public async void ExecuteDownloadCommand()
        {
            _isDownloadCommandRunning = true;
            Task downloadTask;

            downloadTask = _youtubeDownloaderService.DownloadPlaylistAsync(Url, SaveLocation, SelectedQualitySetting, SelectedFormatSetting);

            await downloadTask.ContinueWith(x =>
            {
                _isDownloadCommandRunning = false;
                DownloadCommand.RaiseCanExecuteChanged();
            });
        }

        public bool CanExecuteDownloadCommand()
        {
            if (!IsFieldsFilled())
            {
                return false;
            }

            if (_isDownloadCommandRunning || _isUpdateYoutubeDlCommandRunning)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ExecuteAbortCommand()
        {
            _youtubeDownloaderService.AbortDownload();
            ConsoleOutput += "Download process is manually aborted.";
            AbortCommand.RaiseCanExecuteChanged();
        }

        public bool CanExecuteAbortCommand()
        {
            if (!_isDownloadCommandRunning)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async void ExecuteUpdateYoutubeDlCommand()
        {
            _isUpdateYoutubeDlCommandRunning = true;
            Task updateTask;

            updateTask = _applicationService.UpdateYoutubeDl();

            await updateTask.ContinueWith(x =>
            {
                _isUpdateYoutubeDlCommandRunning = false;
                UpdateYoutubeDlCommand.RaiseCanExecuteChanged();
            });
        }

        public bool CanExecuteUpdateYoutubeDlCommand()
        {
            if (_isDownloadCommandRunning || _isUpdateYoutubeDlCommandRunning)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ExecuteShowFolderDialogCommand()
        {
            string saveLocation = _folderDialogViewModel.ShowDialog();

            if (string.IsNullOrEmpty(saveLocation))
            {
                return;
            }

            SaveLocation = saveLocation;
        }

        public bool CanExecuteShowFolderDialogCommand()
        {
            if (_isDownloadCommandRunning)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
