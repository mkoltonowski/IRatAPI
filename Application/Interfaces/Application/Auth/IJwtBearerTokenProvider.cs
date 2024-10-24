using Domain.Aggregates.Identity;

namespace Application.Interfaces.Application.Auth;

public interface IJwtBearerTokenProvider
{
    Task<string> Authenticate(User user);
    Task<string> Refresh(string accessToken, string refreshToken);
    bool CheckIfExpired(string accessToken);
    Task<string> RevokeTokens(User user);
    void SetTokenCookies(string accessToken, string refreshToken, DateTime? expires);
}