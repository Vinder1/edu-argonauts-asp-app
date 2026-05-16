namespace Argonauts.Application.External;

public interface IScopeFactory
{
    IScopedContext CreateScope();
}

public interface IScopedContext : IDisposable
{
    T Resolve<T>() where T : notnull;
}