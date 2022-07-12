using System.Security.Authentication;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.Security;
using Gig.Framework.Core.Settings;
using Gig.Framework.Core.UserContexts;

namespace Gig.Framework.Core;

public class RequestContext : IRequestContext
{
    private readonly ISecurityManager _securityManager;
    private readonly IRequestMemoryCacheManager _requestMemoryCacheManager;
    private readonly IUserContextService _userContextService;
    private readonly IDataSetting _dataSetting;
    public RequestContext(IUserContextService userContextService, ISecurityManager securityManager,IRequestMemoryCacheManager requestMemoryCacheManager, IDataSetting dataSetting)
    {
        _userContextService = userContextService;
        _securityManager = securityManager;
        _requestMemoryCacheManager = requestMemoryCacheManager;
        _dataSetting = dataSetting;
    }


    public IUserContext GetUserContext()
    {
        if (_userContextService.UserContext == null) throw new AuthenticationException();
        return _userContextService.UserContext;
    }

    public string GetToken()
    {
        var user = GetUserContext();
        var token= _securityManager.GetAccessToken(
            new TokenClaimModel {
                BranchId = user.BranchId,
                CompanyId = user.CompanyId,
                IsAdmin = user.IsAdmin,
                LastName = user.DisplayName,
                UserId = user.UserId,
                LangTypeCode = user.LangTypeCode
            },
            new BearerTokenOptionsModel {
                Audience = "Any",
                Issuer = "Any",
                Key = _dataSetting.TokenKey,
                AccessTokenExpirationMinutes = 30,
                RefreshTokenExpirationMinutes = 60
            });


        return token;
    }

    public async Task<bool> HasPermission(string[] operations)
    {
      //  var accessService = serviceLocator.Current.Resolve<ISecurityManager>();
      var userContext = _userContextService.UserContext;
        var level = await _securityManager.GetAccessLevel( userContext.UserId, userContext.CompanyId, operations);
        await SetCacheAccessLevel(level);
        return level > 0;
    }

    public async Task<int> GetAccessLevel()
    {
      //  var cacheManager = serviceLocator.Current.Resolve<IRequestMemoryCacheManager>();
        if (await _requestMemoryCacheManager.Exists(new AccessLevelCacheKey()))
            return await _requestMemoryCacheManager.GetAsync<int>(new AccessLevelCacheKey());

        return 2;
    }

    private async Task SetCacheAccessLevel(int level)
    {

        var oldLevel = await _requestMemoryCacheManager.Exists(new AccessLevelCacheKey())
            ? await
                _requestMemoryCacheManager.GetAsync<int?>(new AccessLevelCacheKey())
            : null;
        if (oldLevel == null && level > 0)
            await _requestMemoryCacheManager.AddAsync(new AccessLevelCacheKey(), level);
        else if (oldLevel != null && level > oldLevel) await _requestMemoryCacheManager.UpdateAsync(new AccessLevelCacheKey(), level);
    }
}

public class UserContext : IUserContext
{
    private readonly long _companyId;

    public UserContext(string displayName, long userId, long companyId, long branchId,
        string tokenId, string userCompanies, bool isAdmin, int langTypeCode, long subSystemId)
    {
        Created = DateTime.Now;
        TraceId = Guid.NewGuid();
        UserId = userId;
        BranchId = branchId;
        DisplayName = displayName;
        TokenId = tokenId;
        _companyId = companyId;
        UserCompanies = userCompanies?.Split(',').Select(long.Parse).ToList();
        // var cache = ServiceLocator.Current.Resolve<IDistributeCacheManager>();
        // cache.AddOrUpdateAsync(tokenId, token);
        //var operationsJson = cache.Get<string>(UserId + "_" + TokenId);
        IsAdmin = isAdmin;
        LangTypeCode = langTypeCode;
        SubSystemId = subSystemId;
        // if (operationsJson != null)
        // {
        //     var operationsObject = JsonConvert.DeserializeObject<IList<Operation>>(operationsJson);
        //     Operations = operationsObject.Select(x => x.Id).ToArray();
        // }
    }

    public string TokenId { get; }

    public DateTime Created { get; }


    public Guid TraceId { get; }


    public long UserId { get; }


    public long CompanyId
    {
        get
        {
            if (_companyId != 0) return _companyId;

            throw new Exception("دسترسی نامعتبر است");
        }
    }


    public long BranchId { get; }

    public int LangTypeCode { get; }

    public long SubSystemId { get; }


    public string DisplayName { get; }


    public int[] Operations { get; }


    public List<long> UserCompanies { get; }

    public bool IsAdmin { get; }



}