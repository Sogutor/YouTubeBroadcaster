using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcaster.Interfaces;
using Broadcaster.Interfaces.Enums;
using Broadcaster.Interfaces.Helpers;
using Google.Apis.YouTube.v3.Data;

namespace Broadcaster.YouTube
{
    public class YouTubeBroadcastAdapter : IYouTubeBroadcast
    {
        private readonly LiveBroadcast _liveBroadcast;
        private LiveStream _stream;
        public string Url => $"https://www.youtube.com/watch?v={_liveBroadcast.Id}";
        public string Title => _liveBroadcast.Snippet.Title;
        public string Description => _liveBroadcast.Snippet.Description;
        public PrivacyEnum PrivacyStatus => (PrivacyEnum)Enum.Parse(typeof(PrivacyEnum), _liveBroadcast.Status.PrivacyStatus, true);
        public Resolution Resolution => EnumExtensions.GetEnumByDescription<Resolution>(_stream?.Cdn.Resolution);
        public FrameRate FrameRate => EnumExtensions.GetEnumByDescription<FrameRate>(_stream.Cdn.FrameRate);
        public DateTime ScheduledStartTime => (DateTime)_liveBroadcast.Snippet.ScheduledStartTime;
        public string PathToStream => $"{_stream.Cdn.IngestionInfo.IngestionAddress}/{_stream.Cdn.IngestionInfo.StreamName}";
        public string Id => _liveBroadcast.Id;

        public YouTubeBroadcastAdapter(LiveBroadcast liveBroadcast)
        {
            _liveBroadcast = liveBroadcast;
        }
        public LiveBroadcast LiveBroadcast => _liveBroadcast;
        public void BindStream(LiveStream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }
    }
}
