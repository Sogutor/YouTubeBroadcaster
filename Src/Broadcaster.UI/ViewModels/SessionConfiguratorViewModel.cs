using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Broadcaster.Core;
using Broadcaster.Core.Audio;
using Broadcaster.Core.Configuration;
using Broadcaster.Core.Ffmpeg;
using Broadcaster.Interfaces;
using Broadcaster.Interfaces.Helpers;
using JetBrains.Annotations;

namespace Broadcaster.UI.ViewModels
{
    public class SessionConfiguratorViewModel : ViewModelBase
    {
        private readonly IConfig _config;
        private readonly IDeviceManager _deviceManager;
        private readonly BehaviorSubject<FfmpegParams> _ffmpegParamsObserver;
        private double _microphoneVolume;
        private ICommand _selectedIAudioDeviceChangeCommand;
        private ICommand _someChangeCommand;

        private IVolumeController _volumeController;

        public SessionConfiguratorViewModel(IDeviceManager deviceManager, IConfig config)
        {
            _deviceManager = deviceManager;
            _config = config;
            _volumeController = _deviceManager.GetVolumeControl(config.AudioDeviceName);
            _volumeController.Percent = config.MicrophoneVolume;
            InitProperty();
            _ffmpegParamsObserver = new BehaviorSubject<FfmpegParams>(CreateFfmpegParams());
        }

        public ObservableCollection<string> ShowTypes => new ObservableCollection<string>(Enum.GetValues(typeof(ShowType))
            .Cast<ShowType>().Select(showType => showType.GetDescription()));
        [UsedImplicitly]
        public ObservableCollection<string> VideoDevices => new ObservableCollection<string>(_deviceManager.GetVideoDevicesNames());
        [UsedImplicitly]
        public ObservableCollection<string> AudioDevices => new ObservableCollection<string>(_deviceManager.GetAudioDevicesNames());
        [UsedImplicitly]
        public ObservableCollection<string> WindowsNames => new ObservableCollection<string>(Process.GetProcesses().Where(process => !string.IsNullOrEmpty(process.MainWindowTitle)).Select(process => process.MainWindowTitle));

        public float CurrentInputLevel { get; private set; }
        public string SelectedShowTypeMain { get; set; }
        public string SelectedShowTypeSecondary { get; set; }
        public string SelectedVideoDevice { get; set; }
        public string SelectedAudioDevice { get; set; }
        public string SelectWindow { get; set; }
        public bool EnableWindowSelect { get; private set; }


        public double MicrophoneVolume
        {
            get { return _microphoneVolume; }
            set
            {
                _microphoneVolume = value;
                _volumeController.Percent = value;
            }
        }

        public IObservable<FfmpegParams> FfmpegParamsObservable => _ffmpegParamsObserver.AsObservable();
        [UsedImplicitly]
        public ICommand SelectedIAudioDeviceChangeCommand
            => _selectedIAudioDeviceChangeCommand ?? (_selectedIAudioDeviceChangeCommand = new RelayCommand(async () =>
               {
                   await _deviceManager.AudioHelper.StopAsync().ConfigureAwait(true);
                   _deviceManager.AudioHelper.BeginMonitoring(SelectedAudioDevice);
                   _volumeController = _deviceManager.AudioHelper.GetCurrentVolumeController;
                   _microphoneVolume = _volumeController.Percent;
                   OnPropertyChanged(nameof(MicrophoneVolume));
               }));
        [UsedImplicitly]
        public ICommand SomeChangeCommand => _someChangeCommand ?? (_someChangeCommand = new RelayCommand(() =>
        {
            EnableWindowSelect = SelectedShowTypeMain == ShowType.Document.GetDescription();
            OnPropertyChanged(nameof(EnableWindowSelect));
            _ffmpegParamsObserver.OnNext(CreateFfmpegParams());
        }));

        public void InitMicrophoneListener()
        {
            if (_deviceManager.AudioHelper.RecordingState != RecordingState.Stopped) return;
            _deviceManager.AudioHelper.BeginMonitoring(SelectedAudioDevice);
            _deviceManager.AudioHelper.SampleAggregator.MaximumCalculated += SampleAggregatorOnMaximumCalculated;
        }

        private void InitProperty()
        {
            SelectedShowTypeMain = ShowTypes.First();
            SelectedShowTypeSecondary = ShowTypes.Last();
            SetPropertyFromConfig();
            InitMicrophoneListener();
        }

        public void SetPropertyFromConfig()
        {
            SelectedVideoDevice = _config.VideoDeviceName;
            SelectedAudioDevice = _config.AudioDeviceName;
            MicrophoneVolume = _config.MicrophoneVolume;
        }

        private void SampleAggregatorOnMaximumCalculated(object sender, MaxSampleArgs maxSampleArgs)
        {
            CurrentInputLevel = Math.Max(maxSampleArgs.MaxSample, Math.Abs(maxSampleArgs.MinSample)) * 100;
            OnPropertyChanged(nameof(CurrentInputLevel));
        }

        private FfmpegParams CreateFfmpegParams()
        {
            return new FfmpegParams
            {
                AudioDeviceName = SelectedAudioDevice,
                ShowType = EnumExtensions.GetEnumByDescription<ShowType>(SelectedShowTypeMain),
                VideoDeviceName = SelectedVideoDevice,
                WindowName = SelectWindow ?? String.Empty

            };
        }
    }
}