using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Models;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Exceptions;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Responses;
using SpotifyGateway.Services.Abstractions;
using SpotifyGateway.Services.LogServices.Abstractions;

namespace SpotifyGateway.Services
{
    public class TokenService : ITokenService
    {
        private readonly GeneralOptions _generalOptions;
        private readonly IAuthSettings _authSettings;
        private readonly IRedisCacheProvider _redisCacheProvider;
        private readonly ISpotifyUserContext _spotifyUserContext;
        private readonly ILoggerService _loggerService;

        public TokenService(
            IAuthSettings authSettings,
            IOptions<GeneralOptions> generalOptions,
            IRedisCacheProvider redisCacheProvider,
            ISpotifyUserContext spotifyUserContext,
            ILoggerService loggerService
        )
        {
            _generalOptions = generalOptions.Value;
            _authSettings = authSettings;
            _redisCacheProvider = redisCacheProvider;
            _spotifyUserContext = spotifyUserContext;
            _loggerService = loggerService;
        }

        public async Task<BaseResponse<TokenResponse>> GenerateTokenAsync()
        {
            var response = new BaseResponse<TokenResponse>();

            var token = Guid.NewGuid().ToString();
            await _redisCacheProvider.InsertKeyAsync($"{RedisConstants.TokenCache}{token}", RedisConstants.TokenCacheExpiryTime);

            var keyExists = await _redisCacheProvider.CheckExistsAndDeleteAsync($"{RedisConstants.AdFreeUser}{_spotifyUserContext.UserId}");

            response.Result = new TokenResponse
            {
                Token = token,
                ShowAd = !keyExists 
            };

            return response;
        }

        public async Task<bool> DecryptTokenAsync(string token)
        {
            try
            {
                var decryptedToken = token.Decrypt(_authSettings.TokenKey);

                var tokenExists = await _redisCacheProvider.CheckExistsAndDeleteAsync($"{RedisConstants.TokenCache}{decryptedToken}");

                return tokenExists;
            }
            catch (Exception e)
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(token), token }
                };

                await _loggerService.LogErrorAsync(e.Message, nameof(TokenService), nameof(DecryptTokenAsync), logValues, e.StackTrace);

                return false;
            }
        }

        public async Task<Token> ExchangeCodeAsync(Credential credential, string code, DeviceType deviceType)
        {
            var redirectUri = deviceType switch
            {
                DeviceType.Browser => $"{_generalOptions.BaseUrl}{credential.RedirectUri}",
                DeviceType.Mobile => credential.RedirectDeepLink,
                _ => default
            };

            var collection = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "authorization_code"),
                new("code", code),
                new("redirect_uri", redirectUri)
            };

            return await GetTokenAsync(collection, credential);
        }

        public async Task<Token> RefreshTokenAsync(Credential credential, string refreshToken)
        {
            var collection = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "refresh_token"),
                new("refresh_token", refreshToken)
            };

            return await GetTokenAsync(collection, credential);
        }

        private async Task<Token> GetTokenAsync(IEnumerable<KeyValuePair<string, string>> collection, Credential credential)
        {
            HttpContent content = new FormUrlEncodedContent(collection);

            var handler = ProxyConfig.CreateClientHandler();
            var client = new HttpClient(handler);

            var clientInformation = $"{credential.ClientId}:{credential.ClientSecret}".ToBase64String();
            var authorizationHeader = $"Basic {clientInformation}";

            client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

            var response = await client.PostAsync("https://accounts.spotify.com/api/token", content);
            var result = await response.Content.ReadAsStringAsync();

            if (!result.TryGetFromJson(out Token token) || !string.IsNullOrEmpty(token.Error) || !string.IsNullOrEmpty(token.ErrorDescription))
            {
                var logValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(CredentialType), CredentialType.Auth },
                    { nameof(credential.ClientId), credential.ClientId },
                    { nameof(token), token }
                };

                throw new CustomException(token.ToCustomError((int)HttpStatusCode.Unauthorized, ErrorConstants.UserAuthenticationError), nameof(TokenService), nameof(GetTokenAsync), logValues);
            }

            return token;
        }
    }
}