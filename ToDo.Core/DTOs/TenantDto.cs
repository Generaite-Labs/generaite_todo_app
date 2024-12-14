namespace ToDo.Core.DTOs;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<TenantUserDto> Users { get; set; } = new List<TenantUserDto>();
}

public class TenantUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string TenantRole { get; set; } = string.Empty;
}

public class CreateTenantDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateTenantDto
{
    public string Name { get; set; } = string.Empty;
}

public class TenantUserUpdateDto
{
    public string UserId { get; set; } = string.Empty;
    public string TenantRole { get; set; } = string.Empty;
}

public class AddUserToTenantDto
{
    public string UserId { get; set; } = string.Empty;
    public string TenantRole { get; set; } = string.Empty;
}
