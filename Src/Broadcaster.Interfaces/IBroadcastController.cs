using System.Collections.Generic;
using System.Threading.Tasks;

namespace Broadcaster.Interfaces
{
    public interface IBroadcastController
    {
        Task<IBroadcast> InitializeBroadcastAsync(IBroadcast broadcast);
        Task<IBroadcast> ConnectToExistStreamAsync(IBroadcast broadcast);
        Task<List<IBroadcast>> GetBroadcastsAsync();
        Task<bool> DeleteBroadcastAsync(string broadcastId);
        Task<IBroadcast> GetLiveStreamAsync(IBroadcast broadcast);
        Task<bool> IsBroadcastReadyToLifeAsync(IBroadcast broadcast);
        Task<IBroadcast> PrepareToStreamAsync(IBroadcast broadcast);
        Task<IBroadcast> StartLiveStreamAsync(IBroadcast broadcast);
    }
}
