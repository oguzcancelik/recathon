using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;

namespace SpotifyGateway.Data.Repositories.Abstractions
{
    public interface ILoggerRepository : IMongoDbRepository<Log>
    {
    }
}