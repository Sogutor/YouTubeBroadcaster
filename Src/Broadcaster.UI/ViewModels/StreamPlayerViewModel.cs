using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;
using WebEye;

namespace Broadcaster.UI.ViewModels
{
    public class StreamPlayerViewModel : ViewModelBase
    {
        private bool _showWaitingDialog;
        private IDisposable _toDispose;
        private readonly Task<bool> _playerAsync;
        private ICommand _streamStartedCommand;
        private bool _isPlay;
        public bool ShowWaitingDialog
        {
            get { return _showWaitingDialog; }
            set
            {
                _showWaitingDialog = value;
                OnPropertyChanged();
            }
        }
        public StreamPlayerProxy PlayerControl { get; set; }
        public StreamPlayerViewModel()
        {
            var tcs = new TaskCompletionSource<bool>();
            _playerAsync = tcs.Task;
            _toDispose = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(l =>
             {
                 if (PlayerControl == null) return;
                 tcs.SetResult(true);
                 _toDispose.Dispose();
             });
        }
        public async Task StopPlayerAsync()
        {
            if (_isPlay)
            {
                await _playerAsync.ConfigureAwait(false);
                PlayerControl.Stop();
                while (true)
                {
                    if (Process.GetProcesses().Any(process => process.ProcessName == "ffmpeg"))
                    {
                        await Task.Delay(50).ConfigureAwait(false);
                        try
                        {
                            Process.GetProcesses().Where(process => process.ProcessName == "ffmpeg").ToList().ForEach(b => b.Kill());
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            ShowWaitingDialog = false;
            _isPlay = false;

        }
        [UsedImplicitly]
        public ICommand StreamStartedCommand => _streamStartedCommand ?? (_streamStartedCommand = new RelayCommand(() => { ShowWaitingDialog = false; }));
        public async Task StartAsync()
        {
            ShowWaitingDialog = true;
            await _playerAsync.ConfigureAwait(false);
            PlayerControl.StartPlay("tcp://127.0.0.1:2000?listen", TimeSpan.FromSeconds(15));
            _isPlay = true;
        }


    }
}
