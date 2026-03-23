using Domain.Common.Models;
using Domain.Entities.Account;

namespace Domain.Entities.Subsciption;

public class SubscriptionMaster : AuditProperty
{
    public int Id { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationInMonths { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; } = [];
    public ICollection<AccountDetails> AccountDetails { get; set; } = [];
}
