using System;
using System.Collections.Generic;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class ListExtensions
    {
        public static List<T> Shuffle<T>(this List<T> list)
        {
            var random = new Random();

            var x = list.Count;

            while (x > 1)
            {
                x--;

                var y = random.Next(x + 1);
                (list[y], list[x]) = (list[x], list[y]);
            }

            return list;
        }

        public static IEnumerable<List<T>> Split<T>(this List<T> values, int size)
        {
            for (var i = 0; i < values.Count; i += size)
            {
                yield return values.GetRange(i, Math.Min(size, values.Count - i));
            }
        }
    }
}