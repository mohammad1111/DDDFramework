namespace Gig.Framework.EventBus;

public class RetryConfig
{
    /// <summary>
    ///     Count Retry
    /// </summary>
    public int CountRetry { get; set; }

    /// <summary>
    ///     The time interval is repeated each time
    /// </summary>
    public TimeSpan Interval { get; set; }
}