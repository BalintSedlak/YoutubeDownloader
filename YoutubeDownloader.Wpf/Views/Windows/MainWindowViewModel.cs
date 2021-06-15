using System;
using System.Threading.Tasks;
using YoutubeDownloader.Models;
using YoutubeDownloader.Service;
using YoutubeDownloader.Wpf.Views.Dialogs;
using WpfFramework.Core;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace YoutubeDownloader.Wpf.Views.Windows
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly YoutubeDownloaderService _youtubeDownloaderService;
        private readonly ApplicationService _applicationService;
        private readonly FileConverterService _fileConverterService;
        private readonly FolderDialogViewModel _folderDialogViewModel;

        private string _consoleOutput;
        private string _url;
        private string _saveLocation;
        private bool _isDownloadCommandRunning;
        private bool _isConvertCommandRunning;
        private bool _isUpdateYoutubeDlCommandRunning;
        private QualitySetting _selectedQualitySetting;
        private FormatSetting _selectedFormatSetting;
        private IEnumerable<string> _outputFormatItemsSource;
        private string _selectedOutputFormat;
        CancellationTokenSource _cancellationToken;

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
            set { SetField(ref _saveLocation, value, nameof(SaveLocation)); OnPropertyChanged(TempSaveLocation); }
        }

        public string TempSaveLocation => $"{SaveLocation}{Path.DirectorySeparatorChar}Temp";

        public QualitySetting SelectedQualitySetting
        {
            get => _selectedQualitySetting;
            set => SetField(ref _selectedQualitySetting, value, nameof(SelectedQualitySetting));
        }

        public FormatSetting SelectedFormatSetting
        {
            get => _selectedFormatSetting;
            set { SetField(ref _selectedFormatSetting, value, nameof(SelectedFormatSetting)); SelectedFormatChanged(); }
        }

        public IEnumerable<string> OutputFormatItemsSource
        {
            get => _outputFormatItemsSource;
            set => SetField(ref _outputFormatItemsSource, value, nameof(OutputFormatItemsSource));
        }

        public string SelectedOutputFormat
        {
            get => _selectedOutputFormat;
            set => SetField(ref _selectedOutputFormat, value, nameof(SelectedOutputFormat));
        }

        public RelayCommand DownloadCommand { get; set; }
        public RelayCommand AbortCommand { get; set; }
        public RelayCommand UpdateYoutubeDlCommand { get; set; }
        public RelayCommand ShowFolderDialogCommand { get; set; }

        public MainWindowViewModel(YoutubeDownloaderService youtubeDownloaderService, ApplicationService applicationService, FileConverterService fileConverterService, FolderDialogViewModel folderDialogViewModel)
        {
            _youtubeDownloaderService = youtubeDownloaderService;
            _applicationService = applicationService;
            _fileConverterService = fileConverterService;
            _folderDialogViewModel = folderDialogViewModel;

            _youtubeDownloaderService.ConsoleOutputReceived += ConsoleOutputReceived;
            _fileConverterService.ConsoleOutputReceived += ConsoleOutputReceived;

            _cancellationToken = new CancellationTokenSource();

            DownloadCommand = new RelayCommand(ExecuteDownloadCommand, CanExecuteDownloadCommand);
            AbortCommand = new RelayCommand(ExecuteAbortCommand, CanExecuteAbortCommand);
            UpdateYoutubeDlCommand = new RelayCommand(ExecuteUpdateYoutubeDlCommand, CanExecuteUpdateYoutubeDlCommand);
            ShowFolderDialogCommand = new RelayCommand(ExecuteShowFolderDialogCommand, CanExecuteShowFolderDialogCommand);

            _selectedQualitySetting = QualitySetting.Best;
            _selectedFormatSetting = FormatSetting.VideoAndAudio;
            SelectedFormatChanged();
        }

        public void SelectedFormatChanged()
        {
            switch (SelectedFormatSetting)
            {
                case FormatSetting.VideoAndAudio:
                    OutputFormatItemsSource = AvailableOutputFormats.Video;
                    SelectedOutputFormat = AvailableOutputFormats.Video[0];
                    break;
                case FormatSetting.Audio:
                    OutputFormatItemsSource = AvailableOutputFormats.Audio;
                    SelectedOutputFormat = AvailableOutputFormats.Audio[0];
                    break;
                case FormatSetting.Video:
                    OutputFormatItemsSource = AvailableOutputFormats.Video;
                    SelectedOutputFormat = AvailableOutputFormats.Video[0];
                    break;
                default:
                    break;
            }

            OnPropertyChanged(nameof(SelectedOutputFormat));
            OnPropertyChanged(nameof(OutputFormatItemsSource));
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

        public void ExecuteDownloadCommand()
        {
            _cancellationToken = new CancellationTokenSource();
            _isDownloadCommandRunning = true;

            _youtubeDownloaderService.DownloadPlaylistAsync(Url, TempSaveLocation, SelectedQualitySetting, SelectedFormatSetting)
            .ContinueWith(result =>
            {
                if (!result.IsCompletedSuccessfully)
                {
                    _isDownloadCommandRunning = false;
                    return;
                }

                _isConvertCommandRunning = true;
                _isDownloadCommandRunning = false;

                ConsoleOutputReceived(this, "Converting file(s)...");
                _fileConverterService.ConvertFiles(TempSaveLocation, SaveLocation, SelectedOutputFormat, _cancellationToken)
                    .ContinueWith(result =>
                    {
                        if (result.IsCompletedSuccessfully)
                        {
                            ConsoleOutputReceived(this, "File(s) are converted.");
                        }
                        else if (result.IsCanceled)
                        {
                            ConsoleOutputReceived(this, "Convert process is manually aborted.");
                        }

                        _isConvertCommandRunning = false;
                        DownloadCommand.RaiseCanExecuteChanged();
                    });
            });
        }

        public bool CanExecuteDownloadCommand()
        {
            if (!IsFieldsFilled())
            {
                return false;
            }

            if (_isDownloadCommandRunning || _isConvertCommandRunning || _isUpdateYoutubeDlCommandRunning)
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
            if (_isDownloadCommandRunning)
            {
                _youtubeDownloaderService.AbortDownload();
                ConsoleOutputReceived(this, "Download process is manually aborted.");
            }
            if (_isConvertCommandRunning)
            {
                _cancellationToken.Cancel();
            }

            AbortCommand.RaiseCanExecuteChanged();
        }

        public bool CanExecuteAbortCommand()
        {
            if (_isDownloadCommandRunning || _isConvertCommandRunning)
            {
                return true;
            }
            else
            {
                return false;
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
            if (_isDownloadCommandRunning || _isConvertCommandRunning || _isUpdateYoutubeDlCommandRunning)
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
            if (_isDownloadCommandRunning || _isConvertCommandRunning || _isUpdateYoutubeDlCommandRunning)
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
