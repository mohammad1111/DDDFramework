namespace Gig.Framework.Core.Events;

[AttributeUsage(AttributeTargets.Class)]
public class EventPriority : Attribute
{
    public EventPriority(byte priority)
    {
        Priority = priority;
    }

    public byte Priority { get; set; }
}