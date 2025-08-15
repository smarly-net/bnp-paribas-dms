using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Repositories;
using DMS.Contracts.Events;
using DMS.Infrastructure.Read.Configuration;
using DMS.Infrastructure.Read.Projections;
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
        services.AddScoped<IDocumentAccessInviteRepository, DocumentAccessInviteRepository>();
        services.AddScoped<IDocumentAccessUserRequestRepository, DocumentAccessUserRequestRepository>();


        services.AddScoped<AccessInviteIssuedProjector>();
        services.AddScoped<AccessRequestSubmittedProjector>();

        services.AddScoped<Dictionary<string, IProjector>>(sp => new()
        {
            [nameof(AccessInviteIssuedEvent)] = sp.GetRequiredService<AccessInviteIssuedProjector>(),
            [nameof(AccessRequestSubmittedEvent)] = sp.GetRequiredService<AccessRequestSubmittedProjector>(),
        });

        #region Database

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
                var cfg = sp.GetRequiredService<IConfiguration>();
                var dbName = cfg["ReadDatabase:InMemoryName"] ?? "ReadDb-Tests";
                opt.UseInMemoryDatabase(dbName);
            }
            else
            {
                var options = sp.GetRequiredService<IOptions<ReadDbOptions>>().Value;
                var fullPath = Path.GetFullPath(options.ConnectionString.Replace("Data Source=", "").Trim(), Environment.CurrentDirectory);
                var connString = $"Data Source={fullPath}";
                opt.UseSqlite(connString);
            }
        });

        #endregion

        return services;
    }
}
