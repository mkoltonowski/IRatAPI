using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServicesExtension
{

    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Implement MongoDB infrastructure to store user messages
        // - document based solution will do better for horizontal scaling
        // - SQL Queries will be suboptimal in comparison to Mongo aggregations for Realtime messaging
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? 
            configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<RatDbContext>( options => options.UseSqlServer(connectionString) );
    }
}