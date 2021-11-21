using System.Collections.Generic;
using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Middlewares;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseHangfire(this IApplicationBuilder app)
        {
            var hostEnvironment = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
            var hangfireOptions = app.ApplicationServices.GetRequiredService<IOptions<HangfireOptions>>().Value;

            var workers = new List<BackgroundJobServerOptions>
            {
                new()
                {
                    WorkerCount = hangfireOptions.SqlQueueWorkerCount,
                    Queues = new[] { HangfireConstants.SqlQueue }
                }
            };

            foreach (var worker in workers)
            {
                app.UseHangfireServer(worker);
            }

            DashboardOptions dashboardOptions = null;

            if (hostEnvironment.IsProduction())
            {
                var filterOptions = new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = hangfireOptions.Username,
                            PasswordClear = hangfireOptions.Password
                        }
                    }
                };

                dashboardOptions = new DashboardOptions
                {
                    Authorization = new[]
                    {
                        new BasicAuthAuthorizationFilter(filterOptions)
                    }
                };
            }

            app.UseHangfireDashboard($"/{RouteConstants.Hangfire}", dashboardOptions);
        }

        public static void UseEndpointRestriction(this IApplicationBuilder app)
        {
            app.UseWhen(
                context => context.Request.Path.ToString().ContainsIgnoreCase(RouteConstants.Hangfire),
                appBuilder => appBuilder.UseMiddleware<HangfireMiddleware>()
            );

            app.UseWhen(
                context => context.Request.Path.ToString().ContainsIgnoreCase(RouteConstants.Swagger),
                appBuilder => appBuilder.UseMiddleware<SwaggerMiddleware>()
            );

            app.UseWhen(
                context => RequestHelpers.IsAdminRoute(context.Request.Path.ToString()),
                appBuilder => appBuilder.UseMiddleware<AdminMiddleware>()
            );

            app.MapWhen(
                context => RequestHelpers.ShortcutRequest(context.Request.Path.ToString()),
                appBuilder => appBuilder.UseMiddleware<ShortcutMiddleware>()
            );
        }

        public static void UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestInitializerMiddleware>();
        }

        public static void UseSwaggerWithUi(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", nameof(SpotifyGateway));
                options.DocExpansion(DocExpansion.None);
            });
        }

        public static void UseSpa(this IApplicationBuilder app, IHostEnvironment hostEnvironment)
        {
            app.UseSpa(x =>
            {
                x.Options.SourcePath = "frontend";

                if (hostEnvironment.IsDevelopment())
                {
                    x.UseReactDevelopmentServer("start");
                }
            });
        }
    }
}