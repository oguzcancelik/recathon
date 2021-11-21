using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Infrastructure.Extensions;

namespace SpotifyGateway.Models.FilterModels.Base
{
    public class GroupBy
    {
        private List<string> Values { get; }

        public GroupBy(params string[] values)
        {
            Values = values.ToList();
        }

        public string Query()
        {
            if (Values != null && Values.Any())
            {
                var result = Values.Aggregate(string.Empty, (current, value) => current + $"{value.ToSnakeCase()}, ");

                return result[..^2];
            }

            return null;
        }
    }
}