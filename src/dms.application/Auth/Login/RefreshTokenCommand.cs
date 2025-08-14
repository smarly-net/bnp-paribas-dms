using MediatR;

namespace DMS.Application.Auth.Login;

public sealed record RefreshTokenCommand(string RefreshToken, string LastAccessToken) : IRequest<LoginResult>;