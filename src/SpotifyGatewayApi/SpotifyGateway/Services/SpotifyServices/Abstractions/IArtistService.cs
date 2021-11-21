using System.Threading.Tasks;
using SpotifyGateway.Models.Search;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface IArtistService
    {
        Task<AlbumSearchResult> AlbumSearchAsync(SearchModel searchModel);
    }
}