using DMS.Application.Abstractions.Persistence.Read;
using DMS.Infrastructure.Read.Configuration;
using DMS.Infrastructure.Read.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DMS.Infrastructure.Read;

public static class DependencyInjection
{
    public static IServiceCollection AddReadInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        const string sectionName = "ReadDatabase";

        services.AddOptions<ReadDbOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString),
                $"{sectionName}:ConnectionString is required")
            .ValidateOnStart();

        services.AddDbContext<ReadDbContext>((sp, opt) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();

            if (env.IsEnvironment("Testing"))
            {
                opt.UseInMemoryDatabase($"ReadDb-{Guid.NewGuid()}");
            }
            else
            {
                var db = sp.GetRequiredService<IOptions<ReadDbOptions>>().Value;
                opt.UseSqlite(db.ConnectionString);
            }
        }); 

        services.AddScoped<IUserReadRepository, UserReadRepository>();

        return services;
    }
}
