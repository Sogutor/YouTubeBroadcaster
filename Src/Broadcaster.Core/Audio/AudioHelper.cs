using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Broadcaster.Interfaces;
using NAudio.Mixer;
using NAudio.Wave;

namespace Broadcaster.Core.Audio
{


    public class AudioHelper : IAudioHelper
    {
        private WaveIn _waveIn;
        readonly SampleAggregator _sampleAggregator;
        private WaveFormat _recordingFormat;
        RecordingState _recordingState;
        IVolumeController _volumeControl;
        public AudioHelper()
        {
            _sampleAggregator = new SampleAggregator();
            RecordingFormat = new WaveFormat(44100, 1);
            _recordingState = RecordingState.Stopped;
        }
        public WaveFormat RecordingFormat
        {
            get
            {
                return _recordingFormat;
            }
            set
            {
                _recordingFormat = value;
                _sampleAggregator.NotificationCount = value.SampleRate / 10;
            }
        }

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        public void BeginMonitoring(string recordingDevice) => BeginMonitoring(GetMicrophoneIdByName(recordingDevice));
        public void BeginMonitoring(int recordingDevice)
        {
            if (_recordingState != RecordingState.Stopped)
            {
                throw new InvalidOperationException("Can't begin monitoring while we are in this state: " + _recordingState.ToString());
            }
            _waveIn = new WaveIn();
            _waveIn.DeviceNumber = recordingDevice;
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;
            _waveIn.WaveFormat = _recordingFormat;
            _waveIn.StartRecording();
            _volumeControl = GetVolumeControl(recordingDevice);
            _recordingState = RecordingState.Monitoring;

        }

        private TaskCompletionSource<Unit> _promiseStop;
        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            _recordingState = RecordingState.Stopped;
            _promiseStop.SetResult(Unit.Default);


        }
        public async Task StopAsync()
        {

            if (_recordingState == RecordingState.Monitoring || _recordingState == RecordingState.Recording)
            {
                _promiseStop = new TaskCompletionSource<Unit>();

                _recordingState = RecordingState.RequestedStop;
                _waveIn.StopRecording();
                await _promiseStop.Task.ConfigureAwait(false);
            }

        }
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;
            //  WriteToFile(buffer, bytesRecorded); //TODO implement with memory stream

            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((buffer[index + 1] << 8) |
                                        buffer[index + 0]);
                float sample32 = sample / 32768f;
                _sampleAggregator.Add(sample32);
            }
        }

        private int GetMicrophoneIdByName(string name)
        {
            for (int waveInDevice = 0; waveInDevice < WaveIn.DeviceCount; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                if (name.Contains(deviceInfo.ProductName))
                {
                    return waveInDevice;
                }
            }
            throw new NotSupportedException($"can't find device{name}");
        }

        public IVolumeController GetCurrentVolumeController => _volumeControl;
        public SampleAggregator SampleAggregator => _sampleAggregator;
        public RecordingState RecordingState => _recordingState;
        public IVolumeController GetVolumeControl(int deviceId)
        {
            var mixerLine = new MixerLine((IntPtr)deviceId,
              0, MixerFlags.WaveIn);
            foreach (var control in mixerLine.Controls)
            {
                if (control.ControlType == MixerControlType.Volume)
                {
                    return new NaudioVolumeController(control as UnsignedMixerControl);
                }
            }
            throw new NotSupportedException($"device id {deviceId} do not support volume controling");
        }

        public IVolumeController GetVolumeControl(string name) => GetVolumeControl(GetMicrophoneIdByName(name));

    }
}