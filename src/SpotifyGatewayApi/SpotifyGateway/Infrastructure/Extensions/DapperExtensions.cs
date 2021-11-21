using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Hangfire;
using SpotifyGateway.Data.Models;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Helpers;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class DapperExtensions
    {
        private const int PageSize = 250;
        private const string PageParameter = "Ids";

        public static async Task<List<T>> PagedQueryAsync<T>(this IDbConnection connection, string query, object parameters, int pageSize = PageSize, string pageParameter = PageParameter)
        {
            var propertyInfo = parameters?.GetType().GetProperty(pageParameter);

            var values = new List<T>();

            if (propertyInfo != null)
            {
                var ids = (List<int>)propertyInfo.GetValue(parameters);

                if (ids != null)
                {
                    var param = ToDynamicParameters(parameters, pageParameter);

                    var pageCount = ListHelpers.PageCount(ids.Count, pageSize);

                    for (var i = 0; i < pageCount; i++)
                    {
                        var currentIds = ids.Skip(i * pageSize).Take(pageSize).ToList();

                        param.Add(pageParameter, currentIds);

                        var currentValues = await connection.QueryAsync<T>(query, param);

                        values.AddRange(currentValues);
                    }
                }
            }

            return values;
        }

        public static async Task<int> PagedExecuteAsync(this IDbConnection connection, string query, object parameters, int pageSize = PageSize, string pageParameter = PageParameter,
            Action<List<int>> pageAction = null)
        {
            var propertyInfo = parameters?.GetType().GetProperty(pageParameter);

            var totalCount = 0;

            if (propertyInfo != null)
            {
                var ids = (List<int>)propertyInfo.GetValue(parameters);

                if (ids != null)
                {
                    var param = ToDynamicParameters(parameters, pageParameter);

                    var pageCount = ListHelpers.PageCount(ids.Count, pageSize);

                    for (var i = 0; i < pageCount; i++)
                    {
                        var currentIds = ids.Skip(i * pageSize).Take(pageSize).ToList();

                        param.Add(pageParameter, currentIds);

                        var count = await connection.ExecuteAsync(query, param);

                        totalCount += count;

                        if (pageAction != null && count > 0)
                        {
                            pageAction(currentIds);
                        }
                    }
                }
            }

            return totalCount;
        }

        // ReSharper disable once UnusedParameter.Global
        public static void PagedFireForgetExecute(this IDbConnection connection, string query, object parameters, int pageSize = PageSize, string pageParameter = PageParameter)
        {
            var propertyInfo = parameters?.GetType().GetProperty(pageParameter);

            if (propertyInfo != null)
            {
                var ids = (List<int>)propertyInfo.GetValue(parameters);

                if (ids != null)
                {
                    var param = parameters.ToDictionary(pageParameter);

                    var pageCount = ListHelpers.PageCount(ids.Count, pageSize);

                    for (var i = 0; i < pageCount; i++)
                    {
                        var currentIds = ids.Skip(i * pageSize).Take(pageSize).ToList();

                        var model = new BackgroundExecutionModel
                        {
                            Query = query,
                            Parameters = param,
                            Ids = currentIds,
                            PageParameterName = pageParameter
                        };

                        BackgroundJob.Enqueue<IRepository>(repository => repository.ExecuteAsync(model));
                    }
                }
            }
        }

        public static DynamicParameters ToDynamicParameters(this object source, params string[] ignoredProperties)
        {
            DynamicParameters parameters = null;

            if (source != null)
            {
                parameters = new DynamicParameters();

                foreach (var propertyInfo in source.GetType().GetProperties())
                {
                    if (!ignoredProperties.Contains(propertyInfo.Name))
                    {
                        var key = $"@{propertyInfo.Name}";
                        var value = propertyInfo.GetValue(source);

                        parameters.Add(key, value);
                    }
                }
            }

            return parameters;
        }

        public static DynamicParameters ToDynamicParameters(this Dictionary<string, object> source)
        {
            DynamicParameters parameters = null;

            if (source != null && source.Any())
            {
                parameters = new DynamicParameters();

                foreach (var (key, value) in source)
                {
                    parameters.Add($"@{key}", value);
                }
            }

            return parameters;
        }
    }
}