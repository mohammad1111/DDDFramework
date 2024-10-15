using System;
using Gig.Framework.Core.Logging;
using Gig.Framework.Core.Serilizer;

namespace Gig.Framework.Application;

public class CommandLogData<T> : LogData<T>
{
    public CommandLogData(string name, T input, ISerializer serializer) : base(input, Guid.NewGuid(), serializer)
    {
        Name = name;
    }

    public string Name { get; set; }
}