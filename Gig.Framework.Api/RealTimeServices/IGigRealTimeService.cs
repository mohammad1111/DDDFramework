using Gig.Framework.Core.RealTimeServices;

namespace Gig.Framework.Api.RealTimeServices;

public interface IGigRealTimeService<THub> : IGigRealTimeService
    where THub : GigHub
{
}

public interface IGigRealTimeService<THub, TClient>
    where THub : GigHub<TClient>
    where TClient : class, IGigHubClient
{
}