namespace Gig.Framework.Core.Models;

[Serializable]
public abstract class Dto
{
    public long Id { get; set; }

    public long CompanyId { get; set; }

    public long BranchId { get; set; }

    public DateTime CreatedOn { get; set; }

    public long CreatedBy { get; set; }

    public byte[] RowVersion { get; set; }
}