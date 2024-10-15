using System;
using System.Linq;
using MassTransit;
using MassTransit.Futures.Contracts;
using MassTransit.Internals;
using MassTransit.Topology;

namespace Gig.Framework.Bus.EndpointNameFormatterExtensions;

public class CustomEntityNameFormatter : IEntityNameFormatter
{
    private readonly IEntityNameFormatter _entityNameFormatter;

    public CustomEntityNameFormatter(IEntityNameFormatter entityNameFormatter)
    {
        _entityNameFormatter = entityNameFormatter;
    }

    public string FormatEntityName<T>()
    {
        if (typeof(T).ClosesType(typeof(Get<>),
                out Type[] types))
        {
            var name = (string)typeof(IEntityNameFormatter)
                .GetMethod("FormatEntityName")
                .MakeGenericMethod(types)
                .Invoke(_entityNameFormatter,
                    Array.Empty<object>());

            var suffix = typeof(T).Name.Split('`').First();

            return $"{name}-{suffix}";
        }

        return _entityNameFormatter.FormatEntityName<T>();
    }
}