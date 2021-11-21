using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Services.Abstractions
{
    public interface IRecommendationHistoryService
    {
        Task<IEnumerable<string>> GetLastRecommendedTrackIdsAsync(string userId, string playlistId);

        Task<List<string>> GetAlternativeRecommendedTrackIdsAsync(string userId, string playlistId);

        Task InsertAsync(RecommendationHistory recommendationHistory);

        Task<BaseResponse<List<GeneratedPlaylistResponse>>> GetUsersRecommendationHistoryAsync();
    }
}