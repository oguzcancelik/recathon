namespace SpotifyGateway.Infrastructure.Configuration.Options
{
    public class HangfireOptions
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public int SqlQueueWorkerCount { get; set; }
    }
}