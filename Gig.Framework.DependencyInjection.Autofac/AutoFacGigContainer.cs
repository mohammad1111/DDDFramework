using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Gig.Framework.Core.DependencyInjection;

namespace Gig.Framework.DependencyInjection.Autofac;

public class AutoFacGigContainer : IGigContainer
{
    private readonly ILifetimeScope _container;

    public AutoFacGigContainer(ILifetimeScope container)
    {
        _container = container;
    }


    public IEnumerable ResolveAll(Type type)
    {
        return (IEnumerable)_container.Resolve(typeof(IEnumerable<>).MakeGenericType(type));
    }

    public IDisposable CreateScope()
    {
        return _container.BeginLifetimeScope();
    }

    public T Resolve<T>()
    {
        return _container.Resolve<T>();
    }

    public IDisposable RequireScope()
    {
        throw new NotImplementedException();
    }

    public object ResolveType(Type type)
    {
        return _container.Resolve(type);
    }

    public T Resolve<T>(Func<T, bool> selector)
    {
        var allInstances = _container.Resolve<IEnumerable<T>>();
        return allInstances.First(selector);
    }

    public T Resolve<T>(string objectName)
    {
        var allInstances = _container.ResolveNamed<IEnumerable<T>>(objectName);
        return allInstances.First();
    }

    public T TryResolve<T>()
    {
        var result = _container.TryResolve(typeof(T), out var instance);
        return result ? (T)instance : default;
    }


    public T Resolve<T>(Dictionary<string, object> param)
    {
        var parameters = new List<NamedParameter>();
        foreach (var parameter in param) parameters.Add(new NamedParameter(parameter.Key, parameter.Value));
        return _container.Resolve<T>(parameters);
    }

    public IEnumerable<T> ResolveAll<T>()
    {
        return _container.Resolve<IEnumerable<T>>();
    }
}