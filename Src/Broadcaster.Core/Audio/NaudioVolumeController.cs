using Broadcaster.Interfaces;
using NAudio.Mixer;

namespace Broadcaster.Core.Audio
{
    public class NaudioVolumeController : IVolumeController
    {
        private readonly UnsignedMixerControl _unsignedMixerControl;

        public NaudioVolumeController(UnsignedMixerControl unsignedMixerControl)
        {
            _unsignedMixerControl = unsignedMixerControl;
        }
        public double Percent { get { return _unsignedMixerControl.Percent; } set { _unsignedMixerControl.Percent = value; } }
    }
}