using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using SpotifyGateway.Infrastructure.Constants;
using SpotifyGateway.Infrastructure.Extensions;
using SpotifyGateway.Models.Enums;

namespace SpotifyGateway.Infrastructure.Helpers
{
    public static class WorkerHelpers
    {
        public static void CreateRecurringJob(string workerId, Expression<Func<Task>> expression, string cronExpression, bool triggerImmediately)
        {
            RemoveRecurringJob(workerId);

            RecurringJob.AddOrUpdate(workerId, expression, cronExpression);

            if (triggerImmediately)
            {
                TriggerRecurringJob(workerId);
            }
        }

        public static string Enqueue(Expression<Func<Task>> expression)
        {
            return BackgroundJob.Enqueue(expression);
        }

        public static void RemoveRecurringJob(string workerId)
        {
            RecurringJob.RemoveIfExists(workerId);
        }

        public static void TriggerRecurringJob(string workerId)
        {
            RecurringJob.Trigger(workerId);
        }

        public static bool ShouldWorkerRun(WorkerType workerType)
        {
            var now = DateTime.UtcNow.TimeOfDay;
            IImmutableList<TimeSpan> validTimes;

            switch (workerType)
            {
                case WorkerType.Token:
                    validTimes = WorkerConstants.ValidTokenTimes;
                    break;
                default:
                    return true;
            }

            return validTimes.Any(x => (x - now).TotalMinutes.ToAbs() <= 1);
        }
    }
}