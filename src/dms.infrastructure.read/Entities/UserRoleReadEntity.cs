using System;

namespace DMS.Infrastructure.Read.Entities;

public sealed class UserRoleReadEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public UserReadEntity User { get; set; } = null!;
    public RoleReadEntity Role { get; set; } = null!;
}