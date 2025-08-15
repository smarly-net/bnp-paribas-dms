using DMS.Application.Documents.List;
using DMS.Contracts.Common;
using DMS.Contracts.Documents.List;
using DMS.Web.Controllers.Base;
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
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
    }
}