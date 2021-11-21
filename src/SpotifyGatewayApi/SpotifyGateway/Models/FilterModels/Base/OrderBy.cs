using System.Collections.Generic;
using System.Linq;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.FilterModels.Base
{
    public class OrderBy
    {
        private List<(string field, SortType sort)> Values { get; }

        private string Value { get; }

        public OrderBy(string value)
        {
            Value = value;
        }

        public OrderBy(params (string, SortType)[] value)
        {
            Values = value.ToList();
        }

        public string Query()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                return Value;
            }

            if (Values != null && Values.Any())
            {
                var result = Values.Aggregate(string.Empty, (current, value) => current + $"{value.Item1.ToSnakeCase()} {value.Item2.ToValue()}, ");

                return result[..^2];
            }

            return null;
        }
    }
}