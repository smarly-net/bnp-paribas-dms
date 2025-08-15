using DMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DMS.Web.Test.Unit.Controllers;

public class PingControllerTests
{
    private readonly Mock<ILoggerFactory> _loggerMock;
    private readonly PingController _controller;

    public PingControllerTests()
    {
        _loggerMock = new Mock<ILoggerFactory>();
        _controller = new PingController(_loggerMock.Object);
    }

    [Fact]
    public void Get_ReturnsOkWithPong()
    {
        // arrange

        // act
        var result = _controller.Get();

        // assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("pong", ok.Value);
    }
}