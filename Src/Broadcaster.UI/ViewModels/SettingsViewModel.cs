using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using Broadcaster.Core;
using Broadcaster.Core.Configuration;
using Broadcaster.Interfaces.Enums;
using Broadcaster.Interfaces.Helpers;
using JetBrains.Annotations;
using Application = System.Windows.Application;

namespace Broadcaster.UI.ViewModels
{
    //TODO validate config befor save && use automapper for maping vm-> config
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IContainerViewsObserver _containerViewsObserver;
        private readonly IDeviceManager _deviceManager;
        private readonly IWritableConfig _writableConfig;
        private ICommand _cancelCommand;
        private ICommand _saveCommand;
        private ICommand _selectScreenshotsStorageCommand;
        private ICommand _selectVideoStorageCommand;

        public SettingsViewModel(IWritableConfig writableConfig, IDeviceManager deviceManager,
            IContainerViewsObserver containerViewsObserver)
        {
            _writableConfig = writableConfig;
            _containerViewsObserver = containerViewsObserver;
            SelectedResolution = _writableConfig.Config.Resolution.GetDescription();
            PathToScreenshots = _writableConfig.Config.PathToScreenshots;
            PathToVideo = _writableConfig.Config.PathToVideo;
            SelectedAudioDevice = _writableConfig.Config.AudioDeviceName;
            SelectedVideoDevice = _writableConfig.Config.VideoDeviceName;
            MicrophoneVolume = _writableConfig.Config.MicrophoneVolume;
            _deviceManager = deviceManager;
        }

        [UsedImplicitly]
        public ObservableCollection<string> VideoDevices
            => new ObservableCollection<string>(_deviceManager.GetVideoDevicesNames());

        [UsedImplicitly]
        public ObservableCollection<string> AudioDevices
            => new ObservableCollection<string>(_deviceManager.GetAudioDevicesNames());

        [UsedImplicitly]
        public ObservableCollection<string> Resolution
            => new ObservableCollection<string>(Enum.GetValues(typeof(Resolution)).Cast<Resolution>().Select(resolution => resolution.GetDescription()));

        public bool ShowError { get; set; }
        public string SelectedVideoDevice { get; set; }
        public string SelectedAudioDevice { get; set; }
        public string PathToVideo { get; set; }
        public string PathToScreenshots { get; set; }
        public double MicrophoneVolume { get; set; }
        public string SelectedResolution { get; set; }

        public string ErrorMessage { get; private set; }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(() =>
        {
            if (ValidateAndSave())
                _containerViewsObserver.SetActiveViewModel(ViewModelType.NavigationPanel);
        }));

        [UsedImplicitly]
        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(() => _containerViewsObserver.SetActiveViewModel(ViewModelType.NavigationPanel)));

        [UsedImplicitly]
        public ICommand SelectVideoStorageCommand
            => _selectVideoStorageCommand ?? (_selectVideoStorageCommand = new RelayCommand(() =>
            {
                var path = CallFolderBrowserDialog();
                if (string.IsNullOrEmpty(path)) return;
                PathToVideo = path;
                OnPropertyChanged(nameof(PathToVideo));
            }));

        [UsedImplicitly]
        public ICommand SelectScreenshotsStorageCommand
            => _selectScreenshotsStorageCommand ?? (_selectScreenshotsStorageCommand = new RelayCommand(() =>
            {
                var path = CallFolderBrowserDialog();
                if (string.IsNullOrEmpty(path)) return;
                PathToScreenshots = path;
                OnPropertyChanged(nameof(PathToScreenshots));
            }));


        public bool ConfigCheck()
        {
            var sb = new StringBuilder();
            _writableConfig.Config.GetType()
                .GetProperties()
                .Where(info => info.PropertyType == typeof(string))
                .ToList()
                .ForEach(info =>
                {
                    var val = (string)info.GetValue(_writableConfig.Config);
                    if (string.IsNullOrEmpty(val) || string.IsNullOrWhiteSpace(val))
                        if (info.Name != "PathToVideo")
                            sb.AppendLine($"Необходимо заполнить поле {info.Name}");
                });
            ErrorMessage = sb.ToString();
            return sb.Length == 0;
        }

        private string CallFolderBrowserDialog()
        {
            var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog(Application.Current.MainWindow.GetIWin32Window());
            return result == DialogResult.OK ? fbd.SelectedPath : string.Empty;
        }


        private bool ValidateAndSave()
        {
            _writableConfig.Config.Resolution = EnumExtensions.GetEnumByDescription<Resolution>(SelectedResolution);
            _writableConfig.Config.PathToScreenshots = PathToScreenshots;
            _writableConfig.Config.PathToVideo = PathToVideo;
            _writableConfig.Config.AudioDeviceName = SelectedAudioDevice;
            _writableConfig.Config.VideoDeviceName = SelectedVideoDevice;
            _writableConfig.Config.MicrophoneVolume = MicrophoneVolume;
            if (!ConfigCheck())
            {
                ShowError = true;
                OnPropertyChanged(nameof(ShowError));
                OnPropertyChanged(nameof(ErrorMessage));
                Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(l =>
                {
                    ShowError = false;
                    OnPropertyChanged(nameof(ShowError));
                });
                return false;
            }
            _writableConfig.Config.IsSettingsFill = true;
            _writableConfig.Write();
            return true;
        }
    }
}