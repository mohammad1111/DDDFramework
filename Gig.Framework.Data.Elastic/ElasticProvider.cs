using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Gig.Framework.Core.DataProviders.Elastic;
using Gig.Framework.Core.Settings;
using Nest;

namespace Gig.Framework.Data.Elastic;

public class ElasticProvider<T> : IElasticProvider<T> where T : class, IElasticDocument
{
    private readonly string _alias;
    private readonly IDataSetting _dataSetting;
    private DateTime _aliasUpdateDateTime = DateTime.UtcNow.AddYears(-50);


    public ElasticProvider(IDataSetting dataSetting)
    {
        string indexName;
        _dataSetting = dataSetting;
        _alias = _dataSetting.ElasticAlias;
        if (_dataSetting.ElasticIndexPerMonth)
            indexName = $"{_alias}-{DateTime.UtcNow:yyyy-MM}";
        else
            indexName = $"{_alias}-{DateTime.UtcNow:yyyy-MM-dd}";

        var pool = new StaticConnectionPool(new List<Uri>
        {
            new(_dataSetting.ElasticUrl)
        });

        var connectionSettings = new ConnectionSettings(pool)
            .DefaultMappingFor<CrudAuditLogElasticDocument>(m => m
                    .IndexName(indexName)
                // .PropertyName(p => p.Timestamp, "@timestamp")
            )
            .DefaultMappingFor<T>(m => m
                .IndexName(indexName)
            );
        // .DefaultFieldNameInferrer(fieldName => fieldName);

        ElasticClient = new ElasticClient(connectionSettings);

        Initialize();
    }

    protected ElasticClient ElasticClient { get; }

    public async Task AddDocumentAsync(T document)
    {
        var indexRequest = new IndexRequest<T>(document);

        var response = await ElasticClient.IndexAsync(indexRequest);
        if (!response.IsValid) throw new ElasticsearchClientException("Document could not be indexed!");
    }

    public void AddDocument(T document)
    {
        var indexRequest = new IndexRequest<T>(document);

        var response = ElasticClient.Index(indexRequest);
        if (!response.IsValid) throw new ElasticsearchClientException("Document could not be indexed!");
    }

    public async Task<IEnumerable<T>> SearchDocumentsAsync(string simpleQuery = "*", ElasticPaging paging = null)
    {
        paging = GetPaging(paging);
        var searchResponse = await ElasticClient.SearchAsync<T>(s => s
            .SimpleQuery(simpleQuery)
            .ApplyPaging(paging)
        );

        return searchResponse.Documents;
    }

    public IEnumerable<T> SearchDocuments(string simpleQuery = "*", ElasticPaging paging = null)
    {
        paging = GetPaging(paging);
        var searchResponse = ElasticClient.Search<T>(s => s
            .SimpleQuery(simpleQuery)
            .ApplyPaging(paging)
        );

        return searchResponse.Documents;
    }

    public async Task<long> CountDocumentsAsync(string simpleQuery = "*")
    {
        var searchResponse = await ElasticClient.SearchAsync<T>(s => s
            .SimpleQuery(simpleQuery)
        );

        return searchResponse.Total;
    }

    public long CountDocuments(string simpleQuery = "*")
    {
        var searchResponse = ElasticClient.Search<T>(s => s
            .SimpleQuery(simpleQuery)
        );

        return searchResponse.Total;
    }

    private void Initialize()
    {
        EnsureAlias();
    }

    protected ElasticPaging GetPaging(ElasticPaging paging)
    {
        var from = ElasticPaging.Default.From;
        var size = ElasticPaging.Default.Size;
        if (paging == null) return new ElasticPaging(from, size);
        from = paging.From;
        size = paging.Size;
        if (size > 1000)
            // max limit 1000 items
            size = 1000;

        return new ElasticPaging(from, size);
    }

    private void CreateAliasForAllIndices()
    {
        var response =
            ElasticClient.Indices.AliasExists(new AliasExistsRequest(new Names(new List<string> { _alias })));

        if (response.Exists)
            ElasticClient.Indices.DeleteAlias(new DeleteAliasRequest(Indices.Parse($"{_alias}-*"), _alias));

        var responseCreateIndex =
            ElasticClient.Indices.PutAlias(new PutAliasRequest(Indices.Parse($"{_alias}-*"), _alias));
        if (!responseCreateIndex.IsValid) throw response.OriginalException;
    }

    private void CreateAlias()
    {
        if (_dataSetting.AmountOfPreviousElasticIndicesUsedInAlias > 0)
            CreateAliasForLastNIndices(_dataSetting.AmountOfPreviousElasticIndicesUsedInAlias);
        else
            CreateAliasForAllIndices();
    }

    private void CreateAliasForLastNIndices(int amount)
    {
        var responseCatIndices = ElasticClient.Cat.Indices(new CatIndicesRequest(Indices.Parse($"{_alias}-*")));
        var records = responseCatIndices.Records.ToList();
        var indicesToAddToAlias = new List<string>();
        for (var i = amount; i > 0; i--)
            if (_dataSetting.ElasticIndexPerMonth)
            {
                var indexName = $"{_alias}-{DateTime.UtcNow.AddMonths(-i + 1):yyyy-MM}";
                if (records.Exists(t => t.Index == indexName)) indicesToAddToAlias.Add(indexName);
            }
            else
            {
                var indexName = $"{_alias}-{DateTime.UtcNow.AddDays(-i + 1):yyyy-MM-dd}";
                if (records.Exists(t => t.Index == indexName)) indicesToAddToAlias.Add(indexName);
            }

        var response =
            ElasticClient.Indices.AliasExists(new AliasExistsRequest(new Names(new List<string> { _alias })));

        if (response.Exists)
            ElasticClient.Indices.DeleteAlias(new DeleteAliasRequest(Indices.Parse($"{_alias}-*"), _alias));

        Indices multipleIndicesFromStringArray = indicesToAddToAlias.ToArray();
        var responseCreateIndex =
            ElasticClient.Indices.PutAlias(new PutAliasRequest(multipleIndicesFromStringArray, _alias));
        if (!responseCreateIndex.IsValid) throw responseCreateIndex.OriginalException;
    }

    private void EnsureAlias()
    {
        if (_dataSetting.ElasticIndexPerMonth)
        {
            if (_aliasUpdateDateTime.Date < DateTime.UtcNow.AddMonths(-1).Date)
            {
                _aliasUpdateDateTime = DateTime.UtcNow;
                CreateAlias();
            }
        }
        else
        {
            if (_aliasUpdateDateTime.Date < DateTime.UtcNow.AddDays(-1).Date)
            {
                _aliasUpdateDateTime = DateTime.UtcNow;
                CreateAlias();
            }
        }
    }
}