namespace SpotifyGateway.Data.Entities
{
    public class Credential
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public string RedirectUri { get; set; }

        public string RedirectDeepLink { get; set; }

        public bool IsActive { get; set; }

        public string[] Type { get; set; }

        public int UsageCount { get; set; }
    }
}