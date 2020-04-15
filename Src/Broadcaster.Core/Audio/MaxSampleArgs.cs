namespace Broadcaster.Core.Audio
{
    public class MaxSampleArgs
    {
        public MaxSampleArgs(float minValue, float maxValue)
        {
            MaxSample = maxValue;
            MinSample = minValue;
        }
        public float MaxSample { get; private set; }
        public float MinSample { get; private set; }
    }
}