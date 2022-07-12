using Gig.Framework.Core.DataProviders.Elastic;

namespace Gig.Framework.Data.Elastic;

public interface ICrudAuditLogElasticProvider : IElasticProvider<CrudAuditLogElasticDocument>
{
    IEnumerable<CrudAuditLogElasticDocument> GetCrudAuditLogs(string tableName, long id,
        ElasticPaging paging = null);

    Task<IEnumerable<CrudAuditLogElasticDocument>> GetCrudAuditLogsAsync(string tableName, long id,
        ElasticPaging paging = null);
}