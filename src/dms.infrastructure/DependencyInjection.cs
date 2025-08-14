using DMS.Application.Abstractions.Auth.Services;
using DMS.Infrastructure.Auth;

using Microsoft.Extensions.DependencyInjection;

namespace DMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();


        return services;
    }
}
