using System.Linq.Expressions;

namespace Argonauts.Application.External;

public interface IBackgroundScheduler
{
    void Schedule<T>(Expression<Action<T>> job, TimeSpan delay) where T : class;
    public void ScheduleAsync<T>(Expression<Func<T, Task>> job, TimeSpan delay) where T : class;
}