using System;
using System.Reflection;
using System.Windows;
using Broadcaster.Core;
using Broadcaster.UI.ViewModels;
using Ninject;

namespace Broadcaster.UI
{
    public static class NinjectExtension
    {
        public static void BindAsLazyInSingeltonScope<T>(this IKernel kernel)
        {
            kernel.Bind<Lazy<T>>().ToMethod(context => new Lazy<T>(() => (T)kernel.Get(typeof(T)))).InSingletonScope();
        }
    }
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IKernel _kernel;
        private MainViewModel _mainViewModel;
        protected override void OnStartup(StartupEventArgs e)
        {
            SetCallingAssemblyAsEntryAssembly();
            base.OnStartup(e);
            _kernel = new StandardKernel(new BroadcastModule());
            _kernel.Bind<LazyViewsContainer>().ToSelf().InSingletonScope();
            _kernel.Bind<IContainerViewsObserver>().To<ContainerViewsObserver>().InSingletonScope();
            _kernel.BindAsLazyInSingeltonScope<NavigationPanelViewModel>();
            _kernel.BindAsLazyInSingeltonScope<SettingsViewModel>();
            _kernel.BindAsLazyInSingeltonScope<SelectBroadcastViewModel>();
            _kernel.BindAsLazyInSingeltonScope<CreateBroadcastViewModel>();
            _kernel.BindAsLazyInSingeltonScope<BroadcastStreamViewModel>();
            _kernel.BindAsLazyInSingeltonScope<PreparationStreamViewModel>();
            _kernel.BindAsLazyInSingeltonScope<SessionConfiguratorViewModel>();
            _mainViewModel = _kernel.Get<MainViewModel>();
            Current.MainWindow = new MainWindow { DataContext = _mainViewModel };
            Current.MainWindow.Closing += (sender, args) => HelperCameraPreview.WebCameraControl = null;
            Current.MainWindow.Show();
        }

        private void SetCallingAssemblyAsEntryAssembly()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield =
                manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance |
                BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);
            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField =
                domain.GetType().GetField("_domainManager", BindingFlags.Instance |
                BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }
    }
}
