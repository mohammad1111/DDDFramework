using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Gig.Framework.Core.Caching;
using Gig.Framework.Core.DataProviders;
using Gig.Framework.Core.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Gig.Framework.Core.Security;

public class SecurityManager : ISecurityManager
{
    private readonly IDataSetting _dataSetting;
    private readonly IDistributeCacheManager _distributeCacheManager;
    private readonly IUserAccessDataProvider _userAccessDataProvider;

    public SecurityManager(IDistributeCacheManager distributeCacheManager,
        IUserAccessDataProvider userAccessDataProvider, IDataSetting dataSetting)
    {
        _distributeCacheManager = distributeCacheManager;
        _userAccessDataProvider = userAccessDataProvider;
        _dataSetting = dataSetting;
    }

    public IUserContext Validate(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_dataSetting.TokenKey)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new AuthenticationException("Invalid token");
        return new UserContext(
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "DisplayName".ToLower()).Value,
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "userId".ToLower()).Value),
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "companyId".ToLower())
                .Value),
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "branchId".ToLower()).Value),
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "tokenId".ToLower()).Value,
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "ListOfUserCompanies".ToLower()).Value,
            Convert.ToBoolean(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "isAdmin".ToLower())
                .Value),
            Convert.ToInt32(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "langTypeCode".ToLower())
                .Value),
            Convert.ToInt64(_dataSetting.SubSystemId)
        );
    }

    public IUserContext ValidateWithoutExpireTime(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_dataSetting.TokenKey)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new AuthenticationException("Invalid token");
        return new UserContext(
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "DisplayName".ToLower()).Value,
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "userId".ToLower()).Value),
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "companyId".ToLower())
                .Value),
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "branchId".ToLower()).Value),
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "tokenId".ToLower()).Value,
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "ListOfUserCompanies".ToLower()).Value,
            Convert.ToBoolean(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "isAdmin".ToLower())
                .Value),
            Convert.ToInt32(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "langTypeCode".ToLower())
                .Value),
            Convert.ToInt64(_dataSetting.SubSystemId));
    }

    public IUserContext Read(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_dataSetting.TokenKey)),
            ValidateLifetime = false
        };
        SecurityToken securityToken = null;
        var tokenHandler = new JwtSecurityTokenHandler();
        bool validate;
        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            validate = true;
        }
        catch (Exception)
        {
            validate = false;
        }

        if (!validate) throw new AuthenticationException("مجوز شما باطل شده است");
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        return new UserContext(
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "DisplayName".ToLower()).Value,
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "userId".ToLower()).Value),
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "companyId".ToLower())
                .Value),
            Convert.ToInt64(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "branchId".ToLower()).Value),
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "tokenId".ToLower()).Value,
            jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "ListOfUserCompanies".ToLower()).Value,
            Convert.ToBoolean(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "isAdmin".ToLower())
                .Value),
            Convert.ToInt32(jwtSecurityToken.Claims.First(claim => claim.Type.ToLower() == "langTypeCode".ToLower())
                .Value),
            Convert.ToInt64(_dataSetting.SubSystemId));
    }

    public string CreateAccessToken(TokenClaimModel model, BearerTokenOptionsModel options)
    {
        var valueIssuer = options.Issuer;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String, valueIssuer),
            new(JwtRegisteredClaimNames.Iss, valueIssuer, ClaimValueTypes.String, valueIssuer),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64, valueIssuer),
            new(ClaimTypes.NameIdentifier, model.UserId.ToString(), ClaimValueTypes.String, valueIssuer),
            new(ClaimTypes.Name, model.LastName, ClaimValueTypes.String, valueIssuer),
            new("DisplayName", model.LastName, ClaimValueTypes.String, valueIssuer),
            new("userId", model.UserId.ToString(), ClaimValueTypes.String, valueIssuer),
            new("companyId", model.CompanyId.ToString(), ClaimValueTypes.String, valueIssuer),
            new("langTypeCode", model.LangTypeCode.ToString(), ClaimValueTypes.String, valueIssuer),
            new("branchId", model.BranchId.ToString(), ClaimValueTypes.String, valueIssuer),
            // new("ListOfUserCompanies", string.Join(",", model.ListOfUserCompanies), ClaimValueTypes.String,
            //     valueIssuer),
            new("tokenId", Guid.NewGuid().ToString(), ClaimValueTypes.String, valueIssuer),
            new("isAdmin", model.IsAdmin.ToString(), ClaimValueTypes.String, valueIssuer),
          //+  new("TraceId", model.LastName, ClaimValueTypes.String, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var notBefore = DateTime.UtcNow.AddMinutes(-1);
        var expires = notBefore.AddMinutes(options.AccessTokenExpirationMinutes);
        var token = new JwtSecurityToken(
            valueIssuer,
            options.Audience,
            claims,
            notBefore,
            expires,
            credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<int> GetAccessLevel(long userId, long companyId, string permissions)
    {
        if (string.IsNullOrWhiteSpace(permissions)) return 0;
        var lPermissions = permissions.Split(',');
        return await GetAccessLevel(userId, companyId, lPermissions);
    }

    public string GetAccessToken(TokenClaimModel model, BearerTokenOptionsModel options)
    {
        _ = model ?? throw new ArgumentNullException(nameof(model));
        _ = options ?? throw new ArgumentNullException(nameof(options));
        var token = CreateAccessToken(model, options);
        _distributeCacheManager.AddByExpireTimeAsync(token, token, ExpirationMode.Sliding, new TimeSpan(6, 0, 0));
        return token;
    }

    public async Task<int> GetAccessLevel(long userId, long companyId, string[] permissions)
    {
        if (permissions == null || permissions.Length == 0 || userId <= 0 || companyId <= 0) return 0;
        return await _userAccessDataProvider.GetAccessLevel(userId, companyId, permissions);
    }
}