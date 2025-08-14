using DMS.Application.Abstractions.Auth;
using DMS.Application.Abstractions.Outbox;
using DMS.Application.Abstractions.Persistence.Read;
using DMS.Application.Abstractions.Persistence.Write;
using DMS.Contracts.Auth;

using MediatR;

using Microsoft.Extensions.Options;

using System.Runtime;
using System.Security.Cryptography;

namespace DMS.Application.Auth.Login;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    private readonly IRefreshTokenWriteRepository _refreshTokenWriteRepository;
    private readonly IUserReadRepository _userReadRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenHandler(
        IRefreshTokenWriteRepository refreshTokenWriteRepository
        , IUserReadRepository userReadRepositoryReadRepository
        , IJwtService jwtServiceService
        , IOptions<JwtSettings> jwtSettings
        , IUnitOfWork unitOfWork)
    {
        _refreshTokenWriteRepository = refreshTokenWriteRepository;
        _userReadRepository = userReadRepositoryReadRepository;
        _jwtService = jwtServiceService;
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResult> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var refreshToken = await _refreshTokenWriteRepository.GetAsync(request.RefreshToken, ct);
        if (refreshToken is null || refreshToken.ExpiryDate <= DateTime.UtcNow)
        {
            return LoginResult.Fail("Invalid or expired refresh token");
        }

        string lastJti;
        try
        {
            lastJti = _jwtService.GetJti(request.LastAccessToken, allowExpired: true);
        }
        catch
        {
            return LoginResult.Fail("Invalid last access token");
        }

        if (!string.Equals(lastJti, refreshToken.ParentAccessJti, StringComparison.Ordinal))
        {
            return LoginResult.Fail("Token family mismatch");
        }

        var user = await _userReadRepository.GetByIdAsync(refreshToken.UserId, ct);
        if (user is null)
        {
            return LoginResult.Fail("User not found");
        }

        var newAccess = _jwtService.Generate(user.Username, user.Roles);
        var newAccessJti = _jwtService.GetJti(newAccess);

        var newRefresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays);

        await _refreshTokenWriteRepository.RotateAsync(user.Id, refreshToken.Token, newRefresh, expires, newAccessJti, ct);
        await _unitOfWork.Commit(ct);

        return LoginResult.Ok(new LoginResponseDto(newAccess, newRefresh));
    }
}