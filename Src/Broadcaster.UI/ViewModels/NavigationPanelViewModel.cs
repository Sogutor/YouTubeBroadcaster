using System.Windows.Input;
using JetBrains.Annotations;

namespace Broadcaster.UI.ViewModels
{
    public class NavigationPanelViewModel : ViewModelBase
    {
        private readonly IContainerViewsObserver _containerViewsObserver;
        private ICommand _settingsButtonCommand;
        private ICommand _broadcastButtonCommand;
        public NavigationPanelViewModel(IContainerViewsObserver containerViewsObserver)
        {
            _containerViewsObserver = containerViewsObserver;
        }
        [UsedImplicitly]
        public ICommand SettingsButtonCommand => _settingsButtonCommand ??
            (_settingsButtonCommand = new RelayCommand(() => _containerViewsObserver.SetActiveViewModel(ViewModelType.Settings)));
        [UsedImplicitly]
        public ICommand BroadcastButtonCommand => _broadcastButtonCommand ??
           (_broadcastButtonCommand = new RelayCommand(() =>
           {
               _containerViewsObserver.GetViewModel<SessionConfiguratorViewModel>(ViewModelType.SessionConfiguratorViewModel).SetPropertyFromConfig();
               _containerViewsObserver.SetActiveViewModel(ViewModelType.PreparationStreamViewModel);
           }));
    }
}
