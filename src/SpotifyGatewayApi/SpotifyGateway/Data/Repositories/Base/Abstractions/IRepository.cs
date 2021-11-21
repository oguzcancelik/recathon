using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using SpotifyGateway.Data.Models;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Models.DataResults;

namespace SpotifyGateway.Data.Repositories.Base.Abstractions
{
    public interface IRepository
    {
        List<T> Query<T>(string query, object parameters = null);

        Task<List<T>> QueryAsync<T>(string query, object parameters = null);

        Task<T> QuerySingleAsync<T>(string query, object parameters = null);

        Task<T> QueryFirstOrDefaultAsync<T>(string query, object parameters = null);

        Task<DataResult<T1, T2>> QueryMultipleAsync<T1, T2>(string query, object parameters = null);

        Task<DataResult<T1, T2, T3>> QueryMultipleAsync<T1, T2, T3>(string query, object parameters = null);

        Task<DataResult<T1, T2, T3, T4>> QueryMultipleAsync<T1, T2, T3, T4>(string query, object parameters = null);

        Task InsertManyAsync<T>(string query, List<T> values);

        int Execute(string query, object parameters = null);

        Task<int> ExecuteAsync(string query, object parameters = null);

        [Queue(HangfireConstants.SqlQueue)]
        Task ExecuteAsync(BackgroundExecutionModel model);
    }
}