namespace DMS.Application.Abstractions.Persistence.Read;

public sealed record UserRead(Guid Id, string Username, string PasswordHash, string[] Roles);