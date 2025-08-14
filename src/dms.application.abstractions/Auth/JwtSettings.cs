using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Application.Abstractions.Auth;

public class JwtSettings
{
    public string Secret { get; set; } = default!;
    public string Issuer { get; set; } = "issuer";
    public string Audience { get; set; } = "aud";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;
}