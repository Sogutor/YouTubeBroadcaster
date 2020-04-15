using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Broadcaster.Core.Configuration
{
    public interface IWritableConfig
    {
        IConfig Config { get; }
        IWritableConfig BindToWriter(Func<IConfig,bool> writerFunc);
        void Write();
    }

    public class WritableConfig : IWritableConfig
    {
      
        private Func<IConfig,bool> _writerFunc;
        public IConfig Config { get; }

        public WritableConfig(IConfig config)
        {
            Config = config;
        }
        public IWritableConfig BindToWriter(Func<IConfig,bool> writerFunc)
        {
            _writerFunc = writerFunc;
            return this;
        }
        public void Write()
        {
            _writerFunc(Config);
        }
    }
}
