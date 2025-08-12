using DMS.Application.Auth.Login;
using DMS.Contracts.Auth;
using DMS.Contracts.Common;
using DMS.Web.Controllers.Base;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : AuthorizeControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(ILogger<AuthController> logger, IMediator mediator)
        {
            _logger = logger;
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
                return Unauthorized(new ErrorDto(result.Error ?? "Invalid username or password"));
            }

            return Ok(result.Data);
        }
    }
}
