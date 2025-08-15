using DMS.Application.Abstractions.Auth.Models;
using DMS.Application.Abstractions.Auth.Services;
using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Repositories;
using DMS.Application.Auth.Login;
using DMS.Application.Common;
using DMS.Contracts.Auth;
using DMS.Contracts.Auth.Login;

using MediatR;

using Microsoft.Extensions.Options;

using System.Runtime;
using System.Security.Cryptography;

namespace DMS.Application.Auth.RefreshToken;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<LoginToken>>
{
    private readonly IRefreshTokenWriteRepository _refreshTokenWriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenHandler(
        IRefreshTokenWriteRepository refreshTokenWriteRepository
        , IUserRepository userRepositoryRepository
        , IJwtService jwtServiceService
        , IOptions<JwtSettings> jwtSettings
        , IUnitOfWork unitOfWork)
    {
        _refreshTokenWriteRepository = refreshTokenWriteRepository;
        _userRepository = userRepositoryRepository;
        _jwtService = jwtServiceService;
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<LoginToken>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var refreshToken = await _refreshTokenWriteRepository.GetAsync(request.RefreshToken, ct);
        if (refreshToken is null || refreshToken.ExpiryDate <= DateTime.UtcNow)
        {
            return Result<LoginToken>.Fail("Invalid or expired refresh token");
        }

        string lastJti;
        try
        {
            lastJti = _jwtService.GetJti(request.LastAccessToken, allowExpired: true);
        }
        catch
        {
            return Result<LoginToken>.Fail("Invalid last access token");
        }

        if (!string.Equals(lastJti, refreshToken.ParentAccessJti, StringComparison.Ordinal))
        {
            return Result<LoginToken>.Fail("Token family mismatch");
        }

        var user = await _userRepository.GetByIdAsync(refreshToken.UserId, ct);
        if (user is null)
        {
            return Result<LoginToken>.Fail("User not found");
        }

        var newAccess = _jwtService.Generate(user.Id, user.Username, user.Roles);
        var newAccessJti = _jwtService.GetJti(newAccess);

        var newRefresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays);

        await _refreshTokenWriteRepository.RotateAsync(user.Id, refreshToken.Token, newRefresh, expires, newAccessJti, ct);
        await _unitOfWork.Commit(ct);

        return Result<LoginToken>.Ok(new LoginToken(newAccess, newRefresh));
    }
}