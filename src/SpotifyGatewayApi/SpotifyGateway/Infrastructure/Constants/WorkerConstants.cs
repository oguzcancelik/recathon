using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SpotifyGateway.Infrastructure.Helpers;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Constants
{
    public static class WorkerConstants
    {
        public const string SearchWorker = "search-worker";
        public const string TokenWorker = "token-worker";
        public const string CategoryWorker = "category-worker";
        public const string NewDayWorker = "newday-worker";

        public static readonly Dictionary<string, string> Workers = new()
        {
            {nameof(WorkerType.Search), SearchWorker},
            {nameof(WorkerType.Token), TokenWorker},
            {nameof(WorkerType.Category), CategoryWorker},
            {nameof(WorkerType.NewDay), NewDayWorker}
        };

        public const string SearchCronExpression = FifteenSeconds;
        public const string TokenCronExpression = FifteenMinutes;
        public const string CategoryCronExpression = ClockTurn;
        public const string NewDayCronExpression = DayStart;

        #region CronExpressions

        private const string FifteenSeconds = "0/15 * * * * *";
        private const string FifteenMinutes = "*/15 * * * *";
        private const string DayStart = "0 0 * * *";
        private const string ClockTurn = "0 */12 * * *";

        #endregion

        #region CalculatedCronExpressions

        public static readonly IImmutableList<TimeSpan> ValidTokenTimes = DateHelpers.SplitDay(TokenRefreshTime).ToImmutableList();

        #endregion

        #region Configuration

        public const int TokenRefreshTime = 45;

        #endregion
    }
}