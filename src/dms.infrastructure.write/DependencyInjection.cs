using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Repositories;
using DMS.Infrastructure.Write.Configuration;
using DMS.Infrastructure.Write.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DMS.Infrastructure.Write;

public static class DependencyInjection
{
    public static IServiceCollection AddWriteInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IOutbox, OutboxRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IRefreshTokenWriteRepository, RefreshTokenWriteRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentAccessRequestRepository, DocumentAccessRequestRepository>();

        #region Database

        const string sectionName = "WriteDatabase";

        services.AddOptions<WriteDbOptions>()
            .Bind(configuration.GetSection(sectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString),
                $"{sectionName}:ConnectionString is required")
            .ValidateOnStart();

        services.AddDbContext<WriteDbContext>((sp, opt) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();

            if (env.IsEnvironment("Testing"))
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                var dbName = cfg["WriteDatabase:InMemoryName"] ?? "WriteDb-Tests";
                opt.UseInMemoryDatabase(dbName);
            }
            else
            {
                var options = sp.GetRequiredService<IOptions<WriteDbOptions>>().Value;
                var fullPath = Path.GetFullPath(options.ConnectionString.Replace("Data Source=", "").Trim(), Environment.CurrentDirectory);
                var connString = $"Data Source={fullPath}";
                opt.UseSqlite(connString);
            }
        });

        #endregion

        return services;
    }
}
