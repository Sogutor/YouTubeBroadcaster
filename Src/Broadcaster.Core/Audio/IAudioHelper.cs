using System.Threading.Tasks;
using Broadcaster.Interfaces;
using NAudio.Wave;

namespace Broadcaster.Core.Audio
{
    public interface IAudioHelper
    {
        SampleAggregator SampleAggregator { get; }
        RecordingState RecordingState { get; }
        void BeginMonitoring(string recordingDevice);
        void BeginMonitoring(int recordingDevice);
        Task StopAsync();
        IVolumeController GetVolumeControl(int deviceId);
        IVolumeController GetVolumeControl(string name);
        IVolumeController GetCurrentVolumeController { get; }
    }
}