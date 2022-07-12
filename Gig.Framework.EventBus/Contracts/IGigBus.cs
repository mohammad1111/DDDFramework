namespace Gig.Framework.EventBus.Contracts;

public interface IGigBus
{
    void Start();

    Task StartAsync();
}