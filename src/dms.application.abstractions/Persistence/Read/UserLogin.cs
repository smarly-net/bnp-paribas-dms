namespace DMS.Application.Abstractions.Persistence.Read;

public sealed record UserLogin(Guid Id, string Username, string PasswordHash, string[] Roles);