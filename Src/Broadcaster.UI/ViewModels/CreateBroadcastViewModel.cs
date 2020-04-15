using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Broadcaster.Core;
using Broadcaster.Core.Configuration;
using Broadcaster.Interfaces;
using Broadcaster.Interfaces.Enums;
using Broadcaster.Interfaces.Helpers;
using JetBrains.Annotations;

namespace Broadcaster.UI.ViewModels
{
    //TODO use automap
    [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
    public class CreateBroadcastViewModel : ViewModelBase
    {
        private readonly IController _controller;
        private readonly IConfig _config;
        private readonly IContainerViewsObserver _containerViewsObserver;
        private ICommand _createBroadcastCommand;
        private ICommand _cancelCommand;
        private DateTime _minDateTime;
        public string Title { get; set; }
        public string Description { get; set; }
        public string SelectedPrivacy { get; set; }
        public string SelectedResolution { get; set; }
        public string SelectedFrameRate { get; set; }
        public DateTime ScheduledStartDate { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
        public bool ShowError { get; set; }
        private readonly Subject<Unit> _cancelCommandSubject;
        public IObservable<Unit> CancelCommandObservable => _cancelCommandSubject.AsObservable();
        public DateTime MinDateTime
        {
            get { return _minDateTime; }
            set
            {
                _minDateTime = value;
                OnPropertyChanged();
            }
        }
        [UsedImplicitly]
        public ObservableCollection<string> Resolution => new ObservableCollection<string>(Enum.GetValues(typeof(Resolution)).Cast<Resolution>().Select(resolution => resolution.GetDescription()));
        [UsedImplicitly]
        public ObservableCollection<string> PrivacyEnum => new ObservableCollection<string>(Enum.GetValues(typeof(PrivacyEnum)).Cast<PrivacyEnum>().Select(privacy => privacy.GetDescription()));
        [UsedImplicitly]
        public ObservableCollection<string> FrameRate => new ObservableCollection<string>(Enum.GetValues(typeof(FrameRate)).Cast<FrameRate>().Select(frameRate => frameRate.GetDescription()));
        public CreateBroadcastViewModel(IController controller, IConfig config, IContainerViewsObserver containerViewsObserver)
        {
            _controller = controller;
            _config = config;
            _containerViewsObserver = containerViewsObserver;
            InitDefaultProp();
            SelectedFrameRate = Interfaces.Enums.FrameRate.Fps30.GetDescription();
            _cancelCommandSubject = new Subject<Unit>();
            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(l => MinDateTime = DateTime.Today);
        }

        private void InitDefaultProp()
        {

            ScheduledStartDate = DateTime.Now;
            ScheduledStartTime = DateTime.Now;
            SelectedResolution = _config.Resolution.GetDescription();
            SelectedPrivacy = Interfaces.Enums.PrivacyEnum.Public.GetDescription();
            Title = String.Empty;
            Description = String.Empty;
            OnPropertyChanged("");
        }
        public void Reset() => InitDefaultProp();

        private IBroadcast ToBroadcast()
        {

            if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(Title))
            {
                ShowError = true;
                OnPropertyChanged(nameof(ShowError));
                Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(l =>
                    {
                        ShowError = false;
                        OnPropertyChanged(nameof(ShowError));
                    });
                return null;
            }
            return new BroadcastPoco
            {
                Description = Description,
                FrameRate = EnumExtensions.GetEnumByDescription<FrameRate>(SelectedFrameRate),
                Resolution = EnumExtensions.GetEnumByDescription<Resolution>(SelectedResolution),
                PrivacyStatus = EnumExtensions.GetEnumByDescription<PrivacyEnum>(SelectedPrivacy),
                Title = Title,
                ScheduledStartTime = (DateTime)(ScheduledStartDate + ScheduledStartTime.Value.TimeOfDay)
            };
        }
        [UsedImplicitly]
        public ICommand CreateBroadcastCommand => _createBroadcastCommand ?? (_createBroadcastCommand =
                new RelayCommand(async () =>
                {
                    var bc = ToBroadcast();
                    if (bc == null) return;
                    await _controller.CreateBroadcastAsync(bc).ConfigureAwait(false);
                    _cancelCommandSubject.OnNext(Unit.Default);
                }));
        [UsedImplicitly]

        public ICommand CancelCommand => _cancelCommand ?? (_cancelCommand =
                new RelayCommand(() => _cancelCommandSubject.OnNext(Unit.Default)));
    }
}
