using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Broadcaster.UI.ViewModels;

namespace Broadcaster.UI
{
    public class ContainerViewsObserver : IContainerViewsObserver
    {
        private readonly LazyViewsContainer _lazyViewsContainer;
        private readonly Subject<ViewModelBase> _behaviorSubject;
        public IObservable<ViewModelBase> ContainerObserver => _behaviorSubject.AsObservable();

        public ContainerViewsObserver(LazyViewsContainer lazyViewsContainer)
        {
            _lazyViewsContainer = lazyViewsContainer;
            _behaviorSubject = new Subject<ViewModelBase>();
        }
        public void SetActiveViewModel(ViewModelBase viewModel) => _behaviorSubject.OnNext(viewModel);
        public void SetActiveViewModel(ViewModelType viewModelType) => _behaviorSubject.OnNext(_lazyViewsContainer.GetViewModel(viewModelType));

        public T GetViewModel<T>(ViewModelType type) where T : ViewModelBase => (T)_lazyViewsContainer.GetViewModel(type);

    }
}
