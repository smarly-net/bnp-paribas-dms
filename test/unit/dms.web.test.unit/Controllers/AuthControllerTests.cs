using DMS.Application.Auth.Login;
using DMS.Contracts.Auth;
using DMS.Contracts.Common;
using DMS.Web.Controllers;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMS.Web.Test.Unit.Controllers;

public class AuthControllerTests
{
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _controller;
    private readonly Mock<IMediator> _mediatr;

    public AuthControllerTests()
    {
        _loggerMock = new Mock<ILogger<AuthController>>();
        _mediatr = new Mock<IMediator>();
        _controller = new AuthController(_loggerMock.Object, _mediatr.Object);
    }

    [Fact]
    public async Task Login_Should_Return_401_When_Fail()
    {
        // arrange
        _mediatr
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoginResult.Fail("Invalid username or password"));

        // act
        var result = await _controller.Login(new LoginRequestDto("john", "secret"), CancellationToken.None);

        // assert
        var objectResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.IsType<ErrorDto>(objectResult.Value);

        var resultDto = (ErrorDto)objectResult.Value;
        Assert.Equal("Invalid username or password", resultDto.Error);
    }

    [Fact]
    public async Task Login_Should_Return_200_When_Correct()
    {
        // arrange
        Guid userId = Guid.NewGuid();
        string userName = "Person";
        _mediatr
            .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(LoginResult.Ok(new LoginResponseDto(userId, userName)));

        // act
        var result = await _controller.Login(new LoginRequestDto("john", "secret"), CancellationToken.None);

        // assert
        var objectResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<LoginResponseDto>(objectResult.Value);

        var resultDto = (LoginResponseDto)objectResult.Value;

        Assert.Equal(userId, resultDto.UserId);
        Assert.Equal(userName, resultDto.Username);
    }

}