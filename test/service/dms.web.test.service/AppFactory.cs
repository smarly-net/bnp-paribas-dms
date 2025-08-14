
using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Read;
using DMS.Infrastructure.Read;
using DMS.Infrastructure.Read.Configuration;
using DMS.Infrastructure.Read.Entities;
using DMS.Infrastructure.Read.Repositories;
using DMS.Web.BackgroundServices;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DMS.Web.Test.Service
{
    public class AppFactory : WebApplicationFactory<Program>
    {
        public const string SharedReadDbName = "ReadDb-Shared";
        public const string SharedWriteDbName = "WriteDb-Shared";
        private static bool _seeded;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    ["ReadDatabase:ConnectionString"] = "Fake-ReadDB-Connection-String",
                    ["ReadDatabase:InMemoryName"] = SharedReadDbName,
                    ["WriteDatabase:ConnectionString"] = "Fake-WriteDB-Connection-String",
                    ["WriteDatabase:InMemoryName"] = SharedWriteDbName
                };
                config.AddInMemoryCollection(dict!);
            });

            builder.ConfigureServices(services =>
            {
                using var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                db.Database.EnsureCreated();

                if (!_seeded)
                {
                    db.Users.Add(new UserReadEntity
                    {
                        Id = Guid.NewGuid(),
                        Username = "denis.dmitriev",
                        PasswordHash = "hash"
                    });
                    db.SaveChanges();
                    _seeded = true;
                }


                var hosted = services.FirstOrDefault(d =>
                    d.ServiceType == typeof(IHostedService) &&
                    d.ImplementationType == typeof(OutboxProcessor));
                if (hosted is not null) services.Remove(hosted);

                // 2) Подменить IOutbox -> ImmediateOutbox
                services.RemoveAll<IOutbox>();
                services.AddSingleton<IOutbox, ImmediateOutbox>();
            });
        }

    }
}
