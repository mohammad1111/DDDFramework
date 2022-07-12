using Nest;
using Newtonsoft.Json;
using static Gig.Framework.Core.DataProviders.Elastic.ElasticConstants;

namespace Gig.Framework.Core.DataProviders.Elastic;

[ElasticsearchType(RelationName = "_doc")]
public class ElasticDocumentBase : IElasticDocument
{
    protected ElasticDocumentBase()
    {
        Timestamp = DateTimeOffset.Now;
    }

    [JsonProperty(DefaultTimestampFieldName)]
    public DateTimeOffset Timestamp { get; set; }

    // public Dictionary<string, object[]> Renderings {get; set; }
}