using System;
using Dapper;
using SpotifyGateway.Infrastructure.Extensions;

namespace SpotifyGateway.Models.FilterModels.Base
{
    public abstract class BaseFilterModel<T> where T : class
    {
        private object _parameters;

        public Func<T, Select> Select { get; set; }

        public Func<T, Where> Where { get; set; }

        public Func<T, GroupBy> GroupBy { get; set; }

        public Func<T, OrderBy> OrderBy { get; set; }

        public string From { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

        public object Parameters
        {
            get => GetParameters();
            set => _parameters = value;
        }

        public virtual string Query()
        {
            var selectQuery = SelectQuery();
            var fromQuery = FromQuery();
            var whereQuery = WhereQuery();
            var groupByQuery = GroupByQuery();
            var orderByQuery = OrderByQuery();
            var offsetQuery = OffsetQuery();
            var limitQuery = LimitQuery();

            return $"{selectQuery} {fromQuery} {whereQuery} {groupByQuery} {orderByQuery} {offsetQuery} {limitQuery};".RemoveLineBreaks();
        }

        protected virtual string SelectQuery()
        {
            var selectValue = Select?.Invoke(default)?.Query();
            var selectQuery = !string.IsNullOrEmpty(selectValue) ? $"SELECT {selectValue}" : "SELECT *";

            return selectQuery;
        }

        protected virtual string FromQuery()
        {
            return !string.IsNullOrEmpty(From)
                ? $"FROM {From}"
                : $"FROM {typeof(T).Name.ToSnakeCase()}";
        }

        protected virtual string WhereQuery()
        {
            var whereValue = Where?.Invoke(default)?.Query();
            var whereQuery = !string.IsNullOrEmpty(whereValue) ? $"WHERE {whereValue}" : null;

            return whereQuery;
        }

        protected virtual string GroupByQuery()
        {
            var groupByValue = GroupBy?.Invoke(default)?.Query();
            var groupByQuery = !string.IsNullOrEmpty(groupByValue) ? $"GROUP BY {groupByValue}" : null;

            return groupByQuery;
        }

        protected virtual string OrderByQuery()
        {
            var orderByValue = OrderBy?.Invoke(default)?.Query();
            var orderByQuery = !string.IsNullOrEmpty(orderByValue) ? $"ORDER BY {orderByValue}" : null;

            return orderByQuery;
        }

        protected virtual string OffsetQuery()
        {
            return Offset.HasValue ? $"OFFSET {Offset.Value}" : string.Empty;
        }

        protected virtual string LimitQuery()
        {
            return Limit.HasValue ? $"LIMIT {Limit.Value}" : string.Empty;
        }

        protected virtual DynamicParameters GetParameters()
        {
            var parameters = _parameters.ToDynamicParameters();

            var whereParameters = Where?.Invoke(default)?.GetParameters();

            if (whereParameters != null)
            {
                parameters ??= new DynamicParameters();

                foreach (var (key, value) in whereParameters)
                {
                    parameters.Add($"@{key}", value);
                }
            }

            return parameters;
        }
    }
}