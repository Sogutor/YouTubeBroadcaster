using System.Collections.Generic;
using System.Threading.Tasks;
using Broadcaster.Core.Ffmpeg;
using Broadcaster.Interfaces;

namespace Broadcaster.Core
{
    public interface IController
    {
        Task<List<IBroadcast>> GetBroadcastsAsync();
        Task CreateBroadcastAsync(IBroadcast broadcast);
        Task<bool> DeleteBroadcastAsync(string broadcastId);
        Task<IBroadcast> PrepareToStreamAsync(IBroadcast broadcast, FfmpegParams ffmpegParams);
        Task<bool> IsBroadcastReadyToLifeAsync(IBroadcast broadcast);
        Task<IBroadcast> StartLiveStreamAsync(IBroadcast broadcast);
        void StopStreaming();
        void TestShow(FfmpegParams ffmpegParams);
    }
}