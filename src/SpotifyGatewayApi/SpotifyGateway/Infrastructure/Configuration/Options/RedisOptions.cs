namespace SpotifyGateway.Infrastructure.Configuration.Options
{
    public class RedisOptions
    {
        public string Connection { get; set; }

        public int HashDb { get; set; }

        public int QueueDb { get; set; }

        public int LockDb { get; set; }
    }
}