using SpotifyGateway.Models.Cache;
using SpotifyGateway.Models.Requests;

namespace SpotifyGateway.Infrastructure.Mapping
{
    public static class WorkerMapper
    {
        public static SearchQueueModel ToQueueModel(this GeneratePlaylistRequest request)
        {
            return new SearchQueueModel
            {
                PlaylistId = request.PlaylistId,
                GenerateType = request.GenerateType
            };
        }
    }
}