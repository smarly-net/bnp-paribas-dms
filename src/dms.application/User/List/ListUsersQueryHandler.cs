using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Common;

using MediatR;

namespace DMS.Application.User.List;

public sealed class ListUsersQueryHandler
    : IRequestHandler<ListUsersQuery, Result<IReadOnlyCollection<UserItem>>>
{
    private readonly IUserRepository _userRepository;

    public ListUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyCollection<UserItem>>> Handle(ListUsersQuery request, CancellationToken ct)
    {
        var users = await _userRepository.GetAllAsync(ct);
        return Result<IReadOnlyCollection<UserItem>>.Ok(users);
    }
}