using Gig.Framework.Core.DependencyInjection;

namespace Gig.Framework.Application;

public class CommandHandlerFactory : ICommandHandlerFactory
{
    private readonly IServiceLocator _serviceLocator;

    public CommandHandlerFactory(IServiceLocator serviceLocator)
    {
        _serviceLocator = serviceLocator;
    }

    public ICommandHandlerAsync<T, TResult> CreateHandler<T, TResult>()
        where T : ICommand
    {
        return _serviceLocator.Current.Resolve<ICommandHandlerAsync<T, TResult>>();
    }

    public ICommandHandlerAsync<T> CreateHandler<T>()
        where T : ICommand
    {
        return _serviceLocator.Current.Resolve<ICommandHandlerAsync<T>>();
    }
}