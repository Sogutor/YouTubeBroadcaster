using Broadcaster.Interfaces;
using Ninject.Modules;

namespace Broadcaster.YouTube
{
    public class YouTubeModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBroadcastController>().To<YouTubeBroadcastController>();
            Bind<IBroadcastCreator>().To<YouTubeBroadcastCreator>();
        }
    }
}
