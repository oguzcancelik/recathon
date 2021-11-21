using System;
using System.Collections.Generic;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class ListHelpers
    {
        public static IEnumerable<int> GenerateRandomIndexes(int count, int maxValue, int minValue = 0, Random random = null)
        {
            random ??= new Random();

            var indexes = new List<int>();

            while (indexes.Count < count)
            {
                var index = random.Next(minValue, maxValue);

                if (!indexes.Contains(index))
                {
                    indexes.Add(index);
                }
            }

            return indexes;
        }

        public static bool GenerateRandomBoolean(Random random = null)
        {
            random ??= new Random();

            return random.NextDouble() > 0.5;
        }

        public static int PageCount(int count, int pageSize)
        {
            return count % pageSize == 0
                ? count / pageSize
                : count / pageSize + 1;
        }
    }
}