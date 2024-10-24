using Microsoft.IdentityModel.Tokens;

namespace Application.Providers.Auth.Token;

public class JwtBearerConfig
{
    public static string Issuer = "IRat mobile";
    public SigningCredentials SigningCredentials { get; set; }
    public TimeSpan AccessTokenLifetime { get; } = TimeSpan.FromMinutes(15);
    public DateTime AccessTokenExpiration() => DateTime.UtcNow.Add(AccessTokenLifetime);
    public TimeSpan RefreshTokenLifetime { get; } = TimeSpan.FromDays(7);
    public DateTime RefreshTokenExpiration() => DateTime.UtcNow.Add(RefreshTokenLifetime);

}