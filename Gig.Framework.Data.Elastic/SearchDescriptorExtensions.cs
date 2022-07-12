using Gig.Framework.Core.DataProviders.Elastic;
using Nest;
using static Gig.Framework.Core.DataProviders.Elastic.ElasticConstants;

namespace Gig.Framework.Data.Elastic;

public static class SearchDescriptorExtensions
{
    public static SearchDescriptor<T> SimpleQuery<T>(this SearchDescriptor<T> searchDescriptor, string simpleQuery)
        where T : class
    {
        return searchDescriptor.Query(q => q
            .SimpleQueryString(c => c
                .Query(simpleQuery)
            )
        );
    }

    public static SearchDescriptor<T> ApplyPaging<T>(this SearchDescriptor<T> searchDescriptor,
        ElasticPaging paging)
        where T : class
    {
        return searchDescriptor
            .From(paging.From)
            .Size(paging.Size);
    }

    public static SearchDescriptor<T> SortByDescendingTimestamp<T>(this SearchDescriptor<T> searchDescriptor)
        where T : class
    {
        return searchDescriptor
            .Sort(sf => sf.Field(DefaultTimestampFieldName, SortOrder.Descending));
    }

    public static SearchDescriptor<T> QueryAuditLogsForRecord<T>(this SearchDescriptor<T> searchDescriptor,
        string tableName, long id)
        where T : CrudAuditLogElasticDocument
    {
        //todo: find a way to get typed result from serilog logs
        return searchDescriptor
            .Query(q => q
                    .Term(t => t.Field("fields.Audit.TableName")
                        .Value(tableName.ToLower())
                    ) && q
                    .Term(t => t.Field("fields.Audit.Id")
                        .Value(id)
                    )
            );
    }
}