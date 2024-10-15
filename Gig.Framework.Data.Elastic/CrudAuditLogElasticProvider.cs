using System.Collections.Generic;
using System.Threading.Tasks;
using Gig.Framework.Core.DataProviders.Elastic;
using Gig.Framework.Core.Settings;

namespace Gig.Framework.Data.Elastic;

public class CrudAuditLogElasticProvider : ElasticProvider<CrudAuditLogElasticDocument>,
    ICrudAuditLogElasticProvider
{
    public CrudAuditLogElasticProvider(IDataSetting dataSetting) : base(dataSetting)
    {
    }

    public IEnumerable<CrudAuditLogElasticDocument> GetCrudAuditLogs(string tableName, long id,
        ElasticPaging paging = null)
    {
        paging = GetPaging(paging);
        var searchResponse = ElasticClient.Search<CrudAuditLogElasticDocument>(s => s
            .QueryAuditLogsForRecord(tableName, id)
            .ApplyPaging(paging)
            .SortByDescendingTimestamp()
        );

        return searchResponse.Documents;
    }

    public async Task<IEnumerable<CrudAuditLogElasticDocument>> GetCrudAuditLogsAsync(string tableName, long id,
        ElasticPaging paging = null)
    {
        paging = GetPaging(paging);
        var searchResponse = await ElasticClient.SearchAsync<CrudAuditLogElasticDocument>(s => s
            .QueryAuditLogsForRecord(tableName, id)
            .ApplyPaging(paging)
            .SortByDescendingTimestamp()
        );

        return searchResponse.Documents;
    }
}