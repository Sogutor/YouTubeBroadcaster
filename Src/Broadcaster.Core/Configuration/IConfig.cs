using Broadcaster.Interfaces.Enums;

namespace Broadcaster.Core.Configuration
{
    public interface IConfig
    {
        string VideoDeviceName { get; set; }
        string AudioDeviceName { get; set; }
        Resolution Resolution { get; set; }
        double MicrophoneVolume { get; set; }
        string PathToVideo { get; set; }
        string PathToScreenshots { get; set; }
         bool IsSettingsFill { get; set; }
    }
}