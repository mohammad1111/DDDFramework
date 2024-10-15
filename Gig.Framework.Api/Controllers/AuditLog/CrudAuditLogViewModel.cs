using System;
using System.Collections.Generic;
using Gig.Framework.Core.DataProviders.Elastic;
using Gig.Framework.Core.Logging;
using Gig.Framework.Data.Elastic;
using Gig.Framework.ReadModel.Models;

namespace Gig.Framework.Api.Controllers.AuditLog;

public class CrudAuditLogViewModel : ViewModel
{
    public long UserId { get; set; }
    public string Type { get; set; }
    public DateTime DateTime { get; set; }
    public string TableName { get; set; }
    public long PrimaryKey { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }

    public IEnumerable<AuditColumnChanges> Changes { get; set; }

    public static CrudAuditLogViewModel FromElasticDocument(CrudAuditLogElasticDocument document)
    {
        //todo: find a way to get typed result from serilog logs
        var fields = document.Fields.ToAnonymousObject();
        fields.Audit = (fields.Audit as Dictionary<string, object>).ToAnonymousObject();
        var changes = new List<AuditColumnChanges>();
        foreach (Dictionary<string, object> change in fields.Audit.Changes)
            changes.Add(new AuditColumnChanges
            {
                ColumnName = change[nameof(AuditColumnChanges.ColumnName)].ToString(),
                OldValue = change[nameof(AuditColumnChanges.OldValue)].ToString(),
                NewValue = change[nameof(AuditColumnChanges.NewValue)].ToString()
            });

        return new CrudAuditLogViewModel
        {
            UserId = fields.Audit.UserId,
            Type = fields.Audit.Type,
            DateTime = System.DateTime.Parse(fields.Audit.DateTime),
            TableName = fields.Audit.TableName,
            PrimaryKey = fields.Audit.PrimaryKey,
            Changes = changes
        };
    }
}