using MediatR;

namespace DMS.Application.Auth.Login;

public sealed record LoginCommand(string Username, string Password)
    : IRequest<LoginResult>;