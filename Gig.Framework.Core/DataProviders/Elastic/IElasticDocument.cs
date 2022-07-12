namespace Gig.Framework.Core.DataProviders.Elastic;

public interface IElasticDocument
{
    public DateTimeOffset Timestamp { get; set; }
}