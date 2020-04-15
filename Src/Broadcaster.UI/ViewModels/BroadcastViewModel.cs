using System;
using Broadcaster.Interfaces;
using JetBrains.Annotations;

namespace Broadcaster.UI.ViewModels
{
    public class BroadcastViewModel : ViewModelBase
    {
        private readonly IBroadcast _broadcast;
        [UsedImplicitly]
        public string Title => _broadcast.Title;
        public string Url => _broadcast.Url;
        [UsedImplicitly]
        public DateTime ScheduledStartTime => _broadcast.ScheduledStartTime;
        [UsedImplicitly]
        public string Description => _broadcast.Description;

        public string GetId => _broadcast.Id;
        public IBroadcast GetBroadcast() => _broadcast;
        public BroadcastViewModel(IBroadcast broadcast)
        {

            _broadcast = broadcast;
        }
    }
}
