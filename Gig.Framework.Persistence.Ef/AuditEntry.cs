using Gig.Framework.Core.Exceptions;
using Gig.Framework.Core.Logging;
using Gig.Framework.Core.Serilizer;
using Gig.Framework.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Gig.Framework.Persistence.Ef;

public class AuditEntry
{
    private readonly ISerializer _serializer;

    public AuditEntry(EntityEntry entry, ISerializer serializer)
    {
        _serializer = serializer;
        Entry = entry;
    }

    public EntityEntry Entry { get; }
    public long UserId { get; set; }
    public string TableName { get; set; }
    public Dictionary<string, object> KeyValues { get; } = new();
    public Dictionary<string, object> OldValues { get; } = new();
    public Dictionary<string, object> NewValues { get; } = new();
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = new();

    public Audit ToAudit()
    {
        if (!KeyValues.TryGetValue("Id", out var id))
            throw new FrameworkException("Id not found in KeyValues dictionary.");

        if (!long.TryParse(id.ToString(), out var primaryKey))
            throw new FrameworkException("Invalid Id in KeyValues dictionary.");


        var audit = new Audit
        {
            Id = primaryKey,
            UserId = UserId,
            Type = AuditType.ToString(),
            TableName = TableName,
            DateTime = DateTime.Now,
            PrimaryKey = primaryKey,
            Changes = OldValues.OrderBy(p => p.Key).Select(p => new AuditColumnChanges
            {
                ColumnName = p.Key,
                OldValue = _serializer.Serialize(p.Value),
                NewValue = _serializer.Serialize(NewValues[p.Key])
            })
        };
        return audit;
    }
}