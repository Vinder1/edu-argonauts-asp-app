using Argonauts.Application.External;

namespace Argonauts.Web.Services;

/// <summary>
/// 
/// </summary>
public class DIScopeFactory(IServiceScopeFactory msFactory) : IScopeFactory
{
    private readonly IServiceScopeFactory _msFactory = msFactory;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IScopedContext CreateScope()
        => new ScopeContextAdapter(_msFactory.CreateScope());
}

internal sealed class ScopeContextAdapter(IServiceScope scope) : IScopedContext
{
    private readonly IServiceScope _scope = scope;

    public T Resolve<T>() where T : notnull
        => _scope.ServiceProvider.GetRequiredService<T>();

    public void Dispose() => _scope.Dispose();
}