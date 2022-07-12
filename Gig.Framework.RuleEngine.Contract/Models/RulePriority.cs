namespace Gig.Framework.RuleEngine.Contract.Models;

public class RulePriority
{
    public RulePriority(Type type, int priority)
    {
        Type = type;
        Priority = priority;
    }

    public Type Type { get; }

    public int Priority { get; }
}