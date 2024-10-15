using System.Threading.Tasks;
using Gig.Framework.Core.DependencyInjection;

namespace Gig.Framework.Application.ReadModel;

public class QueryBus : IQueryBus
{
    private readonly IServiceLocator _serviceLocator;

    public QueryBus(IServiceLocator serviceLocator)
    {
        _serviceLocator = serviceLocator;
    }

    public async Task<TResult> DispatchAsync<TQueryCommand, TResult>(TQueryCommand command)
        where TQueryCommand : IQuery
    {
        return await _serviceLocator.Current.Resolve<IQueryHandlerAsync<TQueryCommand, TResult>>().HandleAsync(command);
    }

    public TResult Dispatch<TQueryCommand, TResult>(TQueryCommand command)
        where TQueryCommand : IQuery

    {
        return _serviceLocator.Current.Resolve<IQueryHandler<TQueryCommand, TResult>>().Handle(command);
    }
}