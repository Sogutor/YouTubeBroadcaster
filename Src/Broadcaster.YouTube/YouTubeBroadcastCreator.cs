using Broadcaster.Interfaces;
using Google.Apis.YouTube.v3.Data;

namespace Broadcaster.YouTube
{
    internal class YouTubeBroadcastCreator : IBroadcastCreator
    {
        public IBroadcast Create(IBroadcast broadcast)
        {
            var liveBroadcast = new LiveBroadcast
            {
                Snippet = new LiveBroadcastSnippet
                {
                    Title = broadcast.Title,
                    Description = broadcast.Description,
                    ScheduledStartTime = broadcast.ScheduledStartTime
                },
                Status = new LiveBroadcastStatus
                {
                    PrivacyStatus = broadcast.PrivacyStatus.ToString().ToLower()
                },
                ContentDetails = new LiveBroadcastContentDetails { MonitorStream = new MonitorStreamInfo() }
            };
            return new YouTubeBroadcastAdapter(liveBroadcast);
        }
    }
}
