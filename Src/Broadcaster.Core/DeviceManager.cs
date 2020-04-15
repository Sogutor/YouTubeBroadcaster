using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcaster.Core.Audio;
using Broadcaster.Interfaces;
using DirectShowLib;

namespace Broadcaster.Core
{
    public class DeviceManager : IDeviceManager
    {
        private readonly IAudioHelper _audioHelper;

        public DeviceManager(IAudioHelper audioHelper)
        {
            _audioHelper = audioHelper;
        }

        public IAudioHelper AudioHelper => _audioHelper;
        public DsDevice[] GetVideoDevices() => DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
        public DsDevice[] GetAudioDevices() => DsDevice.GetDevicesOfCat(FilterCategory.AudioInputDevice);
        public List<string> GetVideoDevicesNames() => GetVideoDevices().Select(device => device.Name).ToList();
        public List<string> GetAudioDevicesNames() => GetAudioDevices().Select(device => device.Name).ToList();
        public IVolumeController GetVolumeControl(string name) => _audioHelper.GetVolumeControl(name);

    }
}
