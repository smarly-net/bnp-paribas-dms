using DMS.Application.Auth.Login;
using Microsoft.Extensions.DependencyInjection;

namespace DMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

        return services;
    }
}
