using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Data.Repositories.Base;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;

namespace SpotifyGateway.Data.Repositories
{
    public class LoggerRepository : MongoDbRepository<Log>, ILoggerRepository
    {
        public LoggerRepository(IMongoClient mongoClient) : base(mongoClient, DatabaseConstants.MongoDbDatabaseName, DateTime.UtcNow.ToLogCollection())
        {
        }

        public override Log Insert(Log entity)
        {
            var collection = GetCollection();

            entity.Values = entity.Values.JsonToBson();

            collection.InsertOne(entity);

            return entity;
        }

        public override async Task DropCollectionAsync()
        {
            var date = DateTime.UtcNow.AddDays(-7).ToLogCollection();

            await DropCollectionAsync(date);
        }
    }
}