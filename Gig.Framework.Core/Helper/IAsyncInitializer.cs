namespace Gig.Framework.Core.Helper;

public interface IAsyncInitialization
{
    Task Initialization { get; }
}