using DMS.Application.DocumentAccesses.IssueAccessInvite;
using DMS.Application.DocumentAccesses.List;
using DMS.Application.DocumentAccesses.RequestAccess;
using DMS.Contracts.Common;
using DMS.Contracts.DocumentAccesses.IssueAccessInvite;
using DMS.Contracts.DocumentAccesses.List;
using DMS.Contracts.DocumentAccesses.RequestAccess;
using DMS.Contracts.Documents.List;
using DMS.Web.Controllers.Base;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace DMS.Web.Controllers
{
    [Route("api/[controller]")]
    public sealed class DocumentAccessesController : AuthorizeControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IMediator _mediator;

        public DocumentAccessesController(ILoggerFactory factory, IMediator mediator)
        {
            _logger = factory.CreateLogger<DocumentsController>();
            _mediator = mediator;
        }

        [HttpPost("{documentId:guid}/issue-access-invite")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> IssueAccessInvite(Guid documentId, [FromBody] IssueAccessInviteRequestDto dto, CancellationToken ct)
        {
            var result = await _mediator.Send(
                new IssueAccessInviteCommand(documentId, dto.UserId, dto.ExpiresAtUtc),
                ct);

            if (!result.Success)
                return BadRequest(new ErrorDto(result.Error!));

            var appModel = result.Data!;

            return StatusCode(StatusCodes.Status201Created, new IssueAccessInviteResponseDto(appModel.Token, appModel.ExpiresAtUtc));
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetList(CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue("uid");
            if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                return Unauthorized(new ErrorDto("Invalid user identity."));
            }

            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();

            var result = await _mediator.Send(new ListDocumentAccessesQuery(userId, roles), ct);

            if (!result.Success)
            {
                return BadRequest(new ErrorDto(result.Error!));
            }

            var dto = result.Data!
                .Select(x => new DocumentAccessListItemResponseDto(x.InviteId, x.UserName, x.DocumentTitle, x.Reason, x.AccessType, x.RequestDate, x.Status, x.DecisionComment, x.DecisionDate, x.DecisionByUserName))
                .ToArray();

            return Ok(dto);
        }

        [HttpPost("request-access")]
        public async Task<IActionResult> RequestAccess([FromBody] SubmitAccessRequestRequestDto dto, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue("uid");
            if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized(new ErrorDto("Invalid user identity."));

            var result = await _mediator.Send(new SubmitAccessRequestCommand(userId, dto.Token, dto.Reason, dto.Type), ct);

            if (!result.Success)
            {
                return BadRequest(new ErrorDto(result.Error!));
            }

            return StatusCode(StatusCodes.Status201Created, result.Data);
        }
    }
}