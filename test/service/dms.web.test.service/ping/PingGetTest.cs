using FluentAssertions;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Xunit;

namespace dms.web.test.service.ping;

public class PingGetTest : IClassFixture<AppFactory>
{
    private readonly HttpClient _client;

    public PingGetTest(AppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Ping_returns_pong()
    {
        var resp = await _client.GetAsync("/ping");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        string responseMessage = await resp.Content.ReadAsStringAsync();
        responseMessage.Should().Be("pong");
    }

   // private record PingDto(string message);
}