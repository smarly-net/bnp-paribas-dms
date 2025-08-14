using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Auth.Login;
using DMS.Contracts.Auth;
using DMS.Contracts.Common;
using DMS.Infrastructure.Read;

using FluentAssertions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DMS.Infrastructure.Read.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DMS.Web.Test.Service.Auth;

public class LoginPostTest : IClassFixture<AppFactory>
{
    private readonly AppFactory _factory;
    private readonly HttpClient _client;

    public LoginPostTest(AppFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Incorrect_Credentials_Returns_Unauthorized()
    {
        // arrange
        var dbName = $"readdb-{Guid.NewGuid()}";

        using var factory = _factory.WithDb(dbName); 
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
            db.Database.EnsureCreated();
            db.Users.Add(new UserReadEntity { Id = Guid.NewGuid(), Username = "john.doe", PasswordHash = "x" });
            db.SaveChanges();
        }

        var content = new StringContent(JsonSerializer.Serialize(new
        {
            Username = "john.jr.doe",
            Password = "secret"
        }), Encoding.UTF8, "application/json");

        var client = factory.CreateClient();

        // act
        var resp = await client.PostAsync("/api/auth/login", content);

        // assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var dto = await resp.Content.ReadFromJsonAsync<ErrorDto>();
        dto.Should().NotBeNull();
        dto!.Error.Should().Be("Invalid username or password");
    }

    [Fact]
    public async Task Login_Correct_Credentials_Returns_OK()
    {
        // arrange
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            Username = "denis.dmitriev",
            Password = "secret"
        }), Encoding.UTF8, "application/json");

        // act
        var resp = await _client.PostAsync("/api/auth/login", content);

        // assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<LoginResponseDto>();
        dto.Should().NotBeNull();

        dto!.AccessToken.Should().NotBeNullOrWhiteSpace();
        dto!.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }
}