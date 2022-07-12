namespace Gig.Framework.Core.DataProviders.Elastic;

public interface IElasticProvider<T>
{
    void AddDocument(T document);

    Task AddDocumentAsync(T document);

    IEnumerable<T> SearchDocuments(string filter = "*", ElasticPaging paging = null);

    Task<IEnumerable<T>> SearchDocumentsAsync(string filter = "*", ElasticPaging paging = null);

    long CountDocuments(string filter);

    Task<long> CountDocumentsAsync(string simpleQuery = "*");
}