using System;
using Microsoft.Extensions.DependencyInjection;

namespace SpotifyGateway.Infrastructure.Lazy
{
    public class LazyProvider<T> : Lazy<T> where T : class
    {
        public LazyProvider(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<T>)
        {
        }
    }
}