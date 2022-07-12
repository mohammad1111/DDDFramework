using Gig.Framework.Core.Enums;

namespace Gig.Framework.Core.DataProviders;

public class BaseModel
{
    public long Id { get; set; }
    public Guid RecGuid { get;  set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long OwnerId { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedOn { get; protected set; } = DateTime.Now;
    public long ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; } = DateTime.Now;
    public StateCodeEnum StateCode { get; set; }
    public bool IsDeleted { get; protected set; }
    public virtual byte[] RowVersion { get; protected set; }
}