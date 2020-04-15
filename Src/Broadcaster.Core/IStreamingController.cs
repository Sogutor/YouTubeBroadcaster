using Broadcaster.Core.Ffmpeg;
using Broadcaster.Interfaces;

namespace Broadcaster.Core
{
    public interface IStreamingController
    {
        void StartStreaming(FfmpegParams ffmpegParams);
        void StopStreaming();
        void TestShow(FfmpegParams ffmpegParams);
    }
}