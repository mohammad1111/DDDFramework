namespace Gig.Framework.RuleEngine.Contract.Models;

public class GigWarningRuleModel
{
    public string Message { get; set; }

    public WarningMode WarningMode { get; set; }

    public bool IsSelected { get; set; }

    public string Code { get; set; }

    //     public List<SelectableWarningModel> SelectableWarningModels { get; set; }
}