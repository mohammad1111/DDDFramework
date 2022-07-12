using System.Collections;

namespace Gig.Framework.Core.DependencyInjection;

public interface IGigContainer
{
    T Resolve<T>();

    IDisposable RequireScope();

    object ResolveType(Type type);

    T Resolve<T>(Func<T, bool> selector);

    T Resolve<T>(string objectName);

    T TryResolve<T>();

    IEnumerable<T> ResolveAll<T>();

    T Resolve<T>(Dictionary<string, object> parameters);

    IEnumerable ResolveAll(Type type);

    IDisposable CreateScope();
}