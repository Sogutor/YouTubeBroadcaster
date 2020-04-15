using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcaster.Core.Audio;
using Broadcaster.Core.Configuration;
using Broadcaster.Core.Ffmpeg;
using Broadcaster.YouTube;
using Ninject;
using Ninject.Modules;

namespace Broadcaster.Core
{
    public class BroadcastModule : NinjectModule
    {
        public override void Load()
        {
            var cm = new ConfigurationManager<Config>("config.conf", AppContext.BaseDirectory);
            Kernel.Load(new YouTubeModule());
            Bind<IConfigurationManager<Config>>().ToConstant(cm);
            Bind<IController>().To<Controller>();
            Bind<IConfig>().ToConstant(cm.ReadConfiguration());
            Bind<IDeviceManager>().To<DeviceManager>().InSingletonScope();
            Bind<IStreamingController>().To<FFmpegController>().InSingletonScope();
            Bind<IFfmpegCommands>().To<FfmpegCommands>();
            Bind<IAudioHelper>().To<AudioHelper>();
            Bind<IWritableConfig>()
                .ToConstant(Kernel.Get<WritableConfig>()
                .BindToWriter(conf => Kernel.Get<IConfigurationManager<Config>>().WriteConfiguration(conf as Config)));
        }
    }
}
