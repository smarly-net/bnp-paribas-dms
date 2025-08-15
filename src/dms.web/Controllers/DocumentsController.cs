using System.Security.Claims;
using DMS.Application.Documents.IssueAccessInvite;
using DMS.Application.Documents.List;
using DMS.Application.Documents.RequestAccess;
using DMS.Contracts.Common;
using DMS.Contracts.Documents.IssueAccessInvite;
using DMS.Contracts.Documents.List;
using DMS.Contracts.Documents.RequestAccess;
using DMS.Infrastructure.Write.Entities;
using DMS.Web.Controllers.Base;
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Web.Controllers
{
    [Route("api/[controller]")]
    public sealed class DocumentsController : AuthorizeControllerBase
    {
        private readonly ILogger<DocumentsController> _logger;
        private readonly IMediator _mediator;

        public DocumentsController(ILoggerFactory factory, IMediator mediator)
        {
            _logger = factory.CreateLogger<DocumentsController>();
            _mediator = mediator;
        }

        [HttpGet("get-list")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetList(CancellationToken ct)
        {
            var result = await _mediator.Send(new ListDocumentsQuery(), ct);
            if (!result.Success)
                return BadRequest(new ErrorDto(result.Error!));

            var dto = result.Data!
                .Select(x => new DocumentListItemResponseDto(x.Id, x.Title))
                .ToArray();

            return Ok(dto);
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