using System.Linq.Expressions;
using Argonauts.Application.External;
using Hangfire;

namespace Argonauts.Infrastructure.Hangfire;

public class HangfireScheduler : IBackgroundScheduler
{
    public void Schedule<T>(Expression<Action<T>> job, TimeSpan delay) where T : class
    {
        BackgroundJob.Schedule(job, delay);
    }

    public void ScheduleAsync<T>(Expression<Func<T, Task>> job, TimeSpan delay) where T : class
    {
        BackgroundJob.Schedule(job, delay);
    }
}