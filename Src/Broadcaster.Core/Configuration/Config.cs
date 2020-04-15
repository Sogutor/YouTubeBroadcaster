using Broadcaster.Interfaces.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Broadcaster.Core.Configuration
{
    [JsonObject]
    public class Config : IConfig
    {
        [JsonProperty]
        public string VideoDeviceName { get; set; }
        [JsonProperty]
        public string AudioDeviceName { get; set; }
        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public Resolution Resolution { get; set; }
        [JsonProperty]
        public double MicrophoneVolume { get; set; }
        [JsonProperty]
        public string PathToVideo { get; set; }
        [JsonProperty]
        public string PathToScreenshots { get; set; }
        [JsonProperty]
        public bool IsSettingsFill { get; set; }


    }
}
