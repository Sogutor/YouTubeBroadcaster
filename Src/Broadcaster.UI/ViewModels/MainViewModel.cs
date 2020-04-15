using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Broadcaster.Core;
using Broadcaster.Core.Configuration;

namespace Broadcaster.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string ProgramVersion { get; } = $"Encoder v{AppDomain.CurrentDomain.DomainManager.EntryAssembly.GetName().Version.ToString(2)} build {AppDomain.CurrentDomain.DomainManager.EntryAssembly.GetName().Version.Build}";

        private readonly IContainerViewsObserver _containerViewsObserver;
        private ViewModelBase _currentDataContext;
        public ViewModelBase CurrentDataContext
        {
            get { return _currentDataContext; }
            private set
            {
                _currentDataContext = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(IContainerViewsObserver containerViewsObserver, IConfig config)
        {

            _containerViewsObserver = containerViewsObserver;
            _containerViewsObserver.ContainerObserver.Subscribe(viewmodel => CurrentDataContext = viewmodel);
            _containerViewsObserver.SetActiveViewModel(config.IsSettingsFill
                ? ViewModelType.NavigationPanel
                : ViewModelType.Settings);
        }
    }
}
