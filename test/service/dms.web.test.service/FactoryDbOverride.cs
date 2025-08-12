using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DMS.Web.Test.Service
{
    public static class FactoryDbOverride
    {
        public static WebApplicationFactory<Program> WithDb(
            this WebApplicationFactory<Program> baseFactory, string dbName)
        {
            return baseFactory.WithWebHostBuilder(b =>
            {
                b.ConfigureAppConfiguration((ctx, cfg) =>
                    cfg.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ReadDatabase:InMemoryName"] = dbName
                    }));
            });
        }
    }
}
