using System;

namespace Broadcaster.Core.Audio
{
    public class SampleAggregator
    {
        public event EventHandler<MaxSampleArgs> MaximumCalculated;
        public event EventHandler Restart = delegate { };
        private float maxValue;
        private float minValue;
        public int NotificationCount { get; set; }
        int count;

        public void RaiseRestart()
        {
            Restart(this, EventArgs.Empty);
        }

        private void Reset()
        {
            count = 0;
            maxValue = minValue = 0;
        }

        public void Add(float value)
        {
            maxValue = Math.Max(maxValue, value);
            minValue = Math.Min(minValue, value);
            count++;
            if (count < NotificationCount || NotificationCount <= 0) return;
            MaximumCalculated?.Invoke(this, new MaxSampleArgs(minValue, maxValue));
            Reset();
        }
    }
}