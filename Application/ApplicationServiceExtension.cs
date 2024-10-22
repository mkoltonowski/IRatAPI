using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Domain.Aggregates.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // TODO: Implement automapper
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}
