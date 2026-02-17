using Domain.Common.Models;

namespace Domain.Entities;

public class RefreshTokens : AuditProperty
{
    public int Id { get; set; }
    public Guid UserId { get; set; }

    public string IPAddress { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }=false;

    public UserMaster UserMaster { get; set; } = null!;
}
