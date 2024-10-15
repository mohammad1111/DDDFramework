using System;
using Gig.Framework.Core.DataProviders.Elastic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gig.Framework.Data.Elastic;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElasticProviders<T>(this IServiceCollection services) where T : class
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.TryAdd(new ServiceDescriptor(
            typeof(ICrudAuditLogElasticProvider),
            typeof(CrudAuditLogElasticProvider),
            ServiceLifetime.Transient));

        services.TryAdd(new ServiceDescriptor(
            typeof(IElasticProvider<>),
            typeof(ElasticProvider<>),
            ServiceLifetime.Transient));

        return services;
    }
}