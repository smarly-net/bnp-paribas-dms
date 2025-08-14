namespace DMS.Contracts.Auth;

public sealed record RefreshRequestDto(string RefreshToken, string LastAccessToken);