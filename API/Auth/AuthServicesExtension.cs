using System.Text;
using Application.Providers.Auth.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Auth;

public static class AuthServicesExtension
{
    private static TokenValidationParameters MakeTokenValidationParameters(IConfiguration configuration, IHostEnvironment environment)
    {
        var jwtSecretKey = environment.IsDevelopment() 
            ? configuration.GetSection("JwtSecretKey").Value 
            : Environment.GetEnvironmentVariable("JwtSecretKey");

        if (string.IsNullOrWhiteSpace(jwtSecretKey))
        {
            throw new ArgumentException("Missing JwtSecretKey");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        
        if (signingKey == null) 
            throw new ArgumentException("Invalid JwtSecretKey");
        
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    }

    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var jwtTokenValidationParams = MakeTokenValidationParameters(configuration, environment);
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(configureOptions =>
        {
            configureOptions.ClaimsIssuer = JwtBearerConfig.Issuer;
            configureOptions.TokenValidationParameters = jwtTokenValidationParams;
            configureOptions.SaveToken = true;
        });
    }
}