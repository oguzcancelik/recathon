using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Mapping;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using SpotifyAPI.Web.Models;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;

namespace SpotifyGateway.Services.SpotifyServices
{
    public class TrackService : ITrackService
    {
        private readonly IRepository _repository;
        private readonly ISpotifyAppContext _spotifyAppContext;
        private readonly ILoggerService _loggerService;

        public TrackService(
            IRepository repository,
            ISpotifyAppContext spotifyAppContext,
            ILoggerService loggerService
        )
        {
            _repository = repository;
            _spotifyAppContext = spotifyAppContext;
            _loggerService = loggerService;
        }

        public async Task<List<Track>> GetTrackAudioFeaturesAsync(List<Track> tracks, bool isPlaylistTracks = false)
        {
            const int limit = SpotifyApiConstants.TrackAudioFeaturesLimit;

            var groupedTracks = new List<List<Track>>();

            if (isPlaylistTracks)
            {
                for (var i = 0; i < tracks.Count; i += limit)
                {
                    groupedTracks.Add(tracks.Skip(i).Take(limit).ToList());
                }
            }
            else
            {
                var trackCountGroups = tracks
                    .GroupBy(x => x.AlbumId)
                    .Select(x => new {x.First().AlbumId, Count = x.Count()})
                    .Where(x => x.Count <= limit)
                    .OrderByDescending(x => x.Count)
                    .ToList();

                while (trackCountGroups.Count > 0)
                {
                    var currentCount = 0;
                    var albumIds = new List<string>();

                    while (currentCount <= limit)
                    {
                        var matching = trackCountGroups.FirstOrDefault(x => x.Count <= limit - currentCount);

                        if (matching != default)
                        {
                            currentCount += matching.Count;
                            albumIds.Add(matching.AlbumId);
                            trackCountGroups.Remove(matching);
                        }
                        else
                        {
                            break;
                        }
                    }

                    groupedTracks.Add(tracks.Where(x => albumIds.Contains(x.AlbumId)).ToList());
                }
            }

            var tasks = groupedTracks
                .Select(x =>
                {
                    var trackIds = x.Select(y => y.Id).ToList();
                    var albumIds = x.Select(y => y.AlbumId).Distinct().ToList();

                    return new
                    {
                        AlbumIds = albumIds,
                        Task = _spotifyAppContext.Api.GetSeveralAudioFeaturesAsync(trackIds)
                    };
                })
                .ToList();

            await Task.WhenAll(tasks.Select(x => x.Task));

            var audioFeatures = new List<AudioFeatures>();

            foreach (var task in tasks)
            {
                var taskResult = task.Task.Result;
                var albumIds = task.AlbumIds;

                if (taskResult.HasError() || taskResult.AudioFeatures == null)
                {
                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        {nameof(SpotifyEndpoint), SpotifyEndpoint.GetSeveralAudioFeaturesAsync},
                        {nameof(_spotifyAppContext.ClientId), _spotifyAppContext.ClientId},
                        {nameof(_spotifyAppContext.CredentialType), _spotifyAppContext.CredentialType},
                        {nameof(albumIds), albumIds}
                    };

                    tracks.RemoveAll(x => albumIds.Contains(x.AlbumId));

                    await _loggerService.LogErrorAsync(taskResult.Error, nameof(TrackService), nameof(GetTrackAudioFeaturesAsync), logValues);

                    continue;
                }

                audioFeatures.AddRange(taskResult.AudioFeatures.Select(x => x != null && !x.HasError() ? x : new AudioFeatures()));
            }

            tracks = tracks.MapAudioFeatures(audioFeatures);

            return tracks;
        }

        public async Task<List<Track>> GetMultipleTracksAsync(List<string> trackIds)
        {
            var tracks = await _repository.QueryAsync<Track>(QueryConstants.GetMultipleTracksQuery, new {Ids = trackIds});

            return tracks;
        }
    }
}