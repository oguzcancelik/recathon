using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Models.FilterModels.Base
{
    public class Select
    {
        private List<(string, SelectType, string)> Values { get; }

        public Select(params string[] values)
        {
            Values = values.Select(x => x.ToSelectValue()).ToList();
        }

        public Select(params (string, SelectType)[] values)
        {
            Values = values.Select(x => x.ToSelectValue()).ToList();
        }

        public Select(params (string, SelectType, string)[] values)
        {
            Values = values.ToList();
        }

        public string Query()
        {
            if (Values != null && Values.Any())
            {
                var result = new StringBuilder();

                foreach (var (name, select, alias) in Values)
                {
                    var field = name.ToSnakeCase();

                    var condition = select switch
                    {
                        SelectType.Field => field,
                        SelectType.Distinct => $"DISTINCT({field})",
                        SelectType.Count => $"COUNT({field})",
                        SelectType.Avg => $"AVG({field})",
                        SelectType.Sum => $"SUM({field})",
                        _ => string.Empty
                    };

                    if (!string.IsNullOrEmpty(condition))
                    {
                        condition += !string.IsNullOrEmpty(alias)
                            ? $" AS {alias}, "
                            : ", ";
                    }

                    result.Append(condition);
                }

                return result.ToString()[..^2];
            }

            return "*";
        }
    }
}