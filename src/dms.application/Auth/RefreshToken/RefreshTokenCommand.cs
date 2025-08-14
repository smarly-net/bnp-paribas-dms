using DMS.Application.Abstractions.Auth.Models;
using DMS.Application.Common;

using MediatR;

namespace DMS.Application.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken, string LastAccessToken) : IRequest<Result<LoginToken>>;