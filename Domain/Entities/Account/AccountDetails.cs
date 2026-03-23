using Domain.Common.Models;
using Domain.Entities.Subsciption;

namespace Domain.Entities.Account;

public class AccountDetails : AuditProperty
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public bool IsAdmin { get; set; } = false;
    public int SubscriptionId { get; set; } = 0;
    public DateTime? ExpiryDate { get; set; }
    public UserMaster UserMaster { get; set; } = null!;
    public SubscriptionMaster SubscriptionMaster { get; set; } = null!;
}
