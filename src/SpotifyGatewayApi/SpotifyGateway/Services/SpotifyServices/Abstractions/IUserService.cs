using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Models.Responses;

namespace SpotifyGateway.Services.SpotifyServices.Abstractions
{
    public interface IUserService
    {
        Task<string> GetUserIdBySessionGuidAsync(string sessionGuid);

        Task<User> GetUserAsync(string userId);

        Task<User> GetAuthenticatedUserAsync(string userId);

        Task<BaseResponse<List<PlaylistResponse>>> GetCurrentUsersPlaylistsAsync();

        Task<Playlist> GetCurrentUsersSavedTracksAsync(string playlistId);

        Task<Playlist> GetCurrentUsersTopTracksAsync(string playlistId);

        Task<Playlist> GetCurrentUsersRecentTracksAsync(string playlistId);

        Task<string> GetUserTokenAsync(string userId);
    }
}