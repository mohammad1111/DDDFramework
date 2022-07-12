namespace Gig.Framework.RuleEngine.Contract.Models;

public class SelectableWarningModel
{
    public long Id { get; set; }


    public Guid RuleId { get; set; }

    public List<ItemValue> Items { get; set; }
}