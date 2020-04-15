using System;
using Broadcaster.UI.ViewModels;

namespace Broadcaster.UI
{
    public interface IContainerViewsObserver
    {
        IObservable<ViewModelBase> ContainerObserver { get; }
        void SetActiveViewModel(ViewModelBase viewModel);
        void SetActiveViewModel(ViewModelType viewModelType);
        T GetViewModel<T>(ViewModelType type) where T : ViewModelBase;
    }
}