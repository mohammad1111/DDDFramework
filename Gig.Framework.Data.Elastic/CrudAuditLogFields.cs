using Gig.Framework.Core.Logging;

namespace Gig.Framework.Data.Elastic;

public class CrudAuditLogFields : LogFieldsBase
{
    public Guid TraceId { get; set; }
    public Audit Audit { get; set; }
}