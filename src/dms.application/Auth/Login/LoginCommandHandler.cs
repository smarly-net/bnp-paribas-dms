using DMS.Application.Abstractions.Persistence.Read;
using DMS.Contracts.Auth;

using MediatR;

namespace DMS.Application.Auth.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserReadRepository _users;

    public LoginCommandHandler(IUserReadRepository users)
    {
        _users = users;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _users.GetByUsernameAsync(request.Username, ct);
        if (user is null)
            return LoginResult.Fail("Invalid username or password");

        //validate password hash

        return LoginResult.Ok(new LoginResponseDto(user.Id, user.Username));
    }
}