using Application.Providers.Identity;
using Domain.Aggregates.Identity;
using Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServicesExtension
{

    private static void AddIdentityProvider(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>(identityBuilder =>
        {
            identityBuilder.Password.RequireDigit = true;
            identityBuilder.Password.RequiredLength = 10;
            identityBuilder.Password.RequireUppercase = true;
            identityBuilder.Password.RequireNonAlphanumeric = false;
        }).AddEntityFrameworkStores<RatDbContext>()
        .AddUserManager<IdentityUserManager>()
        .AddDefaultTokenProviders();
    }
    
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement MongoDB infrastructure to store user messages
        // - document based solution will do better for horizontal scaling
        // - SQL Queries will be suboptimal in comparison to Mongo aggregations for Realtime messaging
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? 
            configuration.GetConnectionString("DefaultConnection");
        
        services.AddIdentityProvider();
        services.AddDbContext<RatDbContext>( options => options.UseSqlServer(connectionString) );
    }
}