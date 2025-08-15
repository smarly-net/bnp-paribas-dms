using DMS.Application.Abstractions.Auth.Services;
using DMS.Application.Abstractions.Notifications;
using DMS.Infrastructure.Auth;
using DMS.Infrastructure.Notifications;

using Microsoft.Extensions.DependencyInjection;

namespace DMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<INotificationSender, EmailNotificationSender>();


        return services;
    }
}
