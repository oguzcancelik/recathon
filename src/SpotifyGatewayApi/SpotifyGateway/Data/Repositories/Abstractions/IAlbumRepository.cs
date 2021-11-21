using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Models.Search;

namespace SpotifyGateway.Data.Repositories.Abstractions
{
    public interface IAlbumRepository : IRepository
    {
        Task<List<Album>> GetAlbumsForTrackSearchAsync(SearchModel searchModel);
    }
}