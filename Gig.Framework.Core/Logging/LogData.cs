using Gig.Framework.Core.Serilizer;

namespace Gig.Framework.Core.Logging;

public class LogData<T>
{
    public LogData(T input, Guid id, ISerializer serializer)
    {
        var inputSerialize = typeof(T).IsClass
            ? serializer.Serialize(input)
            : input.ToString();

        Input = inputSerialize;
        OccuredOn = DateTime.Now.ToLongDateString();
        Id = id;
    }

    public Guid Id { get; set; }

    public string Input { get; set; }

    public string OccuredOn { get; set; }
}

public class Audit
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Type { get; set; }
    public string TableName { get; set; }
    public DateTime DateTime { get; set; }

    public IEnumerable<AuditColumnChanges> Changes { get; set; }
    public long PrimaryKey { get; set; }
}

public class AuditColumnChanges
{
    public string ColumnName { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}

public enum AuditType
{
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 3
}