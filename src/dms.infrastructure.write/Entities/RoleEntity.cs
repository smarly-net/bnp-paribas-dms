namespace DMS.Infrastructure.Write.Entities;

public class RoleEntity
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = null!;

    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
}