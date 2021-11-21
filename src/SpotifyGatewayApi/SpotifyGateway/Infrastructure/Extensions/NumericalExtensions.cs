using System;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class NumericalExtensions
    {
        public static double ToAbs(this double source)
        {
            return Math.Abs(source);
        }
    }
}