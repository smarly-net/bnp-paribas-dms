using DMS.Contracts.Auth;

namespace DMS.Application.Auth.Login;

public sealed class LoginResult
{
    public bool Success { get; init; }
    public LoginResponseDto? Data { get; init; }
    public string? Error { get; init; }

    public static LoginResult Ok(LoginResponseDto dto) => new() { Success = true, Data = dto };
    public static LoginResult Fail(string error) => new() { Success = false, Error = error };
}