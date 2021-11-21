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
    public class SearchResultRepository : MongoDbRepository<SearchResult>, ISearchResultRepository
    {
        public SearchResultRepository(IMongoClient mongoClient) : base(mongoClient, DatabaseConstants.MongoDbDatabaseName, DateTime.UtcNow.ToSearchResultCollection())
        {
        }

        public override async Task DropCollectionAsync()
        {
            var date = DateTime.UtcNow.AddDays(-7).ToSearchResultCollection();

            await DropCollectionAsync(date);
        }
    }
}