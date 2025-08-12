
using DMS.Application.Abstractions.Persistence.Read;
using DMS.Infrastructure.Read;
using DMS.Infrastructure.Read.Configuration;
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
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    ["ReadDatabase:ConnectionString"] = "Fake-Connection-String"
                };
                config.AddInMemoryCollection(dict!);
            });

            builder.UseEnvironment("Testing");

            //// 4) Инициализация/сидинг по желанию
            //var sp = services.BuildServiceProvider();
            //using var scope = sp.CreateScope();
            //var db = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
            //db.Database.EnsureCreated();
            //// Seed(db); // если нужно
        }

    }
}
