using DMS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace DMS.Web.Test.Unit.Controllers;

public class PingControllerTests
{
    private readonly Mock<ILogger<PingController>> _loggerMock;
    private readonly PingController _controller;

    public PingControllerTests()
    {
        _loggerMock = new Mock<ILogger<PingController>>();
        _controller = new PingController(_loggerMock.Object);
    }

    [Fact]
    public void Get_LogsInformation()
    {
        // arrange

        // act
        var result = _controller.Get();

        // assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("Method Get") == true),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
            ),
            Times.Once
        );
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