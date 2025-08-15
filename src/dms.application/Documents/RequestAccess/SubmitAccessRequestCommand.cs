using DMS.Application.Common;
using DMS.Domain.Documents;
using MediatR;

namespace DMS.Application.Documents.RequestAccess;

public sealed record SubmitAccessRequestCommand(Guid UserId, string Token, string Reason, DocumentAccessRequestType AccessType) : IRequest<Result<Guid>>;