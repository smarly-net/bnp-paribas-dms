namespace DMS.Contracts.Auth.RefreshToken;

public sealed record RefreshResponseDto(string AccessToken, string RefreshToken);