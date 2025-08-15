using DMS.Application.Auth.Login;
using DMS.Application.Auth.RefreshToken;
using DMS.Contracts.Auth.Login;
using DMS.Contracts.Auth.RefreshToken;
using DMS.Contracts.Common;
using DMS.Web.Controllers.Base;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : AuthorizeControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(ILoggerFactory factory, IMediator mediator)
        {
            _logger = factory.CreateLogger<AuthController>();
            _mediator = mediator;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
        {
            var result = await _mediator.Send(new LoginCommand(dto.Username, dto.Password), ct);
            if (!result.Success)
            {
                return Unauthorized(new ErrorDto(result.Error!));
            }

            var responseDto = new LoginResponseDto(result.Data!.AccessToken, result.Data!.RefreshToken);
            return Ok(responseDto);
        }

        [HttpPost]
        [Route("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto, CancellationToken ct)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(dto.RefreshToken, dto.LastAccessToken), ct);
            if (!result.Success)
            {
                return Unauthorized(new ErrorDto(result.Error!));
            }

            var responseDto = new LoginResponseDto(result.Data!.AccessToken, result.Data!.RefreshToken);
            return Ok(responseDto);
        }
    }
}
