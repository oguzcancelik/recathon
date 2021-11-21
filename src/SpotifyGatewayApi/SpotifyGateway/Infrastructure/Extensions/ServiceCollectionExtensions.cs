using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Dapper.FluentMap;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Polly;
using SpotifyGateway.Caching.Providers;
using SpotifyGateway.Caching.Providers.Abstraction;
using SpotifyGateway.Data.Entities;
using SpotifyGateway.Data.EntityMappers;
using SpotifyGateway.Data.Repositories;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Data.Repositories.Base;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Api;
using SpotifyGateway.Infrastructure.Api.Abstractions;
using SpotifyGateway.Infrastructure.Attributes;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Configuration.Settings;
using SpotifyGateway.Infrastructure.Configuration.Settings.Abstraction;
using SpotifyGateway.Infrastructure.Contexts;
using SpotifyGateway.Infrastructure.Contexts.Abstractions;
using SpotifyGateway.Infrastructure.Factories;
using SpotifyGateway.Infrastructure.Factories.Abstractions;
using SpotifyGateway.Infrastructure.Filters;
using SpotifyGateway.Infrastructure.Lazy;
using SpotifyGateway.Managers;
using SpotifyGateway.Managers.Abstractions;
using SpotifyGateway.Models.Enums;
using SpotifyGateway.Models.FilterModels;
using SpotifyGateway.Models.FilterModels.Base;
using SpotifyGateway.Models.Resources;
using SpotifyGateway.Models.Resources.Abstractions;
using SpotifyGateway.ServiceClients;
using SpotifyGateway.ServiceClients.Abstractions;
using SpotifyGateway.ServiceClients.MessageServiceClients;
using SpotifyGateway.ServiceClients.MessageServiceClients.Abstractions;
using SpotifyGateway.Services;
using SpotifyGateway.Services.Abstractions;
using SpotifyGateway.Services.LogServices;
using SpotifyGateway.Services.LogServices.Abstractions;
using SpotifyGateway.Services.SpotifyServices;
using SpotifyGateway.Services.SpotifyServices.Abstractions;
using SpotifyGateway.Services.WorkerServices;
using SpotifyGateway.Services.WorkerServices.Abstractions;
using StackExchange.Redis;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddHangfire(this IServiceCollection services)
        {
            services
                .AddHangfire((serviceProvider, globalConfiguration) =>
                {
                    var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                    globalConfiguration.UsePostgreSqlStorage(databaseOptions.DefaultConnection);
                })
                .AddHangfireServer();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var openApiInfo = new OpenApiInfo
                {
                    Version = "v1",
                    Title = nameof(SpotifyGateway),
                    Description = nameof(SpotifyGateway)
                };

                options.SwaggerDoc("v1", openApiInfo);

                options.SchemaFilter<SchemaFilter>();
                options.OperationFilter<SwaggerParameterFilter>();
            });
        }

        public static void AddOptions(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.Configure<GeneralOptions>(configuration.GetSection(nameof(GeneralOptions)));
            services.Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)));
            services.Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));
            services.Configure<HangfireOptions>(configuration.GetSection(nameof(HangfireOptions)));
            services.Configure<PredictionServiceOptions>(configuration.GetSection(nameof(PredictionServiceOptions)));
            services.Configure<RequestOptions>(configuration.GetSection(nameof(RequestOptions)));
            services.Configure<TelegramServiceOptions>(configuration.GetSection(nameof(TelegramServiceOptions)));
        }

        public static void AddAttributes(this IServiceCollection services)
        {
            services.AddScoped<DevelopmentOnlyAttribute>();
            services.AddScoped<UserAuthenticationAttribute>();
            services.AddScoped<AppAuthenticationAttribute>();
        }

        public static void AddCacheProviders(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var redisOptions = x.GetRequiredService<IOptions<RedisOptions>>().Value;

                return ConnectionMultiplexer.Connect(redisOptions.Connection);
            });

            services.AddScoped<IRedisCacheProvider, RedisCacheProvider>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IAlbumRepository, AlbumRepository>();
            services.AddScoped<IPlaylistRepository, PlaylistRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();

            services.AddSingleton<IMongoClient>(x =>
            {
                var databaseOptions = x.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                ConventionRegistry.Register("camelCase", new ConventionPack { new CamelCaseElementNameConvention() }, _ => true);
                ConventionRegistry.Register("EnumStringConvention", new ConventionPack { new EnumRepresentationConvention(BsonType.String) }, _ => true);

                return new MongoClient(databaseOptions.MongoDbConnection);
            });

            services.AddScoped<ILoggerRepository, LoggerRepository>();
            services.AddScoped<ISearchResultRepository, SearchResultRepository>();
        }

        public static void AddServices(this IServiceCollection services, RequestOptions requestOptions)
        {
            services.AddTransient(typeof(Lazy<>), typeof(LazyProvider<>));
            services.AddScoped<IWorkerServiceFactory, WorkerServiceFactory>();
            services.AddScoped<ISettingFactory, SettingFactory>();
            services.AddScoped<IResourceFactory, ResourceFactory>();
            services.AddScoped(typeof(IResourceFactory<>), typeof(ResourceFactory<>));

            services.AddScoped(typeof(IApiWrapper<>), typeof(ApiWrapper<>));
            services.AddScoped<IMessageServiceClient, TelegramServiceClient>();
            services.AddScoped<IPredictionServiceClient, PredictionServiceClient>();

            services.AddScoped<IStartupManager, StartupManager>();
            services.AddScoped<IWorkerManager, WorkerManager>();
            services.AddScoped<IConfigurationManager, ConfigurationManager>();
            services.AddScoped<IResourceManager, ResourceManager>();

            services.AddScoped<ISpotifyAppContext, SpotifyAppContext>();
            services.AddScoped<ISpotifyUserContext, SpotifyUserContext>();

            services.AddScoped<ILoggerService, LoggerService>();
            services.AddScoped<IAlbumService, AlbumService>();
            services.AddScoped<IArtistService, ArtistService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBrowseService, BrowseService>();
            services.AddScoped<ICredentialService, CredentialService>();
            services.AddScoped<IPlaylistService, PlaylistService>();
            services.AddScoped<IRelatedArtistsService, RelatedArtistsService>();
            services.AddScoped<ITrackService, TrackService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ICategoryWorkerService, CategoryWorkerService>();
            services.AddScoped<INewDayWorkerService, NewDayWorkerService>();
            services.AddScoped<ISearchWorkerService, SearchWorkerService>();
            services.AddScoped<ITokenWorkerService, TokenWorkerService>();

            services.AddScoped<IRecommendationHistoryService, RecommendationHistoryService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddScoped(_ => Policy.Handle<Exception>().RetryAsync(requestOptions.RetryCount));
        }

        public static void AddDbOptions(this IServiceCollection services)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            FluentMapper.Initialize(config => { config.AddMap(new RecommendedTrackResponseMap()); });
        }

        public static void AddSettings(this IServiceCollection services, IRepository repository, GeneralOptions generalOptions)
        {
            var filterModel = new ConfigurationFilterModel
            {
                Where = x => new Where((nameof(x.Application), generalOptions.Name, Operation.Equal))
            };

            var values = repository.Query<Data.Entities.Configuration>(filterModel.Query(), filterModel.Parameters);

            services.AddSettings<IGeneralSettings, GeneralSettings>(values);
            services.AddSettings<IAuthSettings, AuthSettings>(values);
            services.AddSettings<ITelegramSettings, TelegramSettings>(values);
            services.AddSettings<IBrowseSettings, BrowseSettings>(values);
            services.AddSettings<IRecommendationSettings, RecommendationSettings>(values);
        }

        public static void AddResources(this IServiceCollection services, IResourceRepository resourceRepository, GeneralOptions generalOptions)
        {
            var resources = resourceRepository.GetResources();

            services.AddResources<IErrorResources, ErrorResources>(resources, generalOptions);
            services.AddResources<IRecommendationResources, RecommendationResources>(resources, generalOptions);
        }

        private static void AddSettings<TI, T>(this IServiceCollection services, List<Data.Entities.Configuration> configurations) where TI : class where T : TI, ISettings, new()
        {
            var result = new T().SetValues(configurations);

            services.AddSingleton<TI>(result);
            services.AddSingleton<ISettings>(result);
        }

        private static void AddResources<TI, T>(this IServiceCollection services, IEnumerable<Resource> resources, GeneralOptions generalOptions)
            where TI : class, IResources where T : class, TI, new()
        {
            var className = typeof(T).Name;

            var classResources = resources
                .Where(x => className.ContainsIgnoreCase(x.Class))
                .GroupBy(x => x.Language)
                .Where(x => x.Key.TryParse(out Language _))
                .ToDictionary(x => x.Key.Parse<Language>(), x => x.Select(y => y).ToList());

            foreach (var language in generalOptions.ActiveLanguages)
            {
                if (classResources.TryGetValue(language, out var languageResources))
                {
                    var result = new T().SetValues(languageResources);

                    result.Language = language;

                    services.AddSingleton(result);
                    services.AddSingleton<IResources>(result);
                }
            }

            services.AddScoped<TI, T>(x => x.GetRequiredService<IResourceFactory<T>>().Value);
        }
    }
}