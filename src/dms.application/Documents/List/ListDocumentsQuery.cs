using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Common;

using MediatR;

namespace DMS.Application.Documents.List;

public sealed record ListDocumentsQuery : IRequest<Result<IReadOnlyCollection<DocumentItem>>>;