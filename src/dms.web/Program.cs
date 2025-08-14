using DMS.Application;
using DMS.Application.Abstractions.Auth.Services;
using DMS.Infrastructure;
using DMS.Infrastructure.Read;
using DMS.Infrastructure.Write;
using DMS.Web.BackgroundServices;
using DMS.Web.Configuration;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

var builderServices = builder.Services;

builderServices.AddHandlers();
builderServices
    .AddInfrastructure()
    .AddReadInfrastructure(builder.Configuration)
    .AddWriteInfrastructure(builder.Configuration);

builderServices.AddHostedService<OutboxProcessor>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builderServices
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builderServices.AddTransient<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsSetup>();

builderServices.AddAuthorization();
builderServices.AddControllers();

builderServices.AddControllers();

var app = builder.Build();

foreach (var dbContextType in new[] { typeof(ReadDbContext), typeof(WriteDbContext) })
{
    using var scope = app.Services.CreateScope();
    var db = (DbContext)scope.ServiceProvider.GetRequiredService(dbContextType);
    await db.Database.EnsureCreatedAsync();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }