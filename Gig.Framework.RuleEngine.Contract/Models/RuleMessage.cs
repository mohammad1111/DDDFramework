namespace Gig.Framework.RuleEngine.Contract.Models;

public class RuleMessage
{
    public string Message { get; set; }

    public string Code { get; set; }

    public Guid RuleId { get; set; }

    public Exception Exception { get; set; }
}