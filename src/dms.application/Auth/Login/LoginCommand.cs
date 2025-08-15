using DMS.Application.Abstractions.Auth.Models;
using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Common;

using MediatR;

namespace DMS.Application.Auth.Login;

public sealed record LoginCommand(string Username, string Password)
    : IRequest<Result<LoginToken>>;