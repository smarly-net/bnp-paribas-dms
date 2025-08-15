using DMS.Application.Abstractions.Auth.Services;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DMS.Application.Abstractions.Services;

namespace DMS.Infrastructure.Auth;

public sealed class JwtService : IJwtService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> settings, IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService;
        _settings = settings.Value;
    }

    public string Generate(Guid userId, string username, IEnumerable<string> roles)
    {
        var now = _dateTimeService.UtcNow;
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, username),
            new("uid", userId.ToString()),
            new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _dateTimeService.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = creds
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }

    public string GetJti(string token, bool allowExpired = false)
    {
        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _settings.Issuer,
            ValidateAudience = true,
            ValidAudience = _settings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret)),
            ValidateLifetime = !allowExpired,
            ClockSkew = TimeSpan.Zero
        };
        handler.ValidateToken(token, parameters, out var validated);
        return ((JwtSecurityToken)validated).Id; 
    }
}