namespace DMS.Contracts.Auth.Login;

public sealed record LoginResponseDto(string AccessToken, string RefreshToken);