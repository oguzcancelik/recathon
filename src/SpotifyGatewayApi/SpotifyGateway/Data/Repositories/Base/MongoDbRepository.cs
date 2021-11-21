using System.Threading.Tasks;
using MongoDB.Driver;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Base.Abstractions;

namespace SpotifyGateway.Data.Repositories.Base
{
    public class MongoDbRepository<T> : IMongoDbRepository<T> where T : BaseMongoDbEntity
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _databaseName;
        private readonly string _collectionName;

        protected MongoDbRepository(IMongoClient mongoClient, string databaseName, string collectionName)
        {
            _mongoClient = mongoClient;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        protected IMongoDatabase GetDatabase()
        {
            return _mongoClient.GetDatabase(_databaseName);
        }

        protected IMongoCollection<T> GetCollection()
        {
            return GetDatabase().GetCollection<T>(_collectionName);
        }

        public virtual async Task DropCollectionAsync()
        {
            await Task.CompletedTask;
        }

        public virtual async Task DropCollectionAsync(string collectionName)
        {
            var database = GetDatabase();

            await database.DropCollectionAsync(collectionName);
        }

        public virtual T Insert(T entity)
        {
            var collection = GetCollection();

            collection.InsertOne(entity);

            return entity;
        }

        public async Task<T> InsertAsync(T entity)
        {
            var collection = GetCollection();

            await collection.InsertOneAsync(entity);

            return entity;
        }
    }
}