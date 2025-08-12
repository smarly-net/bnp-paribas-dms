
using DMS.Application.Abstractions.Persistence.Read;
using DMS.Infrastructure.Read;
using DMS.Infrastructure.Read.Configuration;
using DMS.Infrastructure.Read.Entities;
using DMS.Infrastructure.Read.Repositories;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DMS.Web.Test.Service
{
    public class AppFactory : WebApplicationFactory<Program>
    {
        public const string SharedDbName = "ReadDb-Shared";
        private static bool _seeded;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    ["ReadDatabase:ConnectionString"] = "Fake-Connection-String",
                    ["ReadDatabase:InMemoryName"] = SharedDbName
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
            });
        }

    }
}
