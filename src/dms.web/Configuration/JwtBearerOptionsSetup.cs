using DMS.Application.Abstractions.Auth;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace DMS.Web.Configuration;

public sealed class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtSettings _settings;

    public JwtBearerOptionsSetup(IOptions<JwtSettings> settings)
        => _settings = settings.Value;

    public void Configure(string? name, JwtBearerOptions options)
        => Configure(options);

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    }
}