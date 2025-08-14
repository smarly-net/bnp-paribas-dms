using DMS.Application.User.List;
using DMS.Contracts.Common;
using DMS.Contracts.Users.List;
using DMS.Web.Controllers.Base;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Web.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public sealed class UsersController : AuthorizeControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILoggerFactory factory, IMediator mediator)
    {
        _logger = factory.CreateLogger<UsersController>();
        _mediator = mediator;
    }

    [HttpGet("get-list")]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListUsersQuery(), ct);
        if (!result.Success)
            return BadRequest(new ErrorDto(result.Error!));

        var dto = result.Data!
            .Select(u => new UserListItemResponseDto(u.Id, u.Username))
            .ToArray();

        return Ok(dto);
    }
}