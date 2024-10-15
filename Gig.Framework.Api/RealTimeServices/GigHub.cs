using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Gig.Framework.Api.RealTimeServices;

public abstract class GigHub<TClient> : Hub<TClient>
    where TClient : class, IGigHubClient
{
    private readonly ILogger _logger;

    public GigHub(
        ILogger logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.Information(
            "Create SignalR Connection connectionId:{ConnectionId},UserId:{UserId}",
            Context.ConnectionId,
            Context.UserIdentifier);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(
        Exception? exception)
    {
        _logger.Information(
            "Disconnect SignalR Connection  connectionId:{ConnectionId},UserId:{UserId} ,Exception:{Exception}",
            Context.ConnectionId,
            Context.UserIdentifier,
            exception?.ToString() ?? string.Empty);
        return base.OnDisconnectedAsync(exception);
    }
}

public abstract class GigHub : Hub
{
    private readonly ILogger _logger;

    public GigHub(
        ILogger logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        _logger.Information(
            "Create SignalR Connection connectionId:{ConnectionId},UserId:{UserId}",
            Context.ConnectionId,
            Context.UserIdentifier);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(
        Exception? exception)
    {
        _logger.Information(
            "Disconnect SignalR Connection  connectionId:{ConnectionId}, UserId:{UserId}و Exception:{Exception}",
            Context.ConnectionId,
            Context.UserIdentifier,
            exception?.ToString() ?? string.Empty);
        return base.OnDisconnectedAsync(exception);
    }
}