using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Application.Interfaces.Application.Auth;
using Application.Providers.Auth.Token;
using Application.Providers.Identity;
using Domain.Aggregates.Identity;
using Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Domain.Constants.Cookies;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Application.Providers.Auth;

public class JwtBearerTokenProvider: IJwtBearerTokenProvider
{
    private readonly IdentityUserManager _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtBearerConfig _jwtBearerConfig;

    public JwtBearerTokenProvider(IdentityUserManager userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        
    }
    
    private AccessToken CreateAccessToken(User user, IList<Claim> claims)
    {   
        var tokenId = Guid.NewGuid().ToString();
        var tokenClaims = new List<Claim>
        {
            new (Claims.Id, tokenId),
            new (Claims.UserType, tokenId),
            new (JwtRegisteredClaimNames.Sub, user.Email),
            new (JwtRegisteredClaimNames.Jti, tokenId),
        }.Concat(claims);
        
        var token = new JwtSecurityToken(
            issuer: JwtBearerConfig.Issuer, 
            claims: tokenClaims,
            expires: _jwtBearerConfig.AccessTokenExpiration(), 
            signingCredentials: _jwtBearerConfig.SigningCredentials
        );
        
        return new AccessToken ()
        {
            TokenId = tokenId,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
        };
    }

    private RefreshToken CreateRefreshToken()
    {
        var randomNumber = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(randomNumber);
        var expires = _jwtBearerConfig.RefreshTokenExpiration();
        var created = DateTime.UtcNow;

        return new RefreshToken() { Token = token, Expires = expires, Created = created };
    }

    public async Task<string> Authenticate(User user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var accesstoken = CreateAccessToken(user, claims);
        var refreshToken = CreateRefreshToken();
        var expires = refreshToken.Expires;
        
        SetTokenCookies(accesstoken.Token, refreshToken.Token, expires);
        await SetUserToken(user, accesstoken, refreshToken);
        return accesstoken.Token;
    }

    public async Task<string> Refresh(string accessToken, string refreshToken)
    {
        var jwtToken = new JwtSecurityToken(accessToken);
        var user = await _userManager.Users
            .Where(user => user.AccessTokenId == jwtToken.Id )
            .Where(user => user.IsEnabled)
            .Where(user => !user.ClientUser.IsBanned)
            .Where(user => user.RefreshToken == refreshToken )
            .Where(user => user.TokenExpires >= DateTime.UtcNow )
            .Where(user => user.ClientUser.BannedUntil <= DateTime.UtcNow )
            .SingleOrDefaultAsync();

        return user is not null ? await Authenticate(user) : await RevokeTokens(user);
    }

    public bool CheckIfExpired(string accessToken)
    {
        var token = new JwtSecurityToken(accessToken);
        return token.ValidTo < DateTime.UtcNow;
    }

    public async Task<string> RevokeTokens(User? user)
    {
        SetTokenCookies(string.Empty, string.Empty, DateTime.UtcNow.AddMinutes(-1));
        if (user is not null) await DeleteUserTokens(user);
        return string.Empty;
    }

    public void SetTokenCookies(string accessToken, string refreshToken, DateTime? expires)
    {
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            IsEssential = true
        };
        
        if (expires.HasValue) cookieOptions.Expires = expires;
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(AccessTokenCookie, accessToken, cookieOptions);
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(RefreshTokenCookie, refreshToken, cookieOptions);
    }
    
    private async Task SetUserToken(User user, AccessToken accessToken, RefreshToken refreshToken)
    {
        user.AccessTokenId = accessToken.TokenId;
        user.RefreshToken = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;

        await _userManager.UpdateAsync(user);
    }
    
    private async Task DeleteUserTokens(User user)
    {
        user.AccessTokenId = null;
        user.RefreshToken = null;
        user.TokenCreated = null;
        user.TokenExpires = null;

        await _userManager.UpdateAsync(user);
    }
}