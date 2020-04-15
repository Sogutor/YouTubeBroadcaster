using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcaster.UI.ViewModels;

namespace Broadcaster.UI
{
    public enum ViewModelType
    {
        Settings,
        NavigationPanel,
        SelectBroadcastViewModel,
        CreateBroadcastViewModel,
        BroadcastStreamViewModel,
        PreparationStreamViewModel,
        SessionConfiguratorViewModel
    }
    public class LazyViewsContainer
    {
        private readonly ConcurrentDictionary<ViewModelType, Func<ViewModelBase>> _viewModelDictionary;
        public LazyViewsContainer(Lazy<SettingsViewModel> settingsViewModel, Lazy<NavigationPanelViewModel> navigationPanelViewModel,
            Lazy<SelectBroadcastViewModel> selectBroadcastViewModel, Lazy<CreateBroadcastViewModel> createBroadcastViewModel, Lazy<BroadcastStreamViewModel> broadcastStreamViewModel,
             Lazy<PreparationStreamViewModel> preparationStreamViewModel, Lazy<SessionConfiguratorViewModel> sessionConfiguratorViewModel)
        {
            _viewModelDictionary = new ConcurrentDictionary<ViewModelType, Func<ViewModelBase>>
            {
                [ViewModelType.Settings] = () => settingsViewModel.Value,
                [ViewModelType.NavigationPanel] = () => navigationPanelViewModel.Value,
                [ViewModelType.SelectBroadcastViewModel] = () =>
                {
                    Task.Run(async () => await selectBroadcastViewModel.Value.UpdateBroadcstListAsync().ConfigureAwait(false));
                    return selectBroadcastViewModel.Value;
                },
                [ViewModelType.CreateBroadcastViewModel] = () => { createBroadcastViewModel.Value.Reset(); return createBroadcastViewModel.Value; },
                [ViewModelType.BroadcastStreamViewModel] = () => broadcastStreamViewModel.Value,
                [ViewModelType.PreparationStreamViewModel] = () => { preparationStreamViewModel.Value.Update(); return preparationStreamViewModel.Value; },
                [ViewModelType.SessionConfiguratorViewModel] = () => sessionConfiguratorViewModel.Value
            };

        }

        public ViewModelBase GetViewModel(ViewModelType viewModelType)
        {
            Func<ViewModelBase> viewModel;
            _viewModelDictionary.TryGetValue(viewModelType, out viewModel);
            return viewModel();
        }
    }
}
