using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface IBrowseService
    {
        Task<BaseResponse<List<CategoryResponse>>> GetCategoryPlaylistsAsync();

        Task GetCategoryPlaylistsFromSpotifyApiAsync();
    }
}