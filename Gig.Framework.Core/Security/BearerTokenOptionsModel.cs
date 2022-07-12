namespace Gig.Framework.Core.Security;

public class BearerTokenOptionsModel
{
    public string Issuer { get; set; }

    public string Audience { get; set; }

    public string Key { get; set; }

    public double AccessTokenExpirationMinutes { get; set; }

    public double RefreshTokenExpirationMinutes { get; set; }
}