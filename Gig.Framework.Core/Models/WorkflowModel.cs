namespace Gig.Framework.Core.Models;

public class WorkflowModel
{
    public long CompanyId { get; set; }
    public long RecId { get; set; }
    public int StatusCode { get; set; }
    public string Paraph { get; set; }
    public long UserId { get; set; }
    public long BranchId { get; set; }
    public Dictionary<string, string> ParmList { get; set; }
}