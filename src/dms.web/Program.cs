using DMS.Application;
using DMS.Infrastructure.Read;

var builder = WebApplication.CreateBuilder(args);

var builderServices = builder.Services;

builderServices.AddHandlers();
builderServices.AddReadInfrastructure(builder.Configuration);

builderServices.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }