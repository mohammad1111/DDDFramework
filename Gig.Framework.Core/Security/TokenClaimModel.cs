namespace Gig.Framework.Core.Security;

public sealed class TokenClaimModel
{
    public long UserId { get; set; }

    public string LastName { get; set; }

    public long CompanyId { get; set; }

    public long BranchId { get; set; }

    public int LangTypeCode { get; set; }

    public List<long> ListOfUserCompanies { get; set; }

    public bool IsAdmin { get; set; }
}