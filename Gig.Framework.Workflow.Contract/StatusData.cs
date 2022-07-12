namespace Gig.Framework.Workflow.Contract;

public class StatusData
{
    public string Name { get; set; }

    public int FromStatus { get; set; }

    public int ToStatus { get; set; }

    public string Bookmark { get; set; }

    public List<long> UserIds { get; set; } = new();
}