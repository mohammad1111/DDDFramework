namespace Gig.Framework.RuleEngine.Contract.Models;

public class RuleEvent
{
    public RuleEventType RuleEventType { get; set; }

    public Guid RuleId { get; set; }

    public string RuleType { get; set; }
    public dynamic EngineEvent { get; set; }
}