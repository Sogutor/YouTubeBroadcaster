using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Broadcaster.Interfaces;
using Broadcaster.Interfaces.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Broadcaster.YouTube
{


    public class YouTubeBroadcastController : IBroadcastController
    {
        private readonly IBroadcastCreator _broadcastCreator;
        private YouTubeService _service;

        public YouTubeBroadcastController(IBroadcastCreator broadcastCreator)
        {
            _broadcastCreator = broadcastCreator;
            LoginAsync().Wait();
        }

        private static string s_clientSecrets = @"{""installed"":{""client_id"":""here will be your client id"",""project_id"":""here will be your project id"",""auth_uri"":""https://accounts.google.com/o/oauth2/auth"",""token_uri"":""https://accounts.google.com/o/oauth2/token"",""auth_provider_x509_cert_url"":""https://www.googleapis.com/oauth2/v1/certs"",""client_secret"":""here will be your secret"",""redirect_uris"":[""urn:ietf:wg:oauth:2.0:oob"",""http://localhost""]}}";
        private async Task LoginAsync()
        {
            UserCredential credentials;
            var stream = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(stream))
            {
                await sw.WriteLineAsync(s_clientSecrets).ConfigureAwait(false);
                await sw.FlushAsync().ConfigureAwait(false);
                stream.Position = 0;
                credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.Load(stream).Secrets,
                new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeForceSsl },
                "YoutubeTranslations", CancellationToken.None, new FileDataStore(GetType().ToString())).ConfigureAwait(false);
            }

            if (credentials == null)
                throw new Exception("Authorization failed");

            _service = new YouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = "YoutubeTranslations"
            });
        }
        public async Task<IBroadcast> InitializeBroadcastAsync(IBroadcast broadcast)
        {
            var youTubeBroadcast = (IYouTubeBroadcast)_broadcastCreator.Create(broadcast);
            var query = _service.LiveBroadcasts.Insert(youTubeBroadcast.LiveBroadcast, "id, snippet, contentDetails, status");
            LiveBroadcast liveBroadcast = await query.ExecuteAsync().ConfigureAwait(false);
            LiveStream liveStream = await CreateLiveStreamAsync(broadcast).ConfigureAwait(false);
            var youTubeBroadcaster = new YouTubeBroadcastAdapter(await BindBroadcastToStreamAsync(liveBroadcast, liveStream).ConfigureAwait(false));
            youTubeBroadcaster.BindStream(liveStream);
            return youTubeBroadcaster;
        }
        private Task<LiveStream> CreateLiveStreamAsync(IBroadcast broadcast)
        {
            var stream = new LiveStream
            {
                Cdn = new CdnSettings
                {
                    Resolution = broadcast.Resolution.GetDescription(),
                    IngestionType = "rtmp",
                    FrameRate = broadcast.FrameRate.GetDescription()
                },
                Snippet = new LiveStreamSnippet
                {
                    Title = broadcast.Title,
                    Description = broadcast.Description
                },
                ContentDetails = new LiveStreamContentDetails { IsReusable = true }
            };
            var query = _service.LiveStreams.Insert(stream, "id, snippet, cdn, status, contentDetails");
            return query.ExecuteAsync();
        }

        public async Task<IBroadcast> ConnectToExistStreamAsync(IBroadcast broadcast)
        {
            var query = _service.LiveStreams.List("id, snippet, cdn, status, contentDetails");
            query.Id = ((IYouTubeBroadcast)broadcast).LiveBroadcast.Id;

            var result = await query.ExecuteAsync().ConfigureAwait(false);
            var stream = result.Items.FirstOrDefault();
            if (stream == null) throw new InvalidOperationException("stream can't find");
            ((IYouTubeBroadcast)broadcast).BindStream(stream);
            return broadcast;
        }
        private Task<LiveBroadcast> BindBroadcastToStreamAsync(LiveBroadcast broadcast, LiveStream stream)
        {
            var query = _service.LiveBroadcasts.Bind(broadcast.Id, "id, snippet");
            query.StreamId = stream.Id;
            return query.ExecuteAsync();
        }

        public async Task<bool> DeleteBroadcastAsync(string broadcastId)
        {
            var query = _service.LiveBroadcasts.Delete(broadcastId);
            var result = await query.ExecuteAsync().ConfigureAwait(false);
            return string.IsNullOrEmpty(result);
        }
        public async Task<List<IBroadcast>> GetBroadcastsAsync()
        {
            var query = _service.LiveBroadcasts.List("id, contentDetails, snippet, status");
            query.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Upcoming;
            var data = await query.ExecuteAsync().ConfigureAwait(false);
            return data.Items.Select(broadcast => (IBroadcast)new YouTubeBroadcastAdapter(broadcast)).ToList();
        }
        public async Task<IBroadcast> GetLiveStreamAsync(IBroadcast broadcast)
        {
            var currentBroadcat = broadcast as YouTubeBroadcastAdapter;
            if (currentBroadcat == null) throw new NotSupportedException(nameof(broadcast));
            var query = _service.LiveStreams.List("id, snippet, cdn, status, contentDetails");
            query.Id = currentBroadcat.LiveBroadcast.ContentDetails.BoundStreamId;
            var result = await query.ExecuteAsync().ConfigureAwait(false);
            currentBroadcat.BindStream(result.Items.FirstOrDefault());
            return currentBroadcat;
        }
        private async Task<LiveBroadcast> GetBroadcastAsync(string Id)
        {
            var query = _service.LiveBroadcasts.List("id, contentDetails, snippet, status");
            query.Id = Id;
            var result = await query.ExecuteAsync().ConfigureAwait(false);
            return result.Items.FirstOrDefault();

        }

        private async Task<IBroadcast> ChangeStatusAsync(string id, LiveBroadcastsResource.TransitionRequest.BroadcastStatusEnum status)
        {
            var query = _service.LiveBroadcasts.Transition(status, id, "id, snippet, contentDetails, status");
            try
            {
                var result = await query.ExecuteAsync().ConfigureAwait(false);
                return new YouTubeBroadcastAdapter(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task<IBroadcast> PrepareToStreamAsync(IBroadcast broadcast)
        {
            try
            {
                broadcast = await GetLiveStreamAsync(broadcast).ConfigureAwait(false);
                broadcast = await ChangeStatusAsync(broadcast.Id, LiveBroadcastsResource.TransitionRequest.BroadcastStatusEnum.Testing).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                //TODO log
            }
            return broadcast;
        }

        public Task<IBroadcast> StartLiveStreamAsync(IBroadcast broadcast)
        {
            return ChangeStatusAsync(broadcast.Id, LiveBroadcastsResource.TransitionRequest.BroadcastStatusEnum.Live);
        }
        public async Task<bool> IsBroadcastReadyToLifeAsync(IBroadcast broadcast)
        {
            var liveBroadcast = await GetBroadcastAsync(broadcast.Id).ConfigureAwait(false);
            if (liveBroadcast.Status.LifeCycleStatus == "ready") await PrepareToStreamAsync(broadcast).ConfigureAwait(false);
            return liveBroadcast != null && liveBroadcast.Status.LifeCycleStatus == "testing";
        }
    }
}
