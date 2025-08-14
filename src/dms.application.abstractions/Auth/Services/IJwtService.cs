namespace DMS.Application.Abstractions.Auth.Services;

public interface IJwtService
{
    string Generate(string username, IEnumerable<string> roles);
    string GetJti(string token, bool allowExpired = false);
}