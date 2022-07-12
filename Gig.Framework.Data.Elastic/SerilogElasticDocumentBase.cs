using Gig.Framework.Core.DataProviders.Elastic;

namespace Gig.Framework.Data.Elastic;

public class SerilogElasticDocumentBase : ElasticDocumentBase
{
    public string Level { get; set; }

    public string MessageTemplate { get; set; }

    public string Message { get; set; }
}