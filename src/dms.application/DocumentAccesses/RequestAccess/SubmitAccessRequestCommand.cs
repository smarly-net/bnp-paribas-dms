using DMS.Application.Common;
using DMS.Domain.DocumentAccesses;
using MediatR;

namespace DMS.Application.DocumentAccesses.RequestAccess;

public sealed record SubmitAccessRequestCommand(Guid UserId, string Token, string Reason, DocumentAccessRequestType AccessType) : IRequest<Result<Guid>>;