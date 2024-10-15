using System.Threading.Tasks;

namespace Gig.Framework.Application.ReadModel;

public interface IQueryHandlerAsync<TQuery, TResult> : IQueryHandler where TQuery : IQuery
{
    Task<TResult> HandleAsync(TQuery command);
}

public interface IQueryHandler<TQuery, TResult> : IQueryHandler where TQuery : IQuery
{
    TResult Handle(TQuery command);
}