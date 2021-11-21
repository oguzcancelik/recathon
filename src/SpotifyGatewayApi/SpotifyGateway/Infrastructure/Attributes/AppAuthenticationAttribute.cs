using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Services.SpotifyServices.Abstractions;

namespace SpotifyGateway.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AppAuthenticationAttribute : Attribute, IAsyncActionFilter
    {
        private readonly ICredentialService _credentialService;
        private readonly ISpotifyAppContext _spotifyAppContext;

        public AppAuthenticationAttribute(
            ICredentialService credentialService,
            ISpotifyAppContext spotifyAppContext
        )
        {
            _credentialService = credentialService;
            _spotifyAppContext = spotifyAppContext;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await SetAppContext();

            await next();
        }

        private async Task SetAppContext()
        {
            var credential = await _credentialService.GetByUsageTypeAsync();

            if (credential != default)
            {
                _spotifyAppContext.Set(credential.ClientId, credential.AccessToken, credential.TokenType);
            }
        }
    }
}