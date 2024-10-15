using System.Threading.Tasks;

namespace Gig.Framework.Application.ReadModel;

public interface IQueryBus
{
    Task<TResult> DispatchAsync<TQueryCommand, TResult>(TQueryCommand command) where TQueryCommand : IQuery;

    TResult Dispatch<TQueryCommand, TResult>(TQueryCommand command)
        where TQueryCommand : IQuery;
}