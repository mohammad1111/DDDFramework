using Microsoft.AspNetCore.SignalR;

namespace Gig.Framework.Api.RealTimeServices;

public class RealtimeServiceDependencies<THub, TClient> : IRealtimeServiceDependencies<THub, TClient>
    where THub : GigHub<TClient>
    where TClient : class, IGigHubClient
{
    public RealtimeServiceDependencies(
        IHubContext<THub, TClient> context)
    {
        Context = context;
    }

    public IHubContext<THub, TClient> Context { get; }
}

public class RealtimeServiceDependencies<THub> : IRealtimeServiceDependencies<THub>
    where THub : GigHub
{
    public RealtimeServiceDependencies(
        IHubContext<THub> context)
    {
        Context = context;
    }

    public IHubContext<THub> Context { get; }
}