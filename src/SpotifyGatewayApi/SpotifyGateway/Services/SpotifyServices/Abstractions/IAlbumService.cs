using System.Threading.Tasks;
using SpotifyGateway.Models.Search;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface IAlbumService
    {
        Task<TrackSearchResult> TrackSearchAsync(SearchModel searchModel);
    }
}