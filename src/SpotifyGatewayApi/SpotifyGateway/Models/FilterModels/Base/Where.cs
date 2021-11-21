using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.FilterModels.Base
{
    public class Where
    {
        private string Value { get; }

        private List<(string, object, Operation)> Values { get; }

        public Where(string value)
        {
            Value = value;
        }

        public Where(params (string, object, Operation)[] values)
        {
            Values = values.ToList();
        }

        public string Query()
        {
            if (!string.IsNullOrEmpty(Value))
            {
                return Value;
            }

            if (Values != null && Values.Any())
            {
                var result = new StringBuilder();

                foreach (var (name, _, operation) in Values)
                {
                    var field = name.ToSnakeCase();

                    var condition = operation switch
                    {
                        Operation.Equal => $"{field} = @{name}",
                        Operation.NotEqual => $"{field} != @{name}",
                        Operation.Less => $"{field} < @{name}",
                        Operation.LessOrEqual => $"{field} <= @{name}",
                        Operation.More => $"{field} > @{name}",
                        Operation.MoreOrEqual => $"{field} >= @{name}",
                        Operation.In => $"@{name} = ANY({field})",
                        Operation.NotIn => $"@{name} != ANY({field})",
                        Operation.Contains => $"position(@{name} in {field}) > 0",
                        _ => string.Empty
                    };

                    result.Append($"{condition} AND ");
                }

                return result.ToString()[..^5];
            }

            return null;
        }

        public Dictionary<string, object> GetParameters()
        {
            if (!string.IsNullOrEmpty(Value) || Values == null || !Values.Any())
            {
                return null;
            }

            return Values.ToDictionary(x => x.Item1, x => x.Item2);
        }
    }
}