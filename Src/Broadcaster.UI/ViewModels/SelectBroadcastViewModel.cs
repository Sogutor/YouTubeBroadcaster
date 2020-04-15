using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using Broadcaster.Core;
using Broadcaster.Interfaces;
using JetBrains.Annotations;

namespace Broadcaster.UI.ViewModels
{
    public class SelectBroadcastViewModel : ViewModelBase
    {
        private readonly IContainerViewsObserver _containerViewsObserver;
        private readonly IController _controller;
        private ICommand _createNewBroadcastCommand;
        private ICommand _cancelCommand;
        private ICommand _deleteCommand;
        private ICommand _startCommand;
        private ICommand _updateCommand;
        private ICommand _selectedBroadcastChangeCommand;
        private ICommand _openInBrowser;
        private List<BroadcastViewModel> _broadcastViewModels = new List<BroadcastViewModel>();

        public bool ContextCommandAvailable
        {
            get { return _contextCommandAvailable; }
            private set
            {
                if (_contextCommandAvailable == value) return;
                _contextCommandAvailable = value;
                OnPropertyChanged();
            }
        }

        private bool _showWaitingDialog;
        private bool _contextCommandAvailable;

        public bool ShowWaitingDialog
        {
            get { return _showWaitingDialog; }
            set
            {
                _showWaitingDialog = value;
                OnPropertyChanged();
            }
        }

        public IObservable<Unit> CommandForShowingDialog => _commandForShowingDialog.AsObservable();
        private readonly Subject<Unit> _commandForShowingDialog;
        public IObservable<IBroadcast> SelectedBroadcastObservable => _selectedBroadcastObservable.AsObservable();
        private readonly Subject<IBroadcast> _selectedBroadcastObservable;
        public BroadcastViewModel SelectedBroadcast { get; set; }
        public ObservableCollection<BroadcastViewModel> BroadcastViewModels => new ObservableCollection<BroadcastViewModel>(_broadcastViewModels);
        public SelectBroadcastViewModel(IContainerViewsObserver containerViewsObserver, IController controller)
        {

            _containerViewsObserver = containerViewsObserver;
            _controller = controller;
            _commandForShowingDialog = new Subject<Unit>();
            _selectedBroadcastObservable = new Subject<IBroadcast>();

        }

        public async Task UpdateBroadcstListAsync()
        {
            ShowWaitingDialog = true;
            _broadcastViewModels = (await _controller.GetBroadcastsAsync().ConfigureAwait(false)).Select(
                   broadcast => new BroadcastViewModel(broadcast)).ToList();
            OnPropertyChanged(nameof(BroadcastViewModels));
            ShowWaitingDialog = false;
        }

        [UsedImplicitly]
        public ICommand CreateNewBroadcastCommand => _createNewBroadcastCommand ?? (_createNewBroadcastCommand = new RelayCommand(
                     () => _commandForShowingDialog.OnNext(Unit.Default)));
        [UsedImplicitly]
        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand =
              new RelayCommand(() => _containerViewsObserver.SetActiveViewModel(ViewModelType.NavigationPanel)));
        [UsedImplicitly]
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(async () =>
               {
                   if (SelectedBroadcast == null) return;
                   if (await _controller.DeleteBroadcastAsync(SelectedBroadcast.GetId).ConfigureAwait(false))
                   {
                       await UpdateBroadcstListAsync().ConfigureAwait(false);
                   }
               }));
        [UsedImplicitly]
        public ICommand UpdateCommand => _updateCommand ?? (_updateCommand = new RelayCommand(async () => await UpdateBroadcstListAsync().ConfigureAwait(false)));
        [UsedImplicitly]
        public ICommand SelectedBroadcastChangeCommand => _selectedBroadcastChangeCommand ?? (_selectedBroadcastChangeCommand = new RelayCommand(() =>
                                                          {
                                                              if (SelectedBroadcast != null)
                                                              {
                                                                  ContextCommandAvailable = true;
                                                                  _selectedBroadcastObservable.OnNext(SelectedBroadcast.GetBroadcast());
                                                              }
                                                              else
                                                              {
                                                                  ContextCommandAvailable = false;
                                                                  _selectedBroadcastObservable.OnNext(null);
                                                              }
                                                          }));
        [UsedImplicitly]
        public ICommand OpenInBrowser => _openInBrowser ?? (_openInBrowser = new RelayCommand(() => Process.Start(SelectedBroadcast.Url)));
    }
}
