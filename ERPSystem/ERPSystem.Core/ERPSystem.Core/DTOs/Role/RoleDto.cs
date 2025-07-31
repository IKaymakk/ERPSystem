using System.ComponentModel.DataAnnotations;

namespace ERPSystem.Core.DTOs.Role;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int UserCount { get; set; } // Bu role sahip kullanıcı sayısı
}