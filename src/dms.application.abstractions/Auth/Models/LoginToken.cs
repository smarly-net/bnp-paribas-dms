namespace DMS.Application.Abstractions.Auth.Models;

public sealed record LoginToken(string AccessToken, string RefreshToken);