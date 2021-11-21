using System.Threading.Tasks;

namespace SpotifyGateway.Data.Repositories.Base.Abstractions
{
    public interface IMongoDbRepository<T>
    {
        Task DropCollectionAsync();

        Task DropCollectionAsync(string collectionName);

        T Insert(T entity);

        Task<T> InsertAsync(T entity);
    }
}