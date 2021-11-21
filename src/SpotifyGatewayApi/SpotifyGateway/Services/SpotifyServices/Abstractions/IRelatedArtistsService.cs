using System.Threading.Tasks;
using SpotifyGateway.Models.Search;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface IRelatedArtistsService
    {
        Task<RelatedArtistSearchResult> RelatedArtistsSearchAsync(SearchModel searchModel);
    }
}