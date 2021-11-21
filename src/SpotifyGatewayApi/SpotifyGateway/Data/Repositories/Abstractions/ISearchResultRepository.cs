using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;

namespace SpotifyGateway.Data.Repositories.Abstractions
{
    public interface ISearchResultRepository : IMongoDbRepository<SearchResult>
    {
    }
}