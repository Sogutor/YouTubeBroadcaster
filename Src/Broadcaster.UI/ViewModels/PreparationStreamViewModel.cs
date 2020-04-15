using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Broadcaster.Core;
using Broadcaster.Core.Ffmpeg;
using Broadcaster.Interfaces;
using JetBrains.Annotations;

namespace Broadcaster.UI.ViewModels
{
    public class PreparationStreamViewModel : ViewModelBase
    {
        private readonly IContainerViewsObserver _containerViewsObserver;
        private readonly IController _controller;
        private readonly IDeviceManager _deviceManager;
        private IBroadcast _broadcast;
        private BroadcastViewModel _broadcastViewModel;
        private ICommand _cancelCommand;
        private ICommand _stratCommand;
        private ICommand _stratTestCommand;
        private FfmpegParams _ffmpegParams;
        private bool _testButtonAvailable;
        private ViewModelBase _rightViewModel;
        private Visibility _contextButtomVisibility;
        public bool CanStart { get; private set; }
        public PreparationStreamViewModel(IController controller, IContainerViewsObserver containerViewsObserver, IDeviceManager deviceManager)
        {
            StreamPlayerViewModel = new StreamPlayerViewModel();
            TestButtonAvailable = true;
            _controller = controller;
            _containerViewsObserver = containerViewsObserver;

            _deviceManager = deviceManager;
            ConnectViewModels();
            PlayerHeight = 200;
            PlayerWidth = 360;

        }

        public void Update()
        {
            _containerViewsObserver.GetViewModel<SessionConfiguratorViewModel>(
                      ViewModelType.SessionConfiguratorViewModel).InitMicrophoneListener();
        }
        private void ConnectViewModels()
        {

            SelectBroadcastViewModel =
                _containerViewsObserver.GetViewModel<SelectBroadcastViewModel>(ViewModelType.SelectBroadcastViewModel);
            SelectBroadcastViewModel.CommandForShowingDialog.Subscribe(_ =>
            {
                RightViewModel = _containerViewsObserver.GetViewModel<CreateBroadcastViewModel>(
                        ViewModelType.CreateBroadcastViewModel);
                ContextButtomVisibility = Visibility.Collapsed;
            });
            _containerViewsObserver.GetViewModel<CreateBroadcastViewModel>(
                    ViewModelType.CreateBroadcastViewModel)
                .CancelCommandObservable.Subscribe(async _ =>
                {
                    ShowSessionConfiguratorViewModel();
                    await SelectBroadcastViewModel.UpdateBroadcstListAsync().ConfigureAwait(false);
                });
            SelectBroadcastViewModel.SelectedBroadcastObservable.Subscribe(broadcast =>
            {
                SetBroadcast(broadcast);
                CanStart = broadcast != null; OnPropertyChanged(nameof(CanStart));

            });
            _containerViewsObserver.GetViewModel<SessionConfiguratorViewModel>(
                ViewModelType.SessionConfiguratorViewModel).FfmpegParamsObservable.Subscribe(ffmpegParams => _ffmpegParams = ffmpegParams);
            ShowSessionConfiguratorViewModel();
        }

        private void ShowSessionConfiguratorViewModel()
        {

            RightViewModel = _containerViewsObserver.GetViewModel<SessionConfiguratorViewModel>(
                      ViewModelType.SessionConfiguratorViewModel);
            ContextButtomVisibility = Visibility.Visible;
        }
        public bool TestButtonAvailable
        {
            get { return _testButtonAvailable; }
            set
            {
                _testButtonAvailable = value;
                OnPropertyChanged();
            }
        }

        public Visibility ContextButtomVisibility
        {
            get { return _contextButtomVisibility; }
            private set { _contextButtomVisibility = value; OnPropertyChanged(); }
        }

        public BroadcastViewModel BroadcastViewModel
        {
            get { return _broadcastViewModel; }
            set
            {
                _broadcastViewModel = value;
                OnPropertyChanged();
            }
        }


        public StreamPlayerViewModel StreamPlayerViewModel { get; set; }
        public SelectBroadcastViewModel SelectBroadcastViewModel { get; private set; }



        public ViewModelBase RightViewModel
        {
            get { return _rightViewModel; }
            private set
            {
                if (_rightViewModel == value) return;
                _rightViewModel = value;
                OnPropertyChanged();
            }
        }

        private volatile bool _useFfmpeg;
        private Visibility _streamPlayerVisibility;
        private Visibility _camerPlayerVisibility;
        private int _playerHeight;
        private int _playerWidth;

        public Visibility StreamPlayerVisibility
        {
            get { return _streamPlayerVisibility; }
            set { _streamPlayerVisibility = value; OnPropertyChanged(); }
        }
        public Visibility CameraPlayerVisibility
        {
            get { return _camerPlayerVisibility; }
            set { _camerPlayerVisibility = value; OnPropertyChanged(); }
        }
        [UsedImplicitly]
        public int PlayerHeight
        {
            get { return _playerHeight; }
            private set
            {
                _playerHeight = value;
                OnPropertyChanged();
            }
        }
        [UsedImplicitly]
        public int PlayerWidth
        {
            get { return _playerWidth; }
            private set
            {
                _playerWidth = value;
                OnPropertyChanged();
            }
        }

        private void ChangeVisibility()
        {
            CameraPlayerVisibility = CameraPlayerVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            StreamPlayerVisibility = StreamPlayerVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
        [UsedImplicitly]
        public ICommand StartTestCommand => _stratTestCommand ?? (_stratTestCommand = new RelayCommand(async () =>
                                            {
                                               
                                                TestButtonAvailable = false;
                                                await StopTestAsync().ConfigureAwait(false);
                                                if (_ffmpegParams.ShowType == ShowType.WebCam || _ffmpegParams.ShowType == ShowType.WebCamH264 || _ffmpegParams.ShowType==ShowType.WebCam43)
                                                {
                                                    if (_useFfmpeg)
                                                    {
                                                        ChangeVisibility();
                                                    }
                                                    _useFfmpeg = false;
                                                    HelperCameraPreview.WebCameraControl.Start(_ffmpegParams.VideoDeviceName);
                                                }
                                                else
                                                {
                                                    if (!_useFfmpeg)
                                                    {
                                                        ChangeVisibility();
                                                    }
                                                    _useFfmpeg = true;
                                                    await StreamPlayerViewModel.StartAsync().ConfigureAwait(false);
                                                    _controller.TestShow(_ffmpegParams);
                                                }
                                                if (_ffmpegParams.ShowType == ShowType.WebCam43)
                                                {
                                                    PlayerHeight = 200;
                                                    PlayerWidth = 260;
                                                }
                                                else
                                                {
                                                    PlayerHeight = 200;
                                                    PlayerWidth = 360;
                                                }
                                                TestButtonAvailable = true;
                                            }));

        private async Task StopTestAsync()
        {
            if (_useFfmpeg)
                await StreamPlayerViewModel.StopPlayerAsync().ConfigureAwait(false);
            else
                HelperCameraPreview.WebCameraControl.Stop();
        }
        [UsedImplicitly]
        public ICommand StartCommand => _stratCommand ?? (_stratCommand = new RelayCommand(async () =>
        {
            await StopTestAsync().ConfigureAwait(false);
            await _deviceManager.AudioHelper.StopAsync().ConfigureAwait(true);
            var broadcastStreamViewModel =
                _containerViewsObserver.GetViewModel<BroadcastStreamViewModel>(
                    ViewModelType.BroadcastStreamViewModel);
            broadcastStreamViewModel.SetBroadcast(_broadcast, _ffmpegParams);
            _containerViewsObserver.SetActiveViewModel(broadcastStreamViewModel);
        }));
        [UsedImplicitly]
        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(async () =>
        {
            await StopTestAsync().ConfigureAwait(false);
            await _deviceManager.AudioHelper.StopAsync().ConfigureAwait(false);
            _containerViewsObserver.SetActiveViewModel(
                ViewModelType.NavigationPanel);
        }));


        private void SetBroadcast(IBroadcast broadcast)
            => BroadcastViewModel = new BroadcastViewModel(_broadcast = broadcast);
    }
}