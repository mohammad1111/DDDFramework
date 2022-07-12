using Gig.Framework.Core.Caching;

namespace Gig.Framework.Core.DataProviders.CacheKeys;

public class AuditingEntityCacheKey : CacheKey
{
    public AuditingEntityCacheKey(
        string schemaName,
        long companyId)
    {
        SchemaName = schemaName;
        CompanyId = companyId;
    }

    public string SchemaName { get; }
    public long CompanyId { get; }
}