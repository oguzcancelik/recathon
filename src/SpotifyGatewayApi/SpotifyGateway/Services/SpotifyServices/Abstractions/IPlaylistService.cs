using System.Threading.Tasks;
using SpotifyGateway.Models.Requests;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Models.Search;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface IPlaylistService
    {
        Task<BaseResponse<GeneratedPlaylistResponse>> GeneratePlaylistAsync(GeneratePlaylistRequest request);

        Task SearchAsync(SearchModel searchModel);
    }
}