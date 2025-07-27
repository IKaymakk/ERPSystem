public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    // Role information
    public int RoleId { get; set; }
    public string RoleName { get; set; }
}