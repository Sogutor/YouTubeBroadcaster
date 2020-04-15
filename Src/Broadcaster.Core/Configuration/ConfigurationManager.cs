using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Broadcaster.Core.Configuration
{
    public class ConfigurationManager<TConfig> : IConfigurationManager<TConfig> where TConfig : IConfig
    {
        public TConfig LastReadingConfig { get; private set; }
        private readonly JsonSerializer _serializer;
        private readonly string _configurationPath;
        public ConfigurationManager(string configName, string pathToDirectory)
        {
            if (string.IsNullOrEmpty(configName) || string.IsNullOrWhiteSpace(configName))
                throw new ArgumentException(nameof(configName));
            if (string.IsNullOrEmpty(pathToDirectory) || string.IsNullOrWhiteSpace(pathToDirectory) || !Directory.Exists(pathToDirectory))
                throw new ArgumentException($"{nameof(pathToDirectory)} : ({pathToDirectory}) directory can't be found");
            _serializer = new JsonSerializer();
            _configurationPath = Path.Combine(pathToDirectory, configName);
        }

        public TConfig ReadConfiguration()
        {
            if (!File.Exists(_configurationPath)) WriteConfiguration(new Func<TConfig>(() =>
            {
                var conf = Activator.CreateInstance<TConfig>();
                conf.PathToScreenshots = AppDomain.CurrentDomain.BaseDirectory;
                return conf;
            })());
            var fs = new FileStream(_configurationPath, FileMode.Open);
            using (var jr = new BsonReader(fs))
            {
                LastReadingConfig = _serializer.Deserialize<TConfig>(jr);
                return LastReadingConfig;
            }
        }
        public bool WriteConfiguration(TConfig config)
        {
            var fs = new FileStream(_configurationPath, FileMode.Create);
            using (var jw = new BsonWriter(fs))
            {
                _serializer.Serialize(jw, config);
                fs.Flush();
            }
            return true;
        }
    }
}
