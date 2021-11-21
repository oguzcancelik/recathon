using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Data.Repositories.Abstractions
{
    public interface IPlaylistRepository : IRepository
    {
        Task<List<RecommendedTrackResponse>> GetRecommendedTrackInformationAsync(List<string> trackIds);
    }
}