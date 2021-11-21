using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface ITrackService
    {
        Task<List<Track>> GetTrackAudioFeaturesAsync(List<Track> tracks, bool isPlaylistTracks = false);

        Task<List<Track>> GetMultipleTracksAsync(List<string> trackIds);
    }
}