using System.Collections.Generic;

namespace Gig.Framework.Data.Elastic;

public class WebApplicationLogElasticDocument : SerilogElasticDocumentBase
{
    //todo: find a way to get typed result from serilog logs
    // public LogFieldsBase Fields { get; set; }
    public Dictionary<string, object> Fields { get; set; }
}