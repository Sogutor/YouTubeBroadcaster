namespace Broadcaster.Core.Configuration
{
    public interface IConfigurationManager<TConfig> where TConfig : IConfig
    {
        TConfig LastReadingConfig { get; }
        TConfig ReadConfiguration();
        bool WriteConfiguration(TConfig config);
    }
}