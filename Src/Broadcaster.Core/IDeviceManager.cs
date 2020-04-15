using System.Collections.Generic;
using Broadcaster.Core.Audio;
using Broadcaster.Interfaces;
using DirectShowLib;

namespace Broadcaster.Core
{
    public interface IDeviceManager
    {
        DsDevice[] GetVideoDevices();
        DsDevice[] GetAudioDevices();
        List<string> GetVideoDevicesNames();
        List<string> GetAudioDevicesNames();
        IVolumeController GetVolumeControl(string name);
        IAudioHelper AudioHelper { get; }
    }
}