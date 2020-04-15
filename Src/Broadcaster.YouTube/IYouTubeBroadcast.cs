using Broadcaster.Interfaces;
using Google.Apis.YouTube.v3.Data;

namespace Broadcaster.YouTube
{
    internal interface IYouTubeBroadcast : IBroadcast
    {
        LiveBroadcast LiveBroadcast { get; }
        void BindStream(LiveStream stream);
    }
}