using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Common;

using MediatR;

namespace DMS.Application.User.List;

public sealed record ListUsersQuery()
    : IRequest<Result<IReadOnlyCollection<UserItem>>>;