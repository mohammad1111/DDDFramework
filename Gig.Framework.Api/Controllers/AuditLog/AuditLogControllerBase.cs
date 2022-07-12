using Gig.Framework.Api.Models;
using Gig.Framework.Core.DataProviders.Elastic;
using Gig.Framework.Data.Elastic;
using Gig.Framework.ReadModel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gig.Framework.Api.Controllers.AuditLog;

public abstract class AuditLogControllerBase : GigController
{
    private readonly ICrudAuditLogElasticProvider _crudAuditLogElasticProvider;

    private readonly IElasticProvider<WebApplicationLogElasticDocument> _elasticProvider;

    protected AuditLogControllerBase(ICrudAuditLogElasticProvider crudAuditLogElasticProvider,
        IGigControllerDependencies dependencies,
        IElasticProvider<WebApplicationLogElasticDocument> elasticProvider) : base(dependencies)
    {
        _crudAuditLogElasticProvider = crudAuditLogElasticProvider;
        _elasticProvider = elasticProvider;
    }

    [NonAction]
    public async Task<GigQueryResultViewModel<CrudAuditLogViewModel>> GetAuditLogs(
        WebApiDataSourceLoadOptions loadOptions,
        string tableName, long id)
    {
        var auditLogs = (await _crudAuditLogElasticProvider.GetCrudAuditLogsAsync(tableName, id))
            .Select(CrudAuditLogViewModel.FromElasticDocument).ToList();
        var fakeId = 0;
        foreach (var auditLog in auditLogs) auditLog.Id = ++fakeId;

        return GigQueryResultViewModel<CrudAuditLogViewModel>.LoadData(auditLogs, loadOptions);
    }

    [NonAction]
    public async Task<GigQueryResultViewModel<WebApplicationLogViewModel>> SearchLogs(
        WebApiDataSourceLoadOptions loadOptions,
        string simpleQuery)
    {
        var logs = (await _elasticProvider.SearchDocumentsAsync(simpleQuery))
            .Select(WebApplicationLogViewModel.FromElasticDocument).ToList();
        var fakeId = 0;
        foreach (var log in logs) log.Id = ++fakeId;

        return GigQueryResultViewModel<WebApplicationLogViewModel>.LoadData(logs, loadOptions);
    }
}