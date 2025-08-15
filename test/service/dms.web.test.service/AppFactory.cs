
using DMS.Application.Abstractions.Outbox;
using DMS.Infrastructure.Write;
using DMS.Infrastructure.Write.Entities;
using DMS.Web.BackgroundServices;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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
                var db = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
                db.Database.EnsureCreated();

                if (!_seeded)
                {
                    db.Users.Add(new UserEntity
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

                services.RemoveAll<IOutbox>();
                services.AddSingleton<IOutbox, ImmediateOutbox>();

                services.AddSingleton<IProjector, NoopProjector>();
            });
        }

    }
}
