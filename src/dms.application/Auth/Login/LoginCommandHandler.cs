using DMS.Application.Abstractions.Auth.Models;
using DMS.Application.Abstractions.Auth.Services;
using DMS.Application.Abstractions.Outbox;
using DMS.Application.Common;
using MediatR;

using Microsoft.Extensions.Options;

using System.Security.Cryptography;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Abstractions.Persistence.Read;

namespace DMS.Application.Auth.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<LoginToken>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenWriteRepository _refreshTokenWriteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutbox _outbox;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;

    public LoginCommandHandler(
        IUserRepository userRepository
        , IJwtService jwtService
        , IOptions<JwtSettings> jwtSettings
        , IRefreshTokenWriteRepository refreshTokenWriteRepository
        , IUnitOfWork unitOfWork
        , IOutbox outbox)
    {
        _userRepository = userRepository;
        _refreshTokenWriteRepository = refreshTokenWriteRepository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<LoginToken>> Handle(LoginCommand request, CancellationToken ct)
    {
        UserLogin? user = await _userRepository.GetByUsernameAsync(request.Username, ct);
        if (user is null)
            return Result<LoginToken>.Fail("Invalid username or password");

        // NOTE: Password-hash generation and verification are not implemented here.
        // This is just a proof-of-concept for the interview task, so we assume the
        // user enters the correct password. The "wrong password" case is simulated
        // by using a username that doesn't exist in the database.
        //
        // In a real production application, you would implement proper password 
        // hashing, verification, and also validate that neither username nor 
        // password is empty.

        var accessToken = _jwtService.Generate(user.Id, request.Username, user.Roles);
        var accessJti = _jwtService.GetJti(accessToken);

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays);

        await _refreshTokenWriteRepository.Add(new Abstractions.Persistence.Write.RefreshToken(user.Id, refreshToken, expires, accessJti), ct);
        await _unitOfWork.Commit(ct);

        return Result<LoginToken>.Ok(new LoginToken(accessToken, refreshToken));
    }
}