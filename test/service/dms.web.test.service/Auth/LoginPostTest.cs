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
    private readonly HttpClient _client;

    public LoginPostTest(AppFactory factory)
    {
        _client = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
        db.Users.Add(new UserReadEntity() { Id = Guid.NewGuid(), Username = "john.doe", PasswordHash = $"{Guid.NewGuid()}"});
        db.SaveChanges();
    }

    [Fact]
    public async Task Login_Incorrect_Credentials_Returns_Unauthorized()
    {
        // arrange
        var content = new StringContent(JsonSerializer.Serialize(new
        {
            Username = "john.doe",
            Password = "incorrect secret"
        }), Encoding.UTF8, "application/json");

        // act
        var resp = await _client.PostAsync("/api/auth/login", content);

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
            Password = "correct secret"
        }), Encoding.UTF8, "application/json");

        // act
        var resp = await _client.PostAsync("/api/auth/login", content);

        // assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<LoginRequestDto>();
        dto.Should().NotBeNull();
        dto!.Username.Should().Be("denis.dmitriev");
    }
}