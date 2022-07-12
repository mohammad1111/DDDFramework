using Gig.Framework.Core.Models;

namespace Gig.Framework.Application;

public interface ICommandBus
{
    Task<GigCommonResultBase> DispatchAsync<T>(T command) where T : ICommand;

    Task<GigCommonResult<TResult>> DispatchAsync<T, TResult>(T command)
        where T : ICommand;
}