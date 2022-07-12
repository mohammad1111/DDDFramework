using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Gig.Framework.Api.RealTimeServices;

public class GigNameUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.FindFirstValue("userId");
    }
}