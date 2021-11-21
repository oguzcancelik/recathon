using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Mapping;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.MessageModels;
using SpotifyGateway.Models.Requests;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using SpotifyAPI.Web;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Models.Responses.Server.Auth;
using SpotifyGateway.ServiceClients.MessageServiceClients.Abstractions;
using SpotifyGateway.Services.Abstractions;

namespace SpotifyGateway.Services.SpotifyServices
{
    public class AuthService : IAuthService
    {
        private readonly ICredentialService _credentialService;
        private readonly IMessageServiceClient _messageServiceClient;
        private readonly GeneralOptions _generalOptions;
        private readonly IAuthSettings _authSettings;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly IRepository _repository;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly ITelegramSettings _telegramSettings;
        private readonly ILoggerService _loggerService;

        public AuthService(
            IAuthSettings authSettings,
            ICredentialService credentialService,
            IMessageServiceClient messageServiceClient,
            IOptions<GeneralOptions> generalOptions,
            IRedisCacheProvider redisCacheProvider,
            IRepository repository,
            ITokenService tokenService,
            IUserService userService,
            ITelegramSettings telegramSettings,
            ILoggerService loggerService
        )
        {
            _authSettings = authSettings;
            _redisCacheProvider = redisCacheProvider;
            _credentialService = credentialService;
            _messageServiceClient = messageServiceClient;
            _generalOptions = generalOptions.Value;
            _repository = repository;
            _tokenService = tokenService;
            _userService = userService;
            _telegramSettings = telegramSettings;
            _loggerService = loggerService;
        }

        public async Task<string> GetAuthUrlAsync()
        {
            var credential = await _credentialService.GetByUsageCountAsync();

            var builder = new StringBuilder("https://accounts.spotify.com/authorize/?");
            builder.Append($"client_id={credential.ClientId}");
            builder.Append("&response_type=code");
            builder.Append($"&redirect_uri={_generalOptions.BaseUrl}{credential.RedirectUri}");
            builder.Append($"&scope={SpotifyApiConstants.Scopes.GetStringAttribute(" ")}");
            builder.Append($"&state={credential.ClientId}");

            return Uri.EscapeUriString(builder.ToString());
        }

        public async Task<BaseResponse<AuthInfoResponse>> GetAuthInfoAsync()
        {
            var credential = await _credentialService.GetByUsageCountAsync();

            var response = new BaseResponse<AuthInfoResponse>
            {
                Result = new AuthInfoResponse
                {
                    ClientId = credential.ClientId,
                    Scope = SpotifyApiConstants.ScopeList,
                    RedirectUri = credential.RedirectDeepLink
                }
            };

            return response;
        }

        public async Task<BaseResponse<string>> AuthenticateUserAsync(SpotifyAuthorizationRequest request)
        {
            var response = new BaseResponse<string>();

            if (string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.State) || !string.IsNullOrEmpty(request.Error))
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(CredentialType), CredentialType.Auth },
                    { nameof(request.Code), request.Code },
                    { nameof(request.ClientId), request.ClientId },
                    { nameof(request.Error), request.Error },
                    { nameof(request.DeviceType), request.DeviceType }
                };

                await _loggerService.LogWarningAsync(request.Error, nameof(AuthService), nameof(AuthenticateUserAsync), logValues);

                response.Errors.Add(new CustomError(ErrorConstants.UserAuthenticationError));

                return response;
            }

            var clientId = request.ClientId;

            if (clientId == default)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(CredentialType), CredentialType.Auth },
                    { nameof(request.DeviceType), request.DeviceType }
                };

                await _loggerService.LogWarningAsync(ErrorConstants.ClientIdNotFoundError, nameof(AuthService), nameof(AuthenticateUserAsync), logValues);

                response.Errors.Add(new CustomError(ErrorConstants.UserAuthenticationError));

                return response;
            }

            var credential = await _credentialService.GetByClientIdAsync(clientId);

            if (credential == default)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(CredentialType), CredentialType.Auth },
                    { nameof(clientId), clientId },
                    { nameof(request.DeviceType), request.DeviceType }
                };

                await _loggerService.LogWarningAsync(ErrorConstants.AuthCredentialNotFoundError, nameof(AuthService), nameof(AuthenticateUserAsync), logValues);

                response.Errors.Add(new CustomError(ErrorConstants.UserAuthenticationError));

                return response;
            }

            var token = await _tokenService.ExchangeCodeAsync(credential, request.Code, request.DeviceType);

            var spotifyApi = new SpotifyWebAPI
            {
                AccessToken = token.AccessToken,
                TokenType = token.TokenType
            };

            var profile = await spotifyApi.GetPrivateProfileAsync();

            var user = await _userService.GetUserAsync(profile.Id);

            if (user != default)
            {
                user.AccessToken = token.AccessToken;
                user.RefreshToken = token.RefreshToken;
                user.DisplayName = profile.DisplayName;
                user.UpdateTime = DateTime.UtcNow;

                if (user.ClientId != credential.ClientId)
                {
                    await _credentialService.UpdateUsageCountAsync(credential.ClientId, UsageCount.Increase);
                    await _credentialService.UpdateUsageCountAsync(user.ClientId, UsageCount.Decrease);

                    user.ClientId = credential.ClientId;
                }

                await _repository.ExecuteAsync(QueryConstants.UpdateUserQuery, user);
            }
            else
            {
                user = token.ToEntity(profile, credential.ClientId);

                await _repository.ExecuteAsync(QueryConstants.InsertUserQuery, user);

                await _credentialService.UpdateUsageCountAsync(credential.ClientId, UsageCount.Increase);

                await _redisCacheProvider.SetAsync($"{RedisConstants.AdFreeUser}{user.Id}", user.Id, RedisConstants.AdFreeUserKeyExpiryTime);

                if (_telegramSettings.IsNewUserAuthenticatedMessagesEnabled)
                {
                    var userAuthenticatedMessage = new UserAuthenticatedMessage
                    {
                        ClassName = nameof(AuthenticateUserAsync),
                        DisplayName = profile.DisplayName,
                        UserId = profile.Id,
                        Subject = MessageSubjectType.NewUserAuthenticated
                    };

                    await _messageServiceClient.SendMessageAsync(userAuthenticatedMessage);
                }
            }

            var sessionGuid = Guid.NewGuid().ToString();

            var userSession = new UserSession
            {
                UserId = profile.Id,
                SessionGuid = sessionGuid,
                CreationTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow
            };

            await _repository.ExecuteAsync(QueryConstants.InsertUserSessionQuery, userSession);

            await CacheModels(user, sessionGuid);

            response.Result = sessionGuid.Encrypt(_authSettings.SessionKey);

            return response;
        }

        private async Task CacheModels(User user, string sessionGuid)
        {
            await _redisCacheProvider.SetAsync($"{RedisConstants.SessionGuidCache}{sessionGuid}", user.Id, RedisConstants.AuthExpiryTime);

            await _redisCacheProvider.SetAsync($"{RedisConstants.UserCache}{user.Id}", user, RedisConstants.AuthExpiryTime);
        }
    }
}