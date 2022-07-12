namespace Gig.Framework.Core.RealTimeServices;

public interface IGigRealTimeService
{
    Task SendToAll(
        string topic,
        object message);

    Task SendToUser(
        string userId,
        string topic,
        object message);
}