using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Broadcaster.Core.Configuration;
using Broadcaster.Core.Ffmpeg;
using Broadcaster.Interfaces;

namespace Broadcaster.Core
{
    public class Controller : IController
    {
        private readonly IBroadcastController _broadcastController;
        private readonly IStreamingController _streamingController;

        public Controller(IBroadcastController broadcastController, IStreamingController streamingController)
        {
            _broadcastController = broadcastController;
            _streamingController = streamingController;
        }

        public Task CreateBroadcastAsync(IBroadcast broadcast) => _broadcastController.InitializeBroadcastAsync(broadcast);
        public Task<bool> DeleteBroadcastAsync(string broadcastId) => _broadcastController.DeleteBroadcastAsync(broadcastId);
        public Task<List<IBroadcast>> GetBroadcastsAsync() => _broadcastController.GetBroadcastsAsync();

        public async Task<IBroadcast> PrepareToStreamAsync(IBroadcast broadcast, FfmpegParams ffmpegParams)
        {
            broadcast = await _broadcastController.GetLiveStreamAsync(broadcast).ConfigureAwait(false);
            ffmpegParams.PathToStream = broadcast.PathToStream;
            ffmpegParams.Resolution = (FfmpegVideoSize)broadcast.Resolution;
            _streamingController.StartStreaming(ffmpegParams);
            return broadcast;
            //return await _broadcastController.PrepareToStreamAsync(broadcast).ConfigureAwait(false);
        }

        public void TestShow(FfmpegParams ffmpegParams) => _streamingController.TestShow(ffmpegParams);
        public Task<bool> IsBroadcastReadyToLifeAsync(IBroadcast broadcast) => _broadcastController.IsBroadcastReadyToLifeAsync(broadcast);
        public async Task<IBroadcast> StartLiveStreamAsync(IBroadcast broadcast)
        {
            while (true)
            {
                try
                {
                    var result = await _broadcastController.StartLiveStreamAsync(broadcast).ConfigureAwait(false);
                    return result;
                }
                catch (Exception ex)
                {


                }
            }

        }

        public void StopStreaming() => _streamingController.StopStreaming();

    }
}
