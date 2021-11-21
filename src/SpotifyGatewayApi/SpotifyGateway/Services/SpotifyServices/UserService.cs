using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Infrastructure.Mapping;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using SpotifyAPI.Web.Models;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Services.Abstractions;

namespace SpotifyGateway.Services.SpotifyServices
{
    public class UserService : IUserService
    {
        private readonly IAuthSettings _authSettings;
        private readonly ICredentialService _credentialService;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IRepository _repository;
        private readonly IGeneralSettings _generalSettings;
        private readonly ISpotifyUserContext _spotifyUserContext;
        private readonly ITokenService _tokenService;
        private readonly ILoggerService _loggerService;

        public UserService(
            IAuthSettings authSettings,
            ICredentialService credentialService,
            IRedisCacheProvider redisCacheProvider,
            IRepository repository,
            IGeneralSettings generalSettings,
            ISpotifyUserContext spotifyUserContext,
            ITokenService tokenService,
            ILoggerService loggerService
        )
        {
            _authSettings = authSettings;
            _credentialService = credentialService;
            _redisCacheProvider = redisCacheProvider;
            _repository = repository;
            _loggerService = loggerService;
            _generalSettings = generalSettings;
            _spotifyUserContext = spotifyUserContext;
            _tokenService = tokenService;
        }

        public async Task<string> GetUserIdBySessionGuidAsync(string sessionGuid)
        {
            var userId = await _redisCacheProvider.GetAsync(
                $"{RedisConstants.SessionGuidCache}{sessionGuid}",
                RedisConstants.AuthExpiryTime,
                async () =>
                {
                    var userSession = await _repository.QueryFirstOrDefaultAsync<UserSession>(QueryConstants.GetUserSessionQuery, new { SessionGuid = sessionGuid });

                    return userSession?.UserId;
                });

            return userId;
        }

        public async Task<User> GetUserAsync(string userId)
        {
            var user = await _redisCacheProvider.GetAsync(
                $"{RedisConstants.UserCache}{userId}",
                RedisConstants.AuthExpiryTime,
                async () => await _repository.QueryFirstOrDefaultAsync<User>(QueryConstants.GetUserQuery, new { Id = userId })
            );

            return user;
        }

        public async Task<User> GetAuthenticatedUserAsync(string userId)
        {
            var user = await GetUserAsync(userId);

            if (user == null)
            {
                return default;
            }

            var totalMinutes = (DateTime.UtcNow - user.UpdateTime).TotalMinutes;

            if (totalMinutes < SpotifyApiConstants.TokenTimeoutLimit)
            {
                return user;
            }

            var isLocked = await _redisCacheProvider.LockAsync($"{RedisConstants.UserTokenLock}{user.Id}", RedisConstants.UserTokenLockExpiryTime);

            if (!isLocked)
            {
                throw new CustomException(ErrorConstants.TooManyRequest, ErrorConstants.TooManyRequest, nameof(UserService), nameof(GetAuthenticatedUserAsync));
            }

            var credential = await _credentialService.GetByClientIdAsync(user.ClientId);

            if (credential == default)
            {
                await _redisCacheProvider.UnlockAsync($"{RedisConstants.UserTokenLock}{user.Id}");

                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(CredentialType), CredentialType.Auth },
                    { nameof(user.ClientId), user.ClientId }
                };

                throw new CustomException(
                    ErrorConstants.AuthCredentialNotFoundError,
                    (int)HttpStatusCode.Unauthorized,
                    ErrorConstants.UserAuthenticationError,
                    nameof(UserService),
                    nameof(GetAuthenticatedUserAsync),
                    logValues
                );
            }

            var token = await _tokenService.RefreshTokenAsync(credential, user.RefreshToken);

            user.AccessToken = token.AccessToken;
            user.UpdateTime = DateTime.UtcNow;

            await _repository.ExecuteAsync(QueryConstants.UpdateUserQuery, user);
            await _redisCacheProvider.UnlockAsync($"{RedisConstants.UserTokenLock}{user.Id}");
            await _redisCacheProvider.SetAsync($"{RedisConstants.UserCache}{user.Id}", user, RedisConstants.AuthExpiryTime);

            return user;
        }

        public async Task<BaseResponse<List<PlaylistResponse>>> GetCurrentUsersPlaylistsAsync()
        {
            var userId = _spotifyUserContext.UserId;
            var spotifyApiModel = _spotifyUserContext.Api;

            var response = new BaseResponse<List<PlaylistResponse>>();

            var playlists = await _redisCacheProvider.GetAsync<List<PlaylistResponse>>($"{RedisConstants.UserPlaylistsCache}{userId}");

            if (playlists is { Count: > 0 })
            {
                response.Result = playlists;

                return response;
            }

            const int limit = SpotifyApiConstants.UserPlaylistsLimit;

            var userPlaylists = await spotifyApiModel.GetUserPlaylistsAsync(userId, limit);

            if (userPlaylists.HasError() || userPlaylists.Items == null)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(userId), userId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                    { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                    { nameof(SpotifyEndpoint), SpotifyEndpoint.GetUserPlaylistsAsync }
                };

                throw new CustomException(userPlaylists.Error, ErrorConstants.UserPlaylistsError, nameof(UserService), nameof(GetCurrentUsersPlaylistsAsync), logValues);
            }

            for (var offset = limit; offset < userPlaylists.Total && offset < SpotifyApiConstants.MaxPlaylistCount; offset += limit)
            {
                var next = await spotifyApiModel.GetUserPlaylistsAsync(userId, limit, offset);

                if (next.HasError() || next.Items == null)
                {
                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { nameof(userId), userId },
                        { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                        { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                        { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                        { nameof(SpotifyEndpoint), SpotifyEndpoint.GetUserPlaylistsAsync },
                        { nameof(offset), offset },
                        { nameof(userPlaylists.Total), userPlaylists.Total }
                    };

                    await _loggerService.LogErrorAsync(next.Error, nameof(UserService), nameof(GetCurrentUsersPlaylistsAsync), logValues);
                }
                else
                {
                    userPlaylists.Items.AddRange(next.Items);
                }
            }

            playlists = userPlaylists.Items.ToResponse();

            var generatedPlaylists = playlists.Where(x => x.Name.StartsWith(ApplicationConstants.AppPrefix));

            playlists = playlists.Where(x => x.TrackCount >= ApplicationConstants.TrackCountLimit).ToList();

            playlists.InsertRange(0, PlaylistConstants.DefaultPlaylists);

            playlists.AddRange(generatedPlaylists);

            response.Result = playlists;

            await _redisCacheProvider.SetAsync($"{RedisConstants.UserPlaylistsCache}{userId}", playlists, RedisConstants.UserPlaylistsCacheExpiryTime);

            return response;
        }

        public async Task<Playlist> GetCurrentUsersSavedTracksAsync(string playlistId)
        {
            var playlist = new Playlist
            {
                Id = playlistId,
                IsCollaborative = false,
                IsPublic = false,
                IsSearchReduced = false,
                LastUpdated = DateTime.UtcNow,
                Name = PlaylistConstants.SavedTracksName,
                OwnerId = _spotifyUserContext.UserId,
                OwnerName = _spotifyUserContext.UserId,
                PlaylistType = PlaylistType.Saved,
                CreationTime = DateTime.UtcNow
            };

            var savedTracks = await _spotifyUserContext.Api.GetSavedTracksAsync(SpotifyApiConstants.UserSavedTracksLimit);

            if (savedTracks.HasError() || savedTracks.Items == null)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                    { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                    { nameof(SpotifyEndpoint), SpotifyEndpoint.GetSavedTracksAsync },
                    { nameof(playlistId), playlistId }
                };

                throw new CustomException(savedTracks.Error, ErrorConstants.UserSavedTracksError, nameof(UserService), nameof(GetCurrentUsersSavedTracksAsync), logValues);
            }

            var indexes = PlaylistHelpers.CalculateIndexes(savedTracks.Total, PlaylistType.Saved);

            if (indexes.Count > 0)
            {
                var tasks = indexes.Select(x => new { Index = x, Task = _spotifyUserContext.Api.GetSavedTracksAsync(SpotifyApiConstants.UserSavedTracksLimit, x) }).ToList();

                await Task.WhenAll(tasks.Select(x => x.Task));

                foreach (var task in tasks)
                {
                    var taskResult = task.Task.Result;
                    var index = task.Index;

                    if (taskResult.HasError() || taskResult.Items == null)
                    {
                        var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                        {
                            { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                            { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                            { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                            { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                            { nameof(SpotifyEndpoint), SpotifyEndpoint.GetSavedTracksAsync },
                            { nameof(playlistId), playlistId },
                            { nameof(indexes), indexes.ToJson() },
                            { nameof(index), index }
                        };

                        await _loggerService.LogErrorAsync(taskResult.Error, nameof(UserService), nameof(GetCurrentUsersSavedTracksAsync), logValues);
                    }
                    else
                    {
                        savedTracks.Items.AddRange(taskResult.Items);
                    }
                }
            }

            playlist.Tracks = savedTracks.Items.ToEntity();

            return playlist;
        }

        public async Task<Playlist> GetCurrentUsersTopTracksAsync(string playlistId)
        {
            var playlist = new Playlist
            {
                Id = playlistId,
                IsCollaborative = false,
                IsLockFailed = false,
                IsPublic = false,
                IsSearchReduced = false,
                LastUpdated = DateTime.UtcNow,
                Name = PlaylistConstants.TopTracksName,
                OwnerId = _spotifyUserContext.UserId,
                OwnerName = _spotifyUserContext.UserId,
                PlaylistType = PlaylistType.Top,
                CreationTime = DateTime.UtcNow
            };

            var isLocked = await _redisCacheProvider.LockAsync($"{RedisConstants.UserTopTracksLock}{_spotifyUserContext.UserId}", RedisConstants.UserTopTracksLockExpiryTime);

            if (!isLocked)
            {
                playlist.IsLockFailed = true;

                return playlist;
            }

            var topTracks = new List<FullTrack>();

            var tasks = PlaylistConstants.TimeRanges
                .Select(x => new { Range = x, Task = _spotifyUserContext.Api.GetUsersTopTracksAsync(x, SpotifyApiConstants.UserTopTracksLimit) })
                .ToList();

            await Task.WhenAll(tasks.Select(x => x.Task));

            foreach (var task in tasks)
            {
                var taskResult = task.Task.Result;
                var range = task.Range;

                if (taskResult.HasError() || taskResult.Items == null)
                {
                    var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                    {
                        { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                        { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                        { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                        { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                        { nameof(SpotifyEndpoint), SpotifyEndpoint.GetUsersTopTracksAsync },
                        { nameof(playlistId), playlistId },
                        { nameof(range), range }
                    };

                    await _loggerService.LogErrorAsync(taskResult.Error, nameof(UserService), nameof(GetCurrentUsersTopTracksAsync), logValues);

                    await _redisCacheProvider.UnlockAsync($"{RedisConstants.UserTopTracksLock}{_spotifyUserContext.UserId}");
                }
                else
                {
                    topTracks.AddRange(taskResult.Items);
                }
            }

            playlist.Tracks = topTracks.ToEntity();

            return playlist;
        }

        public async Task<Playlist> GetCurrentUsersRecentTracksAsync(string playlistId)
        {
            var playlist = new Playlist
            {
                Id = playlistId,
                IsCollaborative = false,
                IsLockFailed = false,
                IsPublic = false,
                IsSearchReduced = false,
                LastUpdated = DateTime.UtcNow,
                Name = PlaylistConstants.RecentTracksName,
                OwnerId = _spotifyUserContext.UserId,
                OwnerName = _spotifyUserContext.UserId,
                PlaylistType = PlaylistType.Recent,
                CreationTime = DateTime.UtcNow
            };

            var isLocked = await _redisCacheProvider.LockAsync($"{RedisConstants.UserRecentTracksLock}{_spotifyUserContext.UserId}", RedisConstants.UserRecentTracksLockExpiryTime);

            if (!isLocked)
            {
                playlist.IsLockFailed = true;

                return playlist;
            }

            var recentTracks = await _spotifyUserContext.Api.GetUsersRecentlyPlayedTracksAsync(SpotifyApiConstants.UserRecentTracksLimit);

            if (recentTracks.HasError() || recentTracks.Items == null)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(_spotifyUserContext.UserId), _spotifyUserContext.UserId },
                    { nameof(_spotifyUserContext.SessionGuid), _spotifyUserContext.SessionGuid },
                    { nameof(_spotifyUserContext.User.ClientId), _spotifyUserContext.User.ClientId },
                    { nameof(_spotifyUserContext.CredentialType), _spotifyUserContext.CredentialType },
                    { nameof(SpotifyEndpoint), SpotifyEndpoint.GetUsersRecentlyPlayedTracksAsync },
                    { nameof(playlistId), playlistId }
                };

                await _redisCacheProvider.UnlockAsync($"{RedisConstants.UserRecentTracksLock}{_spotifyUserContext.UserId}");

                throw new CustomException(recentTracks.Error, ErrorConstants.UserRecentTracksError, nameof(UserService), nameof(GetCurrentUsersRecentTracksAsync), logValues);
            }

            playlist.Tracks = recentTracks.Items.ToEntity();

            return playlist;
        }

        public async Task<string> GetUserTokenAsync(string userId)
        {
            var sessionGuid = await _repository.QueryFirstOrDefaultAsync<string>(QueryConstants.GetUserSessionByUserId, new { UserId = userId });

            return sessionGuid?.Encrypt(_authSettings.SessionKey);
        }
    }
}