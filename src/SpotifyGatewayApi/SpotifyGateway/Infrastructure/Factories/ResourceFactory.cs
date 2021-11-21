using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Factories.Abstractions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.Resources;
using SpotifyGateway.Models.Resources.Abstractions;

namespace SpotifyGateway.Infrastructure.Factories
{
    public class ResourceFactory<T> : IResourceFactory<T> where T : IResources
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public ResourceFactory(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public T Value
        {
            get
            {
                var language = _httpContextAccessor.HttpContext != null
                               && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(HttpConstants.Language, out var header)
                               && header.ToString().TryParse(out Language lang)
                    ? lang
                    : Language.English;

                var services = _serviceProvider.GetServices<T>();

                return services.FirstOrDefault(x => x.Language == language);
            }
        }
    }

    public class ResourceFactory : IResourceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ResourceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<IResources> GetResources(ResourcesClass resourcesClass)
        {
            var resourcesList = resourcesClass switch
            {
                ResourcesClass.All => _serviceProvider.GetServices<IResources>(),
                ResourcesClass.Error => _serviceProvider.GetServices<ErrorResources>(),
                _ => null
            };

            return resourcesList?.ToList();
        }
    }
}