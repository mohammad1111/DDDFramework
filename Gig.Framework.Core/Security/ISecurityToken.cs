namespace Gig.Framework.Core.Security;

public interface ISecurityToken
{
    long CompanyId { get; }


    long BranchId { get; }


    long UserId { get; }


    int LangTypeCode { get; }


    //  bool IsAdmin { get; }


    string Token { get; }


    //   string RefreshToken { get; }


    string FullName { get; }


    IEnumerable<UserBranch> Branches { get; set; }


    string TokenString { get; }
}