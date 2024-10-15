using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Gig.Framework.Api.RealTimeServices;

public abstract class GigRealTimeService<THub> : IGigRealTimeService<THub>
    where THub : GigHub
{
    private readonly IRealtimeServiceDependencies<THub> _realtimeServiceDependencies;

    protected GigRealTimeService(
        IRealtimeServiceDependencies<THub> realtimeServiceDependencies)
    {
        _realtimeServiceDependencies = realtimeServiceDependencies;
    }

    public async Task SendToAll(
        string topic,
        object message)
    {
        await _realtimeServiceDependencies.Context.Clients.All.SendAsync(
            topic,
            message);
    }

    public async Task SendToUser(
        string userId,
        string topic,
        object message)
    {
        await _realtimeServiceDependencies.Context.Clients.User(userId).SendAsync(
            topic,
            message);
    }
}

public abstract class GigRealTimeService<THub, TClient> : IGigRealTimeService<THub, TClient>
    where THub : GigHub<TClient>
    where TClient : class, IGigHubClient
{
    private readonly IRealtimeServiceDependencies<THub, TClient> _realtimeServiceDependencies;

    protected GigRealTimeService(
        IRealtimeServiceDependencies<THub, TClient> realtimeServiceDependencies)
    {
        _realtimeServiceDependencies = realtimeServiceDependencies;
    }
}