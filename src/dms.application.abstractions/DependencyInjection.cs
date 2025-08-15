using DMS.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DMS.Application.Abstractions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationAbstraction(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeService, DateTimeService>();

        return services;
    }
}
