namespace SpotifyGateway.Infrastructure.Configuration.Options
{
    public class DatabaseOptions
    {
        public string DefaultConnection { get; set; }

        public string MongoDbConnection { get; set; }
    }
}