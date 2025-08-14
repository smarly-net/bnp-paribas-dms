namespace DMS.Contracts.Auth;

public sealed record RefreshResponseDto(string AccessToken, string RefreshToken);