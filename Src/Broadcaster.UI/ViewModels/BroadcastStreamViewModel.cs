using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Broadcaster.Core;
using Broadcaster.Core.Audio;
using Broadcaster.Core.Ffmpeg;
using Broadcaster.Interfaces;
using JetBrains.Annotations;

namespace Broadcaster.UI.ViewModels
{
    public class BroadcastStreamViewModel : ViewModelBase
    {
        private readonly IContainerViewsObserver _containerViewsObserver;
        private readonly IController _controller;
        private readonly IDeviceManager _deviceManager;
        private IBroadcast _broadcast;
        private bool _canStart;
        private double _microphoneVolume;
        private ICommand _recordCommand;

        private ICommand _screenshotCommand;
        private ICommand _startCommand;
        private ICommand _stopCommand;
        private IDisposable _toDispose;


        private IVolumeController _volumeController;
        //      private Task<bool> PlayerInitTask;
        public BroadcastStreamViewModel(IController controller, IContainerViewsObserver containerViewsObserver,
            IDeviceManager deviceManager)
        {
            StreamPlayerViewModel = new StreamPlayerViewModel();
            _controller = controller;
            _containerViewsObserver = containerViewsObserver;
            _deviceManager = deviceManager;
            _deviceManager.AudioHelper.SampleAggregator.MaximumCalculated += SampleAggregator_MaximumCalculated;
        }

        public StreamPlayerViewModel StreamPlayerViewModel { get; set; }

        [UsedImplicitly]
        public string TranslationName => _broadcast.Title;

        public bool CanStart
        {
            get { return _canStart; }
            private set
            {
                _canStart = value;
                OnPropertyChanged();
            }
        }

        public double MicrophoneVolume
        {
            get { return _microphoneVolume; }
            set
            {
                _microphoneVolume = value;
                _volumeController.Percent = value;
            }
        }

        public float CurrentInputLevel { get; private set; }

        [UsedImplicitly]
        public ICommand ScreenshotCommand => _screenshotCommand ?? (_screenshotCommand = new RelayCommand(() => { }));

        [UsedImplicitly]
        public ICommand StartCommand => _startCommand ?? (_startCommand = new RelayCommand(async () =>
        {
            _broadcast = await _controller.StartLiveStreamAsync(_broadcast).ConfigureAwait(false);
            CanStart = false;
        }));

        [UsedImplicitly]
        public ICommand StopCommand => _stopCommand ?? (_stopCommand = new RelayCommand(async () =>
        {
            CanStart = false;
            Observable.Timer(TimeSpan.FromMilliseconds(100)).Subscribe(l =>
            {
                Process.GetProcessesByName("ffmpeg").ToList().ForEach(
                    process =>
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception e)
                        {
                        }
                    });
            });
            await StreamPlayerViewModel.StopPlayerAsync().ConfigureAwait(false);
            _controller.StopStreaming();
            _containerViewsObserver.SetActiveViewModel(ViewModelType.PreparationStreamViewModel);
        }));

        [UsedImplicitly]
        public ICommand RecordCommand => _recordCommand ?? (_recordCommand = new RelayCommand(() => { }));

        public void SetBroadcast(IBroadcast broadcast, FfmpegParams ffmpegParams)
        {
            _broadcast = broadcast;
            _deviceManager.AudioHelper.BeginMonitoring(ffmpegParams.AudioDeviceName);
            _volumeController = _deviceManager.AudioHelper.GetCurrentVolumeController;
            MicrophoneVolume = _volumeController.Percent;
            OnPropertyChanged(nameof(MicrophoneVolume));
            Task.Run(async () => await InitStreamAsync(ffmpegParams).ConfigureAwait(false));
        }

        private void SampleAggregator_MaximumCalculated(object sender, MaxSampleArgs maxSampleArgs)
        {
            CurrentInputLevel = Math.Max(maxSampleArgs.MaxSample, Math.Abs(maxSampleArgs.MinSample)) * 100;
            OnPropertyChanged(nameof(CurrentInputLevel));
        }


        private async Task InitStreamAsync(FfmpegParams ffmpegParams)
        {
            StreamPlayerViewModel = new StreamPlayerViewModel();
            await StreamPlayerViewModel.StartAsync().ConfigureAwait(false);
            await _controller.PrepareToStreamAsync(_broadcast, ffmpegParams).ConfigureAwait(false);
            _toDispose = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(_ =>
            {
                if (!_controller.IsBroadcastReadyToLifeAsync(_broadcast).ConfigureAwait(false).GetAwaiter().GetResult())
                    return;
                CanStart = true;
                _toDispose.Dispose();
            });
        }
    }
}