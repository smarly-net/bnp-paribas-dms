using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Common;

using MediatR;

namespace DMS.Application.DocumentAccesses.List;

public sealed record ListDocumentAccessesQuery(Guid UserId, string[] Roles) : IRequest<Result<IReadOnlyCollection<DocumentAccessItem>>>;