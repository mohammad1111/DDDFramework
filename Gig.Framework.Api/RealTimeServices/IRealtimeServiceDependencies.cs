using Microsoft.AspNetCore.SignalR;

namespace Gig.Framework.Api.RealTimeServices;

public interface IRealtimeServiceDependencies<THub, TClient>
    where THub : GigHub<TClient>
    where TClient : class, IGigHubClient
{
    IHubContext<THub, TClient> Context { get; }
}

public interface IRealtimeServiceDependencies<THub>
    where THub : GigHub
{
    IHubContext<THub> Context { get; }
}