using Gig.Framework.Core.Logging;
using Gig.Framework.Core.Serilizer;

namespace Gig.Framework.Core.Events;

public class EventLogData<T> : LogData<T>
{
    public EventLogData(string name, T input, ISerializer serializer) : base(input, Guid.NewGuid(), serializer)
    {
        Name = name;
    }

    public string Name { get; set; }
}