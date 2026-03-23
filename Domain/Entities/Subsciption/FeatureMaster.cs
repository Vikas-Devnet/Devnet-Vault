using Domain.Common.Models;

namespace Domain.Entities.Subsciption;

public class FeatureMaster : AuditProperty
{
    public int Id { get; set; }
    public string FeatureName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<SubscriptionFeature> SubscriptionFeatures { get; set; } = [];
}
