using System.Text.Json.Serialization;
using System.Threading;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SpotifyGateway.Data.Repositories.Abstractions;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Infrastructure.Filters;
using SpotifyGateway.Managers.Abstractions;

namespace SpotifyGateway
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(x =>
                {
                    x.Filters.Add<ExceptionFilter>();
                    x.Filters.Add<ValidationFilter>();
                })
                .AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.Configure<ApiBehaviorOptions>(x => x.SuppressModelStateInvalidFilter = true);

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            services.AddOptions(_configuration);

            services.AddRepositories();

            services.AddDbOptions();

            services.AddAttributes();

            services.AddCacheProviders();

#pragma warning disable ASP0000
            using (var provider = services.BuildServiceProvider())
#pragma warning restore ASP0000
            {
                var repository = provider.GetRequiredService<IRepository>();
                var resourceRepository = provider.GetRequiredService<IResourceRepository>();
                var generalOptions = provider.GetRequiredService<IOptions<GeneralOptions>>().Value;
                var requestOptions = provider.GetRequiredService<IOptions<RequestOptions>>().Value;

                services.AddSettings(repository, generalOptions);

                services.AddResources(resourceRepository, generalOptions);

                services.AddServices(requestOptions);

                if (generalOptions.IsClientAppEnabled)
                {
                    services.AddSpaStaticFiles(x => x.RootPath = "frontend/build");
                }
            }

            services.AddSwagger();

            services.AddHangfire();
        }

        public void Configure(IApplicationBuilder app, IStartupManager startupManager, IOptions<GeneralOptions> generalOptions, IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseHttpsRedirection();
            }

            if (generalOptions.Value.IsClientAppEnabled)
            {
                app.UseStaticFiles();
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            if (hostEnvironment.IsProduction())
            {
                app.UseEndpointRestriction();
            }

            app.UseCustomMiddlewares();

            app.UseEndpoints(x => x.MapControllers());

            app.UseHangfire();

            app.UseSwaggerWithUi();

            if (generalOptions.Value.IsClientAppEnabled)
            {
                app.UseSpa(hostEnvironment);
            }

            ThreadPool.SetMinThreads(generalOptions.Value.WorkerThreadsCount, generalOptions.Value.CompletionThreadsCount);

            startupManager.RunAsync().Wait();
        }
    }
}