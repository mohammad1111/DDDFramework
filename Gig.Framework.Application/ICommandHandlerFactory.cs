namespace Gig.Framework.Application;

public interface ICommandHandlerFactory
{
    ICommandHandlerAsync<T, TResult> CreateHandler<T, TResult>()
        where T : ICommand;

    ICommandHandlerAsync<T> CreateHandler<T>()
        where T : ICommand;

    // where TResult : GigCommandResult;
}