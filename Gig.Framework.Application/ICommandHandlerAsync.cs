using System.Threading.Tasks;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Events;

namespace Gig.Framework.Application;

public interface ICommandHandlerAsync<in TCommand, TResult> : ICommandHandler
    where TCommand : ICommand
{
    IUnitOfWorkProvider UnitOfWorkWorkProvider { get; }
    Task<TResult> HandleAsync(TCommand command);
}

public interface ICommandHandlerAsync<in TCommand> : ICommandHandler
{
    IUnitOfWorkProvider UnitOfWorkWorkProvider { get; }

    Task HandleAsync(TCommand command);
}