namespace DMS.Contracts.Auth.RefreshToken;

public sealed record RefreshRequestDto(string RefreshToken, string LastAccessToken);