using Domain.Common.Models;

namespace Domain.Entities;

public class UserMaster : AuditProperty
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;

    public ICollection<RefreshTokens> RefreshTokens { get; set; } = [];
}
