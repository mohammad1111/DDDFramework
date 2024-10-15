using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Gig.Framework.Core.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Gig.Framework.Api.Security;

public class GigSecurityToken : ISecurityToken
{
    private readonly IEnumerable<Claim> _claims;

    public GigSecurityToken(IHttpContextAccessor httpContextAccessor)
    {
        _claims = httpContextAccessor.HttpContext?.User.Identities.FirstOrDefault()?.Claims;
        BranchId = long.Parse(_claims?.First(x => x.Type == "branchId").Value ?? "0");
        CompanyId = long.Parse(_claims?.First(x => x.Type == "companyId").Value ?? "0");
        UserId = long.Parse(_claims?.First(x => x.Type == "userId").Value ?? "0");
        FullName = _claims?.First(x => x.Type == "DisplayName").Value ?? "";
        LangTypeCode = int.Parse(_claims?.First(x => x.Type == "langTypeCode").Value ?? "0");
        Token = httpContextAccessor.HttpContext?.GetTokenAsync("access_token")?.Result;
    }


    public long CompanyId { get; }


    public long BranchId { get; }


    public long UserId { get; }


    public int LangTypeCode { get; }


    //  public bool IsAdmin { get; }


    public string Token { get; }


    //   public string RefreshToken { get; }


    public string FullName { get; }


    public IEnumerable<UserBranch> Branches { get; set; }

    public string TokenString => $"Bearer {Token}";
}