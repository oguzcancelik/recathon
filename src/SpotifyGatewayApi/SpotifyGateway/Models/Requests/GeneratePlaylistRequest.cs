using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.Requests
{
    public class GeneratePlaylistRequest
    {
        public string PlaylistId { get; set; }

        public string EncryptedToken { get; set; }

        public bool ShowAd { get; set; }

        public PlaylistType PlaylistType { get; set; }

        public GenerateType GenerateType { get; set; }

        public PlaylistSource PlaylistSource { get; set; }
    }
}