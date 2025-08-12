using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using System.Net;

using Xunit;

namespace DMS.Web.Test.Service.Ping;

public class PingGetTest : IClassFixture<AppFactory>
{
    private readonly HttpClient _client;

    public PingGetTest(AppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ping_Returns_Pong()
    {
        // arrange

        // act
        var resp = await _client.GetAsync("api/ping");

        // assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        string responseMessage = await resp.Content.ReadAsStringAsync();
        responseMessage.Should().Be("pong");
    }
}