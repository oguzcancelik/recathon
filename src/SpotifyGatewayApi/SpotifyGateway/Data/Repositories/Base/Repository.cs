using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Hangfire;
using Microsoft.Extensions.Options;
using Npgsql;
using SpotifyGateway.Data.Models;
using SpotifyGateway.Data.Repositories.Base.Abstractions;
using SpotifyGateway.Infrastructure.Configuration.Options;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.DataResults;

namespace SpotifyGateway.Data.Repositories.Base
{
    public class Repository : IRepository
    {
        private readonly string _connectionString;

        public Repository(IOptions<DatabaseOptions> databaseOptions)
        {
            _connectionString = databaseOptions.Value.DefaultConnection;
        }

        public List<T> Query<T>(string query, object parameters = null)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var result = connection.Query<T>(query.RemoveLineBreaks(), parameters);

            return result.ToList();
        }

        public async Task<List<T>> QueryAsync<T>(string query, object parameters = null)
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            var result = await connection.QueryAsync<T>(query.RemoveLineBreaks(), parameters);

            return result.ToList();
        }

        public async Task<T> QuerySingleAsync<T>(string query, object parameters = null)
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            return await connection.QuerySingleAsync<T>(query, parameters);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string query, object parameters = null)
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            var result = await connection.QueryFirstOrDefaultAsync<T>(query.RemoveLineBreaks(), parameters);

            return result;
        }

        public async Task<DataResult<T1, T2>> QueryMultipleAsync<T1, T2>(string query, object parameters = null)
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            using var result = await connection.QueryMultipleAsync(query, parameters);

            var firstList = result.Read<T1>().ToList();
            var secondList = result.Read<T2>().ToList();

            return new DataResult<T1, T2> { FirstList = firstList, SecondList = secondList };
        }

        public async Task<DataResult<T1, T2, T3>> QueryMultipleAsync<T1, T2, T3>(string query, object parameters = null)
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            using var result = await connection.QueryMultipleAsync(query, parameters);

            var firstList = result.Read<T1>().ToList();
            var secondList = result.Read<T2>().ToList();
            var thirdList = result.Read<T3>().ToList();

            return new DataResult<T1, T2, T3> { FirstList = firstList, SecondList = secondList, ThirdList = thirdList };
        }

        public async Task<DataResult<T1, T2, T3, T4>> QueryMultipleAsync<T1, T2, T3, T4>(string query, object parameters = null)
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            using var result = await connection.QueryMultipleAsync(query, parameters);

            var firstList = result.Read<T1>().ToList();
            var secondList = result.Read<T2>().ToList();
            var thirdList = result.Read<T3>().ToList();
            var fourthList = result.Read<T4>().ToList();

            return new DataResult<T1, T2, T3, T4> { FirstList = firstList, SecondList = secondList, ThirdList = thirdList, FourthList = fourthList };
        }

        public async Task InsertManyAsync<T>(string query, List<T> parameters)
        {
            const int size = DatabaseConstants.BulkInsertLimit;

            if (parameters.Count > size)
            {
                var values = parameters.Split(size);

                var tasks = values.Select(x => ExecuteAsync(query, x));

                await Task.WhenAll(tasks);
            }
            else
            {
                await ExecuteAsync(query, parameters);
            }
        }

        public int Execute(string query, object parameters = null)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var result = connection.Execute(query.RemoveLineBreaks(), parameters);

            return result;
        }

        public async Task<int> ExecuteAsync(string query, object parameters = null)
        {
            await using var connection = new NpgsqlConnection(_connectionString);

            var result = await connection.ExecuteAsync(query.RemoveLineBreaks(), parameters);

            return result;
        }

        [Queue(HangfireConstants.SqlQueue)]
        public async Task ExecuteAsync(BackgroundExecutionModel model)
        {
            model.Parameters ??= new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(model.PageParameterName))
            {
                model.Parameters[model.PageParameterName] = model.Ids;
            }

            await using var connection = new NpgsqlConnection(_connectionString);

            await connection.ExecuteAsync(model.Query, model.Parameters.ToDynamicParameters());
        }
    }
}